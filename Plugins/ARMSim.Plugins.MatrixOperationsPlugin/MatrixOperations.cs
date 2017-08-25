using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;
using CSML;
using ARMSim.Plugins.UIControls;

namespace ARMSim.Plugins.MatrixOperationsPlugin
{
    public class MatrixOperations : IARMPlugin
    {
        /// <summary>
        /// Default matrix stack size
        /// </summary>
        private const int mStackSize = 16;

        /// <summary>
        /// This is the UI component of the matrix stack
        /// </summary>
        private MatrixStackDisplay mMatrixStackDisplay;

        /// <summary>
        /// This is where we store the current matrix stack
        /// </summary>
        private FixedMatrixStack mFixedMatrixStack = new FixedMatrixStack(mStackSize);

        /// <summary>
        /// Reference to the ARMSim host interface. Aquired in the Init method.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// This flag indicates if the simulation is currently running.
        /// If true we can avoid updating the matrix UI until the simulation stops.
        /// </summary>
        private bool mSimulationRunning;

        /// <summary>
        /// The Init method of the plugin. This method is called when the plugin has been created by ARMSim.
        /// ARMSim will pass an instance of IARMHost to the plugin so that the plugin can make requests back to ARMSim.
        /// </summary>
        /// <param name="ihost"></param>
        public void Init(IARMHost ihost)
        {
            //cache the interface to ARMSim
            mHost = ihost;

            //set the load handler so we get called when all the plugins are loaded
            mHost.Load += onLoad;

        }//Init

        /// <summary>
        /// The onLoad is called after all plugins have had their init called and are all loaded.
        /// </summary>
        private void onLoad(object sender, EventArgs e)
        {
            mMatrixStackDisplay = new MatrixStackDisplay();
            mMatrixStackDisplay.Dock = DockStyle.Fill;

            //request a panel from ARMSim user interface, create the register64 control and add it to the panel
            Panel panel = mHost.RequestPanel(this.PluginName);
            panel.Controls.Add(mMatrixStackDisplay);
            panel.Resize += new EventHandler(this.panel1_Resize);

            //subscribe to the restart event so we can re-init the register when the simulation is restarted
            mHost.Restart += onRestart;

            //subscribe to the Start and Stop events so we can track when the simulation is running
            mHost.StartSimulation += mHost_Start;
            mHost.StopSimulation += mHost_StopSimulation;

            //request the opcodes from the undefined opcode range of the ARM processor. Specify the execute method
            //that will be called when this opcode is encountered by the simulation engine.
            mHost.RequestOpcodeRange(0x0ef000f0, 0x0ff000f0, this.onExecute);

            //Here we will insert the mnemonics of the new instructions into the parsing tables. The method specified
            //is called whwn the instruction is parsed. It allows us to form the final opcode with the operands
            //encoded into the opcode.
            mHost.RequestMnemonic("MLOADI", 0x0ef000f0, "RRR", this.onFormInstruction3);
            mHost.RequestMnemonic("MLOADF", 0x0ef001f0, "RRR", this.onFormInstruction3);
            mHost.RequestMnemonic("MLOADD", 0x0ef002f0, "RRR", this.onFormInstruction3);
            mHost.RequestMnemonic("MPOP",   0x0ef008f0, "RRR", this.onFormInstruction3);

            mHost.RequestMnemonic("MIDENT", 0x0ef003f0, "R", this.onFormInstruction1);
            mHost.RequestMnemonic("MADD",   0x0ef004f0, "",  this.onFormInstruction1);
            mHost.RequestMnemonic("MSUB",   0x0ef005f0, "",  this.onFormInstruction1);
            mHost.RequestMnemonic("MMUL",   0x0ef006f0, "",  this.onFormInstruction1);
            mHost.RequestMnemonic("MDOT",   0x0ef007f0, "",  this.onFormInstruction1);


        }//onLoad

