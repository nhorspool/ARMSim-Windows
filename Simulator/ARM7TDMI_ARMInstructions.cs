using System;
using System.Collections.Generic;
using System.Text;

namespace ARMSim.Simulator
{
    [Serializable]
    public class UnalignedAccessException : Exception
    {
        public UnalignedAccessException(uint addr)
            : base( String.Format("Access to unaligned memory location 0x{0:X}", addr))
        { }

        public UnalignedAccessException(string s, Exception e)
            : base(s,e)
        { }

        public UnalignedAccessException(string s)
            : base(s)
        { }
    }

    [Serializable]
    public class MemoryAccessException : Exception
    {
        public MemoryAccessException(uint addr, bool isWrite)
            : base(String.Format("{0} invalid memory location 0x{1:X}",
              isWrite? "Store to" : "Read from", addr))
        { }

        public MemoryAccessException(string s, Exception e)
            : base(s, e)
        { }

        public MemoryAccessException(string s)
            : base(s)
        { }
    }
	public partial class ARM7TDMICore
    {
        //register source/destination masks
        internal const uint rn_mask = 0x000f0000;
        internal const uint rd_mask = 0x0000f000;
        internal const uint rs_mask = 0x00000f00;
        internal const uint rm_mask = 0x0000000f;

        internal const uint s_mask = 0x00100000;

        internal const uint op2_mask = 0x00000fff;
        internal const uint up_mask = 0x00800000;		//up,down mask(sbhw opcodes)

        internal const uint pre_mask = 0x01000000;
        internal const uint load_mask = 0x00100000;
        internal const uint write_back_mask = 0x00200000;

        internal const uint byte_mask = 0x00400000;
        internal const uint mode_mask = 0x0000001f;

		/* imm_mask and imm_hw_mask give the bit which sets whether or not an instruction's second argument is an
		 * immediate constant. For instructions which operate on full words, imm_mask is used. For instructions
		 * which operate on halfwords, imm_hw_mask is used.
		*/
        internal const uint imm_mask = 0x02000000; //Bit which determines whether an instruction's second argument is an immediate constant or not
		internal const uint imm_hw_mask = 0x00400000;		//half word versions

        internal const uint link_mask = 0x01000000;
        internal const uint branch_field = 0x00ffffff;
        internal const uint branch_sign = 0x00800000;

        internal const uint undef_mask = 0x0e000010;
        internal const uint undef_code = 0x06000010;
        internal const uint user_mask = 0x00400000;

		internal class PlainARMInstructionRegistry : InstructionRegistry
		{
			internal PlainARMInstructionRegistry() : base(12, DefaultInvalidOpCodeHandler) { }
			protected override uint Narrow(uint opcode)
			{
				//The key parts of the opcode are bits 27 - 20, and 7 - 4
				//We compress the opcode to a 12 bit string of those bits.
				return ((opcode & 0x0ff00000)>>16)| ((opcode&0x000000f0)>>4);
			}
			//A bit mask of the bits which the narrow function keeps.
			//internal uint valid_bits = 0x0ff000f0;
		}
		internal PlainARMInstructionRegistry ARMInstructionRegistry;

		internal void RegisterOpcode(uint opcode, uint wildcard_mask, InstructionFunc handler, string name)
		{
			ARMInstructionRegistry.RegisterOpCode(opcode, wildcard_mask, handler, name);
		}

		//internal void RegisterOpcode(uint opcode, InstructionFunc handler, string name)
		//{
		//	ARMInstructionRegistry.RegisterOpcode(opcode, 0x0, handler, name);
		//}

		//Register an opcode with respect to the provided wildcard mask, but exclude the single bitstring (opcode | exclude_mask)
		//This is the easiest workaround for the MUL instructions and their intersection with other data operations.
		//internal void RegisterOpcodeExclude(uint opcode, uint wildcard_mask, uint exclude_mask, InstructionFunc handler, string name)
		//{
		//	foreach (uint i in PlainARMInstructionRegistry.BitStringWildcard(opcode, wildcard_mask & ARMInstructionRegistry.valid_bits))
		//		if (i != (opcode | exclude_mask))
		//			ARMInstructionRegistry.RegisterOpcode(i, handler, name);
		//}

