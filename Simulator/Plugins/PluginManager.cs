using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

using ARMPluginInterfaces;
using ARMPluginInterfaces.Preferences;

using System.Xml;
using ARMSim.Preferences;

namespace ARMSim.Simulator.Plugins
{
    /// <summary>
    /// This class manages the plugins. It also implements the IARMHost interface
    /// that is passed to the plugins.
    /// </summary>
    public class PluginManager : IARMHost
    {
        //private bool mApplicationClosing;

        public class PluginItem
        {
            public string Assembly { get; set; }
            public IARMPlugin armPlugin { get; set; }
            public bool Activated { get; set; }

            public PluginItem(string assembly, IARMPlugin armPlugin)
            {
                this.Assembly = assembly;
                this.armPlugin = armPlugin;
                this.Activated = false;

            }

            public bool IsEqual(PluginItem item)
            {
                return ((string.Compare(this.Assembly, item.Assembly, true) == 0) &&
                        (string.Compare(this.armPlugin.PluginName, item.armPlugin.PluginName, true) == 0));
            }
        }//class PluginItem

        private List<PluginItem> mAvailablePlugins = new List<PluginItem>();
        public ICollection<PluginItem> AvailablePlugins { get { return mAvailablePlugins; } }

        /// <summary>
        /// holds the requested memory map ranges from the plugins
        /// </summary>
        private List<MemoryRange> mMemoryMaps = new List<MemoryRange>();

        /// <summary>
        /// holds the requested instruction opcodes from the plugins
        /// </summary>
        private List<InstructionRange> mOpcodes = new List<InstructionRange>();

        private const string mActivatedPluginAttribute = "ActivatedPlugin";
        private const string mNameAttribute = "Name";
        private const string mAssemblyAttribute = "Assembly";

        /// <summary>
        ///A reference to the simulator user interface. When plugins make panels
        ///requests, it is created in the boardview
        /// </summary>
        private GUI.Views.PluginsUIView mPluginsUIView;

        /// <summary>
        /// A reference to the outputview of the simulator
        /// </summary>
        private GUI.Views.IOutputView _outputView;

        /// <summary>
        /// A reference to the simulator engine.
        /// </summary>
        private ISimulator _simulatorInterface;

        /// <summary>
        /// PluginManager ctor
        /// The manager is created with a reference to the simulator interface.
        /// </summary>
        public PluginManager(ISimulator simulatorInterface)
        {
            _simulatorInterface = simulatorInterface;
        }

        // IARMHost methods

        /// <summary>
        /// get a general purpose register
        /// </summary>
        /// <param name="reg">register number(r0-r15)</param>
        /// <returns>register value</returns>
        uint IARMHost.getReg(uint reg) { return _simulatorInterface.GPR[reg]; }

        IGPR IARMHost.GPR { get { return _simulatorInterface.GPR; } }

        /// <summary>
        /// set a general purpose register
        /// </summary>
        /// <param name="reg">register number(r0-r15)</param>
        /// <param name="data">value to write</param>
        void IARMHost.setReg(uint reg, uint data) { _simulatorInterface.GPR[reg] = data; }

        /// <summary>
        /// Get a (double)fp number from the floating point registers.
        /// </summary>
        /// <param name="reg">register number(d0-d15)</param>
        /// <returns></returns>
        double IARMHost.getFPDoubleReg(uint reg) { return _simulatorInterface.FPR.ReadD(reg); }

        /// <summary>
        /// Get a (float)fp number from the floating point registers.
        /// </summary>
        /// <param name="reg">register number(f0-f31)</param>
        /// <returns></returns>
        float IARMHost.getFPSingleReg(uint reg) { return _simulatorInterface.FPR.ReadS(reg); }

        /// <summary>
        /// Write a (double)fp number to the floating point registers.
        /// </summary>
        /// <param name="reg">register number(d0-d15)</param>
        /// <param name="value">(double)value to write</param>
        void IARMHost.setFPDoubleReg(uint reg, double value) { _simulatorInterface.FPR.WriteD(reg, value); }

