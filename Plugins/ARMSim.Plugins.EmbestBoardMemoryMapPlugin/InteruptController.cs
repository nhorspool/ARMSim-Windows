using System;
using System.Collections.Generic;
using System.Text;

using ARMPluginInterfaces;

namespace ARMSim.Plugins.EmbestBoardMemoryMapPlugin
{
    public delegate void UpdateInterruptController();

    //todo - implement round robin mode
    //todo - implement vectored interupt mode
    public class InteruptController
    {
        [Flags]
        public enum InterruptTokens : uint
        {
            None = 0,
            BIT_TIMER5 = (0x1 << 8),
            BIT_TIMER4 = (0x1 << 9),
            BIT_TIMER3 = (0x1 << 10),
            BIT_TIMER2 = (0x1 << 11),
            BIT_TIMER1 = (0x1 << 12),
            BIT_TIMER0 = (0x1 << 13),
            BIT_WDT = (0x1 << 15),
            BIT_EINT4567 = (0x1 << 21),
            BIT_EINT1 = (0x1 << 24)
        }

        private bool _vectorMode;
        private bool _IRQEnable;
        private bool _FIQEnable;

        private uint _interuptMode;
        private uint _interuptMask;
        private uint _interuptPending;
        private uint _currentInteruptService;

        private uint _slavePriorities;   //I_PSLV
        private uint _masterPriorities;  //I_PMST

        private uint[] _priorities = new uint[26];

        private InterruptControllerDisplay mInterruptControllerDisplay;
        //public event UpdateInterruptController mUpdateInterruptControllerDisplay;

        private IARMHost mIhost;
        public InteruptController(IARMHost ihost, InterruptControllerDisplay interruptControllerDisplay)
        {
            mIhost = ihost;
            mInterruptControllerDisplay = interruptControllerDisplay;

            //request to be notified when our area of the memory map is written or read
            mIhost.RequestMemoryBlock(0x01e00000, 0xffffffc0, onMemoryAccessRead, onMemoryAccessWrite);
            this.Restart();
        }

        public void Restart()
        {
            Array.Clear(_priorities, 0, _priorities.Length);
            _priorities[0] = 0xffff0001;//INT_ADC
            _priorities[1] = 0xffff0000;//INT_RTC
            for (int ii = 0; ii < 4; ii++)
            {
                _priorities[(ii * 6) + 2] = 0x0000ffff;
                _priorities[(ii * 6) + 3] = 0x0000fffe;
            }//for ii

            //disable irq,fiq and set into non-vectored mode
            _vectorMode = false;
            _IRQEnable = false;
            _FIQEnable = false;

            //all interrupts use irq
            _interuptMode = 0;

            //mask all interrupts
            _interuptMask = 0x07ffffff;

            //clear interrupt pending bits
            _interuptPending = 0;

            //clear current interupt service register
            _currentInteruptService = 0;

            //setup the default slave and master priorities
            SetSlavePriorities(0x1b1b1b1b);//I_PSLV
            SetMasterPriorities(0x00001f1b);//I_PMST

            mInterruptControllerDisplay.UpdateInterruptControllerDisplay();
        }//init

        public uint Mask { get { return _interuptMask; } }
        public uint Mode { get { return _interuptMode; } }
        public uint Pending { get { return _interuptPending; } }
        public uint Current { get { return _currentInteruptService; } }
        public bool IRQEnabled { get { return _IRQEnable; } }
        public bool FIQEnabled { get { return _FIQEnable; } }
        public bool VectorMode { get { return _vectorMode; } }

        //scans the interrupt priority table for all pending, unmasked interrupts.
        //Finds the one with the highest priority and returns its interrupt mask.
        //returns 0 if none found.
        //Input mask is only pending interrupts that are not masked.
        private uint FindHighestPriority(uint mask)
        {
            //if no bits set in mask, return now
            if (mask == 0)
                return 0;

            //scan table looking for highest.
            uint highestPriority = 0xffffffff;
            uint highestMask = 0x00000000;

            for (int ii = 0; ii < 26; ii++)
            {
                //if no bits left in mask, return now
                if (mask == 0)
                    return highestMask;

                //if lsb in mask set, then check this interupts priority
                if (Utils.lsb(mask))
                {
                    //if higher than current, save it
                    if (_priorities[ii] < highestPriority)
                    {
                        highestPriority = _priorities[ii];
                        highestMask = (uint)(0x01 << ii);
                    }//if
                }//if
                //nect bit in mask
                mask >>= 1;
            }//for ii

            //return the highest priority interrupt mask
            //will be 0 if none found
            return highestMask;
        }//FindHighestPriority

        //check the interrupt controlled for any pending interrupts. Find the highest priority
        //of an unmasked pending interrupt and fire the appropriate ARM interrupt(irq or fiq)
        private void ProcessInterupts()
        {
            //if there are no pending interrupts or one is being serviced, then do nothing
            if (_interuptPending == 0 || _currentInteruptService != 0)
                return;

            //check if global mask bit is set, if so ignore interupt
            if ((_interuptMask & 0x04000000) != 0)
                return;

            //find the highest priority pending interupt that is not masked
            uint highestInteruptMask = FindHighestPriority((uint)(_interuptPending & ~_interuptMask));

            //if 0, then no non-masked pending interupts, return
            if (highestInteruptMask == 0)
                return;

            //check if interupt was programed for FIQ or IRQ mode
            bool fiqMode = ((_interuptMode & highestInteruptMask) != 0);

            //check if this mode is masked or not. If it is, ignore interupt
            if (fiqMode && !_FIQEnable)
                return;
            if (!fiqMode && !_IRQEnable)
                return;

            //set the current interrupt service register
            _currentInteruptService = highestInteruptMask;

            //signal ARM engine that interrupt has occurred.
            mIhost.AssertInterrupt(fiqMode);

        }//ProcessInterupts

