using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    public class TimerControl
    {
        /// <summary>
        /// This class represents 1 timer of the timer control.
        /// </summary>
        private class TimerBlock
        {
            /// <summary>
            /// Signature of the callback to call when a timer has counted down to 0
            /// </summary>
            /// <param name="sender"></param>
            public delegate void TimerNotifyDelegate(TimerBlock sender);

            /// <summary>
            /// Event to set of function to call when timer counts down to 0
            /// </summary>
            public event TimerNotifyDelegate TimerNotify;

            /// <summary>
            /// The prescaler value (0-255).
            /// </summary>
            private byte mPrescaler;

            /// <summary>
            /// This is a combination of the prescaler and the Data register. This is the value that is
            /// decremented by cycles and when it reaches 0 the timer fires.
            /// </summary>
            private uint mCurrentCycles;

            /// <summary>
            /// The timer interrupt enable flag
            /// </summary>
            private bool mInterruptEnable;

            /// <summary>
            /// The timer enebale flag. Note that when this is false the timer does not count down.
            /// </summary>
            private bool mTimerEnable;

            /// <summary>
            /// The current value of the control register. Note it is only 16 bits
            /// </summary>
            private ushort mControlRegister;

            /// <summary>
            /// Access to the control regester.
            /// A read simply returns the last value written.
            /// A write will set the control values
            /// </summary>
            public ushort ControlRegister
            {
                get { return mControlRegister; }
                set
                {
                    mControlRegister = value;

                    //extract the prescaler value from the top 8 bits
                    mPrescaler = (byte)((mControlRegister & 0x0000ff00) >> 8);

                    //timer enable flag from bit5
                    mTimerEnable = Utils.bitsSet(mControlRegister, 0x00000020);

                    //interrupt enable from bit 2
                    mInterruptEnable = Utils.bitsSet(mControlRegister, 0x00000004);

                    //compute a new cycles value from the prescaler and the Data register
                    ResetCurrentCycles();
                }//set
            }//ControlRegister

            /// <summary>
            /// The Data register is the data register. This value is auto-loaded into the
            /// current count register when the timer counts down to 0.
            /// </summary>
            public ushort DataRegister { get; set; }

            /// <summary>
            /// The current count register holds the timers current value. When read it will return
            /// the current clock cycle count. Note that the timer counts down.
            /// When written to it will simply recompute the clock cycles and start over.
            /// </summary>
            public ushort CountRegister
            {
                get
                {
                    uint cs = mCurrentCycles;
                    uint ps = (uint)(mPrescaler + 1);
                    uint result = cs / ps;
                    return (ushort)result;
                }//get
                set
                {
                    ResetCurrentCycles();
                }//set
            }//CountRegister

            /// <summary>
            /// The simulation has restarted, set all registers to default state.
            /// </summary>
            public void onRestart()
            {
                mPrescaler = 0x80;
                DataRegister = 0x8000;
                ControlRegister = 0x8000;
                mInterruptEnable = false;
                mTimerEnable = false;
                ResetCurrentCycles();
            }//onRestart

            /// <summary>
            /// Reset currentCycles to its start value based on the prescaler value
            /// and the divisor value
            /// </summary>
            private void ResetCurrentCycles()
            {
                mCurrentCycles = (uint)(DataRegister * (mPrescaler + 1));
            }

            public void onCycles(uint cycles)
            {
                if (!mTimerEnable)
                    return;

                while (cycles > 0)
                {
                    if (mCurrentCycles > cycles)
                    {
                        mCurrentCycles -= cycles;
                        break;
                    }

                    cycles -= (ushort)mCurrentCycles;
                    ResetCurrentCycles();
                    //mCurrentCycles -= cycles;

                    if (mInterruptEnable && this.TimerNotify != null)
                        this.TimerNotify(this);
                }

            }//onClock

        }//class TimerBlock

        /// <summary>
        /// The 2 timers this control has
        /// </summary>
        private TimerBlock[] mTimerBlocks = new TimerBlock[2];

        /// <summary>
        /// The timer interrupt id register. This holds the timer that caused the FIQ interrupt
        /// to fire. The user code inspects this register to determine which timer caused the interrupt.
        /// </summary>
        private uint mInterruptID;

        /// <summary>
        /// Used for thottling the user interface updates while running.
        /// </summary>
        private DateTime mLastUpdate = DateTime.Now;

        /// <summary>
        /// Flag indicating if the simulator is in a running state
        /// </summary>
        private bool mSimulationRunning;

        /// <summary>
        /// Reference to the ARMSim host interface. Aquired in the Init method.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// The user control that houses the timers user interface
        /// </summary>
        private ButtonTimerDisplay mButtonTimerDisplay;

        /// <summary>
        /// Construct a timer control.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="buttonTimerDisplay"></param>
        public TimerControl(IARMHost host, ButtonTimerDisplay buttonTimerDisplay)
        {
            //cache the simulator host interface and the timers user interface
            mHost = host;
            mButtonTimerDisplay = buttonTimerDisplay;

            //subscribe to the Start and Stop events so we can track when the simulation is running
            mHost.StartSimulation += mHost_Start;
            mHost.StopSimulation += mHost_Stop;

            //Create the 2 timers and set their notification events
            mTimerBlocks[0] = new TimerBlock();
            mTimerBlocks[1] = new TimerBlock();
            mTimerBlocks[0].TimerNotify += TimerNotify;
            mTimerBlocks[1].TimerNotify += TimerNotify;

            //request to be notified when our area of the memory map is written or read
            mHost.RequestMemoryBlock(0x01d30000, 0xffffff00, onMemoryAccessRead, onMemoryAccessWrite);

            //request to be notified when cycles are expended so we can update the timers
            mHost.Cycles += mHost_Cycles;

            //request simulator restart notification so we can reset the timers to their default state
            mHost.Restart += mHost_Restart;

            //force timers into default state.
            init();
        }

        /// <summary>
        /// Force timers into a default state.
        /// </summary>
        private void init()
        {
            mTimerBlocks[0].onRestart();
            mTimerBlocks[1].onRestart();
            //cancel any pending timer bits
            mInterruptID = 0;

            //update the user interface with new timer data
            UpdateUserInterface(false);
        }//init

        /// <summary>
        /// This function is called when the simulator executes an instruction and
        /// is notifying the interested plugins that cycles have been expended.
        /// Pass this on to the 2 individual timers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Cycles(object sender, CyclesEventArgs args)
        {
            uint count = (uint)args.CycleCount;
            mTimerBlocks[0].onCycles(count);
            mTimerBlocks[1].onCycles(count);
            UpdateUserInterface(false);
        }//mHost_Cycles

        /// <summary>
        /// This function is called when the simulator has been restarted by the user.
        /// Reset the timers to their default state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }//mHost_Restart

        /// <summary>
        /// Helper function to set the user interface elements based on the state of the timers.
        /// If the simulation is running then throttle the updates to about 5 times per second.
        /// The force flag is for callers who dont want to wait.
        /// </summary>
        private void UpdateUserInterface(bool force)
        {
            if (((DateTime.Now - mLastUpdate).TotalMilliseconds < 200) && !force && mSimulationRunning)
                return;

            mLastUpdate = DateTime.Now;

            mButtonTimerDisplay.Timer1Value = mTimerBlocks[0].DataRegister;
            mButtonTimerDisplay.Timer1Control = mTimerBlocks[0].ControlRegister;
            mButtonTimerDisplay.Timer1Count = mTimerBlocks[0].CountRegister;
            mButtonTimerDisplay.Timer2Value = mTimerBlocks[1].DataRegister;
            mButtonTimerDisplay.Timer2Control = mTimerBlocks[1].ControlRegister;
            mButtonTimerDisplay.Timer2Count = mTimerBlocks[1].CountRegister;
        }//UpdateUserInterface

        /// <summary>
        /// This function is called when the memory locations declared by the timers has been read.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            // only word access allowed
            if (mra.Size != MemorySize.Word)
                return 0;

            //determine the timer block number from bit 4
            int block = ((int)mra.Address >> 4) & 0x01;

            //and access the timer based on the address (bits 0-3)
            switch (mra.Address & 0x0f)
            {
                case 0x00://wtcon
                    return mTimerBlocks[block].ControlRegister;
                case 0x04://wtdat
                    return mTimerBlocks[block].DataRegister;
                case 0x08://wtcnt
                    return mTimerBlocks[block].CountRegister;
                case 0x0c://intID
                    return mInterruptID;
                default: break;
            }//switch
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when the memory locations declared by the timers has been written.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mwa"></param>
        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            // only word access allowed
            if (mwa.Size != MemorySize.Word)
                return;

            //determine the timer block number from bit 4
            int block = ((int)mwa.Address >> 4) & 0x01;

            //grab the value being written, only bottom 16 bits
            ushort value = (ushort)(mwa.Value & 0x0000ffff);

            //and access the timer based on the address (bits 0-3)
            switch (mwa.Address & 0x0f)
            {
                case 0x00://WTCON
                    mTimerBlocks[block].ControlRegister = value;
                    break;
                case 0x04://WTDAT
                    mTimerBlocks[block].DataRegister = value;
                    break;
                case 0x08://WTCNT
                    mTimerBlocks[block].CountRegister = value;
                    break;
                case 0x0c://intID
                    mInterruptID &= (uint)(~value);
                    break;
                default: break;

            }//switch

            //user interface needs updating
            this.UpdateUserInterface(false);

        }//onMemoryAccessWrite

        /// <summary>
        /// The Start event is fired when the simulation starts running. We will set our internal flag true
        /// </summary>
        private void mHost_Start(object sender, EventArgs e)
        {
            mSimulationRunning = true;
        }//mHost_Start

        /// <summary>
        /// The Stop event is fired when the simulation stops running. We will set our internal flag false.
        /// This can happen if the simulations executes the halt swi instruction (swi 0x11), hits a breakpoint
        /// or encounters a severe error.
        /// </summary>
        private void mHost_Stop(object sender, EventArgs e)
        {
            mSimulationRunning = false;

            //make sure user interface is current, force a refresh
            UpdateUserInterface(true);
        }//mHost_Stop

        /// <summary>
        /// This function is called when one of the timers has counted down to 0,
        /// is enabled and has the interrupt enable bit set.
        /// Determine which timer it was, set the timer interrupt bit in the id 
        /// register and finally notify the simulator we wish to assert the FIQ interrupt.
        /// </summary>
        /// <param name="timerBlock"></param>
        private void TimerNotify(TimerBlock timerBlock)
        {
            // set id bit based on which timer fired
            int id = (timerBlock == mTimerBlocks[0]) ? 0x01 : 0x02; 

            //set the bit in the ID register
            mInterruptID |= (uint)(id & 0x03);

            //notify simulator to fire FIQ interrupt.
            mHost.AssertInterrupt(true);
        }//TimerNotify

    }//class TimerControl
}