		internal bool SWIFlag;
		private void RegisterARMInstructions()
		{
			ARMInstructionRegistry = new PlainARMInstructionRegistry();

			//The opcodes are broken into groups roughly corresponding to the table
			//in ARMv7 A5.1
			//Data ops
			RegisterARMDataOps();
			//Load/Store ops (also some extra load/store ops from the Data Ops section like LDRH)
			RegisterARMLoadStoreOps();
			//Multiple Load/Store (just LDM and STM and their variants)
			RegisterOpcode(0x08100000, 0xf1efffff, ldm, "LDM");
			RegisterOpcode(0x08000000, 0xf1efffff, stm, "STM");

			//Branch instructions (the handler is so simple that it's not worth breaking this group up)
			RegisterOpcode(0x0a000000, 0xf1ffffff, branch, "branch");

			//FP coprocessor - 110x xxxx xx
			RegisterOpcode(0x0c000000, 0xf1ffffff, mFPP.Execute, "FPP (1)");
			//FP coprocessor - 1110 xxxx xx
			RegisterOpcode(0x0e000000, 0xf0ffffff, mFPP.Execute, "FPP (2)");
			//Everything in the 1111 xxxx xx range counts as SWI (which, at this point, means
			//"reserved for plugin").
			RegisterOpcode(0x0f000000,0xf0ffffff, delegate(uint opcode) { SWIFlag = true; return 0; }, "SWI");

		}

        private uint ExecuteARMInstruction(uint opcode, out bool swiInstruction)
        {
			
            swiInstruction = false;

            // Check if this op does not have a condition prefix
            if ((opcode & 0xF0000000) == 0xF0000000)
            {
                // It must be BLX(1) or an undefined instruction
                if ((opcode & 0xFE000000) == 0xFE000000)
                    return blx1(opcode);
                return DefaultInvalidOpCodeHandler(opcode);
            }

            //Check condition code and execute it if the condition code allows
            if (!check_cc((byte)(opcode >> 28))) {
                return 1;
            }

			SWIFlag = false;
            uint cycleCount = ARMInstructionRegistry[opcode](opcode);
			swiInstruction = SWIFlag;
            return cycleCount;
        }//ExecuteARMInstruction

        /// <summary>
        /// Check the condition code(cc) of the current instruction and determine if
        /// the instruction should be executed or not.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal bool check_cc(byte condition)
        {
            CPSR cpsr = this.CPSR;
            switch (condition & 0x0f)
            {
                case 0x00: return cpsr.zf;
                case 0x01: return !cpsr.zf;
                case 0x02: return cpsr.cf;
                case 0x03: return !cpsr.cf;
                case 0x04: return cpsr.nf;
                case 0x05: return !cpsr.nf;
                case 0x06: return cpsr.vf;
                case 0x07: return !cpsr.vf;

                //case 0X8: go = and(cf(cpsr), not(zf(cpsr)));                     break;
                case 0x08: return cpsr.cf && !cpsr.zf;

                //case 0X9: go = or(not(cf(cpsr)), zf(cpsr));                      break;
                case 0x09: return !cpsr.cf || cpsr.zf;

                //case 0XA: go = not(xor(nf(cpsr), vf(cpsr)));                     break;
                case 0x0a: return !(cpsr.nf ^ cpsr.vf);

                //case 0XB: go = xor(nf(cpsr), vf(cpsr));                          break;
                case 0x0b: return cpsr.nf ^ cpsr.vf;

                //case 0XC: go = and(not(zf(cpsr)), not(xor(nf(cpsr), vf(cpsr)))); break;
                case 0x0c: return !cpsr.zf && (!(cpsr.nf ^ cpsr.vf));

                //case 0XD: go = or(zf(cpsr), xor(nf(cpsr), vf(cpsr)));            break;
                case 0xd: return cpsr.zf || (cpsr.nf ^ cpsr.vf);

                case 0x0e: return true;
                case 0x0f: return false;

                default: return false;
            }//switch
        }//check_cc




		internal uint branch(uint op_code)
		{
			if (((op_code & link_mask) != 0) || ((op_code & 0xf0000000) == 0xf0000000))
				mGPR.LR = mGPR.PC;

			uint offset = (op_code & branch_field) << 2;
			if ((op_code & branch_sign) != 0)
				offset |= 0xfc000000;

			mGPR.PC += (offset + 4);
			return 3;
		}

