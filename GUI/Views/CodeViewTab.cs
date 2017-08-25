using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.IO;
using ARMSim.Simulator;

namespace ARMSim.GUI.Views
{
    public partial class CodeViewTab : TabPage
    {
        /// <summary>
        /// This class holds the drawing parameters that affect the size of the drawn string.
        /// It is needed so that we may correctly estimate the size of all the strings of the
        /// loaded file and correctly set the horizontal scroll bar.
        /// </summary>
        public class DrawParameters
        {
            public Font CurrentFont { set; get; }    //font used for drawing strings

            //StringFormats hold the tab information. One for normal display, one for errors
            public StringFormat NormalStringFormat { get; private set; }
            public StringFormat ErrorStringFormat { get; private set; }

            //Construct using current font, build StringFormats
            public DrawParameters(Graphics g, Font font)
            {
                CurrentFont = font;
                computeParameters(g);
            }

            //Compute the StringFormats using the current font.
            //Tab stops are set according to the average width of a character
            public void computeParameters(Graphics g)
            {
                int charWidth = (g.MeasureString("0123456789abcdef", CurrentFont)).ToSize().Width / 16;
                int preambleWidth = charWidth * 21;
                int followingWidth = charWidth * 8;

                // Set the tab stops, paint the text specified by myString, and draw the
                // rectangle that encloses the text.
                float[] normalTabStops = new float[] { preambleWidth, followingWidth, followingWidth, followingWidth, followingWidth, followingWidth };
                NormalStringFormat = new StringFormat();
                NormalStringFormat.SetTabStops(0.0f, normalTabStops);

                float[] errorTabStops = new float[] { followingWidth, followingWidth, followingWidth, followingWidth, followingWidth, followingWidth };
                ErrorStringFormat = new StringFormat();
                ErrorStringFormat.SetTabStops(0.0f, errorTabStops);
            }
        }

        // A CodeViewLine instance represents one line of text in the CodeViewList.
        // This is an abstract class that is the base for normal text (with opcodes
        // and labels etc) and error text (assembler errors).
        // A CodeViewList will contain either all normal lines or all error lines.
        // Error lines are of two types -- a listing of a source line or the text
        // of an error message.
        // Normal lines are of two types -- a listing of a source line or the
        // disassembled code from an object file.
        public abstract class CodeViewLine
        {
            protected string Text{ get; set; }
            protected DrawParameters _drawParameters { get; set; }
            protected CodeViewTab _parent { get; set; }

            protected CodeViewLine(DrawParameters drawParameters, string text, CodeViewTab parent)
            {
                _drawParameters = drawParameters;
                Text = text;
                _parent = parent;
            }

            public abstract void Draw(bool selected, DrawItemEventArgs e, Color currentHighlightColour);
            public abstract int CalculateWidth(Graphics g);

        }//class codeViewLine

        protected class ErrorLine : CodeViewLine
        {
            int mLine;

            public ErrorLine(DrawParameters drawParameters, string text, int line, CodeViewTab parent)
                :
                base(drawParameters, text, parent)
            {
                mLine = line;
            }

            public override int CalculateWidth(Graphics g)
            {
                int strLen = (int)g.MeasureString(this.ToString(), _drawParameters.CurrentFont, Int32.MaxValue, _drawParameters.NormalStringFormat).Width;
                strLen += _drawParameters.CurrentFont.Height + 2;
                return strLen;
            }

            public override void Draw(bool selected, DrawItemEventArgs e, Color currentHighlightColour)
            {
                if (selected)
                    using (SolidBrush b = new SolidBrush(currentHighlightColour))
                    {
                        e.Graphics.FillRectangle(b, e.Bounds);
                    }
                else
                    e.DrawBackground();
                int cWidth = e.Bounds.Height - 1;
                Rectangle rect = new Rectangle(new Point(e.Bounds.X + cWidth + 1, e.Bounds.Y), new Size(e.Bounds.Width - cWidth - 1, e.Bounds.Height));
                using (SolidBrush b = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(this.ToString(), _drawParameters.CurrentFont, b, rect, _drawParameters.NormalStringFormat);
                }
            }

