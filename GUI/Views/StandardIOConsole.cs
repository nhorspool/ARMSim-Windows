using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ARMSim.GUI.Views
{
    public partial class StandardIOConsole : UserControl
    {
        private bool mAbort;
        private Queue<char> mKeystrokes = new Queue<char>();

        public StandardIOConsole()
        {
            InitializeComponent();
        }

        public uint ConsoleHandle { get; set; }

        public Font CurrentFont
        {
            get { return textBox1.Font; }
            set { textBox1.Font = value; }
        }
        public Color CurrentBackgroundColour
        {
            get { return textBox1.BackColor; }
            set { textBox1.BackColor = value; }
        }
        public Color CurrentTextColour
        {
            get { return textBox1.ForeColor; }
            set { textBox1.ForeColor = value; }
        }

        public void Write(char chr)
        {
            if ((int)chr == 13)
                return;
            else if ((int)chr == 10)
                textBox1.Text += Environment.NewLine;
            else
                textBox1.Text += chr;

            MoveCaretToEndOfText();

            //string str = string.Empty;
            //if(textBox1.Lines.Length > 0)
            //    str = textBox1.Lines.GetValue(textBox1.Lines.GetLength(0) - 1) as string;
            //str += chr;
            //textBox1.Lines.SetValue(str, textBox1.Lines.GetLength(0) - 1);

        }

        public char Read()
        {
            mAbort = false;
            textBox1.Focus();

            //WaitingOFF();
            //DateTime now = DateTime.Now;
            while (mKeystrokes.Count == 0)
            {
                //make sure we dont consume cpu
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
                if (mAbort)
                {
                    //mAbort = false;
                    return ARMPluginInterfaces.ARMSimStream.ctrlD;
                }

                //if (DateTime.Now.Subtract(now).TotalMilliseconds > 500)
                //{
                //    //if (this.Text.EndsWith("*"))
                //    //    //WaitingOFF();
                //    //else
                //    //    //WaitingON();
                //    now = DateTime.Now;
                //}

            }
            //WaitingOFF();

            char c= mKeystrokes.Dequeue();
            return c;
        }

        public char Peek()
        {
            if (mKeystrokes.Count > 0)
                return mKeystrokes.Peek();
            else
                return (char)0;

        }

        public void SetAbort()
        {
            mAbort = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar == (char)8)
            {
                string[] lines = textBox1.Lines;
                if (lines.Length == 0)
                    return;

                int index = lines.Length - 1;
                string str = lines[index];
                if (str.Length <= 0)
                    return;

                string newStr = str.Substring(0, str.Length - 1);
                lines[index] = newStr;
                textBox1.Lines = lines;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                this.Write((char)10);
            }
            //else if (char.IsControl(e.KeyChar))
            //{
            //    return;
            //}
            else
            {
                this.Write(e.KeyChar);
            }
            MoveCaretToEndOfText();
            mKeystrokes.Enqueue(e.KeyChar);
        }

        private void MoveCaretToEndOfText()
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }


    }
}