        internal uint blx1(uint op_code)
        {
            mGPR.LR = mGPR.PC;

            uint offset = (op_code & branch_field) << 2;
            if ((op_code & branch_sign) != 0)
                offset |= 0xfc000000;
            if ((op_code & 0x01000000) != 0)
                offset += 2;
            mGPR.PC += offset + 4;
            CPSR.tf = true;
            return 3;
        }

#if false
        internal uint blx2(uint op_code)
        {
            if ((op_code & 0x00000020) == 0x00000020)
                mGPR.LR = mGPR.PC;
            uint Rm = op_code & rm_mask;
            uint reg = get_reg(Rm);
            CPSR.tf = (reg & 0x1) != 0;
            mGPR.PC = reg & 0xFFFFFFFE;
            return 3;
        }
#endif

		internal static uint bit_count(uint source, out int first)
		{
			uint count = 0;
			int reg = 0;
			first = -1;

			while (source != 0)
			{
				if (Utils.lsb(source))
				{
					++count;
					if (first < 0) first = reg;
				}
				source = source >> 1;
				++reg;
			}
			return count;
		}

		/// <summary>
		/// STM - perform a multi-register store to memory
		/// </summary>
		/// <returns></returns>
		internal uint stm(uint opcode)
		{
			uint mode = (opcode & 0x01800000) >> 23;            //isolate PU bits
			uint Rn = (opcode & rn_mask) >> 16;                 //get base register
			uint reg_list = opcode & 0x0000ffff;                //get the register list
			bool writeback = (opcode & write_back_mask) != 0;   //determine writeback flag

			uint address = Utils.valid_address(get_reg(Rn));//Bottom 2 bits ignored in address

			int first_reg;
			uint count = bit_count(reg_list, out first_reg);

			uint new_base;
			switch (mode)
			{
				case 0: new_base = address - 4 * count; address = new_base + 4; break;
				case 1: new_base = address + 4 * count; break;
				case 2: new_base = address - 4 * count; address = new_base; break;
				case 3: new_base = address + 4 * count; address = address + 4; break;
				default: return 0;
			}//switch

			bool special = false;
			if (writeback)
			{
				if (Rn == first_reg)
					special = true;
				else
					mGPR[Rn] = new_base;
			}//if

			uint reg = 0;
			while (reg_list != 0)
			{
				if (Utils.lsb(reg_list))
				{
                    this.SetMemory(address, ARMPluginInterfaces.MemorySize.Word, get_reg(reg));
					address += 4;
				}//if
				reg_list >>= 1;
				++reg;
			}//while
			if (special)
				mGPR[Rn] = new_base;

			return (1 + count);
		}//stm

		/// <summary>
		/// LDM - perform a multi-register load from memory
		/// </summary>
		/// <returns>Number of clock cycles executed</returns>
		internal uint ldm(uint opcode)
		{
			uint mode = (opcode & 0x01800000) >> 23;            //isolate PU bits
			uint Rn = (opcode & rn_mask) >> 16;                 //get base register
			uint reg_list = opcode & 0x0000ffff;                //get the register list
			bool writeback = (opcode & write_back_mask) != 0;   //determine writeback flag
			bool userModeLoad = ((opcode & 0x00400000) != 0);    //get user mode load flag

			uint address = Utils.valid_address(get_reg(Rn));  //make sure it is word aligned.
			int first_reg;
			uint count = bit_count(reg_list, out first_reg);
			bool includesPC = ((reg_list & 0x00008000) != 0);

			uint new_base;
			switch (mode)
			{
				case 0: new_base = address - 4 * count; address = new_base + 4; break;  //DA (Decrement After)
				case 1: new_base = address + 4 * count; break;                          //IA (Increment After)
				case 2: new_base = address - 4 * count; address = new_base; break;      //DB (Decrement Before)
				case 3: new_base = address + 4 * count; address = address + 4; break;   //IB (Increment Before)
				default: return 0;
			}//switch

			if (writeback)
				mGPR[Rn] = new_base;

			uint reg = 0;
			uint extraCycles = 0;

			//check
			while (reg_list != 0)
			{
				if (Utils.lsb(reg_list))
				{
					uint data = this.GetMemory(address, ARMPluginInterfaces.MemorySize.Word);
					if (reg == GeneralPurposeRegisters.PCRegisterIndex)
					{//We are loading a new value into the PC, check if entering Thumb or ARM mode
						extraCycles = 2;

						//if the user mode flag is set and r15 is being loaded, we are executing
						//LDM(3) and returning from an exception. We may be returning to Thumb mode or
						//ARM mode. In each case, force align the returning address.
						if (userModeLoad)
						{//we are returning from an exception, check if returning to ARM or Thumb mode
							if ((CPSR.SPSR & 0x0020) != 0)//check thumb bit of saved CPSR
								data = data & 0xfffffffe;   //returning to thumb mode, force align HWord
							else
								data = data & 0xfffffffc;   //returning to arm mode, force align Word
						}
						else if ((data & 0x01) != 0)
						{//Entering Thumb mode. Make sure destination address is HWord aligned
							CPSR.tf = true;
							data = data & 0xfffffffe;
						}//if
						else
						{//Entering ARM mode. Make sure destination address is Word aligned
							CPSR.tf = false;
							data = data & 0xfffffffc;
						}//else

					}//if

					if (userModeLoad & !includesPC)
						mGPR.SetUserModeRegister(reg, data);
					else
						mGPR[reg] = data;

					address += 4;
				}//if
				reg_list >>= 1;
				++reg;
			}//while

			//if the PC was loaded during this instruction and the user mode load was specified
			//then we are returning from an exception.
			if (includesPC && userModeLoad)
				CPSR.RestoreCPUMode();

			return (2 + count + extraCycles);
		}//ldm

		



































