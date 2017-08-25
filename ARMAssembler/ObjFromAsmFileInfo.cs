// File ObjFromAsmFileInfo.cs
//
// Copyright (c) R. Nigel Horspool,  May 2011

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ArmAssembly {

    public class ObjFromAsmFileInfo : ObjFileInfo
    {
        // Instance fields / properties

        public string[] SourceLines { get; private set; }  // content of listing line

        public uint[] Offsets { get; private set; }  // offset in section (0xFFFFFFFF if not present)

        public string[] HexContent { get; private set; }  // content field (null if not present)

        public int[] LineNumbers { get; private set; }  // line numbers in original source file

        public string[] SymTabLines { get; private set; }  // from listing file

        public string AsmFileName { get; private set; }  // original source file name

        // end of instance fields / properties

        public ObjFromAsmFileInfo(string objFileName, string asmFileName,
                    IList<string> source, IList<string> symListing,
                    IDictionary<string, SyEntry> globalSymbols, IDictionary<string, string> externSymbols)
            : base(objFileName, globalSymbols, externSymbols)
        {
            this.AsmFileName = asmFileName;
            transferLines(source);
            SymTabLines = new string[symListing.Count];
            symListing.CopyTo(SymTabLines, 0);
        }

        private void transferLines(IList<string> source)
        {
            SourceLines = new string[source.Count];
            Offsets = new uint[source.Count];
            HexContent = new string[source.Count];
            LineNumbers = new int[source.Count];
            for (int i = 0; i < source.Count; i++) {
                Offsets[i] = 0xFFFFFFFF;
                HexContent[i] = null;
            }
            int lnum = 0;   // index into SourceLines array
            int lineNo = 1; // line number in original source file
            foreach (string s in source) {
                // split the line at the tab character, if there
                int len = s.IndexOf('\t');
                if (len < 0) { // line is empty after offset+contents
                    len = s.Length;
                    if (len > 0 && s[len - 1] == ' ') len--;
                    SourceLines[lnum] = "";
                } else
                    SourceLines[lnum] = s.Substring(len + 1);
                lnum++;
                int ix = 0;
                while (ix < len && Char.IsWhiteSpace(s[ix])) ix++;
                int numStart = ix;
                while (ix < len && Char.IsDigit(s[ix])) ix++;
                int numEnd = ix;  // line number in cols numStart .. numEnd-1
                bool lnumOK = Int32.TryParse(s.Substring(numStart, numEnd - numStart), out lineNo);
                LineNumbers[lnum - 1] = lineNo;
                if (!lnumOK || ix >= len) continue;
                while (ix < len && s[ix] == ' ') ix++;
                if (ix >= len) // no offset, no contents on this line
                    continue;
                // we now have either an offset:contents pair or just the contents
                int offsetStart = ix; // assume an offset is there
                while(ix < len && Char.IsLetterOrDigit(s[ix])) ix++;
                int offsetEnd = ix;  // hex number in cols offsetStart .. offsetEnd-1
                while (ix < len && Char.IsWhiteSpace(s[ix])) ix++;
                if (ix >= len) {  // it was just the contents
                    HexContent[lnum - 1] = reverseByteOrder(s, offsetStart, offsetEnd);
                    continue;
                }
                int hexStart = ix;
                while (ix < len && Char.IsLetterOrDigit(s[ix])) ix++;
                Offsets[lnum - 1] = convertFromHex(s.Substring(offsetStart, offsetEnd - offsetStart));
                HexContent[lnum - 1] = reverseByteOrder(s, hexStart, ix);
            }
        }

        private static string reverseByteOrder(string s, int start, int end)
        {
            if ((end - start) % 2 != 0)
                end--;  // must be even no of hex digits!!
            StringBuilder result = new StringBuilder(end-start);
            int i = end;
            while (i > start) {
                char c1 = s[--i];
                char c2 = s[--i];
                result.Append(c2);
                result.Append(c1);
            }
            return result.ToString();
        }

        private static uint convertFromHex(string s)
        {
            uint result = 0;
            if (uint.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out result))
                return result;
            return 0xFFFFFFFF;
        }
    }

}  // end of namespace ArmAssembly