        /// <summary>
        /// Write a (float)fp number to the floating point registers.
        /// </summary>
        /// <param name="reg">register number(f0-f31)</param>
        /// <param name="value">(float)value to write</param>
        void IARMHost.setFPSingleReg(uint reg, float value) { _simulatorInterface.FPR.WriteS(reg, value); }

        void IARMHost.OutputConsoleString(string fmt, params object[] parms) { _simulatorInterface.OutputConsoleString(fmt, parms); }

        /// <summary>
        /// Read data from main memory
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <returns>read value</returns>
        uint IARMHost.GetMemory(uint address, MemorySize ms) { return _simulatorInterface.MainMemory.GetMemory(address, ms); }

        /// <summary>
        /// Write data to main memory
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size to read</param>
        /// <param name="data">data to write</param>
        void IARMHost.SetMemory(uint address, MemorySize ms, uint data) { _simulatorInterface.MainMemory.SetMemory(address, ms, data); }

        /// <summary>
        /// Command simulator to stop. Pass thru to simulator interface
        /// </summary>
        //public void ExitSimulation() { _simulatorInterface.ExitSimulation(); }

        //access to cpsr flags. Pass thru to simulator interface
        ///<summary>Access zero flag</summary>
        bool IARMHost.zf { get { return _simulatorInterface.CPSR.zf; } set { _simulatorInterface.CPSR.zf = value; } }
        ///<summary>Access carry flag</summary>
        bool IARMHost.cf { get { return _simulatorInterface.CPSR.cf; } set { _simulatorInterface.CPSR.cf = value; } }
        ///<summary>Access negative flag</summary>
        bool IARMHost.nf { get { return _simulatorInterface.CPSR.nf; } set { _simulatorInterface.CPSR.nf = value; } }
        ///<summary>Access overflow flag</summary>
        bool IARMHost.vf { get { return _simulatorInterface.CPSR.vf; } set { _simulatorInterface.CPSR.vf = value; } }
        ///<summary>Access the flags field of the cpsr</summary>
        uint IARMHost.Flags { get { return _simulatorInterface.CPSR.Flags; } }
        /// <summary>
        /// Allocate memory from the heap and return the address
        /// </summary>
        /// <param name="size">size to allocate in bytes</param>
        /// <returns>returned address, always word aligned</returns>
        uint IARMHost.Malloc(uint size) { return _simulatorInterface.MainMemory.malloc(size); }

        /// <summary>
        /// Clear the heap and free all memory
        /// </summary>
        void IARMHost.HeapClear() { _simulatorInterface.MainMemory.HeapClear(); }

        /// <summary>
        /// Interrogate the memory to obtain property values.  These are
        /// DataStart, DataEnd, StackStart, StackEnd, Size and Start.
        /// </summary>
        uint IARMHost.GetMemoryParameter(string propertyName)
        {
            switch (propertyName)
            {
                case "DataEnd": return _simulatorInterface.MainMemory.DataEnd;
                case "DataStart": return _simulatorInterface.MainMemory.DataStart;
                case "StackStart": return _simulatorInterface.MainMemory.StackStart;
                case "StackEnd": return _simulatorInterface.MainMemory.StackEnd;
                case "Size": return _simulatorInterface.MainMemory.Size;
                case "Start": return _simulatorInterface.MainMemory.Start;
                case "HeapStart": return _simulatorInterface.MainMemory.HeapStart;
                case "HeapEnd": return _simulatorInterface.MainMemory.HeapEnd;
                default: throw new Exception("unknown property name: " + propertyName);
            }
        }
        /// <summary>
        /// access to current user directory. Pass thru to simulator interface
        /// </summary>
        string IARMHost.UserDirectory { get { return _simulatorInterface.UserDirectory; } }

        /// <summary>
        /// A plugin is requesting a panel from the user interface. Allocate a panel , size it to the
        /// requested size and locate it. This logic simply places the panel to the right of the last
        /// panel.
        /// </summary>
        /// <returns>Panel object created</returns>
        Panel IARMHost.RequestPanel(string title) { return mPluginsUIView.RequestPanel(title); }

