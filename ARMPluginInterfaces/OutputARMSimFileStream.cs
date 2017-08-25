using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This class defines an output ARMSim file stream. Only writes are valid for
    /// this stream.
    /// The stream can optionally have an output delegate set where any writes are
    /// passed to the callback. This allows clients to tap into the output stream.
    /// </summary>
    public class OutputARMSimFileStream : ARMSimStream
    {

        /// <summary>
        /// Property to get the stream object
        /// </summary>
        public StreamWriter Stream { get; set; }

        public override long Length() { return Stream.BaseStream.Length;  }

        /// <summary>
        /// OutputARMSimFileStream ctor. Construct without a stream object
        /// </summary>
        public OutputARMSimFileStream()
        {
            Stream = null;
        }

        public void SetStream(StreamWriter stream)
        {
            Stream = stream;
        }

        /// <summary>
        /// OutputARMSimFileStream ctor. Construct with a stream object
        /// </summary>
        /// <param name="outStream"></param>
        public OutputARMSimFileStream(StreamWriter outStream)
        {
            Stream = outStream;
        }

        /// <summary>
        /// Close the output stream object (if set)
        /// </summary>
        public override void Close()
        {
            if (Stream != null)
                Stream.Close();
            Stream = null;
        }

        /// <summary>
        /// Property to set/get the output delegate
        /// </summary>
        public CharacterOutputEventHandler OnCharOutput { get; set; }

        /// <summary>
        /// Write a character to the output stream object (if set)
        /// Also write character to output delegate(if set)
        /// </summary>
        /// <param name="ch"></param>
        public override void Write(char chr)
        {
            if (Stream != null)
                Stream.Write(chr);
            if (OnCharOutput != null)
                OnCharOutput(chr);
        }

        /// <summary>
        /// Output a string to the output stream and delegate
        /// </summary>
        /// <param name="str"></param>
        public override void Write(string str)
        {
            foreach (char ch in str)
            {
                Write(ch);
            }
        }//Write

    }//class OutputARMSimFileStream
}