            public override string ToString()
            {
                return string.Format("{0,3} {1}", mLine, Text);
            }

        }//class errorLine

        public class ErrorMsgLine : CodeViewLine
        {
            //private readonly int col;
            public ErrorMsgLine(DrawParameters drawParameters, string errorMsg, CodeViewTab parent)
                : base(drawParameters, errorMsg, parent)
            {
                //this.col = col;
            }

            public override int CalculateWidth(Graphics g)
            {
                int strLen = (int)g.MeasureString(this.ToString(), _drawParameters.CurrentFont, Int32.MaxValue, _drawParameters.ErrorStringFormat).Width + 1;
                return strLen;
            }

            public override void Draw(bool selected, DrawItemEventArgs e, Color currentHighlightColour)
            {
                e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                e.Graphics.DrawString(this.ToString(), e.Font, Brushes.Black, e.Bounds, _drawParameters.ErrorStringFormat);
            }

            public override string ToString()
            {
                return "        " + Text;
            }

        }//class errorMsgLine

        public class ListLine : CodeViewLine
        {
            public uint Address { get; private set; }

            private bool addressValid = false;
            private int opcodeBytes = 0;  // 4 = ARM instuction, 2 = Thumb instruction, 0 = neither
            private string hexContents = null; // hex contents of line

            public ListLine(DrawParameters drawParameters, string text, CodeViewTab parent)
                : base(drawParameters, text, parent) { }

            public ListLine(DrawParameters drawParameters, string text, uint address, CodeViewTab parent)
                : base(drawParameters, text, parent)
            {
                Address = address;
                addressValid = true;
            }//ctor lstLine

            public ListLine(DrawParameters drawParameters, string text, string hexContents, CodeViewTab parent)
                : base(drawParameters, text, parent)
            {
                this.hexContents = hexContents;
                addressValid = false;
            }//ctor lstLine for ARM instruction

            public ListLine(DrawParameters drawParameters, string text, uint address, string hexContents, CodeViewTab parent)
                : base(drawParameters, text, parent)
            {
                Address = address;
                this.hexContents = hexContents;
                if (hexContents.Length == 8)
                    opcodeBytes = 4;
                else if (hexContents.Length == 4)
                    opcodeBytes = 2;
                addressValid = true;
            }//ctor lstLine for ARM instruction

            public bool BreakPoint
            {
                get { return _parent.JM.Breakpoints.AtBreakpoint(Address); }
            }

            public bool Valid
            {
                get { return addressValid && opcodeBytes == 4; }
            }

            public override int CalculateWidth(Graphics g)
            {
                int strLen = (int)g.MeasureString(this.ToString(),
                    _drawParameters.CurrentFont, Int32.MaxValue,
                    _drawParameters.NormalStringFormat).Width;
                strLen += _drawParameters.CurrentFont.Height + 2;
                return strLen;
            }

            public override void Draw(bool selected, DrawItemEventArgs e, Color currentHighlightColour)
            {
                if (selected)
                    e.Graphics.FillRectangle(new SolidBrush(currentHighlightColour), e.Bounds);
                else
                    e.DrawBackground();

                int cWidth = e.Bounds.Height - 1;
                if (this.Valid && this.BreakPoint)
                    e.Graphics.FillEllipse(Brushes.Red,
                        new Rectangle(new Point(e.Bounds.X, e.Bounds.Y),
                        new Size(cWidth, e.Bounds.Height)));
                Rectangle rect = new Rectangle(
                    new Point(e.Bounds.X + cWidth + 1, e.Bounds.Y),
                    new Size(e.Bounds.Width - cWidth - 1, e.Bounds.Height));
                using (SolidBrush b = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(this.ToString(),
                        _drawParameters.CurrentFont, b,
                        rect, _drawParameters.NormalStringFormat);
                }
            }

