using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;
//using System.Diagnostics.CodeAnalysis;


namespace ARMSim.Plugins.ARMRegister64Plugin
{
    /// <summary>
    /// This plugin demonstrates the ability to create new instructions in ARMSim
    /// by creating them from the undefined opcode bits in the ARM instruction set.
    /// In this example we are going to create a series of instructions to manipulate
    /// a virtual 64 bit register. Basic instructions to load this register from the
    /// general purpose registers, add, subtract and store will be created.
    /// In addition, we will create mnemonics to insert into the parsing tables so these new
    /// instructions can be coded with symbols in the original source.
    ///
    /// To display the 64 bit register contents we will create a user control on the Plugin UI
    /// display window to show the register contents. The user control that does this is located
    /// in the UIControls assembly (ARMSim.Plugins.UIControls.Register64TextBox)
    /// This control will only be updated if the simulation is stopped. This prevents the control
    /// from becoming a blurr during the running of the simulation and the overloading 
    /// of the windows message queue.
    /// 
    /// </summary>
    public class ARMRegister64 : IARMPlugin, IDisposable

    {
        /// <summary>
        /// Reference to the ARMSim host interface. Aquired in the Init method.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// The user interface control that will display the current contents of the 64 bit register
        /// </summary>
        private ARMSim.Plugins.UIControls.Register64TextBox mRegister64Control;

        /// <summary>
        /// The current value in the 64 bit register
        /// </summary>
        private long mRegister64;

