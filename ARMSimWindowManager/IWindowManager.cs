/* IWindowManager.cs
 * 
 * The window manager governs the subordinate windows (e.g. code view, registers, etc.) inside the main ARMSim window.
 * A window manager provides a mechanism to create the subordinate windows and display them inside some kind of widget.
 * 
 * 
 * B. Bird - 08/09/2014
 * Modified from original file by Dale Lyons circa 2006.
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace ARMSimWindowManager
{
    public delegate void SaveConfigEventHandler(XmlWriter xmlOut);
    public delegate void LoadConfigEventHandler(XmlReader xmlIn);
    //public delegate void OnRecalLayout(Control sender);


	public delegate void MemoryViewClosedHandler(Control memoryView, int memoryViewIndex);

    public interface IWindowManager
    {
		//TODO remove
		//Returns true if the implementing class considers itself to be a "docking" window style.
        bool IsDockingWindows { get; }


		//TODO document
        void SaveConfigToFile(string settingsFilename);
		void LoadConfigFromFile(string settingsFile);

        //void Create(Form parent, Control toolStrip, Panel panel);
		
		//Create some kind of widget which will contain the subordinate windows.
		//The generated widget will be added to the main ARMSim window.
		Control GenerateManagerControl();

		//Given a Control and a title, return an IContent structure corresponding to a
		//subordinate window containing the Control.
        IContent CreateContent(Control control, string title);

        //void RecalcLayout(Control sender);

		//TODO document
        event SaveConfigEventHandler SaveCustomConfig;
        event LoadConfigEventHandler LoadCustomConfig;


		//Called when a memory view is closed (i.e. permanently killed).
		//The arguments passed to the handler are the same values passed
		//into CreateMemoryView when the view was created.
		event MemoryViewClosedHandler OnMemoryViewClosed;

		//TODO deprecate
		//Create a subordinate window for a memory view.
		//This should be removed soon.
        IContent CreateMemoryView(Control view, int memoryViewIndex);


		//TODO deprecate
        void BringToFront(IContent content);

		//TODO deprecate
		//Mark a given content object as visible or not.
		//This should be superceded by the IContent.IsShowing property...
        void ShowContent(IContent content, bool show);


        //Construct and display the default layout
        void CreateViews(IContent codeView, IContent outputView, IContent watchView, IContent registersView,
                         IContent dataCacheView, IContent instructionCacheView, IContent unifiedCacheView,
                         IContent stackView, IContent pluginUIControlsView);

		//Revert to the default layout (not supported by all window managers).
		//Note that this can only be called after CreateViews.
		void DefaultLayout();
    }
}
