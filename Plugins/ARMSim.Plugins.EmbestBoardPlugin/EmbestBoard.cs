using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardPlugin
{
    class EmbestBoard : IARMPlugin
    {
        private IARMHost mIHost;

        [Flags]
        public enum LedEnum
        {
            LeftLed = 0x02,
            RightLed = 0x01
        };

        private ARMSim.Plugins.UIControls.EmbestBoard mEmbestBoardControl;

        /// <summary>
        /// This property is the Name of the plugin. Plugins names must be unique in the host assembly.
        /// </summary>
        public string PluginName { get { return "EmbestBoardPlugin"; } }

        /// <summary>
        /// This property is the Description string of the plugin. This can be any text that describes
        /// the plugin.
        /// </summary>
        public string PluginDescription { get { return "Simulates the Embest S3CE40 development board"; } }

        /// <summary>
        /// The init function is called once the plugin has been loaded.
        /// From this function you can subscribe to the events the
        /// simulator supports.
        /// </summary>
        /// <param name="IHost"></param>
        public void Init(IARMHost IHost)
        {
            mIHost = IHost;
            mIHost.Load += onLoad;
            mIHost.Restart += onRestart;
        }//init

        /// <summary>
        /// The onLoad function is called after all the plugins have been loaded and their
        /// init methods called.
        /// </summary>
        public void onLoad(object sender, EventArgs e)
        {

            mEmbestBoardControl = new ARMSim.Plugins.UIControls.EmbestBoard();

            Panel panel = mIHost.RequestPanel(this.PluginName);
            panel.Controls.Add(mEmbestBoardControl);

            //request that we get all SWI calls for the range 0x0200 - 0x0x2ff
            mIHost.RequestOpCodeRange(0x0f000200, 0x0fffff00, this.onExecute);

        }//onLoad

        /// <summary>
        /// Called when the simulation has been restarted.
        /// Reset all the controls to their init state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onRestart(object sender, EventArgs e)
        {
            //blank the 8 segment display
            mEmbestBoardControl.EightSegmentDisplay.Code = 0;

            //Turn off both LEDs
            mEmbestBoardControl.Leds.LeftLed = false;
            mEmbestBoardControl.Leds.RightLed = false;

            //clear any button presses
            mEmbestBoardControl.BlackButtons.CheckPressed();
            mEmbestBoardControl.BlueButtons.CheckPressed();

            //Clear the lcd screen
            mEmbestBoardControl.Lcd.ClearScreen();
            mEmbestBoardControl.Lcd.Invalidate();
        }

        //called when our requested opcode(s) have been fetched and need executing
        //In our case, any swi 0x200   to    swi 0x2ff
        public uint onExecute(uint opcode)
        {
            try
            {
                switch (opcode & 0x000002ff)
                {
                    //set eight segment display (r0 - segment pattern)
                    case 0x200:
                        {
                            //get code to display from R0
                            byte code = (byte)mIHost.getReg(0);
                            mEmbestBoardControl.EightSegmentDisplay.Code = code;
                        }
                        break;

                    case 0x201:
                        {
                            uint code = mIHost.getReg(0);
                            mEmbestBoardControl.Leds.LeftLed = ((code & (uint)LedEnum.LeftLed) == (uint)LedEnum.LeftLed);
                            mEmbestBoardControl.Leds.RightLed = ((code & (uint)LedEnum.RightLed) == (uint)LedEnum.RightLed);
                        } break;

                    //check black buttons
                    //returns r0 : bit0:left button, bit1:right button
                    case 0x202:
                        {
                            mIHost.setReg(0, mEmbestBoardControl.BlackButtons.CheckPressed());
                        } break;

                    //check blue buttons
                    //returns r0 : low 16 bits contains button(s) pressed
                    case 0x203:
                        {
                            mIHost.setReg(0, mEmbestBoardControl.BlueButtons.CheckPressed());
                        } break;

                    //SWI_DrawLcdString
                    //R0:xpos
                    //R1:ypos
                    //R2:string to print(null terminated)
                    case 0x204:
                        {
                            uint xpos = mIHost.getReg(0);
                            uint ypos = mIHost.getReg(1);
                            string str = Utils.loadStringFromMemory(mIHost, mIHost.getReg(2), 256);
                            mEmbestBoardControl.Lcd.PrintString(xpos, ypos, str);
                            mEmbestBoardControl.Lcd.Invalidate();
                        } break;
                    //SWI_DrawLcdInt
                    //R0:xpos
                    //R1:ypos
                    //R2:int to print
                    case 0x205:
                        {
                            uint xpos = mIHost.getReg(0);
                            uint ypos = mIHost.getReg(1);
                            string str = mIHost.getReg(2).ToString();
                            mEmbestBoardControl.Lcd.PrintString(xpos, ypos, str);
                            mEmbestBoardControl.Lcd.Invalidate();
                        } break;
                    //ClearLCDDisplay
                    case 0x206:
                        {
                            mEmbestBoardControl.Lcd.ClearScreen();
                            mEmbestBoardControl.Lcd.Invalidate();
                        } break;
                    //DrawLCDCharacter
                    //R0:xpos
                    //R1:ypos
                    //R2:char
                    case 0x207:
                        {
                            uint xpos = mIHost.getReg(0);
                            uint ypos = mIHost.getReg(1);
                            char ch = (char)mIHost.getReg(2);
                            mEmbestBoardControl.Lcd.PrintChar(xpos, ypos, ch);
                            mEmbestBoardControl.Lcd.Invalidate();
                        } break;
                    //ClearLCDLine
                    //R0:line#
                    case 0x208:
                        {
                            uint ypos = mIHost.getReg(0);
                            mEmbestBoardControl.Lcd.ClearLine(ypos);
                            mEmbestBoardControl.Lcd.Invalidate();
                        } break;

                    ////Draw Pixel
                    ////R0:xpos
                    ////R1:ypos
                    ////R2:0 - erase, 1 - draw
                    //case 0x209:
                    //    {
                    //        int xpos = (int)mIHost.getReg(0);
                    //        int ypos = (int)mIHost.getReg(1);
                    //        bool erase = (mIHost.getReg(2) == 0);
                    //        mEmbestBoardControl.Lcd.DrawPixel(xpos, ypos, erase);
                    //    } break;

                    //Return 15bit timer
                    case 0x20a:
                        {
                            uint ticks = (uint)(DateTime.Now.Ticks / 10000);
                            ticks = (ticks & 0x00007fff);
                            mIHost.setReg(0, ticks);
                        } break;

                    default:
                        break;
                }//switch
            }//try
            catch (Exception ex)
            {
                uint where = mIHost.getReg(15) - 0x04;
                mIHost.OutputConsoleString("An error occurred in {0} at 0x{1:X8}\nReason:{2}", this.PluginName, where, ex.Message);
                mIHost.setReg(15, where);
            }//catch
            return 3;
        }//onExecute
    }
}