		/* ****************************** OLD ************************************ */



#if false 
        internal const uint mul_long_bit = 0x00800000;
        internal const uint mul_acc_bit = 0x00200000;

        /// <summary>
        /// Perform the ARM instruction MUL
        /// </summary>
        /// <param name="op_code"></param>
        /// <returns></returns>
        internal uint multiply(uint opcode)
        {
            uint Rs = ((opcode & rs_mask) >> 8);
            uint Rm = (opcode & rm_mask);
            uint Rd = ((opcode & rd_mask) >> 12);
            uint Rn = ((opcode & rn_mask) >> 16);

            uint RsData = get_reg(Rs);
            uint RmData = get_reg(Rm);
            uint M = calculateMultM(RsData);

            if ((opcode & mul_long_bit) == 0)
            {//Normal:mla,mul
                uint cycles = 1;
                uint acc = (RmData * RsData);
                if ((opcode & mul_acc_bit) != 0)
                {
                    acc += get_reg(Rd);
                    ++cycles;
                }
                mGPR[Rn] = acc;

                if ((opcode & s_mask) != 0)
                    CPSR.set_NZ(acc);//flags

                return (cycles + M);
            }//if
            else
            {//Long:xMLAL,xMULL
                uint cycles = 2;
                bool sign = false;

                if ((opcode & 0x00400000) != 0)
                {//Signed
                    if (Utils.msb(RmData))
                    {
                        RmData = ~RmData + 1;
                        sign = true;
                    }
                    if (Utils.msb(RsData))
                    {
                        RsData = ~RsData + 1;
                        sign = !sign;
                    }
                }//if

                //Everything now `positive
                uint tl = (RmData & 0x0000ffff) * (RsData & 0x0000ffff);
                uint th = ((RmData >> 16) & 0X0000ffff) * ((RsData >> 16) & 0X0000ffff);
                uint tm = ((RmData >> 16) & 0X0000ffff) * (RsData & 0X0000ffff);

                RmData = ((RsData >> 16) & 0X0000ffff) * (RmData & 0X0000ffff);  /* Rm no longer needed */
                tm = tm + RmData;
                if (tm < RmData) th = th + 0X00010000;                       /* Propagate carry */
                tl = tl + (tm << 16);
                if (tl < (tm << 16)) th = th + 1;
                th = th + ((tm >> 16) & 0X0000ffff);

                if (sign)
                {
                    th = ~th;
                    tl = ~tl + 1;
                    if (tl == 0) th = th + 1;
                }

                if ((opcode & mul_acc_bit) != 0)
                {
                    ++cycles;
                    tm = tl + get_reg(Rd);
                    if (tm < tl)
                        th = th + 1;//Propagate carry
                    tl = tm;
                    th += get_reg(Rn);
                }

                mGPR[Rd] = tl;
                mGPR[Rn] = th;

                if ((opcode & s_mask) != 0)
                {
                    CPSR.set_NZ(th | (((tl >> 16) | tl) & 0x0000ffff));
                }
                return (cycles + M);
            }//else

        }//multiply

