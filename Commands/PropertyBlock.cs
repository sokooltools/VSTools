using System;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;

namespace SokoolTools.VsTools
{
	//--------------------------------------------------------------------------------------------------------
	/// <summary>
	/// 
	/// </summary>
	//--------------------------------------------------------------------------------------------------------
	internal static class PropertyBlock
	{
		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses a block of code consisting of multiple lines into a single line.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void Collapse() //CollapseToSingleLine()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Get the selection.
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// Make sure full lines are selected.
			sel.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn, true);
			sel.EndOfLine(true);

			// This should never really happen.
			if (sel == null)
				throw new ApplicationException("No text was selected!");

			// Get the text of the selection for processing.
			string sTxt = sel.Text;

			// Remove the trailing tabs and spaces from each line.
			sTxt = Regex.Replace(sTxt, @"[\t ]+$", "", RegexOptions.Multiline);

			// Remove the leading tabs and spaces from each line.
			sTxt = Regex.Replace(sTxt, @"^[\t ]+", "", RegexOptions.Multiline);

			// Replace linefeeds with a space.
			sTxt = sTxt.Replace(Environment.NewLine, " ");

			// This step will add code brackets around the 'return' or 'continue' statement when there aren't some already there.
			if (!sTxt.Contains("{") && Regex.Matches(sTxt, ";").Count == 1)
			{
				Match m = Regex.Match(sTxt, @"([^{]*)[ \t]*((return|continue)[^;]*;)[ \t]*[^}]*", RegexOptions.Multiline);
				if (m.Success)
					sTxt = m.Groups[1].Value + "{" + m.Groups[2].Value + "}";
			}

			// Get the virtual edit points.
			EditPoint topEditPt = sel.TopPoint.CreateEditPoint();
			EditPoint botEditPt = sel.BottomPoint.CreateEditPoint();

			// Replace the selected text with the modified text.
			topEditPt.ReplaceText(botEditPt, sTxt, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Collapses a property block from multiple lines to a single line of code.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void Collapse2()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			//CodeElementExample();

			MyCodeElement myCodeElement = GetCodeElementAtCursor();
			if (myCodeElement.Path != null)
			{
				Logging.Log(3, $"StartPoint={myCodeElement.StartPoint.AbsoluteCharOffset}, EndPoint={myCodeElement.EndPoint.AbsoluteCharOffset}, Message={myCodeElement.Path}");
			}

			// Get the selection.
			var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// This should never really happen.
			if (sel == null)
				throw new ApplicationException("No text was selected!");

			CodeElement codeProperty = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementProperty];// as CodeProperty;
			if (codeProperty == null)
				throw new ApplicationException("The command requires that the cursor be placed somewhere within a property block!");

			sel.MoveToAbsoluteOffset(codeProperty.StartPoint.AbsoluteCharOffset);
			sel.MoveToAbsoluteOffset(codeProperty.EndPoint.AbsoluteCharOffset, true);

			// Get the text of the selection for processing.
			string sTxt = sel.Text;

			// Remove the leading tabs and spaces from each line.
			sTxt = Regex.Replace(sTxt, @"^[\t ]*", "", RegexOptions.Multiline);
			// Replace linefeeds with a space.
			sTxt = sTxt.Replace(Environment.NewLine, " ");

			// Get the virtual edit points.
			EditPoint topEditPt = sel.TopPoint.CreateEditPoint();
			EditPoint botEditPt = sel.BottomPoint.CreateEditPoint();