        /// <summary>
        /// A plugin is requesting a range of memory be reserved for itself. Simply add it to the
        /// memoryMaps list. If the list does not exist, create it.
        /// </summary>
        /// <param name="baseAddress">base address of memory block</param>
        /// <param name="mask">the mask to apply for test</param>
        /// <param name="readDelegate">delegate of read function to execute on memory read</param>
        /// <param name="writeDelegate">delegate of write function to execute on memory write</param>
        void IARMHost.RequestMemoryBlock(uint baseAddress, uint mask, pluginMemoryAccessReadEventHandler readDelegate, pluginMemoryAccessWriteEventHandler writeDelegate)
        {
            mMemoryMaps.Add(new MemoryRange(baseAddress, mask, readDelegate, writeDelegate));
        }

        void IARMHost.RequestOpCodeRange(uint baseOpcode, uint mask, pluginInstructionExecuteEventHandler executeHandler)
        {
            mOpcodes.Add(new InstructionRange(baseOpcode, mask, executeHandler));
        }

        /// <summary>
        /// A plugin is asserting an interrupt line. Pass it to the simulator interface.
        /// </summary>
        /// <param name="fiqType">true - fiq, false - irq</param>
        void IARMHost.AssertInterrupt(bool fiqType)
        {
            _simulatorInterface.RequestException(fiqType ? BaseARMCore.ARMExceptions.FIQ : BaseARMCore.ARMExceptions.IRQ);
        }
       
        //These properties allow the plugins to subscribe to the simulator events. They are simplyadded
        //to the multicast delegate for that event.
        ///<summary>Called when the simulator is starting</summary>
        public event EventHandler<EventArgs> StartSimulation;
        ///<summary>Called when the simulator is stopping</summary>
        public event EventHandler<EventArgs> StopSimulation;
        ///<summary>Called when the simulator is restarting</summary>
        public event EventHandler<EventArgs> Restart;
        ///<summary>Called when the simulator is loading</summary>
        public event EventHandler<EventArgs> Load;
        ///<summary>Called when the simulator is unloading</summary>
        public event EventHandler<EventArgs> Unload;
        ///<summary>Called when the simulator has executed an instruction and needs to update the cycle count</summary>
        public event EventHandler<CyclesEventArgs> Cycles;

        private uint consoleCnt = 0;

        /// <summary>
        /// Creates a new standard console. A tab is created in the output view 
        /// with the title set to the one given.
        /// The console is added to the list of standard consoles and a handle
        /// returned to the caller.
        /// </summary>
        /// <param name="title">title of the new standard console</param>
        /// <returns>handle of console</returns>
        uint IARMHost.CreateStandardConsole(string title) {
            if (_outputView == null) return consoleCnt++;
            else  return _outputView.CreateStandardConsole(title);
        }

        /// <summary>
        /// Close a standard console. Removes it from the tab control and remove from the list
        /// of consoles.
        /// </summary>
        /// <param name="handle">handle of console to close</param>
        void IARMHost.CloseStandardConsole(uint handle) {
            if (_outputView != null) _outputView.CloseStandardConsole(handle);
        }

        /// <summary>
        /// Write a character to a standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <param name="ch">character to write</param>
        void IARMHost.WriteStandardConsole(uint handle, char chr) {
            if (_outputView != null)
                _outputView.WriteStandardConsole(handle, chr);
            else
            {
                Console.Write(chr);
            }
        }

        /// <summary>
        /// Read a character from the standard console.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        char IARMHost.ReadStandardConsole(uint handle) {
            if (_outputView != null)
                return _outputView.ReadStandardConsole(handle);
            else
                return (char)Console.Read();
        }

        /// <summary>
        /// Peek at the next character in the specified console.
        /// If none is available, the console waits for a keystroke.
        /// </summary>
        /// <param name="handle">handle of console</param>
        /// <returns>character read</returns>
        char IARMHost.PeekStandardConsole(uint handle) {
            if (_outputView != null)
                return _outputView.PeekStandardConsole(handle);
            else
                return (char)Console.Read();
        }

