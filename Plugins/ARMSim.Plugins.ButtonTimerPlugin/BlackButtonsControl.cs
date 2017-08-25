using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public class BlackButtonsControl
    {
        /// <summary>
        /// This is the base memory mapped address of the black buttons.
        /// When read, this register provides an indication of which black
        /// button was depressed that caused the interrupt.
        /// When written to, the button indication bits are reset.
        /// Writing a '1' into a bit position will clear it.
        /// </summary>
        private uint mBaseAddress = 0x01d00000;

        /// <summary>
        /// This word implements the button status register.
        /// </summary>
        private uint mBlackButtonID;

        /// <summary>
        /// A reference to the simulation host that we can request services from.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// A reference to the black button display part of this control. This object implements the graphical
        /// part of the control.
        /// </summary>
        private BlackButtonsDisplay mBlackButtonsDisplay;

        /// <summary>
        /// Control ctor.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="BlackButtonsDisplay"></param>
        public BlackButtonsControl(IARMHost host, BlackButtonsDisplay blackButtonsDisplay)
        {
            mHost = host;
            mBlackButtonsDisplay = blackButtonsDisplay;
            mBlackButtonsDisplay.Notify += BlackButtonPressNotify;

            //request to be notified when our area of the memory map is written or read
            mHost.RequestMemoryBlock(mBaseAddress, 0xffffffff, onMemoryAccessRead, onMemoryAccessWrite);

            //get notified when the simulation is restarted so we can put buttons back to its initial state
            mHost.Restart += mHost_Restart;

            //and make sure display is init
            init();
        }

        /// <summary>
        /// Called when the simulation is restarted
        /// We need to make sure buttons are in the initial state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }//mHost_Restart

        /// <summary>
        /// Set buttons to intitial power-on state
        /// </summary>
        private void init()
        {
            mBlackButtonID = 0;
        }//init

        /// <summary>
        /// This function is called when a memory read has occurred on the button id register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            //we only allow word access
            if (mra.Size == MemorySize.Word)
            {
                return mBlackButtonID;
            }
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when a memory write has occurred on the button id register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mwa"></param>
        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            //we only allow word access
            if (mwa.Size == MemorySize.Word)
            {
                //clear bits where a '1' is written
                mBlackButtonID &= (uint)(~mwa.Value);
            }
        }//onMemoryAccessWrite

        /// <summary>
        /// Button press notification.
        /// This is called when the user presses the graphical button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BlackButtonPressNotify(object sender, BlackButtonEventArgs args)
        {
            // set the id of the button that was pressed
            mBlackButtonID |= (uint)((int)args.Button & 0x03);

            //assert the interrupt in the simulation (false means IRQ)
            mHost.AssertInterrupt(false);
        }//BlackButtonPressNotify

    }//class BlackButtonsControl
}