			// Replace the selected text with the modified text.
			topEditPt.ReplaceText(botEditPt, sTxt, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Expands a property block from a single line to multiple lines of code.
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		public static void Expand()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Logging.Log(2);

			// Get the selection.

			// This should never really happen.
			if (!(Connect.objDte2.ActiveWindow.Document.Selection is TextSelection sel))
				throw new ApplicationException("No text was selected!");

			// NOTE for this to work requires the document to be part of a loaded project.
			CodeElement codeProperty =
				Connect.objDte2.ActiveDocument.ProjectItem.FileCodeModel.CodeElementFromPoint(sel.ActivePoint, vsCMElement.vsCMElementProperty);

			//CodeElement codeProperty = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementProperty];// as CodeProperty;
			if (codeProperty == null)
				throw new ApplicationException("The command requires that the cursor be placed somewhere within a property block!");

			// _TODO: Need to check for codeProperty.Comment && codeProperty.DocComment here.

			// Select the entire block.
			sel.MoveToAbsoluteOffset(codeProperty.StartPoint.AbsoluteCharOffset);
			sel.MoveToAbsoluteOffset(codeProperty.EndPoint.AbsoluteCharOffset, true);

			// Get the text of the selection for processing.
			string sTxt = sel.Text;

			// Determine if it's an Auto-Property.
			if (Regex.IsMatch(sTxt, @"get[\t ]*;") && Regex.IsMatch(sTxt, @"set[\t ]*;"))
			{
				// Determine its scope.
				string scope = Regex.IsMatch(sTxt, @"^[ \t]*(public|private|internal)[\t ]*static")
					? "private static"
					: "private";
				string variable = "_" + codeProperty.Name.Substring(0, 1).ToLowerInvariant() + codeProperty.Name.Remove(0, 1);
				sTxt = scope + " " + codeProperty.GetType() + " " + variable + ";" + sTxt;
				sTxt = Regex.Replace(sTxt, @"get[\t ]*;", @"get{return " + variable + @";}");
				sTxt = Regex.Replace(sTxt, @"set[\t ]*;", @"set{" + variable + @"=value;}");
			}

			// Perform the meat of this function.
			sTxt = sTxt.Replace("{", "\n{\n");
			sTxt = sTxt.Replace("}", "\n}\n");
			sTxt = sTxt.Replace(";", ";\n");
			sTxt = Regex.Replace(sTxt, "[ \t\n]*\n", "\n", RegexOptions.Multiline);
			if (sTxt.EndsWith("\n"))
				sTxt = sTxt.Remove(sTxt.Length - 1);

			// Get the virtual edit points.
			EditPoint topEditPt = sel.TopPoint.CreateEditPoint();
			EditPoint botEditPt = sel.BottomPoint.CreateEditPoint();

			// Replace the selected text with the modified text.
			topEditPt.ReplaceText(botEditPt, sTxt, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the code element at the cursor.
		/// </summary>
		/// <returns></returns>
		//----------------------------------------------------------------------------------------------------
		private static MyCodeElement GetCodeElementAtCursor()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Get the selection.
            var sel = (TextSelection)Connect.objDte2.ActiveWindow.Document.Selection;

			// Should never happen.
			if (sel == null)
				throw new ApplicationException("No text was selected!");

			var myCodeElement = new MyCodeElement();

			CodeElement cn = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementNamespace];// as CodeNamespace;
			if (cn != null)
			{
				myCodeElement.StartPoint = cn.StartPoint;
				myCodeElement.EndPoint = cn.EndPoint;
				myCodeElement.Path += cn.Name;
			}

			CodeElement cc = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementClass];// as CodeClass;
			if (cc != null)
			{
				myCodeElement.StartPoint = cc.StartPoint;
				myCodeElement.EndPoint = cc.EndPoint;
				myCodeElement.Path += "." + cc.Name;
			}

			CodeElement cf = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementFunction];// as CodeFunction2;
			if (cf != null)
			{
				myCodeElement.StartPoint = cf.StartPoint;
				myCodeElement.EndPoint = cf.EndPoint;
				myCodeElement.Path += "." + cf.Name;
				string sRes = null;
				CodeElementChildren(cf.Children, ref sRes, 0);
				if (sRes != null)
				{
					if (sRes.EndsWith(", "))
						sRes = sRes.Remove(sRes.Length - 2);
					myCodeElement.Path += "(" + sRes + ")";
				}
				else
					myCodeElement.Path += "()";
			}

			CodeElement cp = sel.ActivePoint.CodeElement[vsCMElement.vsCMElementProperty];// as CodeProperty;
			if (cp == null)
				return myCodeElement;

			myCodeElement.StartPoint = cp.StartPoint;
			myCodeElement.EndPoint = cp.EndPoint;
			myCodeElement.Path += "." + cp.Name;
			return myCodeElement;
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Codes the element children.
		/// </summary>
		/// <param name="codeElements">The code elements.</param>
		/// <param name="sRes">The string resource.</param>
		/// <param name="level">The level.</param>
		//----------------------------------------------------------------------------------------------------
		private static void CodeElementChildren(IEnumerable codeElements, ref string sRes, int level)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            level += 1;
			foreach (CodeElement clt in codeElements)
			{
				for (int i = 1; i < level; i++)
					sRes += "\t";
				try
				{
					//sRes += string.Format("{0}, Kind = {1}  Line:{2} ~ {3} ", clt.Name, clt.Kind, clt.StartPoint.Line, clt.EndPoint.Line);
					sRes += clt.Name;
				}
				catch
				{
					if (clt is CodeImport import)
					{
						CodeImport ci = import;
						//sRes += string.Format("{0}, Kind = {1}  Line:{2} ~ {3} ", ci.Namespace, clt.Kind, clt.StartPoint.Line, clt.EndPoint.Line);
						sRes += ci.Namespace;
					}
					else
					{
						//sRes += string.Format("     , Kind = {0}  Line:{1} ~ {2} ", clt.Kind, clt.StartPoint.Line, clt.EndPoint.Line);
						sRes += " ";
					}
				}
				sRes += ", ";
				try
				{
					if (clt.Children != null)
						CodeElementChildren(clt.Children, ref sRes, level);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
		}

		public static void CodeElementExample()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Before this example can be run, a code document must be open in a project
            // and the insertion point placed anywhere inside the source code.
            try
			{
				var sel = (TextSelection)Connect.objDte2.ActiveDocument.Selection;
				TextPoint pnt = sel.ActivePoint;

				// Discover every code element containing the insertion point.
				string elems = "";
				const vsCMElement SCOPES = 0;

				foreach (vsCMElement scope in Enum.GetValues(SCOPES.GetType()))
				{
					CodeElement elem = pnt.CodeElement[scope];

					if (elem != null)
						elems += elem.Name + " (" + scope + ")\n";
				}

				Logging.Log(3, "The following elements contain the insertion point:\n\n" + elems);
			}
			catch (Exception ex)
			{
				Logging.Log(0, ex.Message);
			}
		}

		//----------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		//----------------------------------------------------------------------------------------------------
		private sealed class MyCodeElement
		{
			public TextPoint StartPoint;
			public TextPoint EndPoint;
			public string Path;
		}
	}
}