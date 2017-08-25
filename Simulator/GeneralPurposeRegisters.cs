#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;

namespace ARMSim.Simulator
{
	/// <summary>
    /// This class represents the general purpose registers of the ARM cpu.
    /// The 16x32bit registers are represented(r0-r15) and also the banked
    /// registers for the other 5 operating modes.
	/// </summary>
    public class GeneralPurposeRegisters : ARMPluginInterfaces.IGPR
	{
        //constant register numbers and their function
        public const int PCRegisterIndex = 15;
        public const int LRRegisterIndex = 14;
        public const int SPRegisterIndex = 13;
        public const int IPRegisterIndex = 12;
        public const int FPRegisterIndex = 11;
        public const int SLRegisterIndex = 10;

		//the 16 general purpose registers
		private uint [] r_user = new uint[16];

        //these are the banked registers for each of the cpu modes
        //each mode except fiq mode have 2 registers.
        //fiq mode has 7 registers
        //see ARM Architecture Reference Manual pA2-4 for details.
        private uint[] r_svc = new uint[2];
        private uint[] r_abt = new uint[2];
        private uint[] r_und = new uint[2];
        private uint[] r_irq = new uint[2];
        private uint[] r_fiq = new uint[7];

        //the delegate for register changed handler
        private ARMPluginInterfaces.RegisterChangedDelegate _registerChangedHandler;

        //a reference to the cpsr register. Needed to determine the current operating mode.
        private CPSR _cpsr;

        /// <summary>
        /// GeneralPurposeRegisters ctor
        /// Requires a reference to the cpsr for current operating mode
        /// </summary>
        /// <param name="cpsr">the simulation CPSR register</param>
		public GeneralPurposeRegisters(CPSR cpsr)
		{
            _cpsr = cpsr;
            this.Reset();
		}

        public void SetUserModeRegister(uint reg, uint newValue)
        {
            //make sure register number is valid
            if (reg >= r_user.Length)
                return;

            r_user[reg] = newValue;
        }

        //Set a register value. Valid register numbers are 0-15 only
        //Based on the cpu mode and the register number/

        /// <summary>
        /// Set a register value. Valid register numbers are 0-15 only.
        /// Based on the cpu mode and the register number
        /// </summary>
        /// <param name="reg">register number to set(0-15)</param>
        /// <param name="newValue">value to set</param>
		private void setRegister(uint reg,uint newValue)
		{
            //make sure register number is valid
            if (reg >= r_user.Length)
                return;

            //get the current cpu mode
            CPSR.CPUModeEnum cpuMode = _cpsr.Mode;

            //and perform register transfer according to mode
            switch (cpuMode)
            {
                //user and system mode use only the normal 16 gp registers
                case CPSR.CPUModeEnum.User:
                case CPSR.CPUModeEnum.System:
                    r_user[reg] = newValue; break;

                //these modes have their own r13 and r14
                case CPSR.CPUModeEnum.Supervisor:
                case CPSR.CPUModeEnum.Abort:
                case CPSR.CPUModeEnum.Undefined:
                case CPSR.CPUModeEnum.IRQ:
                    if (reg < 13 || reg == PCRegisterIndex)
                        r_user[reg] = newValue;
                    else
                    {
                        if (cpuMode == CPSR.CPUModeEnum.Supervisor) r_svc[reg - 13] = newValue;
                        else if (cpuMode == CPSR.CPUModeEnum.Abort) r_abt[reg - 13] = newValue;
                        else if (cpuMode == CPSR.CPUModeEnum.Undefined) r_und[reg - 13] = newValue;
                        else r_irq[reg - 13] = newValue;
                    }
                    break;

                //fiq mode has its own r8-r14
                case CPSR.CPUModeEnum.FIQ:
                    if (reg < 8 || reg == PCRegisterIndex)
                        r_user[reg] = newValue;
                    else
                        r_fiq[reg - 8] = newValue;
                    break;
            }//switch

            //if the register changed handler is set, call it
            if (_registerChangedHandler != null)
                 _registerChangedHandler(reg);

         }//setRegister

        /// <summary>
        /// Get a register value. Valid register numbers are 0-15 only.
        /// Based on the cpu mode and the register number
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private uint getRegister(uint reg)
        {
            //make sure register number is valid
            if (reg >= r_user.Length)
                return 0;

            //get the current cpu mode
            CPSR.CPUModeEnum cpuMode = _cpsr.Mode;

            //and perform register transfer according to mode
            switch (cpuMode)
            {
                //user and system mode use only the normal 16 gp registers
                case CPSR.CPUModeEnum.User:
                case CPSR.CPUModeEnum.System:
                    return r_user[reg];

                //these modes have their own r13 and r14
                case CPSR.CPUModeEnum.Supervisor:
                case CPSR.CPUModeEnum.Abort:
                case CPSR.CPUModeEnum.Undefined:
                case CPSR.CPUModeEnum.IRQ:
                    if (reg < 13 || reg == PCRegisterIndex)
                        return r_user[reg];
                    else
                    {
                        if (cpuMode == CPSR.CPUModeEnum.Supervisor) return r_svc[reg - 13];
                        else if (cpuMode == CPSR.CPUModeEnum.Abort) return r_abt[reg - 13];
                        else if (cpuMode == CPSR.CPUModeEnum.Undefined) return r_und[reg - 13];
                        else return r_irq[reg - 13];
                    }
                //fiq mode has its own r8-r14
                case CPSR.CPUModeEnum.FIQ:
                    if (reg < 8 || reg == PCRegisterIndex)
                        return r_user[reg];
                    else
                        return r_fiq[reg - 8];
            }//switch
            return 0;
        }//getRegister

