using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This class defines an input ARMSim file stream.
    /// Only reads are valid for this stream.
    /// </summary>
    public class InputARMSimFileStream : ARMSimStream
    {
 
        /// <summary>
        /// Property to set/get the stream object
        /// </summary>
        //public StreamReader Stream { set { _inStream = value; } }
        public StreamReader Stream { get; set; }

        public override long Length() { return Stream.BaseStream.Length; }

        /// <summary>
        /// InputARMSimFileStream ctro without stream object
        /// </summary>
        public InputARMSimFileStream()
        {
        }

        /// <summary>
        /// InputARMSimFileStream ctro with stream object
        /// </summary>
        /// <param name="inStream"></param>
        public InputARMSimFileStream(StreamReader inStream)
        {
            this.Stream = inStream;
        }

        /// <summary>
        /// Peek at the next character to be read. Do not remove it from the read buffer.
        /// Call the base version if the stream object is not set.
        /// </summary>
        /// <returns></returns>
        public override char Peek()
        {
            if (this.Stream == null)
                return base.Peek();

            return (char)this.Stream.Peek();
        }

        /// <summary>
        /// Read the next character to be read from the read buffer.
        /// Call the base version if the stream object is not set.
        /// </summary>
        /// <returns></returns>
        public override int Read()
        {
            if (this.Stream == null)
                return base.Read();

            return (int)this.Stream.Read();
        }

        /// <summary>
        /// Read a line of text from the read buffer.
        /// Call the base version if the stream object is not set.
        /// </summary>
        /// <returns></returns>
        public override string ReadLine()
        {
            if (this.Stream == null)
                return base.ReadLine();

            return this.Stream.ReadLine();
        }

        /// <summary>
        /// Close the input stream object(if set)
        /// </summary>
        public override void Close()
        {
            if (this.Stream != null)
                this.Stream.Close();
            this.Stream = null;
        }//Close

    }//class InputARMSimFileStream
}