            public override string ToString()
            {
                StringBuilder str = new StringBuilder();
                if (addressValid)
                    str.Append(Address.ToString("X8"));
                else
                    str.Append("        ");
                if (hexContents != null)
                {
                    str.Append(":");
                    str.Append(hexContents);
                }
                str.Append("\t" + Text);
                return str.ToString();
            }//ToString

        }//class ListLine

        protected class ObjectCodeLine : ListLine
        {
            public ObjectCodeLine(DrawParameters drawParameters, uint address, uint hexContents, string contents, CodeViewTab parent)
                : base(drawParameters, contents, address, hexContents.ToString("X8"), parent)
            {
            }//ctor ObjectCodeLine

            public ObjectCodeLine(DrawParameters drawParameters, uint address, string op, string operands, CodeViewTab parent)
                : base(drawParameters, String.Format("    {0,-6} {1}", op, operands), address, parent)
            {
            }//ctor ObjectCodeLine

        }//class ObjectCodeLine

        private CodeViewList _codeViewList;

        //colour of the highlight line
        private Color _currentHighlightColour = Color.Blue;

        //callback to the main forms character output handler to OutputView
        private CharacterOutputDelegate _characterOutputHandler;

        //flag indicating if the file being displayed has changed.
        private bool _fileChanged;

        //info about the assembled program
        private ArmAssembly.ArmFileInfo _PI;

        //current drawing parameters for text in CodeViewList
        private DrawParameters _drawParameters;

        public CodeViewTab(ArmAssembly.ArmFileInfo progInfo, ApplicationJimulator jm)
        {
            JM = jm;
            _PI = progInfo;
            string fileName = progInfo.FileName;

            InitializeComponent();

            this.SuspendLayout();
            this.Location = new System.Drawing.Point(4, 22);
            this.Name = fileName;
            this.Size = new System.Drawing.Size(464, 312);
            this.TabIndex = 0;
            this.Text = Path.GetFileName(fileName);

            _codeViewList = new CodeViewList(this);
            _codeViewList.DrawItem += this.drawItem;
            _codeViewList.VisibleChanged += this.createLines;
            this.Controls.Add(_codeViewList);

            this.fileSystemWatcher1.Path = Path.GetDirectoryName(fileName);
            this.fileSystemWatcher1.Filter = Path.GetFileName(fileName);
            //_fileChanged = false;
            this.ResumeLayout(false);

            _drawParameters = new DrawParameters(_codeViewList.CreateGraphics(), _codeViewList.Font);

        }

        public ApplicationJimulator JM { get; private set; }

        public bool Errors { get; private set; }

        //Property for the character output callback
        public CharacterOutputDelegate CharOutputHandler
        {
            get { return _characterOutputHandler; }
            set { _characterOutputHandler += value; }
        }

