using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SokoolTools.VsTools
{
	public class Xtra
	{

		//internal void WindowEvents_WindowActivated(EnvDTE.Window gotFocus, EnvDTE.Window lostFocus)
		//{
		//	if (gotFocus.Document == null)
		//		return;
		//	Document curDoc = gotFocus.Document;
		//	Debug.Write("Activated : " + curDoc.FullName);
		//	IVsTextView ivsTextView = GetIVsTextView(curDoc.FullName); // Call the helper method to retrieve IVsTextView object.
		//	if (ivsTextView == null)
		//		return;
		//	IVsTextLines curDocTextLines;
		//	ivsTextView.GetBuffer(out curDocTextLines); //Getting Current Text Lines 
		//	//Getting Buffer Adapter to get ITextBuffer which holds the current Snapshots as well.
		//	Microsoft.VisualStudio.Text.ITextBuffer curDocTextBuffer = AdaptersFactory.GetDocumentBuffer(curDocTextLines as IVsTextBuffer);
		//	Debug.Write("\r\nContentType: " + curDocTextBuffer.ContentType.TypeName + "\nTest: " + curDocTextBuffer.CurrentSnapshot.GetText());
		//}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns an IVsTextView for the given file path, if the given file is open in Visual Studio.
		/// </summary>
		/// <param name="filePath">Full Path of the file you are looking for.</param>
		/// <returns>The IVsTextView for this file, if it is open, null otherwise.</returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static IVsTextView GetIVsTextView(string filePath)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            object dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
			var sp = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte2;
			var serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(sp);

			Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy uiHierarchy;
			uint itemID;
			Microsoft.VisualStudio.Shell.Interop.IVsWindowFrame windowFrame;
			//Microsoft.VisualStudio.Text.Editor.IWpfTextView wpfTextView = null;
			return Microsoft.VisualStudio.Shell.VsShellUtilities.IsDocumentOpen(serviceProvider, filePath, Guid.Empty,
				out uiHierarchy, out itemID, out windowFrame)
				? Microsoft.VisualStudio.Shell.VsShellUtilities.GetTextView(windowFrame) : null;
		}
	}

	public static class Class2
	{
		public static CodeElement GetCodeElementAtCursor()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            CodeElement objCodeElement = null;
			try
			{
				TextPoint objCursorTextPoint = GetCursorTextPoint();

				if (objCursorTextPoint != null)
				{
					// Get the class at the cursor
					objCodeElement = GetCodeElementAtTextPoint(vsCMElement.vsCMElementClass,
						Connect.ApplicationObject.ActiveDocument.ProjectItem.FileCodeModel.CodeElements, objCursorTextPoint);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			//if (objCodeElement == null)
			//	MessageBox.Show(@"No class found at the cursor!");
			//else
			//	MessageBox.Show(@"Class at the cursor: " + objCodeElement.FullName);

			return objCodeElement;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the virtual text point located at the cursor.
		/// </summary>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static TextPoint GetCursorTextPoint()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextPoint objCursorTextPoint = null;

			try
			{
				var objTextDocument = Connect.ApplicationObject.ActiveDocument as TextDocument;
				if (objTextDocument != null)
					objCursorTextPoint = objTextDocument.Selection.ActivePoint;
			}
			catch (Exception)
			{
			}
			return objCursorTextPoint;
		}

		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the code element at the text point.
		/// </summary>
		/// <param name="eRequestedCodeElementKind">Kind of requested code element coming from an enum.</param>
		/// <param name="colCodeElements">The code elements collection.</param>
		/// <param name="objTextPoint">The object text point.</param>
		/// <returns></returns>
		//------------------------------------------------------------------------------------------------------------------------
		private static CodeElement GetCodeElementAtTextPoint(vsCMElement eRequestedCodeElementKind, IEnumerable colCodeElements, TextPoint objTextPoint)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            CodeElement objResultCodeElement = null;

			if (colCodeElements == null)
				return null;

			foreach (CodeElement objCodeElement in colCodeElements)
			{
				if (objCodeElement.StartPoint.GreaterThan(objTextPoint))
				{
					// The code element starts beyond the point
				}
				else if (objCodeElement.EndPoint.LessThan(objTextPoint))
				{
					// The code element ends before the point
					// The code element contains the point
				}
				else
				{
					if (objCodeElement.Kind == eRequestedCodeElementKind)
					{
						// Found
						objResultCodeElement = objCodeElement;
					}

					// We enter in recursion, just in case there is an inner code element that also 
					// satisfies the conditions, for example, if we are searching a namespace or a class
					CodeElements colCodeElementMembers = GetCodeElementMembers(objCodeElement);

					CodeElement objMemberCodeElement = GetCodeElementAtTextPoint(eRequestedCodeElementKind, colCodeElementMembers, objTextPoint);

					if (objMemberCodeElement != null)
					{
						// A nested code element also satisfies the conditions
						objResultCodeElement = objMemberCodeElement;
					}

					break; // TODO: might not be correct. Was : Exit For
				}
			}
			return objResultCodeElement;
		}

		private static CodeElements GetCodeElementMembers(CodeElement objCodeElement)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            CodeElements colCodeElements = null;
			try
			{
				var codeType = objCodeElement as CodeType;
				if (codeType != null)
					colCodeElements = codeType.Members;
				else
				{
					var codeFunction = objCodeElement as CodeFunction;
					if (codeFunction != null)
						colCodeElements = codeFunction.Parameters;
				}
			}
			catch (Exception)
			{
			}
			return colCodeElements;
		}
	}

	internal class Class3
	{
		public void Test1()
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Container for results
            var classes = new List<string>();
			var functions = new List<string>();

			// Get selected projects from solution explorer
			var projects = (Array)Connect.ApplicationObject.ActiveSolutionProjects;

			// Get all ProjectItems inside of the selected Projects
			IEnumerable<ProjectItem> projectItems = projects
				.OfType<Project>()
				.Where (
                                                        p => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); 
                                                        return p.ProjectItems != null; }
                ).SelectMany
                (p => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); 
                                                        return p.ProjectItems.OfType<ProjectItem>()
                                                               .Where(pi => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                                                        return pi.FileCodeModel != null; }); }
                );

			// Iterate over all of these ProjectItems 
			foreach (ProjectItem projectItem in projectItems)
			{
				// Get all of the CodeElements (Interfaces and Classes) inside of the current ProjectItem (recursively)
				IEnumerable<CodeElement> elements = projectItem.FileCodeModel.CodeElements
					.OfType<CodeElement>()
					.SelectMany(GetCodeElements).ToList();

				// Do something with the CodeElements that were found
				classes.AddRange(elements.Where(el => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return el.Kind == vsCMElement.vsCMElementClass; }).Select(el => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return el.Name; }));
				functions.AddRange(elements.Where(el => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return el.Kind == vsCMElement.vsCMElementFunction; }).Select(el => { Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread(); return el.Name; }));
			}
			foreach (string c in classes)
				Debug.WriteLine(c);
			foreach (string i in functions)
				Debug.WriteLine(i);
		}

		// Possible implementation of GetCodeElements:
		private static IEnumerable<CodeElement> GetCodeElements(CodeElement root)
		{
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var result = new List<CodeElement>();
			if (root == null)
				return result;

			// If the current CodeElement is an Interface or a class, add it to the results
			if (root.Kind == vsCMElement.vsCMElementClass || root.Kind == vsCMElement.vsCMElementFunction)
				result.Add(root);

			// Check children recursively
			if (root.Children == null || root.Children.Count == 0)
				return result;

			foreach (CodeElement item in root.Children.OfType<CodeElement>())
				result.AddRange(GetCodeElements(item));
			return result;
		}
	}
}
