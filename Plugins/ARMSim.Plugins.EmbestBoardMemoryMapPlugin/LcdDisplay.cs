using System;
using System.Collections.Generic;
using System.Text;

//using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class LcdDisplay
    {
        static uint LCD_ACTIVBUFFER = 0xc300000;
        //static uint LCD_SIZE_BYTES = 0x12c0;

        static uint PIXELS_WIDTH = 320;
        static uint PIXELS_HEIGHT = 240;
        static uint PIXELS_PER_WORD = 8;
        //static uint BITS_PER_PIXELS = 4;

        //static uint rLCDCON1=0x1f00000;
        //static uint rLCDCON2=0x1f00004;
        //static uint rLCDCON3=0x1f00040;
        //static uint rLCDSADDR1=0x1f00008;
        //static uint rLCDSADDR2=0x1f0000c;
        //static uint rLCDSADDR3=0x1f00010;
        //static uint rREDLUT=0x1f00014;
        //static uint rGREENLUT=0x1f00018;
        //static uint rBLUELUT=0x1f0001c;
        //static uint rDP1_2=0x1f00020;
        //static uint rDP4_7=0x1f00024;
        //static uint rDP3_5=0x1f00028;
        //static uint rDP2_3=0x1f0002c;
        //static uint rDP5_7=0x1f00030;
        //static uint rDP3_4=0x1f00034;
        //static uint rDP4_5=0x1f00038;
        //static uint rDP6_7=0x1f0003c;
        //static uint rDITHMODE = 0x1f00044;

        private int[] mLcdBuffer = new int[(PIXELS_WIDTH * PIXELS_HEIGHT) / PIXELS_PER_WORD];


        private IARMHost mIhost;
        private ARMSim.Plugins.UIControls.Lcd mLcd;
        public LcdDisplay(IARMHost ihost, ARMSim.Plugins.UIControls.Lcd lcd)
        {
            mIhost = ihost;
            mLcd = lcd;

            mIhost.RequestMemoryBlock(0x0c300000, 0xffff0000, onMemoryAccessRead, onMemoryAccessWrite);
            this.Restart();
        }

        public void Restart()
        {
            Array.Clear(mLcdBuffer, 0, mLcdBuffer.Length);
            mLcd.ClearScreen();
        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            if (mra.Size != MemorySize.Word)
                return 0;

            uint memOffset = (mra.Address - LCD_ACTIVBUFFER) / 4;
            if (memOffset >= mLcdBuffer.Length)
                return 0;

            return (uint)mLcdBuffer[memOffset];

        }

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            if (mwa.Size != MemorySize.Word)
                return;

            uint memOffset = (mwa.Address - LCD_ACTIVBUFFER) / 4;
            if (memOffset >= mLcdBuffer.Length)
                return;

            int ypos = (int)(memOffset / (PIXELS_WIDTH / PIXELS_PER_WORD));
            int xpos = (int)((memOffset % (PIXELS_WIDTH / PIXELS_PER_WORD))*8);

            int pixels = (int)mwa.Value;
            mLcdBuffer[memOffset] = pixels;
            //mLcd.DrawPixel8(xpos, ypos, (uint)pixels);
        }

    }
}