        /// <summary>
        /// This method is called when the parsing of the source code has encountered an mnemonic that was added earlier
        /// and has exactly 1 operand. Note that instructions with no operands (ie MADD) still are parsed with 
        /// 1 operand.
        /// For our purposes all we need to do is encode the source/destination register into the least significant
        /// 4 bits of the opcode.
        /// </summary>
        /// <param name="baseCode"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        private uint onFormInstruction1(uint baseCode, params uint[] operands)
        {
            if (operands.Length != 1)
                return baseCode;

            return baseCode | (operands[0] & 0x0f);

        }//onFormInstruction1

        /// <summary>
        /// This method is called when the parsing of the source code has encountered an mnemonic that was added earlier
        /// and has exactly 3 operands. We will encode the 3 operands into bit positions 0, 12 and 16 (Rd, Rn, Rm) into
        /// the base opcode.
        /// </summary>
        /// <param name="baseCode"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        private uint onFormInstruction3(uint baseCode, params uint[] operands)
        {
            if (operands.Length != 3)
                return baseCode;

            //encode register into opcode and return the formed opcode
            baseCode |= (operands[0] & 0x0f) << 16;
            baseCode |= (operands[1] & 0x0f) << 12;
            return baseCode | (operands[2] & 0x0f);

        }//onFormInstruction3

        /// <summary>
        /// Callback function when the UI panel has been resized. This lets us manage the matrix display
        /// and update scroll bars.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Resize(object sender, EventArgs e)
        {
            mMatrixStackDisplay.Size = mMatrixStackDisplay.Parent.Size;
        }

        private delegate double loadMatrixElement(ref uint address);
        private double LoadMatrixInt(ref uint address)
        {
            int value = (int)mHost.GetMemory(address, MemorySize.Word);
            address += 4;
            return (double)value;
        }
        private double LoadMatrixFloat(ref uint address)
        {
            int value = (int)mHost.GetMemory(address, MemorySize.Word);
            address += 4;

            float fvalue = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
            return (double)fvalue;
        }
        private double LoadMatrixDouble(ref uint address)
        {
            uint value1 = mHost.GetMemory(address, MemorySize.Word);
            address += 4;
            uint value2 = mHost.GetMemory(address, MemorySize.Word);
            address += 4;

            byte[] bytes = new byte[8];
            Array.Copy(BitConverter.GetBytes(value1), 0, bytes, 0, 4);
            Array.Copy(BitConverter.GetBytes(value2), 0, bytes, 4, 4);

            double dvalue = BitConverter.ToDouble(bytes, 0);
            return dvalue;
        }
        private void StoreMatrixDouble(ref uint address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (uint ii = 0; ii < 8; ii++)
            {
                mHost.SetMemory(address, MemorySize.Byte, bytes[ii]);
                address++;
            }
        }

        private Matrix LoadMatrix(uint regRows, uint regCols, uint regAddress, loadMatrixElement lme)
        {
            int rows = (int)mHost.getReg(regRows);
            int cols = (int)mHost.getReg(regCols);
            uint address = mHost.getReg(regAddress);

            Matrix m = new Matrix((int)rows, (int)cols);
            for (int row = 1; row <= rows; row++)
            {
                for (int col = 1; col <= cols; col++)
                {
                    double value = lme(ref address);
                    m[row, col] = new Complex(value);
                }
            }
            return m;
        }

