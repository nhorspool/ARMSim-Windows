/* DockManagerControl.cs
 * 
 * A wrapper for the parent area of the docked windows.
 * 
 * 
 * B. Bird - 08/09/2014
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

using WeifenLuo.WinFormsUI.Docking;

namespace DockingWindows2
{

	public class DockManagerControl : WeifenLuo.WinFormsUI.Docking.DockPanel
	{

		//A rather heavy-handed hack to save layout data temporarily
		//(by dumping it to XML and saving it into a byte array...)
		public class SavedLayout
		{
			internal byte[] layoutData;
			internal SavedLayout(byte[] layoutData)
			{
				this.layoutData = layoutData;
			}

			public static SavedLayout LoadFromXML(XmlReader xmlReader)
			{
                using (MemoryStream mstream = new MemoryStream())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(mstream, new XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true }))
                    {
                        xmlWriter.WriteNode(xmlReader, false);
                        xmlWriter.Close();
                    }
                    return new SavedLayout(mstream.ToArray());
                }
			}
			public void SaveToXML(XmlWriter xmlWriter)
			{
                using (XmlReader mx = XmlReader.Create(new MemoryStream(layoutData)))
                {
                    xmlWriter.WriteNode(mx, false);
                }
			}
		}


		DockContentWrapper RegistersView;
		DockContentWrapper OutputView;
		DockContentWrapper WatchView;
		DockContentWrapper DataCacheView;
		DockContentWrapper InstructionCacheView;
		DockContentWrapper UnifiedCacheView;
		DockContentWrapper StackView;
		DockContentWrapper PluginUIControlsView;
		DockContentWrapper CodeView;


		Dictionary<String,DockContentWrapper> allViews;
		ICollection<MemoryViewWrapper> AllMemoryViews
		{
			get {
				List<MemoryViewWrapper> result = new List<MemoryViewWrapper>();
				foreach(DockContentWrapper c in allViews.Values){
					if (c is MemoryViewWrapper)
						result.Add((MemoryViewWrapper)c);
				}
				return result;
			}

		}

		public DockManagerControl()
		{
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DockBackColor = System.Drawing.SystemColors.AppWorkspace;
			this.DockBottomPortion = 150D;
			this.DockLeftPortion = 200D;
			this.DockRightPortion = 200D;
			this.DockTopPortion = 150D;
			this.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
			this.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "DockManagerControl";
			this.RightToLeftLayout = true;
			this.Size = new System.Drawing.Size(600, 600);

			allViews = new Dictionary<string, DockContentWrapper>();
 
		}

		private void EnrollView(DockContentWrapper view, String PersistString)
		{
			view.PersistString = PersistString;
			allViews[PersistString] = view;
		}
		private void UnenrollView(DockContentWrapper view)
		{
			allViews.Remove(view.PersistString);
		}
		private DockContentWrapper GetViewByName(string persistString)
		{
			if (allViews.ContainsKey(persistString))
				return allViews[persistString];
			return null;
		}

		//Removes all child windows from the dock panel (but does not close or hide them, so
		//they can later be added back if needed).
		public void ClearContents()
		{
			List<IDockContent> toRemove = new List<IDockContent>(this.Contents);
			foreach (IDockContent d in toRemove)
			{
				d.DockHandler.DockPanel = null;
			}
            toRemove.Clear();
		}

		public void RegisterViews(DockContentWrapper codeView, DockContentWrapper outputView, DockContentWrapper watchView, DockContentWrapper registersView,
								DockContentWrapper dataCacheView, DockContentWrapper instructionCacheView, DockContentWrapper unifiedCacheView,
								DockContentWrapper stackView, DockContentWrapper pluginUIControlsView)
		{
			//TODO make named constants for the strings below.
			EnrollView(registersView,"RegistersView");
			EnrollView(outputView,"OutputView");
			EnrollView(watchView,"WatchView");
			EnrollView(dataCacheView,"DataCacheView");
			EnrollView(instructionCacheView,"InstructionCacheView");
			EnrollView(unifiedCacheView,"UnifiedCacheView");
			EnrollView(stackView,"StackView");
			EnrollView(pluginUIControlsView,"PluginUIControlsView");
			EnrollView(codeView,"CodeView");

			this.RegistersView = registersView;
			this.OutputView = outputView;
			this.WatchView = watchView;
			this.DataCacheView = dataCacheView;
			this.InstructionCacheView = instructionCacheView;
			this.UnifiedCacheView = unifiedCacheView;
			this.StackView = stackView;
			this.PluginUIControlsView = pluginUIControlsView;
			this.CodeView = codeView;
		}

		public void DefaultLayout()
		{
			//The default layout doesn't have any docked memory views, but the process of clearing the layout will detach
			//any memory views found from the docking system. It is important that no zombie memory views exist (i.e. every
			//memory view should either be visible, docked and hidden, or killed). If there are any memory views in our heirarchy,
			//we will add them to the lower pane after the watch view.

			ClearContents();

			//Arranging the subwindows cannot be done consistently between the different docking window styles.
			if (this.DocumentStyle == DocumentStyle.SystemMdi)
			{
				//If the layout is "SystemMdi", the docking system is completely bypassed, and the resulting layout is pretty bad.
				Form ParentForm = (Form)this.Parent;
				ParentForm.IsMdiContainer = true;
				RegistersView.MdiParent = OutputView.MdiParent = WatchView.MdiParent = DataCacheView.MdiParent =
						InstructionCacheView.MdiParent = UnifiedCacheView.MdiParent = StackView.MdiParent =
						PluginUIControlsView.MdiParent = CodeView.MdiParent = (Form)this.Parent;
				RegistersView.Show();
				OutputView.Show();
				WatchView.Show();
				DataCacheView.Show();
				InstructionCacheView.Show();
				UnifiedCacheView.Show();
				StackView.Show();
				PluginUIControlsView.Show();
				CodeView.Show();
			}
			else
			{
				//If the layout is "DockingMdi", "DockingSdi" or "DockingWindow", we can construct the classic ARMSim interface.
				if (DocumentStyle == DocumentStyle.DockingMdi)
				{
					Form ParentForm = (Form)this.Parent;
					ParentForm.IsMdiContainer = true;
				}
				//Put registers on the left
				RegistersView.Show(this, DockState.DockLeft);
				CodeView.Show(this, DockState.Document);

				//Put output and watch views in a tabbed pane on the bottom.
				OutputView.Show(CodeView.Pane, DockAlignment.Bottom, 0.25);
				WatchView.Show(OutputView.Pane, null);

				//Add any memory views after the watch view (sorted by index)
				ICollection<MemoryViewWrapper> allMV = AllMemoryViews;
				if (allMV.Count > 0)
				{
					MemoryViewWrapper[] SortedMV = allMV.ToArray();
					Array.Sort(SortedMV,delegate(MemoryViewWrapper x, MemoryViewWrapper y) { return x.Index - y.Index; });
					foreach (MemoryViewWrapper mv in SortedMV)
					{
						mv.Show(OutputView.Pane, null);
					}
				}

				OutputView.Pane.ActiveContent = OutputView;


				//Put the PluginUI, followed by the three cache displays, in a tabbed
				//pane at the top of the code window and hide all three of them.
				//(They can be enabled manually through the menus).


				PluginUIControlsView.Show(CodeView.Pane, DockAlignment.Top, 0.25);

				//Put the three cache views on the top and hide.
				DataCacheView.Show(PluginUIControlsView.Pane, null);
				InstructionCacheView.Show(PluginUIControlsView.Pane, null);
				UnifiedCacheView.Show(PluginUIControlsView.Pane, null);

				PluginUIControlsView.Hide();
				DataCacheView.Hide();
				InstructionCacheView.Hide();
				UnifiedCacheView.Hide();

				//Put the stack view in a hidden window on the right
				StackView.Show(this, DockState.DockRightAutoHide);

			}
		}

		public void AddMemoryView(MemoryViewWrapper view, bool display)
		{
			//TODO use a named constant for the string below
			string viewName = "MemoryView" + ":" + view.Index;
			EnrollView(view,viewName);

			view.FormClosed += delegate (object sender, FormClosedEventArgs e){
				UnenrollView(view);
			};

			if (!display)
				return;

			//Arranging the subwindows cannot be done consistently between the different docking window styles.
			if (this.DocumentStyle == DocumentStyle.SystemMdi)
			{
				//If the layout is "SystemMdi", the docking system is completely bypassed, and the resulting layout is pretty bad.
				view.Parent = (Form)this.Parent;
			}
			else
			{
				//The Docking Window library uses a single default size for all floating windows, and the only orthodox
				//way to set the window size is to specify a full set of bounds (including screen position). I rather like
				//the default screen position assigned to new windows (which causes them to cascade), so the below code is
				//a workaround which saves the current size (which is the original size of the memory view), then restores
				//it once the window is created.

				//In theory, the view.Pane.FloatWindow object should always be part of the API and non-null. 
				//If a future update of the library causes some kind of problem with this code in the future, 
				//that line can be removed without breaking the rest of the code.
				Size s = view.Size;
				view.Show(this, DockState.Float);
				view.Pane.FloatWindow.Size = view.Size = s;
				
			}
		}

		public void LoadLayout(SavedLayout layout)
		{
			ClearContents();
            using (MemoryStream mstream = new MemoryStream(layout.layoutData))
            {
                LoadFromXml(mstream, new DeserializeDockContent(GetViewByName));
            }
		}

		public SavedLayout SaveLayout()
		{
            using (MemoryStream mstream = new MemoryStream())
            {
                SaveAsXml(mstream, Encoding.Unicode, true);
                mstream.Seek(0, SeekOrigin.Begin);
                return new SavedLayout(mstream.ToArray());
            }
		}

	}
}