        public void initErrors()
        {
            _codeViewList.SuspendLayout();
            _codeViewList.Items.Clear();
            _codeViewList.ResumeLayout();

            //_assembledProgram = null;
            Errors = true;

            if (!this.Name.EndsWith(".s") && !this.Name.EndsWith(".S")) return;
            if (_PI.SourceLine == null) return;

            // createLines();
            _codeViewList.SuspendLayout();

            IList<ArmAssembly.ErrorReport> errList = JM.ErrorReports.GetErrorsList(this.Name);
            errList.Add(new ArmAssembly.ErrorReport(Int32.MaxValue, 0, null));
            IEnumerator<ArmAssembly.ErrorReport> errors = (IEnumerator<ArmAssembly.ErrorReport>)errList.GetEnumerator();
            int errLineNum = Int32.MaxValue;
            ArmAssembly.ErrorReport nextErr = null;
            if (errors.MoveNext())
            {
                nextErr = errors.Current;
                errLineNum = nextErr.Line;
            }
            try
            {
                // Read and display lines from the file until the end of 
                // the file is reached.
                linesCreated = false;
                _codeViewList.Items.Clear();
                int currentLine = 0;
                while (currentLine < _PI.SourceLine.Count || errLineNum < Int32.MaxValue)
                {
                    while (currentLine >= errLineNum)
                    {
                        _codeViewList.Items.Add(
                            new ErrorMsgLine(_drawParameters, nextErr.ErrorMsg, this));
                        if (errors.MoveNext())
                        {
                            nextErr = errors.Current;
                            errLineNum = nextErr.Line;
                        }
                        else errLineNum = Int32.MaxValue;
                    }
                    while (currentLine < errLineNum && currentLine < _PI.SourceLine.Count)
                    {
                        string line = _PI.SourceLine[currentLine++];
                        _codeViewList.Items.Add(new ErrorLine(_drawParameters, line, currentLine, this));
                    }
                }
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("error processing error messages, reason:" + ex.Message);
            }
            linesCreated = true;

            computeMaxWidth();
            _codeViewList.ResumeLayout();
        }

        public void SelectCodeLine(int line)
        {
            if (!createLines()) return;
            if (line > 0)
            {
                _codeViewList.TopIndex = line - 1;
            }
        }

        //converts a memory address to listbox line number
        private int addressToLine(uint address)
        {
            int result = -1;
            for (int i = 0; i < _codeViewList.Items.Count; i++)
            {
                ListLine lst = (ListLine)(_codeViewList.Items[i]);
                if (lst.Address == address)
                    result = i;
                else if (lst.Address > address && result >= 0)
                    break;
            }
            return result;
        }

        public void SelectCodeAddress(uint address)
        {
            createLines();
            _codeViewList.SelectedIndex = addressToLine(address);
        }//SelectCodeAddress

        public bool HasAddress(uint address)
        {
            createLines();
            foreach (ListLine lst in _codeViewList.Items)
            {
                if (lst.Address == address)
                {
                    //lastSearchLine = lst;
                    //lastSearchAddr = address;
                    return true;
                }
            }//foreach
            return false;
        }//HasAddress

        public Font CurrentFont
        {
            get { return _drawParameters.CurrentFont; }
            set
            {
                _drawParameters.CurrentFont = value;
                _drawParameters.computeParameters(_codeViewList.CreateGraphics());
                _codeViewList.Font = value;
                _codeViewList.ItemHeight = value.Height + 1;
                //this.computeMaxWidth();
                _codeViewList.Invalidate();
            }
        }

        public Color CurrentTextColour
        {
            get { return _codeViewList.ForeColor; }
            set { _codeViewList.ForeColor = value; }
        }

        public Color CurrentBackgroundColour
        {
            get { return _codeViewList.BackColor; }
            set { _codeViewList.BackColor = value; }
        }
        public Color CurrentHighlightColour
        {
            get { return _currentHighlightColour; }
            set { _currentHighlightColour = value; _codeViewList.Invalidate(); }
        }

        public ListLine ItemFromPoint(Point pt)
        {
            if (!createLines()) return null;
            int index = _codeViewList.IndexFromPoint(_codeViewList.PointToClient(pt));
            if (index < 0) return null;
            return _codeViewList.Items[index] as ListLine;
        }

        //this function is called whenever the file loaded into this tab is changed.
        //simply set the changed flag. The Mainform of the application will check this
        //flag when the application becomes active.
        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            _fileChanged = true;
            linesCreated = null;
        }

        public bool Changed {
            get { return _fileChanged; }
            set {
                _fileChanged = value;
                if (_fileChanged) linesCreated = null;
            }
        }