        //These methods are called by the simulator when an important event has occurred. If the subscriber
        //delegate is set, then invoke it. This will notify all subscribed plugins of the event.
        ///<summary>This is called when the simulator is restarting</summary>
        public void onRestart() { if (this.Restart != null) this.Restart((this as IARMHost), new EventArgs()); }
        ///<summary>This is called when the simulator is loading</summary>
        public void onLoad() { if (this.Load != null) this.Load((this as IARMHost), new EventArgs()); }
        ///<summary>This is called when the simulator is unloading</summary>
        public void onShutdown() { if (this.Unload != null) this.Unload((this as IARMHost), new EventArgs()); }
        ///<summary>This is called when the simulator is starting</summary>
        public void onStart() { if (this.StartSimulation != null) this.StartSimulation((this as IARMHost), new EventArgs()); }
        ///<summary>This is called when the simulator is stopping</summary>
        public void onStop() { if (this.StopSimulation != null) this.StopSimulation((this as IARMHost), new EventArgs()); }
        /// <summary>Call all the suscribers that want to be informed about cycles execcuted.</summary>
        public void onCycles(int count) { if (this.Cycles != null) this.Cycles((this as IARMHost), new CyclesEventArgs(count)); }//Cycles

        /// <summary>
        /// Scan thru the assembly for all types that match the types we are interested in. If we find one,
        /// then it is a plugin, create an instance of one and add to the appropriate collection.
        /// </summary>
        /// <param name="pluginAssembly"></param>
        private void loadFromAssembly(Assembly pluginAssembly)
        {
            try
            {
                foreach (Type type in pluginAssembly.GetTypes())
                {
                    //ARMPluginInterfaces.Utils.OutputDebugString("Found type:" + type.ToString());
                    if (type.GetInterface(typeof(IARMPlugin).ToString(), false) != null)
                    {

                        //if(type.GetCustomAttributes(typeof(PlugDisplayNameAttribute),false).Length!=1)
                        //ARMPluginInterfaces.Utils.OutputDebugString("Activating type:" + type.ToString());
                        //create an instance of the plugin
                        IARMPlugin plugin = (Activator.CreateInstance(type) as IARMPlugin);

                        PluginItem pluginItem = new PluginItem(Path.GetFileName(pluginAssembly.CodeBase), plugin);
                        mAvailablePlugins.Add(pluginItem);

                        //call the plugins init method, pass this IARMHost interface
                        //plugin.init(this);

                    }//if
                }//foreach
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Problems loading assembly:" + ex.Message);
            }

        }//loadFromAssembly

        public void DeactivateAllPlugins()
        {
            this.onShutdown();

            if (mPluginsUIView != null)
                mPluginsUIView.ResetTabs();

            foreach (PluginItem availableItem in mAvailablePlugins)
            {
                availableItem.Activated = false;
            }//foreach
            mMemoryMaps.Clear();
            mOpcodes.Clear();

            //in case the plugins dont't un-subscribe to events, clear them out
            this.Load = null;
            this.StartSimulation = null;
            this.StopSimulation = null;
            this.Restart = null;
            this.Unload = null;
            this.Cycles = null;

        }//DeactivateAllPlugins

        public void ActivatePlugins(ICollection<PluginSettingsItem> plugins)
        {
            foreach (PluginItem availableItem in mAvailablePlugins)
            {
                foreach (PluginSettingsItem settingsItem in plugins)
                {
                    if ((string.Compare(availableItem.armPlugin.PluginName, settingsItem.Name, true) == 0) &&
                        (string.Compare(availableItem.Assembly, settingsItem.Assembly, true) == 0))
                    {
                        availableItem.armPlugin.Init(this);
                        availableItem.Activated = true;
                        break;
                    }
                }//foreach
            }//foreach
            this.onLoad();
        }//ActivatePlugins

