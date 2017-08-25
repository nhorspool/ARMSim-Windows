using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Simulator.VFP
{
    //public delegate double fpTwoOperandD(double op1,double op2);
    //public delegate float fpTwoOperandS(float op1, float op2);

    /// <summary>
    /// Defines a floating point exception
    /// </summary>
    [Serializable]
    public class FloatingPointException : Exception
    {
        /// <summary>
        /// FloatingPointException ctor
        /// </summary>
        /// <param name="msg">message to construct exception with</param>
        public FloatingPointException(string msg)
            : base(msg) { }
    }//class FloatingPointException

    /// <summary>
    /// Defines an invalid operation exception
    /// </summary>
    [Serializable]
    public class InvalidOperationFloatingPointException : FloatingPointException
    {
        /// <summary>
        /// InvalidOperationFloatingPointException ctor
        /// </summary>
        /// <param name="instruction">the instruction that caused the exception</param>
        /// <param name="singleType">true if single</param>
        public InvalidOperationFloatingPointException(string instruction, bool singleType) 
            : base("Invalid Operation:" + instruction + (singleType ? "s" : "d")) { }
    }//class InvalidOperationFloatingPointException

    /// <summary>
    /// Defines an division by 0 exception
    /// </summary>
    [Serializable]
    public class DivisionByZeroFloatingPointException : FloatingPointException
    {
        /// <summary>
        /// DivisionByZeroFloatingPointException ctor
        /// </summary>
        /// <param name="instruction">the instruction that caused the exception</param>
        /// <param name="singleType">true if single</param>
        public DivisionByZeroFloatingPointException(string instruction, bool singleType)
            : base("Division By Zero:" + instruction + (singleType ? "s" : "d")) { }
    }//class DivisionByZeroFloatingPointException

    /// <summary>
    /// Defines an overflow exception
    /// </summary>
    [Serializable]
    public class OverflowFloatingPointException : FloatingPointException
    {
        /// <summary>
        /// OverflowFloatingPointException ctor
        /// </summary>
        /// <param name="instruction">the instruction that caused the exception</param>
        /// <param name="singleType">true if single</param>
        public OverflowFloatingPointException(string instruction, bool singleType)
            : base("Overflow:" + instruction + (singleType ? "s" : "d")) { }
    }//class OverflowFloatingPointException

    /// <summary>
    /// Defines an underflow exception
    /// </summary>
    [Serializable]
    public class UnderflowFloatingPointException : FloatingPointException
    {
        /// <summary>
        /// UnderflowFloatingPointException ctor
        /// </summary>
        /// <param name="instruction">the instruction that caused the exception</param>
        /// <param name="singleType">true if single</param>
        public UnderflowFloatingPointException(string instruction, bool singleType)
            : base("Underflow:" + instruction + (singleType ? "s" : "d")) { }
    }//class UnderflowFloatingPointException

    /// <summary>
    /// Class to simulate a VFP(Vector Floating Point) Processor
    /// </summary>
    public partial class FloatingPointProcessor
    {
        private BaseARMCore _jm;                      //reference back to ARM processor
        private FloatingPointRegisters _FPR;        //floating point registers
        private FPSCR _FPSCR;                       //floating point control register

        /// <summary>
        /// FloatingPointProcessor ctor
        /// create instance of VFP, obtain back reference to ARM
        /// </summary>
        /// <param name="jm"></param>
        public FloatingPointProcessor(BaseARMCore Jm)
        {
            _jm = Jm;
            _FPR = new FloatingPointRegisters();
            _FPSCR = new FPSCR();
        }

        ///<summary>Access to floating point registers</summary>
        public FloatingPointRegisters FPR { get { return _FPR; } }
        ///<summary>Access to floating point cpsr register</summary>
        public FPSCR FPSCR { get { return _FPSCR; } }

        /// <summary>
        /// handy function to inspect opcode and determine if instruction is single or double precision
        /// </summary>
        /// <param name="opcode">opcode to test</param>
        /// <returns>true if single</returns>
        public static bool isSingle(uint opCode)
        {
            return((opCode & 0x00000f00) == 0x00000a00);
        }

        /// <summary>
        /// handly function to extract a single or double precision register number from the opcode.
        /// inputs:
        /// opcode - the opcode to extract from
        /// shiftS1 - shift value for upper 4 bits if single (negative is shift left)
        /// shiftS2 - shift value for low bit if single
        /// shiftD  - shift value for 4 bit register number if double
        /// </summary>
        /// <param name="opcode">opcode to extract registers from</param>
        /// <param name="shiftS1">shift code 1</param>
        /// <param name="shiftS2">shift code 2</param>
        /// <param name="shiftD">shft d value</param>
        /// <returns>register number</returns>
        public static uint Unpack(uint opcode,int shiftS1, int shiftS2, int shiftD)
        {
            uint Fx;
            if (isSingle(opcode))
            {
                Fx = (shiftS1 >= 0) ? (opcode >> (shiftS1)) : (opcode << (-shiftS1));
                Fx &= 0x1e;
                Fx |= ((opcode >> shiftS2) & 0x01);
            }
            else
            {
                Fx = (opcode >> shiftD) & 0x0f;
            }
            return Fx;
        }//Unpack

        /// <summary>
        /// Extracts the Fn register from an opcode(first operand)
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>register number</returns>
        public static uint UnpackFn(uint opCode)
        {
            return Unpack(opCode, 15, 7, 16);
        }

        /// <summary>
        /// Extracts the Fd register from an opcode(destination register)
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>register number</returns>
        public static uint UnpackFd(uint opCode)
        {
            return Unpack(opCode, 11, 22, 12);
        }

        /// <summary>
        /// Extracts the Fn register from an opcode(destination register)
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>register number</returns>
        public static uint UnpackFm(uint opCode)
        {
            return Unpack(opCode, -1, 5, 0);
        }

        /// <summary>
        /// Extracts the Rd register, a general purpose register number
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>register number</returns>
        public static uint UnpackRd(uint opCode)
        {
            return (opCode >> 12) & 0x0f;
        }

        /// <summary>
        /// Extracts the Rn register, a general purpose register number
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>register number</returns>
        public static uint UnpackRn(uint opCode)
        {
            return (opCode >> 16) & 0x0f;
        }

        /// <summary>
        /// Extracts the PQRS bits
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>PQRS bits</returns>
        public static uint UnpackPQRS(uint opCode)
        {
            uint pqrs = 0;
            pqrs |= ((opCode >> 20) & 0x08);
            pqrs |= ((opCode >> 19) & 0x04);
            pqrs |= ((opCode >> 19) & 0x02);
            pqrs |= ((opCode >> 6) & 0x01);
            return pqrs;
        }

        /// <summary>
        /// Extracts the PUW bits(multi-load/store instructions)
        /// </summary>
        /// <param name="opcode">opcode to extract register from</param>
        /// <returns>PUW bits</returns>
        public static uint UnpackPUW(uint opCode)
        {
            return (((opCode & 0x01800000) >> 22) | ((opCode >> 21) & 0x01));
        }

        /// <summary>
        /// processor reset.
        /// </summary>
        public void reset()
        {
            this._FPR.reset();          //reset floating point registers
            this._FPSCR.reset();        //reset control register
        }

        /// <summary>
        /// Execute a floating point instruction.
        /// Called from main simulator execute function when a floating point
        /// instruction is detected.
        /// Here we determine the class of instruction and dispatch it
        /// </summary>
        /// <param name="opCode">opcode to execute</param>
        /// <returns>number of clock cyces used</returns>
        public uint Execute(uint opCode)
        {
            try
            {
                //must be a coprocessor 0xa or 0xb instruction. If not we have an undefined instruction
                uint coprocessor = opCode & 0x00000f00;
                if (coprocessor != 0x00000a00 && coprocessor != 0x00000b00)
                {
                    return 0;
                }
                bool singleType = isSingle(opCode);

                //one of these 3 classes of instructions
                if ((opCode & 0x0f000010) == 0x0e000000)
                    return data_processing(opCode, singleType);
                else if ((opCode & 0x0e000000) == 0x0c000000)
                    return load_store(opCode);
                else if ((opCode & 0x0f000070) == 0x0e000010)
                    return register_transfer(opCode);
                else
                    return 0;
            }
            catch (FloatingPointException ex)
            {
                //catch any floating point exceptions here. Dump message to console
                //and stop simulator
                _jm.OutputConsoleString("Floating point exception occurred:{0}\n", ex.Message);
                _jm.HaltSimulation();
                return 0;
            }
        }//Execute

        /// <summary>
        /// Create a string from a double value. Uses a simple FP format
        /// suitable for the registers view and swi instructions.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DoubleToString(double value)
        {
            return double.IsNaN(value) ? "NaN" : value.ToString("0.###E+0");
        }

        /// <summary>
        /// Create a string from a float value. Uses a simple FP format
        /// suitable for the registers view and swi instructions.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FloatToString(float value)
        {
            return float.IsNaN(value) ? "NaN" : value.ToString("0.###E+0");
        }

    }//class FloatingPointProcessor
}