        /// <summary>
        /// Perform a reset on the registers.
        /// Set them all to 0
        /// </summary>
		public void Reset()
		{
            Array.Clear(r_user,0,r_user.Length);
            Array.Clear(r_svc,0,r_svc.Length);
            Array.Clear(r_abt,0,r_abt.Length);
            Array.Clear(r_und,0,r_und.Length);
            Array.Clear(r_irq,0,r_irq.Length);
            Array.Clear(r_fiq,0,r_fiq.Length);
		}

        /// <summary>
        /// Get/Set the sp(r13) of the current cpu mode
        /// Note that sp is forced to be word aligned.
        /// </summary>
		public uint SP
		{
            get { return getRegister(SPRegisterIndex); }
            set { setRegister(SPRegisterIndex, value); }
		}//SP

        /// <summary>
        /// Get/Set the lr(r14) of the current cpu mode
        /// Note that lr is forced to be word aligned.
        /// </summary>
        public uint LR
		{
            get { return getRegister(LRRegisterIndex); }
            set { setRegister(LRRegisterIndex, value); }
		}//LR

        /// <summary>
        /// Get/Set the pc(r15) of the current cpu mode
        /// Note that pc is forced to be word aligned.
        /// </summary>
        public uint PC
		{
            get { return getRegister(PCRegisterIndex); }
            set { setRegister(PCRegisterIndex, value); }

            //Created this short circut to speed up access a little.
            //r15 is in the User registers for all ARM modes. - dale
            //get { return r_user[PCRegisterIndex]; }
            //set { r_user[PCRegisterIndex] = value; }
		}//PC

        /// <summary>
        /// Register indexer. Can get/set register using syntax:
        /// _gpr[12]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		public uint this [uint index]   // Indexer declaration
		{
			get { return getRegister(index); }
			set { setRegister(index,value); }
		}//this[]

        /// <summary>
        /// Determines if a string represents a cpu register.
        /// Returns a bool for success and a reg number if true.
        /// A register can be specified in one of 3 ways
        /// Directly   ie    r15, r1 etc
        /// Leading $  ie    $r15 $r1 etc
        /// or symbolically   sp,lr,pc  etc
        /// </summary>
        /// <param name="register"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static bool isRegister(string register, ref uint reg)
		{
			//strip off all the whitespace, force lowercase
			string str=register.Trim().ToLower();

            //eliminate if not a valid size
			if(str.Length > 4 || str.Length<2) return false;

            //check for symbolic representations
            if (str.Equals("sl") || str.Equals("$sl")) { reg = SLRegisterIndex; return true; }
            if (str.Equals("fp") || str.Equals("$fp")) { reg = FPRegisterIndex; return true; }
            if (str.Equals("ip") || str.Equals("$ip")) { reg = IPRegisterIndex; return true; }
            if (str.Equals("sp") || str.Equals("$sp")) { reg = SPRegisterIndex; return true; }
            if (str.Equals("lr") || str.Equals("$lr")) { reg = LRRegisterIndex; return true; }
            if (str.Equals("pc") || str.Equals("$pc")) { reg = PCRegisterIndex; return true; }

			//check if it is a register name. ie  r14, r1
			string regnum;
			if( str[0] == 'r' )
			{
				regnum=str.Substring(1);
			}
			else if ( str.StartsWith("$r") )
			{
				regnum=str.Substring(2);
			}
			else
			{
				return false;
			}

            //make sure register numbers are all valid digits
			foreach(char c in regnum)
			{
				if(!Char.IsDigit(c))
				{
					return false;
				}
			}//foreach

            //must be ok, go ahead and parse it
			reg = uint.Parse(regnum);
            return (reg >= 0 && reg <= PCRegisterIndex);
        }//isRegister

        /// <summary>
        /// Convert a register number to a string.
        /// Append symbolic name if it is > 11
        /// ie    R12(ip)
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
		public static string registerToString(uint reg)
		{
			string str="R"+reg;
			if(reg<10)str+=" ";
            if (reg == PCRegisterIndex)
				str+="(pc)";
            else if (reg == LRRegisterIndex)
				str+="(lr)";
            else if (reg == SPRegisterIndex)
				str+="(sp)";
            else if (reg == IPRegisterIndex)
				str+="(ip)";
            else if (reg == FPRegisterIndex)
				str+="(fp)";
            else if (reg == SLRegisterIndex)
				str+="(sl)";
			else
			{
				str+="    ";
			}
			str+=":";
			return str;
		}//registerToString

        /// <summary>
        /// Property to set the register changed handler
        /// </summary>
        public ARMPluginInterfaces.RegisterChangedDelegate RegisterChangedHandler
		{
			set{_registerChangedHandler=value;}
		}

    }//class GeneralPurposeRegisters
}