        private void computeMaxWidth()
        {
            createLines();
            Graphics g = _codeViewList.CreateGraphics();
            int maxWidth = -1;
            foreach (CodeViewLine cvl in _codeViewList.Items)
            {
                int width = cvl.CalculateWidth(g);
                if (width > maxWidth)
                    maxWidth = width;
            }
            _codeViewList.HorizontalExtent = maxWidth;
        }

        private void drawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            if (!createLines()) return;
            CodeViewList listBox = (CodeViewList)sender;
            if (e.Index < 0 || e.Index >= listBox.Items.Count) return;

            CodeViewLine cvl = (CodeViewLine)(listBox.Items[e.Index]);
            cvl.Draw(e.Index == listBox.SelectedIndex, e, _currentHighlightColour);
        }//drawItem

        /// <summary>
        /// Invoked when VisibleChanged event occurs for the CodeViewList.
        /// Except when the codeview is initially created, we need to force
        /// generation of the lines to be displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createLines(object sender, System.EventArgs e)
        {
            if (!JM.CreatingCodeView)
                createLines();
        }

        private bool? linesCreated = null;

        private bool createLines()
        {
            if (linesCreated.HasValue) return (bool)linesCreated;
            linesCreated = false; // temporary result until work completed

            ArmAssembly.AssembledProgram ap = JM.AssembledProgram;
            if (ap == null) return false;  // added by NH: 10-04-2015

            _codeViewList.SuspendLayout();
            _codeViewList.Items.Clear();
            Errors = false;

            // it is an object code or executable file

            ArmAssembly.ObjFromAsmFileInfo pi = _PI as ArmAssembly.ObjFromAsmFileInfo;
            if (pi != null)
            {
                ArmAssembly.SectionType sect = ArmAssembly.SectionType.Text;
                for (int i = 0; i < pi.SourceLines.Length; i++)
                {
                    ListLine newLine = analyzeLine(pi, i, ref sect);
                    _codeViewList.Items.Add(newLine);
                }
            } else {
                uint textAddress = (uint)(_PI.SectionAddress[(int)(ArmAssembly.SectionType.Text)]);
                uint dataAddress = (uint)(_PI.SectionAddress[(int)(ArmAssembly.SectionType.Data)]);
                uint bssAddress = (uint)(_PI.SectionAddress[(int)(ArmAssembly.SectionType.Bss)]);

                // get sorted list of code labels to intersperse in the code listing
                AddressLabelPair[] cl;
                int clLen;
                if (JM.CodeLabels == null)
                {
                    cl = null;
                    clLen = 0;
                }
                else
                {
                    cl = JM.CodeLabels.CodeLabelList();
                    clLen = cl.Length;
                }
                ArmAssembly.DisassembleARM.CurrentLabels = cl;
                int clix = 0;
                while (clix < clLen && cl[clix].Address < textAddress)
                    clix++;
                uint addrss = textAddress;
                uint endAddress = (uint)(textAddress + _PI.SectionSize[(int)(ArmAssembly.SectionType.Text)]);
                while (addrss < endAddress)
                {
                    while (clix < clLen && cl[clix].Address <= addrss)
                    {
                        _codeViewList.Items.Add(
                            new ListLine(_drawParameters, cl[clix].Label + ":", cl[clix].Address, this));
                        clix++;
                    }
                    string contents;
                    uint opcode;
                    if (ap != null)
                    {
                        opcode = ap.LoadWord((int)addrss);
                        contents = "    " + ArmAssembly.DisassembleARM.DisassembleARMInstruction(opcode, addrss);
                    }
                    else
                    {
                        opcode = 0;
                        contents = "     <<disassembly not available>>";
                    }
                    _codeViewList.Items.Add(new ObjectCodeLine(_drawParameters, addrss, opcode, contents, this));
                    addrss += 4;
                }
                while (clix < clLen && cl[clix].Address < dataAddress)
                    clix++;
                addrss = dataAddress;
                endAddress = (uint)(dataAddress + _PI.SectionSize[(int)(ArmAssembly.SectionType.Data)]);
                // Generate labels for the defined data area.  We display up to 16 bytes per line.
                while (addrss < endAddress)
                {
                    string hex;
                    uint nextLabel = clix < clLen ? cl[clix].Address : (uint)ap.BssStart;
                    int bytesToDisplay = (int)nextLabel - (int)addrss;
                    if ((addrss & 0x3) != 0)
                    {
                        int oddBytes = 4 - (int)(addrss & 0x3);
                        if (oddBytes > bytesToDisplay) oddBytes = bytesToDisplay;
                        hex = convertBytes(oddBytes, addrss);
                        _codeViewList.Items.Add(
                            new ObjectCodeLine(_drawParameters, addrss, ".byte", hex, this));
                        bytesToDisplay -= oddBytes;
                        addrss += (uint)oddBytes;
                    }
                    while (bytesToDisplay >= 16)
                    {
                        hex = String.Format(
                            "0x{0,8:X8}, 0x{1,8:X8}, 0x{2,8:X8}, 0x{3,8:X8}",
                            ap.LoadWord((int)addrss), ap.LoadWord((int)addrss + 4),
                            ap.LoadWord((int)addrss + 8), ap.LoadWord((int)addrss + 12));
                        _codeViewList.Items.Add(
                            new ObjectCodeLine(_drawParameters, addrss, ".word", hex, this));
                        addrss += 16;
                        bytesToDisplay -= 16;
                    }
                    if (bytesToDisplay > 0)
                    {
                        if (bytesToDisplay >= 4)
                        {
                            hex = "";
                            string sep = "";
                            uint addr = addrss;
                            while (bytesToDisplay >= 4)
                            {
                                hex += String.Format("{0} 0x{1,8:X8}", sep, ap.LoadWord((int)addrss));
                                sep = ",";
                                addrss += 4;
                                bytesToDisplay -= 4;
                            }
                            _codeViewList.Items.Add(
                                new ObjectCodeLine(_drawParameters, addr, ".word", hex, this));
                        }
                        if (bytesToDisplay > 0)
                        {
                            hex = convertBytes(bytesToDisplay, addrss);
                            _codeViewList.Items.Add(
                                new ObjectCodeLine(_drawParameters, addrss, ".byte", hex, this));
                            addrss += (uint)bytesToDisplay;
                        }
                    }
                    if (clix < clLen)
                    {
                        _codeViewList.Items.Add(
                            new ListLine(_drawParameters, cl[clix].Label + ":", cl[clix].Address, this));
                        clix++;
                    }
                }
                while (clix < clLen && cl[clix].Address < bssAddress)
                    clix++;
                addrss = bssAddress;
                endAddress = (uint)(bssAddress + _PI.SectionSize[(int)(ArmAssembly.SectionType.Bss)]);
                // Generate labels in the BSS region
                while (addrss < endAddress)
                {
                    uint nextLabel = clix < clLen ? cl[clix].Address : (uint)ap.EndAddress;
                    int bytesToDisplay = (int)nextLabel - (int)addrss;
                    if (bytesToDisplay > 0)
                    {
                        _codeViewList.Items.Add(
                            new ObjectCodeLine(_drawParameters, addrss, ".space", bytesToDisplay.ToString(), this));
                        addrss = nextLabel;
                    }
                    if (clix < clLen)
                    {
                        _codeViewList.Items.Add(
                            new ListLine(_drawParameters, cl[clix].Label + ":", cl[clix].Address, this));
                        clix++;
                    }
                }
            }

            computeMaxWidth();
            _codeViewList.ResumeLayout();
            linesCreated = true;
            return true;
        }//init

        protected string convertBytes(int numberBytes, uint address)
        {
            string r = "";
            switch (numberBytes)
            {
                case 1:
                    r = String.Format("0x{0,2:X2}",
                        JM.AssembledProgram.LoadByte((int)address));
                    break;
                case 2:
                    r = String.Format("0x{0,2:X2}, 0x{1,2:X2}",
                       JM.AssembledProgram.LoadByte((int)address),
                       JM.AssembledProgram.LoadByte((int)address + 1));
                    break;
                case 3:
                    r = String.Format(" 0x{0,2:X2}, 0x{1,2:X2}, 0x{2,2:X2}",
                       JM.AssembledProgram.LoadByte((int)address),
                       JM.AssembledProgram.LoadByte((int)address + 1),
                       JM.AssembledProgram.LoadByte((int)address + 2));
                    break;
            }
            return r;
        }

        protected ListLine analyzeLine(ArmAssembly.ObjFromAsmFileInfo pi, int x, ref ArmAssembly.SectionType sect) {
            string s = pi.SourceLines[x];
            if (s == null) s = "";
            uint offsetInFile = pi.Offsets[x];
            string hexcontents = pi.HexContent[x];
            ListLine result = null;
            int i = 0;
            while(i < s.Length && Char.IsWhiteSpace(s[i])) i++;
            if (i >= s.Length || s[i] == '@' || (!Char.IsLetterOrDigit(s[i]) && s[i] != '.'))
                return makeResult(s, offsetInFile, hexcontents, sect, pi);

            // it's a label, or opcode, or directive
            int start = i;
            while(i < s.Length && (Char.IsLetterOrDigit(s[i]) || s[i] == '.'))
                i++;
            int length = i-start;
            // skip any white space
            while(i < s.Length && Char.IsWhiteSpace(s[i])) i++;
            // if it was a label, or not a directive, nothing to do
            if (i < s.Length && s[i] == ':' || s[start] != '.')
                return makeResult(s, offsetInFile, hexcontents, sect, pi);

            string directive = s.Substring(start,length).ToLower();
            switch(directive) {
                case ".text":
                case ".data":
                case ".rodata":
                case ".rodata.str1.4":
                case ".bss":
                    sect = handleSect(directive);
                    break;
                case ".section":
                    while(i < s.Length && Char.IsWhiteSpace(s[i])) i++;
                    if (i >= s.Length) return result;
                    start = i;
                    while(i < s.Length && (Char.IsLetterOrDigit(s[i]) || s[i] == '.')) i++;
                    length = i-start;
                    sect = handleSect(s.Substring(start,length));
                    break;
                default:
                    break;
            }
            return makeResult(s, offsetInFile, hexcontents, sect, pi);
        }

        protected ListLine makeResult(string s, uint offsetInFile, string hexcontents,
            ArmAssembly.SectionType sect, ArmAssembly.ArmFileInfo pi)
        {
            // attempt to eliminate a bug in the mono implementation where tabs do not
            // always seem to be honoured when displaying the line of text
            if (!ARMSim.ARMSimUtil.RunningOnWindows && !String.IsNullOrEmpty(s) && s[0] == '\t')
                s = "        " + s.Substring(1);
            if (offsetInFile == 0xFFFFFFFF) {
                if (hexcontents != null)
                    return new ListLine(_drawParameters, s, hexcontents, this);
                return new ListLine(_drawParameters, s, this);
            }
            if (hexcontents != null)
                return new ListLine(_drawParameters, s, (uint)(offsetInFile + pi.SectionAddress[(int)sect]), hexcontents, this);
            return new ListLine(_drawParameters, s, (uint)(offsetInFile + pi.SectionAddress[(int)sect]), this);
        }

        static protected ArmAssembly.SectionType handleSect(string name)
        {
            switch (name)
            {
                case ".text":
                    return ArmAssembly.SectionType.Text;
                case ".data":
                case ".rodata":
                case ".rodata.str1.4":
                    return ArmAssembly.SectionType.Data;
                case ".bss":
                    return ArmAssembly.SectionType.Bss;
            }
            return ArmAssembly.SectionType.Abs;
        }

    }//class CodeViewTab
}
