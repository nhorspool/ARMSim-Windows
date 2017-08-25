using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public delegate void timerNotify(InteruptController.InterruptTokens notifyToken);
    public class PWMTimer
    {
        private readonly InteruptController.InterruptTokens mBIT_TIMER0 = InteruptController.InterruptTokens.BIT_TIMER0;
        private readonly InteruptController.InterruptTokens mBIT_TIMER1 = InteruptController.InterruptTokens.BIT_TIMER1;
        private readonly InteruptController.InterruptTokens mBIT_TIMER2 = InteruptController.InterruptTokens.BIT_TIMER2;
        private readonly InteruptController.InterruptTokens mBIT_TIMER3 = InteruptController.InterruptTokens.BIT_TIMER3;
        private readonly InteruptController.InterruptTokens mBIT_TIMER4 = InteruptController.InterruptTokens.BIT_TIMER4;
        private readonly InteruptController.InterruptTokens mBIT_TIMER5 = InteruptController.InterruptTokens.BIT_TIMER5;

        private TimerBlock[] _timerBlocks = new TimerBlock[3];


        private IARMHost mIhost;
        private InteruptController mInteruptController;
        public PWMTimer(IARMHost ihost, InteruptController interuptController)
        {
            mIhost = ihost;
            mInteruptController = interuptController;

            //request to be notified when our area of the memory map is written or read
            mIhost.RequestMemoryBlock(0x01d50000, 0xffffff80, onMemoryAccessRead, onMemoryAccessWrite);
            mIhost.Cycles += onClock;
            this.Restart();
        }

        public void Restart()
        {
            //create the 3 main timer blocks, flags indicate:
            //has32Divider, hasInverter, hasDeadzone
            //timer5 in timerblock 2 has no 1/32 clock divider
            //timer5 in timerblock 2 does not have inverted output
            //timer0 in timerblock 0 has a deadzone generator
            _timerBlocks[0] = new TimerBlock(true, true, true, this.TimerFired, mBIT_TIMER0, mBIT_TIMER1);
            _timerBlocks[1] = new TimerBlock(true, true, false, this.TimerFired, mBIT_TIMER2, mBIT_TIMER3);
            _timerBlocks[2] = new TimerBlock(false, true, false, this.TimerFired, mBIT_TIMER4, mBIT_TIMER5);
        }

        private void onClock(object sender, CyclesEventArgs e)
        {
            uint cycles = (uint)e.CycleCount;
            //clock cycles has occurred, lets notify the 3 timer blocks
            _timerBlocks[0].onClock(cycles);
            _timerBlocks[1].onClock(cycles);
            _timerBlocks[2].onClock(cycles);
        }

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            return 0;
        }

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            switch (mwa.Address)
            {
                case 0x01d50000:
                    {//TCFG0
                        if (mwa.Size != MemorySize.Word)
                            break;
                        _timerBlocks[0].PreScaler = (byte)(mwa.Value & 0xff);
                        _timerBlocks[1].PreScaler = (byte)(mwa.Value >> 8 & 0xff);
                        _timerBlocks[2].PreScaler = (byte)(mwa.Value >> 16 & 0xff);
                    } break;
                case 0x01d50004:
                    {//TCFG1
                        if (mwa.Size != MemorySize.Word)
                            break;
                        _timerBlocks[0].MUX = (byte)(mwa.Value & 0xff);
                        _timerBlocks[1].MUX = (byte)(mwa.Value >> 8 & 0xff);
                        _timerBlocks[2].MUX = (byte)(mwa.Value >> 16 & 0xff);
                    } break;
                case 0x01d50008:
                    {//TCON
                        if (mwa.Size != MemorySize.Word)
                            break;
                        _timerBlocks[0].SetFlags((mwa.Value & 0x1f), (mwa.Value >> 8 & 0x0f));
                        _timerBlocks[1].SetFlags((mwa.Value >> 12 & 0x0f), (mwa.Value >> 16 & 0x0f));
                        _timerBlocks[2].SetFlags((mwa.Value >> 20 & 0x0f), (mwa.Value >> 24 & 0x07));

                    } break;
                default:
                    {//TCNTBx,TCMPBx,TCNTOx
                        if (mwa.Size == MemorySize.Byte)
                            break;
                        uint baseAddress = (mwa.Address - 0x01d5000c);
                        int timer = (int)((baseAddress / 12) % 2);
                        int timerBlock = timer >> 1;
                        if (timerBlock >= _timerBlocks.Length) return;
                        _timerBlocks[timerBlock].SetTimerCounts(timer, (baseAddress % 12), (mwa.Value & 0x0000ffff));
                    } break;//default
            }//switch
        }//onMemoryAccessWrite

        private void TimerFired(InteruptController.InterruptTokens notifyToken)
        {
            mInteruptController.InteruptNotify(notifyToken);
        }


    }//class PWMTimer
}