        //call by an interrupt source(ie PWMTimer) to notify interrupt controller
        //that an interrupt has occurred.
        //token was passed to the interrupt source at started. In this case it is the
        //bit mask of the source interrupt.
        public void InteruptNotify(InterruptTokens token)
        {
            //set the pending bit
            _interuptPending |= (uint)token;

            //process pending interrupts looking for highest priority
            ProcessInterupts();

            mInterruptControllerDisplay.UpdateInterruptControllerDisplay();

        }//InteruptNotify

        private uint onMemoryAccessRead(object sender, MemoryAccessReadEventArgs mra)
        {
            switch (mra.Address)
            {
                case 0x01e00000:
                    {//INTCON
                        uint value = _vectorMode ? (uint)0x04 : 0x00;
                        value |= _IRQEnable ? (uint)0x02 : 0x00;
                        value |= _FIQEnable ? (uint)0x01 : 0x00;
                        return value;
                    }
                case 0x01e00004:
                    {//INTPND
                        if (mra.Size != MemorySize.Word) break;
                        return _interuptPending;
                    }
                case 0x01e00008:
                    {//INTMOD
                        if (mra.Size != MemorySize.Word) break;
                        return _interuptMode;
                    }
                case 0x01e0000c:
                    {//INTMSK
                        if (mra.Size != MemorySize.Word) break;
                        return _interuptMask;
                    }
                case 0x01e00010:
                    {//I_PSLV
                        if (mra.Size != MemorySize.Word) break;
                        return _slavePriorities;
                    }
                case 0x01e00014:
                    {//I_PMST
                        if (mra.Size == MemorySize.Byte) break;//cannot read a byte here
                        return _masterPriorities;
                    }
                case 0x01e00018:
                    {//I_CSLV
                        if (mra.Size != MemorySize.Word) break;//must be a full word
                        return _slavePriorities;
                    }
                case 0x01e0001c:
                    {//I_CMST
                        if (mra.Size == MemorySize.Byte) break;//cannot write a byte here
                        return _masterPriorities;
                    }
                case 0x01e00020:
                    {//I_ISPR
                        if (mra.Size != MemorySize.Word) break;//must be a full word
                        return _currentInteruptService;
                    }
            }//switch
            return 0;
        }//onMemoryAccessRead

        private void SetSlavePriorities(uint mask, int slaveUnit)
        {
            for (int ii = 0; ii < 4; ii++)
            {
                int index = (slaveUnit * 6) + 4 + ii;
                uint priority = (mask & 0x00000003);
                _priorities[index] = (_priorities[index] & 0xffff0000) | priority;
                mask >>= 2;
            }//for ii
        }//SetSlavePriorities

        private void SetSlavePriorities(uint mask)
        {
            _slavePriorities = mask;
            SetSlavePriorities((mask  & 0x0ff), 0);       //mGD
            SetSlavePriorities(((mask >> 8) & 0x0ff), 1); //mGC
            SetSlavePriorities(((mask >> 16) & 0x0ff), 2);//mGB
            SetSlavePriorities(((mask >> 24) & 0x0ff), 3);//mGA
        }

        //todo - implement
        private void SetMasterPriorities(uint mask)
        {
            _masterPriorities = mask;
            for (int ii = 0; ii < 4; ii++)
            {
                int index = (ii * 6) + 2;
                uint priority = ((mask & 0x00000003) << 16);
                for (int jj = 0; jj < 6; jj++)
                {
                    _priorities[index+jj] = (_priorities[index+jj] & 0x0000ffff) | priority;
                }
                mask >>= 2;
            }//for ii
        }//SetMasterPriorities

        private void onMemoryAccessWrite(object sender, MemoryAccessWriteEventArgs mwa)
        {
            switch (mwa.Address)
            {
                case 0x01e00000:
                    {//INTCON
                        uint value = (mwa.Value & 0x07);
                        _vectorMode = ((value & 0x04)==0);
                        _IRQEnable = ((value & 0x02)==0);
                        _FIQEnable = ((value & 0x01)==0);
                    } break;
                case 0x01e00008:
                    {//INTMOD
                        if (mwa.Size != MemorySize.Word) break;
                        _interuptMode = mwa.Value;
                    } break;
                case 0x01e0000c:
                    {//INTMSK
                        if (mwa.Size != MemorySize.Word) break;
                        _interuptMask = mwa.Value;
                    } break;
                case 0x01e00010:
                    {//I_PSLV
                        if (mwa.Size != MemorySize.Word) break;
                        SetSlavePriorities(mwa.Value);
                    } break;
                case 0x01e00014:
                    {//I_PMST
                        if (mwa.Size == MemorySize.Byte) break;//cannot write a byte here
                        SetMasterPriorities(mwa.Value);
                    } break;
                case 0x01e00024:
                case 0x01e0003c:
                    {//I_ISPC,F_ISPC
                        //todo - implement
                        if (mwa.Size != MemorySize.Word) break;//must be a full word
                        uint mask = mwa.Value;
                        _interuptPending &= ~mask;
                        _currentInteruptService = 0;
                    } break;
            }//switch
            ProcessInterupts();
            mInterruptControllerDisplay.UpdateInterruptControllerDisplay();
        }//onMemoryAccessWrite
    }//class InteruptController
}