        /// <summary>
        /// Determine if this is a signed byte half word instruction
		/// BB - 10/13/2014 - This refers to instructions like STRH
		/// (The ARM manual calls these "Extra load/store instructions"
        /// </summary>
        /// <param name="op_code"></param>
        /// <returns></returns>
        internal static bool is_it_sbhw(uint op_code)
        {
			//(XXXX 000X XXXX XXXX XXXX XXXX 1XX1 XXXX) AND (XXXX XXXX XXXX XXXX XXXX XXXX X~~X XXXX) where ~~ is not 00
			//AND (XXXX XXXX XXX~ XXXX XXXX XXXX X~XX XXXX) where ~~ is not 01
			//THEN either (XXXX XXXX X1XX XXXX XXXX XXXX XXXX XXXX) or (XXXX XXXX XXXX XXXX XXXX 0000 XXXX XXXX)
            if (((op_code & 0x0e000090) == 0x00000090) && ((op_code & 0x00000060) != 0x00000000)	//No multiplies
                    && ((op_code & 0x00100040) != 0x00000040))										//No signed stores
                return ((op_code & 0x00400000) != 0) || ((op_code & 0x00000f00) == 0);
            return false;
        }//is_it_sbhw

		
        




        //----------------------------------------------------------------------------*/
        internal uint transfer_sbhw(uint op_code)
        {
            ARMPluginInterfaces.MemorySize ms;
            switch (op_code & 0x00000060)
            {
                case 0x20: ms = ARMPluginInterfaces.MemorySize.HalfWord; break;					//H
                case 0x40: ms = ARMPluginInterfaces.MemorySize.Byte; break;						//SB
                case 0x60: ms = ARMPluginInterfaces.MemorySize.HalfWord; break;					//SH
                default: return 0;
            }

            uint Rd = ((op_code & rd_mask) >> 12);
            uint Rn = ((op_code & rn_mask) >> 16);

            uint address = get_reg(Rn);
            int offset = transfer_offset((op_code & op2_mask), ((op_code & up_mask) != 0), ((op_code & imm_hw_mask) != 0), true);

            if ((op_code & pre_mask) != 0)
                address = (uint)((int)address + offset);//pre-index

            if (TrapUnalignedMemoryAccess)
            {
                if ((ms == ARMPluginInterfaces.MemorySize.Word && ((address & 0x03) != 0))
                    || (ms == ARMPluginInterfaces.MemorySize.HalfWord && ((address & 0x01) != 0)))
                    throw new UnalignedAccessException(address);
            }

            uint cycles;
            if ((op_code & load_mask) == 0)
            {//store
                this.SetMemory(address, ms, get_reg(Rd));
                cycles = 2;
            }
            else
            {//load
                cycles = 3;
                uint data = this.GetMemory(address, ms);
                if (Rd == GeneralPurposeRegisters.PCRegisterIndex)
                {//We are loading into the PC, must check if thumb mode or not
                    cycles = 5;
                    if ((data & 0x01) != 0)
                    {//Entering Thumb mode. Make sure address is HWord aligned
                        CPSR.tf = true;
                        data = (data & 0xfffffffe);
                    }
                    else
                    {//Entering ARM mode. Make sure address is Word aligned
                        CPSR.tf = false;
                        data = (data & 0xfffffffc);
                    }//else
                }//if

                //update target register
                mGPR[Rd] = data;
            }//else(Load)

            //post index
            if ((op_code & pre_mask) == 0)//post index with writeback
                mGPR[Rn] = (uint)((int)address + offset);
            else if ((op_code & write_back_mask) != 0)
                mGPR[Rn] = address;

            return cycles;
        }



        /*----------------------------------------------------------------------------*/
       