        /// <summary>
        /// This flag indicates if the simulation is running. It is set/cleared in the
        /// onStart/onStop event handlers.
        /// If set, the UI will not be updated.
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
        public void onLoad(object sender, EventArgs e)
        {
            //subscribe to the restart event so we can re-init the register when the simulation is restarted
            mHost.Restart += onRestart;

            //subscribe to the Start and Stop events so we can track when the simulation is running
            mHost.StartSimulation += onStart;
            mHost.StopSimulation += onStop;

            //request a panel from ARMSim user interface, create the register64 control and add it to the panel
            Panel panel = mHost.RequestPanel(this.PluginName);
            mRegister64Control = new ARMSim.Plugins.UIControls.Register64TextBox();
            panel.Controls.Add(mRegister64Control);

            //request the opcodes from the undefined opcode range of the ARM processor. Specify the execute method
            //that will be called when this opcode is encountered by the simulation engine.
            mHost.RequestOpcodeRange(0x0ef000f0, 0x0ffff0f0, this.onExecute);

            //Here we will insert the mnemonics of the new instructions into the parsing tables. The method specified
            //is called when the instruction is parsed. It allows us to form the final opcode with the operands
            //encoded into the opcode.
            mHost.RequestMnemonic("LOAD64",  0x0ef000f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("STORE64", 0x0ef001f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("ADDS64",  0x0ef002f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("ADD64",   0x0ef003f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("MULS64",  0x0ef004f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("MUL64",   0x0ef005f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("MULAS64", 0x0ef006f0, "R", this.onFormInstruction64);
            mHost.RequestMnemonic("MULA64",  0x0ef007f0, "R", this.onFormInstruction64);

            //set the current internal 64 bit value(0) into the UI control
            //since the simulation is not running it will update
            UpdateRegister64();

        }//onLoad

        /// <summary>
        /// The restart event is called when the simulation has been restarted. In our case all we want to do
        /// is reset the register value to 0 and update the user interface
        /// </summary>
        public void onRestart(object sender, EventArgs e)
        {
            //make sure we indicate simulation is not running
            mSimulationRunning = false;

            //zero register
            mRegister64 = 0;

            //and update the UI control
            UpdateRegister64();
        }//onRestart

        /// <summary>
        /// This method is called when the parsing of the source code has encountered an mnemonic that was added earlier.
        /// For our purposes all we need to do is encode the source/destination register into the least significant
        /// 4 bits of the opcode.
        /// The operands array contain the parsed operands. We do a sanity check to make sure there is only 1 entry in the array
        /// </summary>
        /// <param name="baseCode"></param>
        /// <param name="operands"></param>
        /// <returns></returns>
        public uint onFormInstruction64(uint baseCode, params uint[] operands)
        {
            //if not exactly 1 operand, do nothing
            if (operands.Length != 1)
                return baseCode;

            //encode register into opcode and return the formed opcode
            return(baseCode | (operands[0] & 0x0f));
        }//onFormInstruction64


        /// <summary>
        /// This method is called when our requested opcode(s) have been fetched and need executing.
        /// We will inspect the opcode and perform the operation specified in the instruction bits
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public uint onExecute(uint opcode)
        {
            uint Rm = (opcode & 0x000f);
            switch (opcode & 0x00000f00)
            {
                //LOAD64 rm
                //load the 64 bit register from general registers rm and rm+1
                case 0x0000:
                    {
                        uint RmL = Rm + 1;
                        if (RmL > 15)
                            RmL = 0;

                        uint highWord = mHost.GPR[Rm];
                        uint lowWord = mHost.GPR[RmL];
                        //uint highWord = mHost.getReg(Rm);
                        //uint lowWord = mHost.getReg(RmL);
                        mRegister64 = ((long)highWord << 32) + lowWord;
                    } break;

                //STORE64 rm
                //store the 64 bit register into general registers rm and rm+1
                case 0x0100:
                    {
                        uint RmL = Rm + 1;
                        if (RmL > 15)
                            RmL = 0;

                        uint highWord = (uint)(mRegister64 >> 32);
                        uint lowWord = (uint)mRegister64;

                        mHost.setReg(Rm, highWord);
                        mHost.setReg(RmL, lowWord);

                    } break;

                //ADDS64  rm
                //perform a signed add of the 64 bit register and general register rm
                case 0x0200:
                    {
                        int addVaue = (int)mHost.getReg(Rm);
                        long longAddValue = (long)addVaue;
                        mRegister64 += longAddValue;
                    } break;

                //ADD64  rm
                //perform an unsigned add of the 64 bit register and general register rm
                case 0x0300:
                    {
                        uint addVaue = (uint)mHost.getReg(Rm);
                        long longAddValue = (long)addVaue;
                        mRegister64 += longAddValue;
                    } break;

                //MULS64 rm
                //perform a signed multiply of the 64 bit register and general registers rm and rm+1
                case 0x0400:
                    {
                        uint RmL = Rm + 1;
                        if (RmL > 15)
                            RmL = 0;

                        int highWord = (int)mHost.getReg(Rm);
                        int lowWord = (int)mHost.getReg(RmL);
                        mRegister64 = highWord * lowWord;
                    } break;

                //MUL64 rm
                //perform an unsigned multiply of the 64 bit register and general registers rm and rm+1
                case 0x0500:
                    {
                        uint RmL = Rm + 1;
                        if (RmL > 15)
                            RmL = 0;

                        uint highWord = (uint)mHost.getReg(Rm);
                        uint lowWord = (uint)mHost.getReg(RmL);
                        mRegister64 = highWord * lowWord;
                    } break;

                default: break;
            }//switch
            UpdateRegister64();

            //return number of clock cycles required to execute this opcode
            return 3;
        }//onExecute

        /// <summary>
        /// This helper function will update the 64 bit register UI control with the current value help
        /// in the register, but will only do it if the simulation is stopped.
        /// </summary>
        private void UpdateRegister64()
        {
            //if simulation is running, do nothing
            if (mSimulationRunning)
                return;

            //update control
            mRegister64Control.Value = mRegister64;
        }//UpdateRegister64

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin name
        /// </summary>
        public string PluginName { get { return "Register64"; } }

        /// <summary>
        /// Required by the IARMPlugin interface, the plugin description
        /// </summary>
        public string PluginDescription { get { return "Internal 64 bit ARM Register"; } }

        /// <summary>
        /// The Start event is fired when the simulation starts running. We will set our internal flag true
        /// </summary>
        public void onStart(object sender, EventArgs e)
        {
            mSimulationRunning = true;
        }//onStart

        /// <summary>
        /// The Stop event is fired when the simulation stops running. We will set our internal flag false.
        /// This can happen if the simulations executes the halt swi instruction (swi 0x11), hits a breakpoint
        /// or encounters a severe error.
        /// </summary>
        public void onStop(object sender, EventArgs e)
        {
            mSimulationRunning = false;
            UpdateRegister64();
        }//onStop

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                mRegister64Control.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }//class ARMRegister64
}
