using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.GUI.Views
{
    public partial class CodeViewList : ListBox
    {
        private CodeViewTab _parent;

        //holds the currently selected(highlighted) code line
        private int _selectedIndex = -1;

        public CodeViewList(CodeViewTab parent)
        {
            InitializeComponent();

            _parent = parent;

            //this.BackColor = System.Drawing.Color.DarkGray;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.HorizontalScrollbar = true;
            this.IntegralHeight = true;
            //this.ItemHeight = 20;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = _parent.Name;
            this.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.Size = new System.Drawing.Size(464, 304);
            this.TabIndex = 0;

        }

        ////toggle the breakpoint setting at the specified line
        //private void doubleClick(CodeViewTab.lstLine lst)
        //{
        //    //if in error mode, do nothing
        //    if (_parent.Errors) return;

        //    //and if a valid code line, toggle the breakpoint flag
        //    if (lst.Valid)
        //    {
        //        //lst.BreakPoint = !lst.BreakPoint;
        //        _parent.JM.ToggleBreakpoint(lst.Address);
        //        this.Invalidate();
        //    }//if
        //}

        //toggle the breakpoint setting at the specified line
        private void doubleClick(int line)
        {
            //if in error mode, do nothing
            if (_parent.Errors) return;

            //get the data item at the line
            CodeViewTab.ListLine lst = this.Items[line] as CodeViewTab.ListLine;

            //and if a valid code line, toggle the breakpoint flag
            if (lst.Valid)
            {
                _parent.JM.Breakpoints.Toggle(lst.Address);
                this.Invalidate();
            }//if
        }

        //toggle the breakpoint flag at the specified coordinates
        private void doubleClick(int x, int y)
        {
            //if in error mode, do nothing
            if (_parent.Errors) return;

            //convert coordinate to a line number
            int index = this.IndexFromPoint(x, y);

            //if valid line, then pass to line version
            if (index >= 0)
            {
                doubleClick(index);
            }//if
        }//doubleClick

        //Override the normal WndProc, 2 reasons
        //1) Intercept the left button down click event so lines cannot be selected that way
        //2) Intercept the double click event and handle it (toggle breakpoints)
        protected override void WndProc(ref Message m)
        {
            const int WM_LBUTTONDOWN = 0x201;
            const int WM_LBUTTONDBLCLK = 0x203;

            switch (m.Msg)
            {
                //case ARMSim.GUI.DockingWindows.Win32.Gdi32.WM_LBUTTONDOWN: return;
                //case ARMSim.GUI.DockingWindows.Win32.Gdi32.WM_LBUTTONDBLCLK:
                case WM_LBUTTONDOWN: return;
                case WM_LBUTTONDBLCLK:
                    {
                        int x = m.LParam.ToInt32() & 0x0000ffff;
                        int y = (m.LParam.ToInt32() >> 16) & 0x0000ffff;
                        doubleClick(x, y);
                        return;
                    }
            }
            base.WndProc(ref m);
        }

        private Rectangle LineToRect(int line)
        {
            int top = (line - this.TopIndex) * this.ItemHeight;
            return new Rectangle(0, top, this.ClientSize.Width, this.ItemHeight);
        }

        public override int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                int org_index = _selectedIndex;
                _selectedIndex = value;
                if (_selectedIndex < this.TopIndex)
                {
                    this.TopIndex = _selectedIndex;
                    this.Invalidate();
                    this.Update();
                }
                else if (_selectedIndex >= (this.TopIndex + (this.ClientSize.Height / this.ItemHeight)))
                {
                    this.TopIndex = _selectedIndex;
                    this.Invalidate();
                    this.Update();
                }
                else
                {
                    if (org_index >= 0)
                    {
                        this.Invalidate(LineToRect(org_index));
                        this.Update();
                    }
                    if (_selectedIndex >= 0)
                    {
                        this.Invalidate(LineToRect(_selectedIndex));
                        this.Update();
                    }
                }
            }
        }

        //public void ToggleBreakpoint(int index)
        //{
        //    doubleClick(index);
        //}
        //public void ToggleBreakpoint(CodeViewTab.lstLine lst)
        //{
        //    doubleClick(lst);
        //}
        //public void ToggleBreakpoint()
        //{
        //    doubleClick(this.SelectedIndex);
        //}

    }//class CodeViewList
}