        //----------------------------------------------------------------------------
        internal uint normal_data_op(uint op_code, uint operation)
        {
            //extract the S bit value from the opcode
            bool SBit = ((op_code & s_mask) != 0);

            //extract destination register from opcode
            uint Rd = ((op_code & rd_mask) >> 12);

            //extract operand register and get first operand
            uint Rn = ((op_code & rn_mask) >> 16);
            uint a = get_reg(Rn);

            bool shift_carry = false;

            uint b;
            if ((op_code & imm_mask) == 0)
                b = b_reg(op_code & op2_mask, ref shift_carry);
            else
                b = b_immediate(op_code & op2_mask, ref shift_carry);

            uint rd = 0;
            switch (operation)                                               /* R15s @@?! */
            {
                case 0x0: rd = a & b; break;					//AND
                case 0x1: rd = a ^ b; break;					//EOR
                case 0x2: rd = a - b; break;					//SUB
                case 0x3: rd = b - a; break;					//RSB
                case 0x4: rd = a + b; break;					//ADD
                case 0x5: rd = a + b;							//ADC
                    if (CPSR.cf) ++rd;
                    break;
                case 0x6: rd = a - b - 1;						//SBC
                    if (CPSR.cf) ++rd;
                    break;

                case 0x7: rd = b - a - 1;						//RSC
                    if (CPSR.cf) ++rd;
                    break;

                case 0x8: rd = a & b; break;					//TST
                case 0x9: rd = a ^ b; break;					//TEQ
                case 0xa: rd = a - b; break;					//CMP
                case 0xb: rd = a + b; break;					//CMN
                case 0xc: rd = a | b; break;					//ORR
                case 0xd: rd = b; break;					//MOV
                case 0xe: rd = a & ~b; break;					//BIC
                case 0xf: rd = ~b; break;					//MVN
            }//switch

            if ((operation & 0xc) != 0x8)							//Return result unless a compare
            {
                //write result into destination register
                mGPR[Rd] = rd;

                //if S bit is set and if the destination register is r15(pc) then this instruction has
                //special meaning. We are returning from a non-user mode to user-mode.
                //ie  movs pc,r14
                if (SBit && (Rd == GeneralPurposeRegisters.PCRegisterIndex))
                {
                    CPSR.RestoreCPUMode();
                    return 1;
                }//if
            }//if

            //if S bit is not set, we do not need to set any cpu flags, so we are done here
            if (!SBit) return 1;

            switch (operation)
            {                                                           //LOGICALs
                case 0x0:                                               //AND
                case 0x1:                                               //EOR
                case 0x8:                                               //TST
                case 0x9:                                               //TEQ
                case 0xc:                                               //ORR
                case 0xd:                                               //MOV
                case 0xe:                                               //BIC
                case 0xf:                                               //MVN
                    CPSR.set_NZ(rd);
                    CPSR.cf = shift_carry;
                    break;

                case 0x2:                                               //SUB
                case 0xa:                                               //CMP
                    CPSR.set_flags_sub(a, b, rd, true);
                    break;

                case 0x6:                                               //SBC
                    CPSR.set_flags_sub(a, b, rd, CPSR.cf);
                    break;

                case 0x3:                                               //RSB
                    CPSR.set_flags_sub(b, a, rd, true);
                    break;

                case 0x7:                                               //RSC
                    CPSR.set_flags_sub(b, a, rd, CPSR.cf);
                    break;

                case 0x4:                                               //ADD
                case 0xb:                                               //CMN
                    CPSR.set_flags_add(a, b, rd, false);
                    break;

                case 0x5:                                               //ADC
                    CPSR.set_flags_add(a, b, rd, CPSR.cf);
                    break;
                default: break;
            }//switch
            return 1;
        }//normal_data_op
        //----------------------------------------------------------------------------
        internal uint data_op(uint op_code)
        {
            //check if Arithmetic instruction extension
            //first test is for:mul,muls,mla,mlas
            //second test:umull,umulls,umlal,umlals,smull,smulls,smlal,smlals
            if (((op_code & 0x0fc000f0) == 0x00000090) || ((op_code & 0x0f8000f0) == 0x00800090))
            {
                return multiply(op_code);
            }
            else if (is_it_sbhw(op_code))
            {
                return transfer_sbhw(op_code);
            }
			// XXXX 0001 0X00 XXXX XXXX 0000 1001 XXXX
            else if ((op_code & 0x0fb00ff0) == 0x01000090)
            {
				//DONE
                return swap(op_code);
            }
            else
            {
                //TST, TEQ, CMP, CMN - all lie in following range, but have S set
				//XXXX 0001 0XX0 XXXX XXXX XXXX XXXX XXXX
                if ((op_code & 0x01900000) == 0x01000000)
                {//PSR transfers OR BX
                    if ((op_code & 0x0fbf0fff) == 0x010f0000)
                    {
						//DONE
                        return mrs(op_code);
                    }

                    //else if (((op_code & 0x0db6f000) == 0x0120f000) && ((op_code & 0x02000010) != 0x00000010))
                    //fixed a bug in above code. Test was only allowing field mask==0x9, which is really all you need,
                    //but sometines code can be compiled in CPSR_cxsf, which is field mask==0xf, which is legal.
                    else if (((op_code & 0x0db0f000) == 0x0120f000) && ((op_code & 0x02000010) != 0x00000010))
                    {
						//DONE
                        return msr(op_code);
                    }
                    else if ((op_code & 0x0fffffd0) == 0x012fff10)
                    {
						//DONE
                        return old_bx((op_code & rm_mask), ((op_code & 0x00000020) != 0));
                    }
                    else if ((op_code & 0x0fff0ff0) == 0x016f0f10)
                    {
						//DONE
                        return clz(op_code);
                    }
                }//if
                else
                {//All data processing operations
                    uint operation = (op_code & 0x01e00000) >> 21;
                    return normal_data_op(op_code, operation);
                }//else
                return 0;
            }//else
        }

/// <summary>
		/// 
		/// </summary>
		/// <param name="op2"></param>
		/// <param name="add"></param>
		/// <param name="imm"></param>
		/// <param name="sbhw"></param>
		/// <returns></returns>
		internal int transfer_offset(uint op2, bool add, bool imm, bool sbhw)
		{//add and imm are zero/non-zero Booleans
			int offset;
			bool cf = false;            // dummy parameter

			if (!sbhw)                 // Addressing mode 2
			{
				if (imm)
					offset = (int)b_reg(op2, ref cf);     // bit(25) = 1 -> reg
				else
					offset = (int)(op2 & 0x0fff);
			}
			else                       // Addressing mode 3
			{
				if (imm)
					offset = (int)(((op2 & 0xf00) >> 4) | (op2 & 0x00f));
				else
					// offset = (int)b_reg(op2, ref cf);
					offset = (int)get_reg(op2 & 0x0f); // bugfix -- NH 19/3/2011
			}

			return add ? offset : -offset;
		}

