using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Simulator.VFP
{
    /// <summary>
    /// delegate defines a callback when a CPU status bit is written to
    /// </summary>
    /// <param name="flag"></param>
    public delegate void FPSRChangedDelegate(FPSCR.CPUFlag flag);

    /// <summary>
    /// This class represents the cpsr register in the floating point processor.
    /// The 4 flags of the register( c,n,v,z ) work in the same fashion as the
    /// ARM cpu cpsr register.
    /// In addition, the stride and length register values are maintained. These
    /// fields are for vector operations of the processor.
    /// </summary>
    public class FPSCR
    {
        /// <summary>
        /// Helper class to handle a single bit for the status bits
        /// the 2 interrupt disable bits(I and F), and the thumb bit(T)
        /// </summary>
        public class CPUFlag
        {
            private readonly int _index;
            private readonly uint _mask;
            private bool _state;
            /// <summary>
            /// CPUFlag ctor
            /// </summary>
            /// <param name="index"></param>
            /// <param name="mask"></param>
            public CPUFlag(int index, uint mask)
            {
                _index = index;
                _mask = mask;
                //_state = false;
            }
            /// <summary>Returns the flag index</summary>
            public int Index { get { return _index; } }
            /// <summary>Returns the flag mask</summary>
            public uint Mask { get { return _mask; } }
            /// <summary>Returns the flag state</summary>
            public bool State { get { return _state; } set { _state = value; } }
        }//class CPUFlag

        //create the status and interrupt disable bit structures
        private FPSCR.CPUFlag _negative = new FPSCR.CPUFlag(0, 0x80000000);
        private FPSCR.CPUFlag _zero = new FPSCR.CPUFlag(1, 0x40000000);
        private FPSCR.CPUFlag _carry = new FPSCR.CPUFlag(2, 0x20000000);
        private FPSCR.CPUFlag _overflow = new FPSCR.CPUFlag(3, 0x10000000);

        //handler to call when the cpsr is written to.
        private FPSRChangedDelegate _FPSRChangedHandler;

        /// <summary>
        /// The stride for vector operations.
        /// Default value is 0.
        /// </summary>
        private uint _stride;

        /// <summary>
        /// The length for vector operations.
        /// Default value is 0.
        /// </summary>
        private uint _length;

        /// <summary>
        /// Ctor for register. reset all values to default state
        /// </summary>
        public FPSCR()
        {
            this.reset();
        }

        /// <summary>
        /// Resets the cpsr to its default state
        /// All flags are set to false
        /// </summary>
        public void reset()
        {
            _negative.State = false;
            _zero.State = false;
            _carry.State = false;
            _overflow.State = false;
            _length = _stride = 0;
        }//reset

        /// <summary>
        /// Fire the changed handler if it is set.
        /// </summary>
        /// <param name="flag"></param>
        private void fireChangedHandler(FPSCR.CPUFlag flag)
        {
            if (_FPSRChangedHandler != null)
                _FPSRChangedHandler(flag);
        }

        /// <summary>
        /// Converts the internal CPSR data items to a 32bit integer and vice-versa
        /// Allows us to maintain the internal state of the CPSR as discreet data items
        /// and show them as a 32bit int to the outside.
        /// </summary>
        public uint Flags
        {
            get
            {
                uint flags = 0;
                if (_negative.State) flags |= _negative.Mask;
                if (_zero.State) flags |= _zero.Mask;
                if (_carry.State) flags |= _carry.Mask;
                if (_overflow.State) flags |= _overflow.Mask;

                flags |= ((_stride & 0x3) << 20);
                flags |= ((_length & 0x7) << 16 );

                return flags;
            }//get
            set
            {
                this.nf = ((value & _negative.Mask) == _negative.Mask);
                this.zf = ((value & _zero.Mask) == _zero.Mask);
                this.cf = ((value & _carry.Mask) == _carry.Mask);
                this.vf = ((value & _overflow.Mask) == _overflow.Mask);

                _stride = ((value >> 20) & 0x3);
                _length = ((value >> 16) & 0x7);

            }//set
        }//Flags

        ///<summary>Access to the current stride field of fcpsr</summary>
        public uint Stride { get { return _stride; } }
        ///<summary>Access to the current length field of fcpsr</summary>
        public uint Length { get { return _length; } }

        /// <summary>
        /// get/set the state of the zero flag
        /// if set, then fire the cpsr changed handler.
        /// </summary>
        public bool zf
        {
            get { return _zero.State; }
            set { _zero.State = value; fireChangedHandler(_zero); }
        }
        /// <summary>
        /// get/set the state of the carry flag
        /// if set, then fire the cpsr changed handler.
        /// </summary>
        public bool cf
        {
            get { return _carry.State; }
            set { _carry.State = value; fireChangedHandler(_carry); }
        }
        /// <summary>
        /// get/set the state of the negative flag
        /// if set, then fire the cpsr changed handler.
        /// </summary>
        public bool nf
        {
            get { return _negative.State; }
            set { _negative.State = value; fireChangedHandler(_negative); }
        }
        /// <summary>
        /// get/set the state of the overflow flag
        /// if set, then fire the cpsr changed handler.
        /// </summary>
        public bool vf
        {
            get { return _overflow.State; }
            set { _overflow.State = value; fireChangedHandler(_overflow); }
        }

        /// <summary>
        /// Set the state of the flags if the result of a floating point compare was equal.
        /// An example is the result of the "fcmps" instruction.
        /// </summary>
        public void setEqual()
        {
            this.nf = false;
            this.zf = true;
            this.cf = true;
            this.vf = false;
        }
        /// <summary>
        /// Set the state of the flags if the result of a floating point compare was less than.
        /// An example is the result of the "fcmps" instruction.
        /// </summary>
        public void setLessThan()
        {
            this.nf = true;
            this.zf = false;
            this.cf = false;
            this.vf = false;
        }
        /// <summary>
        /// Set the state of the flags if the result of a floating point compare was greater than.
        /// An example is the result of the "fcmps" instruction.
        /// </summary>
        public void setGreaterThan()
        {
            this.nf = false;
            this.zf = false;
            this.cf = true;
            this.vf = false;
        }//setGreaterThan
        /// <summary>
        /// Set the state of the flags if the result of a floating point compare is indeterminant
        /// because one of the operands was a Nan(not a number)
        /// An example is the result of the "fcmps" instruction.
        /// </summary>
        public void setUnordered()
        {
            this.nf = false;
            this.zf = false;
            this.cf = true;
            this.vf = true;
        }//setUnordered

        /// <summary>
        /// The cpsr changed handler property
        /// </summary>
        public FPSRChangedDelegate FPSRChangedHandler
        {
            set { _FPSRChangedHandler = value; }
        }//FPSRChangedDelegate

    }//class FPSCR
}
