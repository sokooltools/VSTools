//===========================================================================================================
// Sample usage:
//
//        EllipsisTextBox txtBox = new EllipsisTextBox();
//        txtBox.EllipsisType = EllipsisTextBox.EllipsisLocation.Path;
//        txtBox.Text = @"C:\directory\subdirectory\filename.ext";
//
// Depending on the width of the textbox, it will display something like "C:\di…\filename.ext".
//
// However,
//        string boo = txtBox.Text; yields
//        boo == @"C:\directory\subdirectory\filename.ext", the original string, NOT the truncated string.
////===========================================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Creates a TextBox with built-in ellipsis control.<para><c>
	/// (1) Specify EllipsisTextBox.EllipsisType (e.g. = EllipsisLocation.Path)
	/// (2) Put text into the textbox (e.g., EllipsisTextBox.Text = @"c:\directory\file.txt")
	/// The displayed text is the original text modified by the EllipsisType.  (e.g., "…\file.txt" if that's what fills the small 
	/// textbox)
	/// The string returned by EllipsisTextBox.Text is the original text, which may differ from what is showing in the 
	/// textbox.</c></para>
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(TextBox))]
	//[DesignerCategory(@"code")]
	public sealed class EllipsisTextBox : TextBox
	{
		//..................................................................................................................................

		#region Private Fields

		private bool _isTextChangedIgnored;
		private bool _isTabKeyPressed;

		private string _initialText;
		private string _fullText;
		private string _ellipsisText;
		private EllipsisLocation _ellipsisLocation = EllipsisLocation.None;
		private TextFormatFlags _ellipsisTextFormatFlag = 0;

		private Button _btnExpText;
		private Panel _pnlExpText;
		private RichTextBox _txtExpText;
		private readonly ToolTip _tooltip;
		private static MyFilter _mf;

		private char _lineDelimiter = ';';

		#endregion

		//..................................................................................................................................

		#region EllipsisLocation Enumeration

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Defines the location where the ellipsis should appear when the text does not fit the width of the field.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public enum EllipsisLocation { Path, Word, End, None };

		#endregion

		//..................................................................................................................................

		#region Public Constructor

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="EllipsisTextBox"/> class.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public EllipsisTextBox()
		{
			_fullText = String.Empty;
			base.Text = String.Empty;

			if (_mf == null)
			{
				_mf = new MyFilter();
				Application.AddMessageFilter(_mf);
			}
			_mf.MouseDown += Mf_MouseDown;
			//_mf.KeyUp += mf_KeyUp;
			_tooltip = new ToolTip();
		}

		#endregion

		//..................................................................................................................................

		#region Public Properties

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Indicates where the ellipsis appears inside the text.
		/// (e.g. TextFormatFlags.PathEllipsis, TextFormatFlags.EndEllipsis, TextFormatFlags.WordEllipsis...)
		/// </summary>
		/// <value>The type of the ellipsis.</value>
		//------------------------------------------------------------------------------------------------------------------------
		[Category("Misc"), Description("Indicates where the ellipsis appears inside the text.")]
		[DefaultValue(EllipsisLocation.None)]
		public EllipsisLocation EllipsisType
		{
			get => _ellipsisLocation;
			set
			{
				_ellipsisLocation = value;
				switch (value)
				{
					case EllipsisLocation.End:
						_ellipsisTextFormatFlag = TextFormatFlags.EndEllipsis;
						break;
					case EllipsisLocation.None:
						_ellipsisTextFormatFlag = 0;
						break;
					case EllipsisLocation.Path:
						_ellipsisTextFormatFlag = TextFormatFlags.PathEllipsis;
						break;
					case EllipsisLocation.Word:
						_ellipsisTextFormatFlag = TextFormatFlags.WordEllipsis;
						break;
					default:
						_ellipsisTextFormatFlag = 0;
						_ellipsisLocation = EllipsisLocation.None;
						break;
				}
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the number of lines to show when the expand button is clicked. [Default=5].
		/// </summary>
		/// <value>The expansion line count.</value>
		//------------------------------------------------------------------------------------------------------------------------
		[DefaultValue(5)]
		[Category("Misc"), Description("The number of lines to show when the expand button (down-arrow) is clicked."
			+ " [Default=5]")]
		public int ExpansionLineCount { get; set; } = 5;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the character used for delimiting the lines of data when multiple lines are compressed into a single 
		/// line ('' indicates newline).  [Default=';']
		/// </summary>
		/// <value>The delimiter.</value>
		//------------------------------------------------------------------------------------------------------------------------
		[DefaultValue(';')]
		[Category("Misc"), Description("The character used to delimit the lines of data when multiple lines are compressed into"
			+ " a single line ('' indicates newline). [Default=';']")]
		public char LineDelimiter
		{
			// ReSharper disable once MemberCanBePrivate.Global
			get => _lineDelimiter;
			set => _lineDelimiter = value == '\0' ? '\n' : value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Prevents multiline from being changed.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		[ReadOnly(true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Multiline
		{
			get => base.Multiline;
			private set => base.Multiline = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets an indication as to whether this control's text has been modified by the user.
		/// </summary>
		/// <remarks>
		/// This is different from the 'Modified' property since this one resets itself if the modification is removed.
		/// </remarks>
		//------------------------------------------------------------------------------------------------------------------------
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsModified { get; private set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the original, unmodified text assigned to the control or sets a copy of the unmodified assigned text, but
		/// displays it modified by the <see cref="EllipsisType"/> whenever necessary.
		/// </summary>
		/// <value></value>
		/// <returns>The text displayed in the control.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		[Category("Misc"), Description("Gets the original, unmodified assigned text or sets a copy of the unmodified assigned"
									+ " text, but displays it modified by the EllipsisType.")]
		public new string Text
		{
			get => base.Focused ? base.Text : _fullText;
			set
			{
				// Ensure we get a trimmed copy of the original string, NOT a reference to the original string.
				// _NOTE: seems to be a "_bug" in .Trim()
				// It returns a reference to the original string if .Trim() does not need to modify the original string, 
				// otherwise, it returns a reference to a new string.
				// We could use…
				// string truncText = (new StringBuilder(value)).ToString();
				// but drop the below to see the .Trim() anomaly!
				_fullText = (value + " ").Trim(); // We want a new copy, not a reference to value!

				if (!Focused && !_isTextChangedIgnored)
					_initialText = (value + " ").Trim();

				SetEllipsisText();
				base.Text = HideSelection && (!Focused || _isTabKeyPressed) ? _ellipsisText : _fullText;
				_isTabKeyPressed = false;
			}
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether the control currently has the focus.
		/// </summary>
		/// <returns>true if the control has focus; otherwise, false.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		public override bool Focused => base.Focused || IsExpPanelVisible;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether the control can accept data that the user drags onto it.
		/// </summary>
		/// <value></value>
		/// <returns>Always returns true.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		[Category("Misc"), Description("Gets or sets a value indicating whether the control can accept data that the user drags"
									   + " onto it [default is False].")]
		[DefaultValue(false)]
		//[Browsable(false)]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowDrop { get; set; }

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Appends text to the current text of a text box.
		/// </summary>
		/// <param name="text">The text to append to the current contents of the text box.</param>
		//------------------------------------------------------------------------------------------------------------------------
		public new void AppendText(string text)
		{
			if (text.Length == 0)
				return;
			base.AppendText(text);
			Text = base.Text;
			OnTextChanged(EventArgs.Empty);
		}

		#endregion

		//..................................................................................................................................

		#region Protected Overrides

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.TextChanged"/> event raised when the text of this control is 
		/// changed.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnTextChanged(EventArgs e)
		{
			if (_isTextChangedIgnored)
				return;
			if (Focused)
				IsModified = _initialText != Text;
			base.OnTextChanged(e);
			_fullText = Text;
			SetEllipsisText();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised when the mouse is hovering over the control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
			if (_fullText == base.Text)
				_tooltip.Hide(this);
			else if (_btnExpText == null && ExpansionLineCount < 2)
				_tooltip.SetToolTip(this, Text);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.Enter"/> event raised when the mouse enters this control while 
		/// it is in an expanded state.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnEnter(EventArgs e)
		{
			if (_btnExpText == null && ExpansionLineCount > 1)
			{
				Control frm = FindForm();
				if (frm == null)
					return;

				CreateExpTextBox(frm);
				CreateExpButton(frm);
			}
			if (_btnExpText != null)
			{
				_btnExpText.Top = _pnlExpText.Top + 1;
				_btnExpText.Visible = true;
			}
			base.OnEnter(e);
			if (HideSelection == false)
				return;
			_isTextChangedIgnored = true;
			base.Text = _fullText;
			_isTextChangedIgnored = false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.Leave" /> event raised when the mouse leaves this control while 
		/// it is not in an expanded state.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnLeave(EventArgs e)
		{
			if (ExpansionLineCount > 1 && RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
				return;
			_tooltip.Hide(this);
			if (_btnExpText != null)
				_btnExpText.Visible = false;
			base.OnLeave(e);

			if (HideSelection == false)
				return;
			_isTextChangedIgnored = true;
			Text = base.Text;
			_isTextChangedIgnored = false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.KeyUp" /> event raised when a key is pressed and released while 
		/// this control has the focus.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (_btnExpText != null)
			{
				NativeMethods.GetCaretPos(out Point caretPos);
				_btnExpText.Visible = Width - caretPos.X > Height;
			}
			base.OnKeyUp(e);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.PreviewKeyDown" /> event raised when a key is pressed down 
		/// which is before the keydown event occuring while the control has focus.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			_isTabKeyPressed = e.KeyCode == Keys.Tab;
			base.OnPreviewKeyDown(e);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.Resize"/> event raised when this control is resized.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (base.Focused)
				return;
			_isTextChangedIgnored = true;
			Text = _fullText;
			_isTextChangedIgnored = false;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.DragEnter"/> event raised when a file is dragged on top of this 
		/// control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnDragEnter(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the <see cref="E:System.Windows.Forms.Control.DragDrop"/> event raised when a file is dropped onto this 
		/// control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		protected override void OnDragDrop(DragEventArgs e)
		{
			var filePath = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			if (filePath.Length > 0)
				Text = filePath[0];
		}

		#endregion

		//..................................................................................................................................

		#region Private EventHandlers

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised when the mousebutton is held down over an area outside the 'ExpandText' textbox control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void Mf_MouseDown()
		{
			if (IsExpPanelVisible && !_pnlExpText.RectangleToScreen(_pnlExpText.ClientRectangle).Contains(Cursor.Position))
				HideExpPanel();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised when the 'Expand Text' button is clicked.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void BtnExpText_Click(object sender, EventArgs e)
		{
			_btnExpText.Visible = false;
			_pnlExpText.Visible = true;

			string replacement = LineDelimiter == '\n' ? LineDelimiter.ToString() : LineDelimiter + "\n";
			_txtExpText.Text = Regex.Replace(_fullText, "[\t ]*[" + LineDelimiter + ",]+[ \t]*[\n]*", replacement).Trim('\n');
			_txtExpText.Focus();
			_txtExpText.SelectAll();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised when the text of the 'ExpandText' textbox control is changed by the user.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void TxtExpText_TextChanged(object sender, EventArgs e)
		{
			if (((RichTextBox)sender).Focused)
				SetTextUsingExpText();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Handles the event raised by the 'ExpandText' textbox control when it loses focus.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void TxtExpText_LostFocus(object sender, EventArgs e)
		{
			HideExpPanel();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets a value indicating whether the expand panel is visible.
		/// </summary>
		/// <value><c>true</c> if the expand panel is visible; otherwise, <c>false</c>.</value>
		//------------------------------------------------------------------------------------------------------------------------
		private bool IsExpPanelVisible => _pnlExpText != null && _pnlExpText.Visible;

		#endregion

		//..................................................................................................................................

		#region Private Methods

		private void SetTextUsingExpText()
		{
			if (!_pnlExpText.Visible)
				return;

			string txt = Regex.Replace(_txtExpText.Text.Trim(LineDelimiter, '\n'), "[ \t]*\n+[\t ]*", LineDelimiter.ToString());
			txt = Regex.Replace(txt, "[\t ]*[" + LineDelimiter + "]+[\t ]*", LineDelimiter.ToString());
			if (Text != txt)
				Text = txt;
		}

		private void HideExpPanel()
		{
			_pnlExpText.Visible = false;
			_btnExpText.Visible = false;
			Focus();
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the value for the text displayed with embedded ellipsis.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void SetEllipsisText()
		{
			_ellipsisText = (_fullText + " ").Trim(); // Want a new copy to modify, else fullText would be modified, too!
			TextRenderer.MeasureText(_ellipsisText, Font, new Size(Width, Height), TextFormatFlags.ModifyString | _ellipsisTextFormatFlag);
			int idx = _ellipsisText.IndexOf('\0');
			if (idx > -1)
				_ellipsisText = _ellipsisText.Remove(idx);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the expanded textbox control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CreateExpTextBox(Control frm)
		{
			string s = String.Concat(Enumerable.Repeat("Q\n", ExpansionLineCount)).Trim('\n');
			int panelHeight = TextRenderer.MeasureText(s, Font).Height + 4; // Assures the lines are all shown at once.

			_txtExpText = new RichTextBox
			{
				BorderStyle = BorderStyle.None,
				Dock = DockStyle.Fill,
				Font = Font,
				Margin = new Padding(0),
				Multiline = true,
				Name = Name,
				ReadOnly = ReadOnly,
				WordWrap = false
			};
			_txtExpText.LostFocus += TxtExpText_LostFocus;
			_txtExpText.TextChanged += TxtExpText_TextChanged;

			Point locationOnForm = frm.PointToClient(Parent.PointToScreen(Location));
			_pnlExpText = new Panel
			{
				Anchor = Anchor,// AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left,
				BorderStyle = BorderStyle.FixedSingle,
				Location = locationOnForm,
				Padding = new Padding(1),
				Size = new Size(Width, panelHeight),
				Visible = false,
				Width = Width
			};
			_pnlExpText.Controls.Add(_txtExpText);
			_pnlExpText.Controls.SetChildIndex(_txtExpText, 0);
			frm.Controls.Add(_pnlExpText);
			frm.Controls.SetChildIndex(_pnlExpText, 0);
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the Expand button control.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private void CreateExpButton(Control frm)
		{
			int hgt = Height - 2;
			var loc = new Point(Right - Height + 1, Top + 1);
			_btnExpText = new Button
			{
				Anchor = AnchorStyles.Right,
				Cursor = Cursors.Arrow,
				Font = new Font("Small Font", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
				Location = frm.PointToClient(Parent.PointToScreen(loc)),
				Name = "btnExpand" + Name,
				Size = new Size(hgt, hgt),
				//Text = @"…",
				UseVisualStyleBackColor = true,
				Visible = true
				,
				Image = Resources.Exp
			};
			_btnExpText.Click += BtnExpText_Click;
			frm.Controls.Add(_btnExpText);
			frm.Controls.SetChildIndex(_btnExpText, 0);
		}

		#endregion

		//..................................................................................................................................

		#region MyFilter Class

		private sealed class MyFilter : IMessageFilter
		{
			//private const int WM_KEYDOWN = 0x100;
			private const int WM_KEYUP = 0x101;
			private const int WM_LBUTTONDOWN = 0x201;
			//private const int WM_LBUTTONUP = 0x202;

			public delegate void KeyPressUp(IntPtr target);
			// ReSharper disable once EventNeverSubscribedTo.Local
			public event KeyPressUp KeyUp;

			public delegate void LeftButtonDown();
			public event LeftButtonDown MouseDown;

			//public delegate void LeftButtonUp();
			//public event LeftButtonUp MouseUp;

			bool IMessageFilter.PreFilterMessage(ref Message m)
			{
				switch (m.Msg)
				{
					// Raise our KeyUp() event whenever a key is released in our app
					case WM_KEYUP:
						KeyUp?.Invoke(m.HWnd);
						break;
					// Raise our MouseDown() event whenever the mouse is left clicked somewhere in our app
					case WM_LBUTTONDOWN:
						MouseDown?.Invoke();
						break;
						//case WM_LBUTTONUP:
						//    MouseUp?.Invoke();
						//    break;
				}
				return false;
			}
		}

		#endregion

		//..................................................................................................................................

		#region NativeMethods Class 

		private static class NativeMethods
		{
			[DllImport("User32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetCaretPos(out Point lpPoint);
		}

		#endregion
	}

	//..................................................................................................................................

	#region Not Currently Used

	//private void mf_KeyUp(IntPtr target)
	//{
	//   if (PanelVisible && !target.Equals(IntPtr.Zero) && !IsTargetMine(target))
	//      TogglePanel();
	//}
	//
	//private bool IsTargetMine(IntPtr target)
	//{
	//   return IsTargetMine(this, target);
	//}
	//
	//private static bool IsTargetMine(Control ctl, IntPtr target)
	//{
	//   return ctl.Controls.Cast<Control>().Any(child => child.Handle.Equals(target) 
	//      || (child.HasChildren && IsTargetMine(child, target)));
	//}
	//
	//public static class RichTextBoxResizer
	//{
	//   public static void ResizeToContents(this RichTextBox richTextBox, ContentsResizedEventArgs e)
	//   {
	//      richTextBox.Width = e.NewRectangle.Width;
	//      richTextBox.Height = e.NewRectangle.Height;
	//      richTextBox.Width += richTextBox.Margin.Horizontal +
	//          SystemInformation.HorizontalResizeBorderThickness +
	//          SystemInformation.HorizontalScrollBarThumbWidth;
	//      richTextBox.Height += richTextBox.Margin.Vertical +
	//          SystemInformation.VerticalResizeBorderThickness;
	//   }
	//   public static void ResizeToContentsHorizontally(this RichTextBox richTextBox, ContentsResizedEventArgs e)
	//   {
	//      richTextBox.Width = e.NewRectangle.Width;
	//      richTextBox.Width += richTextBox.Margin.Horizontal +
	//          SystemInformation.HorizontalResizeBorderThickness +
	//          SystemInformation.HorizontalScrollBarThumbWidth;
	//   }
	//   public static void ResizeToContentsVertically(this RichTextBox richTextBox, ContentsResizedEventArgs e)
	//   {
	//      richTextBox.Height = e.NewRectangle.Height;
	//      richTextBox.Height += richTextBox.Margin.Vertical + SystemInformation.VerticalResizeBorderThickness;
	//   }
	//}

	#endregion
}