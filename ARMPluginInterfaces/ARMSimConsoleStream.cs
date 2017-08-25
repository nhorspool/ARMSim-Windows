using System;
using System.Collections.Generic;
using System.Text;

//using System.IO;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This class defines a standard console stream. They are created at runtime.
    /// The simulator creates the first one that hosts the stdout,stderr,stdin files.
    /// More can be created by the program running.
    /// Reads and Writes are valid for this stream.
    /// </summary>
    public class ARMSimConsoleStream : ARMSimStream
    {
        private uint _handle;
        private IARMHost _ihost;

        /// <summary>
        /// ARMSimConsoleStream ctor
        /// </summary>
        /// <param name="handle">handle of this console</param>
        /// <param name="ihost">a reference back to the simulator</param>
        public ARMSimConsoleStream(uint handle, IARMHost ihost)
        {
            _handle = handle;
            _ihost = ihost;
        }

        /// <summary>
        /// The Peek method of the console. We need to wait for the user to type something.
        /// We also need to process application events and abort if the simulator is stopping.
        /// </summary>
        /// <returns></returns>
        public override char Peek()
        {
            return _ihost.PeekStandardConsole(_handle);
        }//Peek

        /// <summary>
        /// Close the standard console.
        /// </summary>
        public override void Close()
        {
            _ihost.CloseStandardConsole(_handle);
        }//Close

        /// <summary>
        /// Read a character from the console. May block.
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            return _ihost.ReadStandardConsole(_handle);
        }//Read

        ///// <summary>
        ///// Read a line of text from the console. May block
        ///// </summary>
        ///// <returns></returns>
        //public override string ReadLine()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (; ; )
        //    {
        //        char ch = Read();
        //        if ((int)ch == ctrlD || ch == (char)System.Windows.Forms.Keys.Enter)
        //            return sb.ToString();

        //        sb.Append(ch);
        //    }//for
        //}//ReadLine

        /// <summary>
        /// Write a char to the console
        /// </summary>
        /// <param name="ch">char to write</param>
        public override void Write(char chr)
        {
            _ihost.WriteStandardConsole(_handle, chr);
        }//Write


        /// <summary>
        /// Write a string to the console
        /// </summary>
        /// <param name="str">string to write</param>
        public override void Write(string str)
        {
            foreach (char ch in str)
            {
                _ihost.WriteStandardConsole(_handle, ch);
            }
        }//Write

    }//class ARMSimConsoleStream
}