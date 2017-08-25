using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public class TimerBlock
    {
        private uint _prescaler;
        private uint[] _divisor;

        private uint[] _tcmpx;
        private uint[] _tcntx;
        private uint[] _currentCycles;

        private timerNotify _notify = null;
        private InteruptController.InterruptTokens[] _notifyTokens;

        private bool[] _started;
        private bool[] _manualUpdate;
        private bool[] _outputInvert;
        private bool[] _autoReload;
        private bool[] _deadZoneEnable;

        private readonly bool _has32Divider;
        private readonly bool _hasInverter;
        private readonly bool _hasDeadzone;
        public TimerBlock(bool has32Divider, bool hasInverter, bool hasDeadzone, timerNotify notify, InteruptController.InterruptTokens notifyToken1, InteruptController.InterruptTokens notifyToken2)
        {
            _has32Divider = has32Divider;
            _hasInverter = hasInverter;
            _hasDeadzone = hasDeadzone;

            _notify = notify;
            _notifyTokens = new InteruptController.InterruptTokens[2] { notifyToken1, notifyToken2 };

            init();
        }

        private void init()
        {
            _prescaler = 0;
            _divisor = new uint[2] { 2, 2 };

            if (_hasInverter)
                _tcmpx = new uint[1] { 0 };
            else
                _tcmpx = new uint[2] { 0, 0 };

            _tcntx = new uint[2] { 0, 0 };
            _currentCycles = new uint[2] { 0, 0 };

            _started = new bool[2] { false, false };
            _manualUpdate = new bool[2] { false, false };
            _outputInvert = new bool[2] { false, false };
            _autoReload = new bool[2] { false, false };
            _deadZoneEnable = new bool[2] { false, false };

        }//init

        //reset currentCycles to its start value based on the prescaler value
        //and the divisor value
        private void resetCurrentCycles(int timer)
        {
            uint A = (_prescaler + 1);
            uint B = (uint)Math.Pow(2, _divisor[timer]);
            uint C = _tcntx[timer];
            _currentCycles[timer] = A * B * C;
        }
        private void resetCurrentCycles()
        {
            resetCurrentCycles(0);
            resetCurrentCycles(1);
        }

        public uint PreScaler
        {
            set { _prescaler = value; resetCurrentCycles(); }
//            get { return _prescaler; }
        }

        private void setMUXDivisor(int timer, uint value)
        {
            uint divisor;
            switch (value)
            {
                case 0: divisor = 2; break;
                case 1: divisor = 4; break;
                case 2: divisor = 8; break;
                case 3: divisor = 16; break;
                case 4:
                case 5:
                case 6:
                    {
                        if (!_has32Divider) return;
                        divisor = 32; break;
                    }
                default: return;
            }
            _divisor[timer] = divisor;
        }//setMUXDivisor

        public uint MUX
        {
            set
            {
                setMUXDivisor(0, (uint)(value & 0x0f));
                setMUXDivisor(1, (uint)((value >> 4) & 0x0f));
                resetCurrentCycles();
            }//set
        }

        private void setFlags(int timer, uint flags)
        {
            _started[timer] = Utils.lsb(flags);
            _manualUpdate[timer] = Utils.bitsSet(flags, 0x02);

            if (_hasInverter)
            {
                _outputInvert[timer] = Utils.bitsSet(flags, 0x04);
                _autoReload[timer] = Utils.bitsSet(flags, 0x08);
            }
            else
            {
                _autoReload[timer] = Utils.bitsSet(flags, 0x04);
            }

            if (_hasDeadzone)
            {
                _deadZoneEnable[timer] = Utils.bitsSet(flags, 0x10);
            }
        }//setFlags

        public void SetFlags(uint flags1, uint flags2)
        {
            setFlags(0, flags1);
            setFlags(1, flags2);
        }

        private void onClock(int timer, uint cycles)
        {
            if (!_started[timer] || _currentCycles[timer] == 0)
                return;

            if (cycles < _currentCycles[timer])
            {//cycles less than current, simply subtract and return
                _currentCycles[timer] -= cycles;
                return;
            }

            //compute remainder left amd reset current back to base value
            uint remainder = cycles - _currentCycles[timer];
            _currentCycles[timer] = 0;

            if (_autoReload[timer])
            {
                resetCurrentCycles(timer);
                _currentCycles[timer] -= remainder;
            }

            _notify(_notifyTokens[timer]);

        }//onClock

        public void onClock(uint cycles)
        {
            onClock(0, cycles);
            onClock(1, cycles);
        }//onClock

        public void SetTimerCounts(int timer, uint address, uint value)
        {
            if (address == 0)
            {//TCNTBx
                _tcntx[timer] = value;
                resetCurrentCycles(timer);
            }
            else if (address == 4)
            {//TCMPBx(TCNTO for timer 5)
                if (_hasInverter)
                    _tcmpx[timer] = value;
            }
        }
    }//class TimerBlock
}
