using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ARMSim.GUI.Views
{
    /// <summary>
    /// This interface is implemented by all the simulator views.
    /// It allows the simulator user interface to treat all the views
    /// the same way.
    /// </summary>
    public interface IView
    {
        ///<summary>Called when a single step is starting</summary>
        void stepStart();
        ///<summary>Called when a single step has ended</summary>
        void stepEnd();
        ///<summary>Called when it is time to reset the view.</summary>
        void resetView();
        ///<summary>Called when it is time to update the view when the program stops</summary>
        void updateView();

        ///<summary>set/get the font for the view</summary>
        Font CurrentFont { get; set; }
        ///<summary>set/get the text color for the view</summary>
        Color CurrentTextColour { get; set; }
        ///<summary>set/get the background color for the view</summary>
        Color CurrentBackgroundColour { get; set; }
        ///<summary>set/get the highlight color for the view</summary>
        Color CurrentHighlightColour { get; set; }

        ///<summary>Get the name of this view</summary>
        //string ViewName { get; }

        ///<summary>Get a control object of this view</summary>
        Control ViewControl { get; }

        void TerminateInput();

        //event ARMSimWindowManager.OnRecalLayout OnRecalLayout; 

    }//interface IView
}
