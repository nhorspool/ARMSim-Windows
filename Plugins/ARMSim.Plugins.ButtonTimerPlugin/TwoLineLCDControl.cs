using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.ButtonTimerPlugin
{
    ///Certain assumptions about the LCD display and the simulation:
    ///1) 8 bit operation, 2 line and 5x7 point font are supported.
    ///   Writing to the "SET FUNCTION" command will do nothing.
    ///2) Only the ascii characters 0x20 - 0x7f are used
    ///   Characters 0x00-0x1f and 0x80-0xff will display a blank character.
    ///3) When the cursor moves right past the end of the display it will wrap
    ///   around and move to the next line.
    ///4) When reading the command register, the Read Busy flag(BF) will always be 0 (Ready state)
    ///5) When the display is OFF, commands to configure and output to the display work. When the display
    ///   is turned ON, any updates will be seen. Turning the display off simply powers off the display,
    ///   all the control logic is still active.
    ///6) Setting the CG RAM location has no effect (I don't know what CG RAM is???)

    /// <summary>
    /// Simulates a Truly LCD MODULE MTC-C162DPRN-2N
    /// http://www.electronics123.net/amazon/datasheet/MTC-C162DPRN-2N_V10_.pdf
    /// @http://www.electronics123.com/s.nl/it.A/id.53/.f
    /// 
    /// The display part of this control is implemented in TwoLineLCDDisplay
    /// </summary>
    public class TwoLineLCDControl
    {
        /// <summary>
        /// This is the base memory mapped address of the LCD display.
        /// There are 2 registers: Command and Data
        /// The Command register is mapped to the base address, the Data register
        /// is base address + 4
        /// </summary>
        private uint mBaseAddress = 0x02f00000;

        /// <summary>
        /// A reference to the simulation host that we can request services from.
        /// </summary>
        private IARMHost mHost;

        /// <summary>
        /// A reference to the lcd display part of this control. This object implements the graphical
        /// part of the control.
        /// </summary>
        private TwoLineLCDDisplay mTwoLineLCDDisplay;

        /// <summary>
        /// Control ctor.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="twoLineLCDDisplay"></param>
        public TwoLineLCDControl(IARMHost host, TwoLineLCDDisplay twoLineLCDDisplay)
        {
            mHost = host;
            mTwoLineLCDDisplay = twoLineLCDDisplay;

            //request to be notified when our area of the memory map is written or read
            //0x02f00000:Data Register
            //0x02f00004:Command Register
            mHost.RequestMemoryBlock(mBaseAddress, 0xfffffff0, onMemoryAccessRead, onMemoryAccessWrite);

            //get notified when the simulation is restarted so we can put display back to its initial state
            mHost.Restart += mHost_Restart;

            //and make sure display is init
            init();
        }//TwoLineLCDControl

        /// <summary>
        /// Called when the simulation is restarted
        /// We need to make sure display is in the initial state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void mHost_Restart(object sender, EventArgs args)
        {
            init();
        }//mHost_Restart

        /// <summary>
        /// Set display to intitial power-on state
        /// </summary>
        private void init()
        {
            mTwoLineLCDDisplay.ClearLCD();
            mTwoLineLCDDisplay.CurrentCursorLocation.Home();
            mTwoLineLCDDisplay.CursorDirectionRight = true;
            mTwoLineLCDDisplay.DisplayShifted = false;
            mTwoLineLCDDisplay.CursorBlinking = false;
            mTwoLineLCDDisplay.CursorEnabled = false;
            mTwoLineLCDDisplay.DisplayEnabled = false;
            mTwoLineLCDDisplay.Invalidate();
        }//init

        /// <summary>
        /// This function is called when a memory read has occurred on either the Command or Data register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mra"></param>
        /// <returns></returns>
        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            //we only allow word access
            if (mra.Size == MemorySize.Word)
            {
                //determine which register was accessed and handle it
                if (mra.Address == mBaseAddress)
                    //The Command register. Return the current cursor location
                    return mTwoLineLCDDisplay.CurrentCursorLocation.ToInt();
                else
                    //The Data register. Return the character at the current cursor location
                    return (uint)mTwoLineLCDDisplay.GetChar();
            }
            return 0;
        }//onMemoryAccessRead

        /// <summary>
        /// This function is called when a memory write has occurred on either the Command or Data register.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mwa"></param>
        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            //we only allow word access
            if (mwa.Size == MemorySize.Word)
            {
                //determine which register was accessed and handle it
                if (mwa.Address == mBaseAddress)
                    CommandRegisterWrite(mwa.Value);
                else
                    //The Data register. Write character to the current cursor location
                    mTwoLineLCDDisplay.PutChar((int)(mwa.Value & 0x7f));

            }
            mTwoLineLCDDisplay.Invalidate();
        }//onMemoryAccessWrite

        /// <summary>
        /// Handles a write to the Command register. See page 5 of the data sheet for the list
        /// of bit locations and meanings.
        /// </summary>
        /// <param name="cmd"></param>
        private void CommandRegisterWrite(uint cmd)
        {
            //Set cursor location
            if (TestBit(cmd, 0x80))
            {
                mTwoLineLCDDisplay.CurrentCursorLocation.SetCursorPosition(cmd);
            }
            //Display or Cursor shift
            else if (TestBit(cmd, 0x10))
            {
                //determine shift direction
                bool shiftRight = TestBit(cmd, 0x04);

                //and either shift the cursor or display
                if (TestBit(cmd, 0x08))
                    mTwoLineLCDDisplay.ShiftDisplay(shiftRight);
                else
                    mTwoLineLCDDisplay.CurrentCursorLocation.Shift(shiftRight);
            }
            //Display On/Off. Cursor On/Off. Cursor blink
            else if (TestBit(cmd, 0x08))
            {
                mTwoLineLCDDisplay.DisplayEnabled = TestBit(cmd, 0x04);
                mTwoLineLCDDisplay.CursorEnabled =  TestBit(cmd, 0x02);
                mTwoLineLCDDisplay.CursorBlinking = TestBit(cmd, 0x01);
            }//if
            //Entry Mode Set. Cursor direction. Display shift
            else if (TestBit(cmd, 0x04))
            {
                mTwoLineLCDDisplay.CursorDirectionRight = TestBit(cmd, 0x02);
                mTwoLineLCDDisplay.DisplayShifted = TestBit(cmd, 0x01);
            }
            //Return cursor to home position
            else if (TestBit(cmd, 0x02))
            {
                mTwoLineLCDDisplay.CurrentCursorLocation.Home();
            }
            //Clear display
            else if (TestBit(cmd, 0x01))
            {
                mTwoLineLCDDisplay.ClearLCD();
            }

        }//CommandRegisterWrite

        /// <summary>
        /// Helper function to test a bit and return a bool
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        private static bool TestBit(uint cmd, uint bit)
        {
            return ((cmd & bit) != 0);
        }

    }//class TwoLineLCDControl
}