        /// <summary>
        /// Init the plugin Manager.
        /// Scan the Application directory for all assemblies. Pass each assembly off and determine if any 
        /// plugins are present. We also scan this assembly for plugins (the swi handler is one)
        /// Once all plugins are loaded, call the Load delegate to let all plugins know the load process is complete.
        /// </summary>
        /// <param name="boardControl"></param>
        /// <param name="outputView"></param>
        public void Init(GUI.Views.PluginsUIView pluginsUIView, GUI.Views.IOutputView outputView, PluginPreferences pluginPreferences)
        {
            //make sure the static variables are initialized before initing the plugins.
            //some plugins add instructions and require this module initalized.
            // ArmAssembly.ArmInstructionTemplate.ForceInitialization();

            //save the boardview and outputview interface
            mPluginsUIView = pluginsUIView;
            _outputView = outputView;

            try
            {
                //scan thru the dll's in the current directory and load each one. Then scan the loaded assembly and check
                //if any plugins of interest are in it.
                foreach (string file in Directory.GetFiles(System.Windows.Forms.Application.StartupPath, "*.dll"))
                {
                    ARMPluginInterfaces.Utils.OutputDebugString("Found plugin dll:" + file);
                    System.Reflection.Assembly pluginAssembly = System.Reflection.Assembly.LoadFrom(file);
                    loadFromAssembly(pluginAssembly);
                }//foreach

                //attempt to load any plugins defined in the running .exe
                loadFromAssembly(System.Reflection.Assembly.GetCallingAssembly());

                this.DeactivateAllPlugins();
                this.ActivatePlugins(pluginPreferences.SettingsPlugins);

            }//try
            catch (BadImageFormatException ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Application attempted to load a incorrectly formatted DLL: " + ex.Message);
            }
            catch (Exception ex)
            {
                ARMPluginInterfaces.Utils.OutputDebugString("Application failed to load a plugin assembly: " + ex.Message);
            }
        }//init

        /// <summary>
        /// An opcode that cannot be decoded has been encountered. Let the plugins
        /// have a shot at it.
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public uint onExecute(uint opcode)
        {
            //search the overridden opcodes and determine if a plugin has
            //overridden it. If so, execute its callback and return the result.
            foreach (InstructionRange ir in mOpcodes)
            {
                if (ir.hitTest(opcode))
                {
                    return ir.onExecute(opcode);
                }
            }
            return 0;
        }//onExecute

        /// <summary>
        /// A memory read has occurred and it is not in main memory.
        /// Let the plugins have a shot at it
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public uint MemoryRead(uint address, MemorySize ms)
        {
            //search the overridden memory and determine if a plugin has
            //overridden it. If so, execute its callback and return the result.
            foreach (MemoryRange mr in mMemoryMaps)
            {
                if (mr.hitTest(address))
                {
                    return mr.onRead((this as IARMHost), address, (MemorySize)ms);
                }
            }
            return 0;
        }//MemoryRead

        /// <summary>
        /// A write has occurred and it is not in main memory.
        /// Let the plugins have a shot at it.
        /// Return true if some plugin handles it, false otherwise
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <param name="data"></param>
        public void MemoryWrite(uint address, MemorySize ms, uint data)
        {
            foreach (MemoryRange mr in mMemoryMaps)
            {
                if (mr.hitTest(address))
                {
                    mr.onWrite((this as IARMHost), address, (MemorySize)ms, data);
                    return;
                }//if
            }//foreach
            throw new MemoryAccessException(address, true);
        }//MemoryWrite

        void IARMHost.RequestMnemonic(string mnemonic, uint code, string operands, pluginFormOpCodeEventHandler formOpcode)
        {
            //ArmAssembly.ArmInstructionPlugin.AddMnemonic(mnemonic, code, operands, formOpcode);
            throw new Exception("RequestMnemonic is unsupported");
        }

        /// <summary>
        /// Test if a given memory address and size has been requested by a plugin.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public bool InRange(uint address, ARMPluginInterfaces.MemorySize ms)
        {
            //search the overridden memory and determine if a plugin has
            //overridden it. If so, execute its callback and return the result.
            foreach (MemoryRange mr in mMemoryMaps)
            {
                if (mr.hitTest(address))
                {
                    return true;
                }
            }
            return false;
        }

        public void EnablePlugin(IARMPlugin plugin)
        {
            plugin.Init(this);
            this.onShutdown();
            this.onLoad();
        }

        public void HaltSimulation()
        {
            _simulatorInterface.HaltSimulation();
        }

        /// <summary>
        /// Return a read-only instance of ARMSim preferences
        /// </summary>
        IARMPreferences IARMHost.ARMPreferences { get { return _simulatorInterface.ARMPreferences; } }

        /// <summary>
        /// Return a read-only version of the commandline parameters (parsed)
        /// </summary>
        IARMSimArguments IARMHost.ARMSimArguments { get { return _simulatorInterface.ARMSimArguments; } }

    }//class PluginManager
}