        internal uint transfer(uint op_code)
        {
            if ((op_code & undef_mask) == undef_code) return 0;

            ARMPluginInterfaces.MemorySize ms =
                ((op_code & byte_mask) == 0) ? ARMPluginInterfaces.MemorySize.Word : ARMPluginInterfaces.MemorySize.Byte;
            uint Rd = (op_code & rd_mask) >> 12;
            uint Rn = (op_code & rn_mask) >> 16;
            uint address = get_reg(Rn);
            int offset = transfer_offset((op_code & op2_mask), ((op_code & up_mask) != 0), ((op_code & imm_mask) != 0), false);	//bit(25) = 1 -> reg

            if ((op_code & pre_mask) != 0)
                //Pre-index
                address = (uint)((int)address + offset);

            if (TrapUnalignedMemoryAccess)
            {
                if ((ms == ARMPluginInterfaces.MemorySize.Word && ((address & 0x03) != 0))
                    || (ms == ARMPluginInterfaces.MemorySize.HalfWord && ((address & 0x01) != 0)))
                    throw new UnalignedAccessException(address);
            }

            uint cycles;
            if ((op_code & load_mask) == 0)
            {
                this.SetMemory(address, ms, get_reg(Rd));
                cycles = 2;
            }
            else
            {
                mGPR[Rd] = this.GetMemory(address, ms);
                cycles = (Rd == GeneralPurposeRegisters.PCRegisterIndex) ? (uint)5 : (uint)3;
            }

            if ((op_code & pre_mask) == 0)//Post-index
                //Post index write-back
                mGPR[Rn] = (uint)((int)address + offset);
            else if ((op_code & write_back_mask) != 0)
                //Pre index write-back
                mGPR[Rn] = address;

            return cycles;
        }

#endif

		/*----------------------------------------------------------------------------*/
        internal uint old_bx(uint Rm, bool link) /* Link is performed if "link" is NON-ZERO */
        {
            uint dest = get_reg(Rm);
            if ((dest & 0x1) != 0)
            {// entry into Thumb mode of execution
                CPSR.tf = true;
                dest = dest & 0xfffffffe;
            }
            else
            {
                CPSR.tf = false;
                dest = dest & 0xfffffffc;
            }

            //uint offset = Utils.valid_address(dest);
            if (link)
            {
                mGPR.LR = mGPR.PC;
            }
            mGPR.PC = dest;
            return 3;
        }


		

        //---------------------------------------------------------------------------



