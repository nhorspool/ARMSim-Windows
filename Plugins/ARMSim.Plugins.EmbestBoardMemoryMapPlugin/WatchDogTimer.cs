using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public delegate void wdgNotify(uint notifyToken);
    public class WatchDogTimer
    {
        private uint    _prescaler;
        private uint    _divisor;
        private uint    _wtcon;
        private uint    _wtdat;
        private uint    _wtcnt;

        private uint    _currentCycles;

        private bool    _wdgEnable;
        private bool    _intEnable;
        private bool    _rstEnable;

        private readonly InteruptController.InterruptTokens mToken = InteruptController.InterruptTokens.BIT_WDT;

        private IARMHost mIhost;
        private InteruptController _interuptController;
        public WatchDogTimer(IARMHost ihost, InteruptController interuptController)
        {
            mIhost = ihost;
            _interuptController = interuptController;

            //request to be notified when our area of the memory map is written or read
            mIhost.RequestMemoryBlock(0x01d30000, 0xfffffff0, onMemoryAccessRead, onMemoryAccessWrite);
            mIhost.Cycles += onClock;
            this.Restart();
        }

        public void Restart()
        {
            _prescaler = 0x80;
            _wtcnt = _wtdat = 0x8000;
            _wtcon = 0x00008021;
            _wdgEnable = true;
            _intEnable = false;
            _rstEnable = true;
            _divisor = 16;
            this.ResetCurrentCycles();
        }

        private void onClock(object sender, CyclesEventArgs e)
        {
            uint cycles = (ushort)e.CycleCount;
            if (!_wdgEnable)
                return;

            if (_currentCycles > cycles)
            {
                _currentCycles -= cycles;
                return;
            }

            cycles -= (ushort)_currentCycles;
            this.ResetCurrentCycles();
            _currentCycles -= cycles;

            if (!_intEnable)
                return;

            _interuptController.InteruptNotify(mToken);

        }//onClock

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            if (mra.Size == MemorySize.Byte) return 0;
            switch (mra.Address)
            {
                case 0x01d30000:
                    //wtcon
                    return _wtcon;
                case 0x01d30004:
                    //wtdat
                    return _wtdat;
                case 0x01d30008:
                    //wtcnt
                    return _currentCycles / ((_prescaler + 1) * (uint)Math.Pow(2, _divisor));
            }//switch
            return 0;
        }//onMemoryAccessRead

        //reset currentCycles to its start value based on the prescaler value
        //and the divisor value
        private void ResetCurrentCycles()
        {
            _currentCycles = _wtdat * ((_prescaler + 1) * (uint)Math.Pow(2, _divisor));
        }

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            if (mwa.Size == MemorySize.Byte) return;
            switch (mwa.Address)
            {
                case 0x01d30000:
                    {//WTCON
                        _wtcon = (mwa.Value & 0x0000ffff);
                        _prescaler = (_wtcon & 0x0000f000) >> 8;
                        _wdgEnable = Utils.bitsSet(_wtcon, 0x00000020);
                        _intEnable = Utils.bitsSet(_wtcon, 0x00000004);
                        _rstEnable = Utils.bitsSet(_wtcon, 0x00000001);

                        uint code = ((_wtcon & 0x18) >> 3) & 0x03;
                        _divisor = (uint)Math.Pow(2.0, (code + 4));

                        this.ResetCurrentCycles();

                    } break;
                case 0x01d30004:
                    {//WTDAT
                        _wtdat = mwa.Value;
                        this.ResetCurrentCycles();
                    } break;
                case 0x01d30008:
                    {//WTCNT
                        _wtcnt = mwa.Value;
                        this.ResetCurrentCycles();
                    } break;
            }//switch
        }//onMemoryAccessWrite

    }//class WatchDogTimer
}
