// File AssemblerErrors.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - Oct 2007.

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmAssembly
{
    public sealed class AssemblerErrors
    {
        private static AssemblerErrorsArray _compilerErrors;

        // Prevent class from being instantiated
        private AssemblerErrors() { }

        public static AssemblerErrorsArray ErrorReports {
            get { return _compilerErrors; }
        }

        static public void Initialize()
        {
            _compilerErrors = new AssemblerErrorsArray();
        }

        static public void AddError(string fileName, int lineNum, int colNum,
            string fmt, params Object[] args)
        {
            AddError(fileName, lineNum, colNum, string.Format(fmt, args));
        }

        static public void AddError(string fileName, int lineNum, int colNum, string msg)
        {
            _compilerErrors.AddError(fileName, lineNum, msg);
            if (colNum > 0)
                Console.WriteLine("line {0}, col {1}: {2}", lineNum, colNum, msg);
            else
                Console.WriteLine("line {0}: {1}", lineNum, msg);

        }

        static public void AddError(string fileName, string msg)
        {
            _compilerErrors.AddError(fileName, 0, msg);
            Console.WriteLine("{0}", msg);
        }

    }


    public class AssemblerErrorsArray
    {
        private IDictionary<string, IList<ErrorReport>> errorLists;
        IList<ErrorReport> theList;  // the messages for the current file
        // int lastErrorIx = -1;        // index of last error message in current file
        // string lastFileName = null;  // source file with last error message
        // int lastLineNum = -1;        // line number of last error message

        public AssemblerErrorsArray()
        {
            errorLists = new Dictionary<string, IList<ErrorReport>>();
        }

        public IDictionary<string, IList<ErrorReport>> ErrorLists
        {
            get { return errorLists; }
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (IList<ErrorReport> ht in errorLists.Values)
                {
                    count += ht.Count;
                }
                return count;
            }
        }

        public IList<ErrorReport> GetErrorsList(string fileName)
        {
            IList<ErrorReport> result;
            if (!errorLists.TryGetValue(fileName, out result))
                result = errorLists[fileName] = new List<ErrorReport>();
            return result;
        }

        public void AddError(string fileName, int line, int column, string str)
        {
            if (!errorLists.TryGetValue(fileName, out theList))
                errorLists[fileName] = theList = new List<ErrorReport>();
            ErrorReport msg = new ErrorReport(line, column, str);
            //lastFileName = fileName;
            //lastLineNum = line;
            int ix = theList.Count;
            if (ix > 0 && theList[ix - 1].Line < line)
            {
                // insert new message at correct position to maintain sorted order
                ix = 0;
                foreach (ErrorReport ce in theList)
                {
                    if (ce.Line > line) break;
                    ix++;
                }
                theList.Insert(ix, msg);
                //lastErrorIx = ix;
            }
            else
            {
                //lastErrorIx = theList.Count;
                theList.Add(msg);
            }
        }

        public void AddError(string fileName, int line, string str)
        {
            AddError(fileName, line, 0, str);
        }
    }


    public class ErrorReport : ARMPluginInterfaces.IErrorReport
    {
        private readonly int mLine;
        private readonly int mCol;
        private readonly string mError;
        public ErrorReport(int line, int col, string error)
        {
            mLine = line;
            mCol = col;
            mError = error;
        }
        public int Line { get { return mLine; } }
        public int Col { get { return mCol; } }
        public string ErrorMsg { get { return mError; } }
    }

    [Serializable]
    public class AsmException : Exception
    {
        public AsmException() { }

        public AsmException(String msg, params Object[] args)
            :
            base("Internal error while processing ARM code:\n\t" +
                String.Format(msg, args)) { }
    }

    [Serializable]
    public class ARMSimException : Exception
    {
        public ARMSimException() { }

        public ARMSimException(String msg, params Object[] args)
            :
            base(String.Format(msg, args)) { }
    }
} // end of namespace