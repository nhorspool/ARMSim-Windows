using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces
{
    /// <summary>
    /// This interface is implemented by plugins. An assembly that is in the Application
    /// directory of the simulator will be scanned for objects implementing this interface. 
    /// Once found they are loaded and the init method is called with a reference to the
    /// simulators host interface. Plugins can make calls on the host interface to subscribe
    /// to simulator events and make changes to the simulator state.
    /// </summary>
    public interface IARMPlugin
    {
        /// <summary>
        /// The name property identifies the plugin in the preferences interface
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// A nice description to let the user know what the plugin does.
        /// Displayed in the preferences interface
        /// </summary>
        string PluginDescription { get; }

        /// <summary>
        /// The init method called when the plugin is loaded. Pass plugin a reference
        /// back to the simulator.
        /// </summary>
        /// <param name="iHost"></param>
        void Init(IARMHost iHost);

    }//interface IARMPlugin
}
