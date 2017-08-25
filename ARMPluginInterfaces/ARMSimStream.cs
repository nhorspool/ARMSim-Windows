using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This class defines the base behaviour of all the ARMSim streams.
    /// ARMSim streams are used to control stdout/stdin/stderr io, file io and
    /// access to the standard consoles.
    /// </summary>
    public abstract class ARMSimStream
    {
        /// <summary>
        /// need an end of file marker
        /// </summary>
        public const char ctrlD = (char)0x04;

        /// <summary>
        /// all streams must implement a close
        /// </summary>
        public abstract void Close();

        public virtual long Length() { return -1L; }

        /// <summary>
        /// Write a string to the stream. Only required for output stream.
        /// </summary>
        /// <param name="str"></param>
        public virtual void Write(string str)
        {
            //throw new Exception("Write operation not provided");
        }

        /// <summary>
        /// Write a char to the stream. Only required for output stream.
        /// </summary>
        /// <param name="ch"></param>
        public virtual void Write(char chr) { }

        /// <summary>
        /// Peek at the next character in the read buffer. Do not remove. Not needed for output stream.
        /// </summary>
        /// <returns></returns>
        public virtual char Peek() { throw new Exception("Peek operation not provided"); }

        /// <summary>
        /// Read a char from the input stream. Not needed for output streams.
        /// Returns -1 if at end of file, otherwise the character code
        /// </summary>
        /// <returns></returns>
        public virtual int Read() { throw new Exception("Read operation not provided"); }

        /// <summary>
        /// Read a string from the input stream. Not needed for output streams.
        /// </summary>
        /// <returns></returns>
        public virtual string ReadLine()
        {
            StringBuilder s = new StringBuilder(16);
            while(true)
            {
                int c = Read();
                if (c == 8)
                {
                    if(s.Length > 0)
                        s.Remove(s.Length - 1, 1);
                }
                else if (c == ctrlD || c == '\n' || c == '\r' || c < 0)
                {
                    break;
                }
                else
                {
                    s.Append((char)c);
                }
            }
            return s.ToString();
        }

        /// <summary>
        /// A helper function to read characters from a stream and form an integer.
        /// This logic skips whitespace and processes characters until a non-digit is
        /// scanned. It then parses the integer and returns it.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>
        /// Success of the parse
        /// </returns>
        public bool GetInteger(ref int result)
        {
            result = 0;
            StringBuilder s = new StringBuilder(16);
            // try
            {
                int c;
                // skip over leading white space
                for (; ; )
                {
                    c = Read();
                    if (c == ctrlD || c < 0)
                        return true;
                    if (!Char.IsWhiteSpace((char)c))
                        break;
                }
                // process optional sign
                if (c == '-' || c == '+')
                {
                    s.Append((char)c);
                    c = Read();
                    if (c == ctrlD || c < 0)
                        return true;
                }

                while (Char.IsDigit((char)c))
                {
                    s.Append((char)c);
                    c = Read();
                    if (c == ctrlD || c < 0)
                        break;
                }
                return !Int32.TryParse(s.ToString(), out result);
            }
        }//getInteger

        /// <summary>
        /// A helper function to read characters from a stream and form a floating
        /// point number.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>
        /// Success of the parse
        /// </returns>
        public bool GetDouble(ref double result)
        {
            result = 0;
            StringBuilder s = new StringBuilder(32);
            // try
            {
                int c;
                // skip over leading white space
                for (; ; )
                {
                    c = Read();
                    if (c == ctrlD || c < 0)
                        return true;
                    if (!Char.IsWhiteSpace((char)c))
                        break;
                }
                // process optional sign
                if (c == '-' || c == '+')
                {
                    s.Append((char)c);
                    c = Read();
                    if (c == ctrlD || c < 0)
                        return true;
                }
                while (Char.IsDigit((char)c))
                {
                    s.Append((char)c);
                    c = Read();
                    if (c == ctrlD || c < 0)
                        break;
                }
                if (c == 'e' || c == 'E')   // process an exponent
                {
                    s.Append((char)c);
                    c = Read();
                    if (c == ctrlD || c < 0)
                        return true;
                    if (c == '-' || c == '+')
                    {
                        s.Append((char)c);
                        c = Read();
                        if (c == ctrlD || c < 0)
                            return true;
                    }
                    while (Char.IsDigit((char)c))
                    {
                        s.Append((char)c);
                        c = Read();
                        if (c == ctrlD || c < 0)
                            break;
                    }

                }
                return !Double.TryParse(s.ToString(), out result);
            }
        }//getDouble

    }//class ARMSimStream
}
