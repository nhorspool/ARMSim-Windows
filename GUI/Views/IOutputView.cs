using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.GUI.Views
{
    /// <summary>
    /// This interface is implemented by the OutputView to give
    /// access to the standard consoles.
    /// </summary>
    public interface IOutputView
    {
        /// <summary>
        /// Sets the wait indicator on the console while waiting for input.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <param name="value">true - on, false - off</param>
        //void SetWaitIndicator(uint handle, bool value);

        /// <summary>
        /// Creates a new standard console. A tab is created in the output view 
        /// with the title set to the one given.
        /// The console is added to the list of standard consoles and a handle
        /// returned to the called.
        /// </summary>
        /// <param name="title">title of the new standard console</param>
        /// <returns>handle of console</returns>
        uint CreateStandardConsole(string title);

        /// <summary>
        /// Close a standard console. Removes it from the tab control and remove from the list
        /// of consoles.
        /// </summary>
        /// <param name="handle">handle of console to close</param>
        void CloseStandardConsole(uint handle);

        /// <summary>
        /// Write a character to a standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <param name="ch">character to write</param>
        void WriteStandardConsole(uint handle, char chr);

        /// <summary>
        /// Read a character from the standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        char ReadStandardConsole(uint handle);

        /// <summary>
        /// Peek at the next character in the specified console.
        /// If none is available, the console waits for a keystroke.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        char PeekStandardConsole(uint handle);
    }//interface IOutputView
}
