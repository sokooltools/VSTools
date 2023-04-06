using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace SokoolTools.VsTools
{
	//----------------------------------------------------------------------------------------------------------------------------
	/// <summary>
	/// This class inherits from <seealso cref="OleMenuCommand" /> and is used to implement a very specific type of command which 
	/// will increment the count each time the user clicks on it. Its text is changed to show the current count.
	/// </summary>
	//----------------------------------------------------------------------------------------------------------------------------
	internal class DynamicTextCommand : OleMenuCommand
	{
		// Counter for the clicks.
		private int _clickCount;

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a new DynamicTextCommand object with a specific CommandID and base text.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public DynamicTextCommand(CommandID id, string text) : base(ClickCallback, id, text)
		{
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// If a command is defined with the TEXTCHANGES flag in the VSCT file and this package is loaded, then Visual Studio 
		/// will call this property to get the text to display.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		public override string Text
		{
			get => String.Format(Resources.DynamicTextFormat, base.Text, _clickCount);
			set => base.Text = value;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// This is the function that is called when the user clicks on the menu command.
		/// It will check that the selected object is actually an instance of this class and increment its click counter.
		/// </summary>
		//------------------------------------------------------------------------------------------------------------------------
		private static void ClickCallback(object sender, EventArgs args)
		{
			if (sender is DynamicTextCommand cmd)
				cmd._clickCount++;
		}
	}
}