        private uint onExecute(uint opcode)
        {
            //extract the register numbers from the opcode. See onFormInstruction for bit locations
            //of the 3 registers required
            uint Rm = ((opcode >> 16) & 0x0f);
            uint Rn = ((opcode >> 12) & 0x0f);
            uint Rd = (opcode & 0x0f);

            switch (opcode & 0x00000f00)
            {
                //MLOADI Rd:address, Rm:rows, Rn:cols
                case 0x0000:
                    {
                        Matrix m = LoadMatrix(Rm, Rn, Rd, this.LoadMatrixInt);
                        if (m != null)
                        {
                            mFixedMatrixStack.Push(m);
                        }

                    }break;

                //MLOADF Rd:address, Rm:rows, Rn:cols
                case 0x0100:
                    {
                        Matrix m = LoadMatrix(Rm, Rn, Rd, this.LoadMatrixFloat);
                        if (m != null)
                            mFixedMatrixStack.Push(m);

                    } break;

                //MLOADD Rd:address, Rm:rows, Rn:cols
                case 0x0200:
                    {
                        Matrix m = LoadMatrix(Rm, Rn, Rd, this.LoadMatrixDouble);
                        if (m != null)
                            mFixedMatrixStack.Push(m);

                    } break;

                //MIDENT Rd:rows (and columns)
                case 0x0300:
                    {
                        uint size = mHost.getReg(Rd);
                        Matrix m1 = Matrix.Identity((int)size);
                        mFixedMatrixStack.Push(m1);
                    } break;

                //MADD
                case 0x0400:
                    {
                        Matrix m1 = mFixedMatrixStack.Pop();
                        Matrix m2 = mFixedMatrixStack.Pop();

                        if ((m1.ColumnCount != m2.ColumnCount) ||
                           (m1.RowCount != m2.RowCount))
                            break;

                        Matrix m3 = m1 + m2;
                        mFixedMatrixStack.Push(m3);

                    } break;

                //MMUL
                case 0x0600:
                    {
                        Matrix m1 = mFixedMatrixStack.Pop();
                        Matrix m2 = mFixedMatrixStack.Pop();

                        Matrix m3 = m1 * m2;
                        mFixedMatrixStack.Push(m3);

                    } break;

                //MDOT
                case 0x0700:
                    {
                        Matrix m1 = mFixedMatrixStack.Pop();
                        Matrix m2 = mFixedMatrixStack.Pop();
                        Complex value = Matrix.Dot(m1, m2);

                        Matrix m3 = new Matrix(value);
                        mFixedMatrixStack.Push(m3);

                    } break;

                //MPOP
                case 0x0800:
                    {
                        Matrix m = mFixedMatrixStack.Pop();
                        mHost.setReg(Rm, (uint)m.RowCount);
                        mHost.setReg(Rn, (uint)m.ColumnCount);

                        uint address = mHost.getReg(Rd);

                        for (int row = 1; row <= m.RowCount; row++)
                        {
                            for (int col = 1; col <= m.ColumnCount; col++)
                            {
                                double value = m[row, col].Re;
                                StoreMatrixDouble(ref address, value);
                            }
                        }

                    } break;


            }
            this.UpdateMatrixDisplay();
            return 3;
        }//onExecute

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin name
        /// </summary>
        public string PluginName { get { return "MatrixOperations"; } }

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin description
        /// </summary>
        public string PluginDescription { get { return "Plugin to implement matrix operations"; } }

        /// <summary>
        /// The restart event is called when the simulation has been restarted. In our case all we want to do
        /// is reset the register value to 0 and update the user interface
        /// </summary>
        private void onRestart(object sender, EventArgs e)
        {
            //make sure we indicate simulation is not running
            mSimulationRunning = false;

            // Clear the matrix stack
            mFixedMatrixStack.Clear();

            //and update the UI control
            UpdateMatrixDisplay();

        }//onRestart

        /// <summary>
        /// Simulatation has started a run. No need to keep updating UI so set a flag
        /// indicating this.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Start(object sender, EventArgs args)
        {
            mSimulationRunning = true;
        }

        /// <summary>
        /// A simulation run has ended. Update the UI flag and refresh the matrix display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_StopSimulation(object sender, EventArgs args)
        {
            mSimulationRunning = false;
            UpdateMatrixDisplay();
        }
        
        /// <summary>
        /// Helper function to update matrix display. This call will check if the simulation is
        /// currently running. If it is there is no need to update UI.
        /// </summary>
        private void UpdateMatrixDisplay()
        {
            if(!mSimulationRunning)
                mMatrixStackDisplay.UpdateMatrixDisplay(mFixedMatrixStack.ToArray());
        }

    }//class MatrixOperations
}
