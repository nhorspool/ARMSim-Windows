using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Simulator
{
    /// <summary>
    /// Maintains a list of breakpoints for the simulation engine.
    /// </summary>
    public class Breakpoints
    {
        private ICollection<uint> mBreakpoints = new HashSet<uint>();

        /// <summary>
        /// Clear all breakpoints from the list
        /// </summary>
        public void Clear()
        {
            mBreakpoints.Clear();
        }//Clear

        /// <summary>
        /// If a breakpoint is at the specified address, toggle it.
        /// </summary>
        /// <param name="address"></param>
        public void Toggle(uint address)
        {
            if (mBreakpoints.Contains(address))
                mBreakpoints.Remove(address);
            else
                mBreakpoints.Add(address);
        }//Toggle

        /// <summary>
        /// Determine if a breakpoint exists at the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool AtBreakpoint(uint address)
        {
            return mBreakpoints.Contains(address);
        }//AtBreakpoint

    }//class Breakpoints
}