		/// <summary>
		/// STM - perform a multi-register store to memory
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="Rn"></param>
		/// <param name="reg_list"></param>
		/// <param name="write_back"></param>
		/// <returns></returns>
		private uint old_stm(uint mode, uint Rn, uint reg_list, bool write_back)
		{
			uint address = Utils.valid_address(get_reg(Rn));//Bottom 2 bits ignored in address

			int first_reg;
			uint count = bit_count(reg_list, out first_reg);

			uint new_base;
			switch (mode)
			{
				case 0: new_base = address - 4 * count; address = new_base + 4; break;
				case 1: new_base = address + 4 * count; break;
				case 2: new_base = address - 4 * count; address = new_base; break;
				case 3: new_base = address + 4 * count; address = address + 4; break;
				default: return 0;
			}//switch

			bool special = false;
			if (write_back)
			{
				if (Rn == first_reg)
					special = true;
				else
					mGPR[Rn] = new_base;
			}//if

			uint reg = 0;
			while (reg_list != 0)
			{
				if (Utils.lsb(reg_list))
				{
                    this.SetMemory(address, ARMPluginInterfaces.MemorySize.Word, get_reg(reg));
					address += 4;
				}//if
				reg_list >>= 1;
				++reg;
			}//while
			if (special)
				mGPR[Rn] = new_base;

			return (1 + count);
		}//stm

		/// <summary>
		/// LDM - perform a multi-register load from memory
		/// </summary>
		/// <param name="mode">Mode:the PU bits in the opcode</param>
		/// 00:DA (Decrement After)
		/// 01:IA (Increment After)
		/// 10:DB (Decrement Before)
		/// 11:IB (Increment Before)
		/// <param name="Rn">base register value</param>
		/// <param name="reg_list">list of registers to load(bottom 16 bits)</param>
		/// <param name="write_back">flag indicating if base register be updated</param>
		/// <param name="userModeLoad">flag indicating if values loaded into user mode registers</param>
		/// <returns>Number of clock cycles executed</returns>
		private uint old_ldm(uint mode, uint Rn, uint reg_list, bool write_back, bool userModeLoad)
		{
			uint address = Utils.valid_address(get_reg(Rn));  //make sure it is word aligned.
			int first_reg;
			uint count = bit_count(reg_list, out first_reg);
			bool includesPC = ((reg_list & 0x00008000) != 0);

			uint new_base;
			switch (mode)
			{
				case 0: new_base = address - 4 * count; address = new_base + 4; break;  //DA (Decrement After)
				case 1: new_base = address + 4 * count; break;                          //IA (Increment After)
				case 2: new_base = address - 4 * count; address = new_base; break;      //DB (Decrement Before)
				case 3: new_base = address + 4 * count; address = address + 4; break;   //IB (Increment Before)
				default: return 0;
			}//switch

			if (write_back)
				mGPR[Rn] = new_base;

			uint reg = 0;
			uint extraCycles = 0;

			//check
			while (reg_list != 0)
			{
				if (Utils.lsb(reg_list))
				{
					uint data = this.GetMemory(address, ARMPluginInterfaces.MemorySize.Word);
					if (reg == GeneralPurposeRegisters.PCRegisterIndex)
					{//We are loading a new value into the PC, check if entering Thumb or ARM mode
						extraCycles = 2;

						//if the user mode flag is set and r15 is being loaded, we are executing
						//LDM(3) and returning from an exception. We may be returning to Thumb mode or
						//ARM mode. In each case, force align the returning address.
						if (userModeLoad)
						{//we are returning from an exception, check if returning to ARM or Thumb mode
							if ((CPSR.SPSR & 0x0020) != 0)//check thumb bit of saved CPSR
								data = data & 0xfffffffe;   //returning to thumb mode, force align HWord
							else
								data = data & 0xfffffffc;   //returning to arm mode, force align Word
						}
						else if ((data & 0x01) != 0)
						{//Entering Thumb mode. Make sure destination address is HWord aligned
							CPSR.tf = true;
							data = data & 0xfffffffe;
						}//if
						else
						{//Entering ARM mode. Make sure destination address is Word aligned
							CPSR.tf = false;
							data = data & 0xfffffffc;
						}//else

					}//if

					if (userModeLoad & !includesPC)
						mGPR.SetUserModeRegister(reg, data);
					else
						mGPR[reg] = data;

					address += 4;
				}//if
				reg_list >>= 1;
				++reg;
			}//while

			//if the PC was loaded during this instruction and the user mode load was specified
			//then we are returning from an exception.
			if (includesPC && userModeLoad)
				CPSR.RestoreCPUMode();

			return (2 + count + extraCycles);
		}//ldm

    }
}
