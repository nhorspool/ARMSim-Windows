using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARMSim.Simulator
{
	public partial class ARM7TDMICore
	{


		internal void RegisterARMDataOps()
		{

			//The "SBHW" instructions were handled by a particularly
			//thorny and arbitrary mechanism in the old version.
			//Some reverse engineering seems to indicate that the SBHW
			//handler referred to the "Extra load/store instructions"
			//in ARMv7 A5.2.8
			//These instructions are grouped with "data operations" in the ARM reference
			//but in ARMSim they are created in the LoadStore section.
			//The encodings are taken from ARMv7 A5.2.8
			/*
			RegisterOpcode(0x000000b0, 0xf1afff0f, null, "STRH"); //STRH (Register)
			RegisterOpcode(0x001000b0, 0xf1afff0f, null, "LDRH"); //LDRH (Register)
			RegisterOpcode(0x004000b0, 0xf1afff0f, null, "STRH"); //STRH (immediate)
			RegisterOpcode(0x005000b0, 0xf1afff0f, null, "LDRH"); //LDRH (immediate)

			//LDRD is v5TE only
			//RegisterOpcode(0x000000d0, 0xf1afff0f, null, "LDRD"); //LDRD (Register)
			RegisterOpcode(0x001000d0, 0xf1afff0f, null, "LDRSB"); //LDRSB (Register)
			//RegisterOpcode(0x004000d0, 0xf1afff0f, null, "LDRD"); //LDRD (immediate)
			RegisterOpcode(0x005000d0, 0xf1afff0f, null, "LDRSB"); //LDRSB (immediate)

			RegisterOpcode(0x000000f0, 0xf1afff0f, null, "STRD"); //STRD (Register)
			RegisterOpcode(0x001000f0, 0xf1afff0f, null, "LDRSH"); //LDRSH (Register)
			RegisterOpcode(0x004000f0, 0xf1afff0f, null, "STRD"); //STRD (immediate)
			RegisterOpcode(0x005000f0, 0xf1afff0f, null, "LDRSH"); //LDRSH (immediate)
			*/


			//Missing: The "extra load/store instructions (unprivileged)" (which are
			//v6T2 only) in ARMv7 A5.2.9

			//SWP and SWPB (ARMv7 A8.8.229)
			RegisterOpcode(0x01000090, 0xf00fff0f, swap, "SWP");
			RegisterOpcode(0x01400090, 0xf00fff0f, swap, "SWPB");



			//Note regarding MRS/MSR: In the ARMv7 reference, in A5.2.12, there
			//are two special v7-only MRS/MSR instructions (with B = 1 in the table).
			//Both are forwarded to the regular MRS/MSR handler here. 
			//TODO The above may be a problem

			//MRS (ARMv7 B9.3.7) - This is encoding A1 (ARMv4T, v5, v6, v7)
			//These two cases can be optimized into two separate functions.
			RegisterOpcode(0x01000000, 0xf00fff0f, mrs, "MRS"); //Case where read_spsr = 0
			RegisterOpcode(0x01400000, 0xf00fff0f, mrs, "MRS"); //Case where read_spsr = 1 

			//XXXX 0001 0010 XXXX XXXX XXXX XXX0 XXXX
			//XXXX 0001 0110 XXXX XXXX XXXX XXX0 XXXX
			//MSR (ARMv7, based on the summary in A5.2.12)
			//It is unlikely that the implemented version correctly handles all cases
			//These are the versions with register operands
			RegisterOpcode(0x01200000, 0xf00fffef, msr, "MSR");
			RegisterOpcode(0x01600000, 0xf00fffef, msr, "MSR");
			//These are the versions with immediate operands
			RegisterOpcode(0x03200000, 0xf00fffff, msr, "MSR");
			RegisterOpcode(0x03600000, 0xf00fffff, msr, "MSR");

			//XXXX 0001 0010 XXXX XXXX XXXX 0001 XXXX
			//BX/BLX(2) (ARMv7 A5.2.12)
			RegisterOpcode(0x01200010, 0xf00fff2f, bx, "BX");

			//XXXX 0001 0110 XXXX XXXX XXXX 0001 XXXX
			//CLZ (ARMv7 A5.2.12 and A8.8.33) - This is encoding A1.
			//Note that ARMv4T (i.e. ARM7TDMI) does not support this instruction...
			RegisterOpcode(0x01600010, 0xf00fff0f, clz, "CLZ");

			//"Normal" Data ops
			//Note that bits 4-7 technically have a meaning for the below instructions,
			//but for the purposes of registering instructions we completely ignore them
			//(hence the 0xf00fffff mask in the RegisterOpcode calls in the Data Op registration
			//functions). As a result, the opcodes below set those bits to 0 (otherwise an assert
			//would fail due to the wildcard mask and the opcode not being disjoint).
			//Additionally, the S bit (bit 20) and IMM bit (bit 25) are set to 0, since the
			//registration function takes care of mapping those bits for instructions that allow it.
			//Short version: The hex opcodes below only cover bits 21-24 and 26-27 of the corresponding 
			//instruction if the instruction allows the S flag and bits 20-24 and 26- 27 otherwise.
			//This is equivalent to saying that the opcodes below always correspond to the non-S, non-immed
			//version of the instruction.

			//Logical Data Ops which support S flag
			//AND
			//XXXX 0000 000S XXXX XXXX
			RegisterDataOpLogical((a, b) => a & b, 0x00000000, "AND");
			//BIC
			RegisterDataOpLogical((a, b) => a & ~b, 0x01c00000, "BIC");
			//EOR
			RegisterDataOpLogical((a, b) => a ^ b, 0x00200000, "EOR");
			//MOV
			RegisterDataOpLogical((x) => x, 0x01a00000, "MOV");
			//MVN
			RegisterDataOpLogical((x) => ~x, 0x01e00000, "MVN");
			//ORR
			RegisterDataOpLogical((a, b) => a | b, 0x01800000, "ORR");


			//Logical Data Ops for comparisons
			//TEQ
			RegisterDataOpLogical_CMP((a, b) => a ^ b, 0x01300000, "TEQ");
			//TST
			RegisterDataOpLogical_CMP((a, b) => a & b, 0x01100000, "TST");

			//Arithmetic Data ops (add/subtract) which support S flag

			//ADC (ARMv7 A8.8.3)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(a, b, CPSR.cf), 0x00a00000, "ADC");
			//ADD (ARMv7 A8.8.8)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(a, b, 0), 0x00800000, "ADD");
			//RSB (ARMv7 A8.8.154)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(~a, b, 1), 0x00600000, "RSB");
			//RSC (ARMv7 A8.8.157)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(~a, b, CPSR.cf), 0x00e00000, "RSC");
			//SBC (ARMv7 A8.8.163)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(a, ~b, CPSR.cf), 0x00c00000, "SBC");
			//SUB (ARMv7 A8.8.224)
			RegisterDataOpAWC((a, b) => ARMAddWithCarry(a, ~b, 1), 0x00400000, "SUB");

			//Arithmetic Data Ops for comparisons
			//CMN (ARMv7 A8.8.36)
			RegisterDataOpAWC_CMP((a, b) => ARMAddWithCarry(a, b, 0), 0x01700000, "CMN");
			//CMP (ARMv7 A8.8.39)
			RegisterDataOpAWC_CMP((a, b) => ARMAddWithCarry(a, ~b, 1), 0x01500000, "CMP");


			//Multiply instructions
			//The encodings are taken from the summary in ARMv7 A5.2.5
			//(they should correspond to the "Encoding A1" variant which
			//covers ARMv4-v7)

			//MUL (ARMv7 A8.8.114)
			RegisterDataOpMultiply((Int32 addend, Int32 a, Int32 b) => new MultiplyOpResult(1, a * b), 0x00000090, true, "MUL");
			//MLA (ARMv7 A8.8.100)
			RegisterDataOpMultiply((Int32 addend, Int32 a, Int32 b) => new MultiplyOpResult(2, a * b + addend), 0x00200090, true, "MLA");
			//UMAAL and MLS are v6 only
			//RegisterDataOpMultiply(null, 0x00400090, false, "UMAAL");
			//RegisterDataOpMultiply(null, 0x00600090, false, "MLS");

			//UMULL (ARMv7 A8.8.257)
			RegisterDataOpMultiply((UInt64 addend, UInt64 a, UInt64 b) => new LongMultiplyOpResult(2, a * b), 0x00800090, true, "UMULL");
			//UMLAL (ARMv7 A8.8.256)
			RegisterDataOpMultiply((UInt64 addend, UInt64 a, UInt64 b) => new LongMultiplyOpResult(2, a * b + addend), 0x00a00090, true, "UMLAL");
			//UMULL (ARMv7 A8.8.189)
			RegisterDataOpMultiply((Int64 addend, Int64 a, Int64 b) => new LongMultiplyOpResult(2, a * b), 0x00c00090, true, "SMULL");
			//UMULL (ARMv7 A8.8.178)
			RegisterDataOpMultiply((Int64 addend, Int64 a, Int64 b) => new LongMultiplyOpResult(2, a * b + addend), 0x00e00090, true, "SMLAL");

		}

		internal delegate uint DataOpEvaluateOperand2Func(uint opcode, ref bool shift_carry);

		internal delegate AWCResult DataOpAWCOperation(uint a, uint b);
		internal delegate uint DataOpLogicalBinary(uint a, uint b);
		internal delegate uint DataOpLogicalUnary(uint a);

		internal delegate void DataOpSaveResultFunc(uint Rd, uint result);

		internal delegate void DataOpHandleSetFlagsFunc(uint opcode, uint Rd, uint result, bool shift_carry);
		internal delegate void DataOpHandleSetFlagsAWCFunc(uint opcode, uint Rd, AWCResult awcResult);

		internal uint DataOpEvaluateOperand2_Immediate(uint opcode, ref bool shift_carry)
		{
			return b_immediate(opcode & op2_mask, ref shift_carry);
		}
		internal uint DataOpEvaluateOperand2_Register(uint opcode, ref bool shift_carry)
		{
			return b_reg(opcode & op2_mask, ref shift_carry);
		}

		internal void DataOpNoSaveResult(uint Rd, uint result) { }
		internal void DataOpSaveResult(uint Rd, uint result) {
			mGPR[Rd] = result;
		}

		internal void DataOpHandleSetFlagsLogical_NOP(uint opcode, uint Rd, uint result, bool shift_carry){ }
		internal void DataOpHandleSetFlagsAWC_NOP(uint opcode, uint Rd, AWCResult awcResult){ }

		internal void DataOpHandleSetFlagsLogical_CMP(uint opcode, uint Rd, uint result, bool shift_carry) {
			//The CPSR is always set the same way for logical operations
			//cf. MOV (ARMv7 A8.8.102)
			CPSR.set_NZCV((result & 0x80000000) != 0, result == 0, shift_carry, CPSR.vf);
		}
		internal void DataOpHandleSetFlagsAWC_CMP(uint opcode, uint Rd, AWCResult awcResult)
		{
			//The CPSR is set the same way for all AWC-based instructions (the only difference
			//is the input to AWC).
			CPSR.set_NZCV((awcResult.result & 0x80000000) != 0, awcResult.result == 0, awcResult.carry_out, awcResult.overflow);
		}

		internal void DataOpHandleSetFlagsLogical_NonCMP(uint opcode, uint Rd, uint result, bool shift_carry)
		{
			if (Rd == GeneralPurposeRegisters.PCRegisterIndex)
				CPSR.RestoreCPUMode();
			else
				DataOpHandleSetFlagsLogical_CMP(opcode, Rd, result, shift_carry);
		}
		internal void DataOpHandleSetFlagsAWC_NonCMP(uint opcode, uint Rd, AWCResult awcResult)
		{
			if (Rd == GeneralPurposeRegisters.PCRegisterIndex)
				CPSR.RestoreCPUMode();
			else
				DataOpHandleSetFlagsAWC_CMP(opcode, Rd, awcResult);
		}

		static internal InstructionFunc GenerateLogicalDataOpUnary(
            DataOpEvaluateOperand2Func EvaluateOperand2, DataOpLogicalUnary UnaryOp, DataOpSaveResultFunc SaveResult, DataOpHandleSetFlagsFunc SetFlags)
		{
			return delegate(uint opcode)
			{
				uint Rd = ((opcode & rd_mask) >> 12);
				//Unary operations only have operand 2 (which is denoted by b everywhere else)
				bool shift_carry = false;
				uint operand = EvaluateOperand2(opcode, ref shift_carry);
				uint result = UnaryOp(operand);
				SaveResult(Rd, result);
				SetFlags(opcode, Rd, result, shift_carry);
				return 1;
			};
		}

		internal InstructionFunc GenerateLogicalDataOpBinary(DataOpEvaluateOperand2Func EvaluateOperand2, DataOpLogicalBinary BinaryOp, DataOpSaveResultFunc SaveResult, DataOpHandleSetFlagsFunc SetFlags)
		{
			return delegate(uint opcode)
			{
				uint Rd = ((opcode & rd_mask) >> 12);
				uint Rn = ((opcode & rn_mask) >> 16);
				uint a = get_reg(Rn);

				bool shift_carry = false;
				uint b = EvaluateOperand2(opcode, ref shift_carry);

				uint result = BinaryOp(a, b);
				SaveResult(Rd, result);
				SetFlags(opcode, Rd, result, shift_carry);
				return 1;
			};
		}

		internal InstructionFunc GenerateAWCDataOp(DataOpEvaluateOperand2Func EvaluateOperand2, DataOpAWCOperation AWCOp, DataOpSaveResultFunc SaveResult, DataOpHandleSetFlagsAWCFunc SetFlags)
		{
			return delegate(uint opcode)
			{
				uint Rd = ((opcode & rd_mask) >> 12);
				uint Rn = ((opcode & rn_mask) >> 16);
				uint a = get_reg(Rn);

				bool shift_carry = false;
				uint b = EvaluateOperand2(opcode, ref shift_carry);

				AWCResult awcResult = AWCOp(a, b);
				SaveResult(Rd, awcResult.result);
				SetFlags(opcode, Rd, awcResult);
				return 1;
			};
		}
		internal abstract class DataOpBuilder
		{
			//The instance of the outside class that created this object
			//(we need this to use the handler functions).
			public ARM7TDMICore owner;

			public uint opcode;
			protected uint wildcard_mask;
			public string name;
			protected DataOpEvaluateOperand2Func EvaluateOperand2;
			protected DataOpSaveResultFunc SaveResult;

			public DataOpBuilder()
			{
				//For all data instructions, the only bits that determine any part
				//of the opcode itself are 27-20, 7 and 4
				wildcard_mask = 0xf00fff6f;
			}
			protected virtual void Register(InstructionFunc BuiltHandler)
			{
				owner.RegisterOpcode(opcode, wildcard_mask, BuiltHandler, name);
			}
			//Final Phase: generates an InstructionFunc and calls Register
			protected abstract void FinishBuild();
			//Phase 3, calls FinishBuild();
			protected virtual void BuildOperand2()
			{
				opcode &= ~imm_mask;
				wildcard_mask &= ~imm_mask;
				//Clear bits 7-4
				opcode &= 0xffffff0f;
				System.Diagnostics.Debug.Assert((wildcard_mask & 0x00000090) == 0);

				//Register version
				//There are exactly two forms of the register version: 
				//Immediate shift (bits 7-4 = xxx0) and Register shift (bits 7-4 = 0xx1)
				//Immediate shift
				wildcard_mask |= 0x000000e0;
				EvaluateOperand2 = owner.DataOpEvaluateOperand2_Register;
				FinishBuild();
				opcode &= 0xffffff0f;
				wildcard_mask &= 0xffffff0f;
				//Register shift
				wildcard_mask |= 0x00000060;
				opcode |= 0x00000010;
				EvaluateOperand2 = owner.DataOpEvaluateOperand2_Register;
				FinishBuild();
				opcode &= 0xffffff0f;
				wildcard_mask &= 0xffffff0f;

				//Immediate version
				opcode |= imm_mask;
				wildcard_mask |= 0x000000f0;
				EvaluateOperand2 = owner.DataOpEvaluateOperand2_Immediate;
				FinishBuild();

				wildcard_mask &= 0xffffff0f;
				opcode &= ~imm_mask;
			}
			//Phase 2, calls BuildOperand2
			protected abstract void BuildSetFlags();
			//Phase 1, calls BuildSetFlags
			protected virtual void BuildSaveResult()
			{
				SaveResult = owner.DataOpSaveResult;
				BuildSetFlags();
			}
			public virtual void StartBuild()
			{
				BuildSaveResult();
			}
		}
		internal class DataOpBuilderLogical : DataOpBuilder
		{
			public bool IsUnaryOperation;
			public DataOpLogicalBinary BinaryOp;
			public DataOpLogicalUnary UnaryOp;
			protected DataOpHandleSetFlagsFunc SetFlags;
			protected override void FinishBuild()
			{
				InstructionFunc handler;
				if (IsUnaryOperation)
					handler = ARM7TDMICore.GenerateLogicalDataOpUnary(EvaluateOperand2, UnaryOp, SaveResult, SetFlags);
				else
					handler = owner.GenerateLogicalDataOpBinary(EvaluateOperand2, BinaryOp, SaveResult, SetFlags);
				Register(handler);
			}
			protected override void BuildSetFlags()
			{
				string original_name = name;
				opcode &= ~s_mask;
				wildcard_mask &= ~s_mask;


				//non-S version
				SetFlags = owner.DataOpHandleSetFlagsLogical_NOP;
				BuildOperand2();

				//S version
				SetFlags = owner.DataOpHandleSetFlagsLogical_NonCMP;
				opcode |= s_mask;
				name += "S";
				BuildOperand2();

				name = original_name;
				opcode &= ~s_mask;
			}
		}
		internal class DataOpBuilderLogicalCMP : DataOpBuilderLogical
		{
			protected override void BuildSetFlags()
			{
				SetFlags = owner.DataOpHandleSetFlagsLogical_CMP;
				BuildOperand2();
			}
			protected override void BuildSaveResult()
			{
				SaveResult = owner.DataOpNoSaveResult;
				BuildSetFlags();
			}
		}
		internal class DataOpBuilderAWC : DataOpBuilder
		{

			public DataOpAWCOperation opHandler;
			protected DataOpHandleSetFlagsAWCFunc SetFlags;
			protected override void FinishBuild()
			{
				InstructionFunc handler = owner.GenerateAWCDataOp(EvaluateOperand2, opHandler, SaveResult, SetFlags);
				Register(handler);
			}
			protected override void BuildSetFlags()
			{
				string original_name = name;
				opcode &= ~s_mask;
				wildcard_mask &= ~s_mask;


				//non-S version
				SetFlags = owner.DataOpHandleSetFlagsAWC_NOP;
				BuildOperand2();

				//S version
				SetFlags = owner.DataOpHandleSetFlagsAWC_NonCMP;
				opcode |= s_mask;
				name += "S";
				BuildOperand2();

				name = original_name;
				opcode &= ~s_mask;
			}
		}
		internal class DataOpBuilderAWCCMP : DataOpBuilderAWC
		{
			protected override void BuildSetFlags()
			{
				SetFlags = owner.DataOpHandleSetFlagsAWC_CMP;
				BuildOperand2();
			}
			protected override void BuildSaveResult()
			{
				SaveResult = owner.DataOpNoSaveResult;
				BuildSetFlags();
			}
		}


		internal struct AWCResult
		{
			public uint result;
			public bool carry_out;
			public bool overflow;
			public AWCResult(uint result, bool carry_out, bool overflow)
			{
				this.result = result;
				this.carry_out = carry_out;
				this.overflow = overflow;
			}
		}
		/* The ARMAddWithCarry function produces the output of the AddWithCarry
		 * pseudocode in the ARM reference (ARMv7 A2.2.1). */
		static AWCResult ARMAddWithCarry(uint a, uint b, uint carry_in)
		{
			//The pseudocode in the specification explicitly requires arithmetic to be carried out
			//on "unbounded" values. The carry_out and overflow bits are determined based on what happens
			//when the unbounded result is converted back to a 32bit value.
			UInt64 unsigned_sum = (UInt64)a + (UInt64)b + (UInt64)carry_in;
			Int64 signed_sum = ((Int64)(Int32)a) + ((Int64)(Int32)b) + ((Int64)(Int32)carry_in); //We need double casts to ensure sign-extension.
			UInt32 result = (UInt32)(unsigned_sum & 0xffffffff);
			bool carry_out = (UInt32)result != unsigned_sum;
			bool overflow = (Int32)result != signed_sum;
			return new AWCResult((uint)result, carry_out, overflow);
		}

		static AWCResult ARMAddWithCarry(uint a, uint b, bool carry_in)
		{
			return ARMAddWithCarry(a, b, (uint)(carry_in ? 1 : 0));
		}


		internal struct MultiplyOpResult
		{
			public uint cycles;
			public UInt32 output_value;
			public MultiplyOpResult(uint cycles, UInt32 output_value)
			{
				this.cycles = cycles;
				this.output_value = output_value;
			}
			public MultiplyOpResult(uint cycles, Int32 output_value) : this(cycles, (UInt32)output_value) { }
		}
		internal struct LongMultiplyOpResult
		{
			public uint cycles;
			public UInt64 output_value;
			public LongMultiplyOpResult(uint cycles, UInt64 output_value)
			{
				this.cycles = cycles;
				this.output_value = output_value;
			}
			public LongMultiplyOpResult(uint cycles, Int64 output_value) : this(cycles, (UInt64)output_value) { }
		}

		internal delegate LongMultiplyOpResult DataOpMultiplyLongRaw(UInt64 rdhilo_value, UInt32 rs_value, UInt32 rm_value);
		internal delegate LongMultiplyOpResult DataOpMultiplyLongUnsigned(UInt64 rdhilo_value, UInt64 rs_value, UInt64 rm_value);
		internal delegate LongMultiplyOpResult DataOpMultiplyLongSigned(Int64 rdhilo_value, Int64 rs_value, Int64 rm_value);
		internal delegate MultiplyOpResult DataOpMultiplyRaw(UInt32 rn_value, UInt32 rs_value, UInt32 rm_value);
		internal delegate MultiplyOpResult DataOpMultiplySigned(Int32 rn_value, Int32 rs_value, Int32 rm_value);

		/// <summary>
		/// Compute the number of clock cycles required for a multiply operation
		/// based on one of the operands.
		/// </summary>
		/// <param name="RsData"></param>
		/// <returns></returns>
		internal static uint calculateMultM(uint RsData)
		{
			//replaced the code below with this. Simpler
			//and safer - dale
			//BB - 10/16/2014 - I have no idea where this came from
			//but it was here when I started...
			if (RsData < 0x00000100)        //256
				return 1;
			else if (RsData < 0x00010000)   //65535
				return 2;
			else if (RsData < 0x01000000)   //16777216
				return 3;
			else
				return 4;

		}//calculateMultM


		//32bit multiply instructions (MUL/MLA)
		internal uint ExecuteDataOpMultiply(DataOpMultiplyRaw opHandler, uint opcode, bool setflags)
		{
			//The original code used the masks rd_mask, rn_mask, rm_mask and rs_mask here
			//But the operands are in a strange order so the masks were used in a confusing way.
			//Therefore, we have one of the rare cases where it's better to use magic numbers...
			uint Rm = (opcode)&0xf;
			uint Rs = (opcode>>8)&0xf;
			uint Rd = (opcode>>16)&0xf;
			uint Rn = (opcode>>12)&0xf;

			uint rs_value = get_reg(Rs);
			uint rm_value = get_reg(Rm);
			uint rn_value = get_reg(Rn);
			//The number of cycles taken for multiplication varies depending
			//on the magnitude of rs_value.
			uint M = calculateMultM(rs_value);

			MultiplyOpResult result = opHandler(rn_value, rs_value, rm_value);
			mGPR[Rd] = result.output_value;

			if (setflags)
				CPSR.set_NZCV((result.output_value & 0x80000000) != 0, result.output_value == 0, CPSR.cf, CPSR.vf);

			return M + result.cycles;

		}

		//64bit multiply instructions (SMULL/UMULL/SMLAL/UMLAL)
		internal uint ExecuteDataOpMultiplyLong(DataOpMultiplyLongRaw opHandler, uint opcode, bool setflags)
		{
			//See comment in ExecuteDataOpMultiply
			uint Rm = (opcode) & 0xf;
			uint Rs = (opcode >> 8) & 0xf;
			uint RdHi = (opcode >> 16) & 0xf;
			uint RdLo = (opcode >> 12) & 0xf;

			uint rs_value = get_reg(Rs);
			uint rm_value = get_reg(Rm);
			UInt64 rdhilo_value = (((UInt64)get_reg(RdHi)) << 32) | (UInt64)get_reg(RdLo);
			//The number of cycles taken for multiplication varies depending
			//on the magnitude of rs_value.
			uint M = calculateMultM(rs_value);


			LongMultiplyOpResult result = opHandler(rdhilo_value, rs_value, rm_value);
			mGPR[RdHi] = (UInt32)((result.output_value >> 32) & 0xffffffff);
			mGPR[RdLo] = (UInt32)((result.output_value) & 0xffffffff);
			//If we set the flags after a long multiply, the N flag is based on bit 63, not bit 31.
			if (setflags)
				CPSR.set_NZCV((result.output_value & 0x8000000000000000) != 0, result.output_value == 0, CPSR.cf, CPSR.vf);

			return result.cycles + M;

		}



		/************* FUNCTIONS TO CREATE AND REGISTER DATA OPS *************/



		//Register an arithmetic opcode which allows the S bit.
		internal void RegisterDataOpAWC(DataOpAWCOperation opHandler, uint base_opcode, string basename)
		{
#if DEBUG
			if ((base_opcode & s_mask) != 0)
				throw new Exception("base_opcode and s_mask not disjoint registering " + basename);
#endif
			DataOpBuilderAWC b = new DataOpBuilderAWC();
			b.opcode = base_opcode;
			b.name = basename;
			b.opHandler = opHandler;
			b.owner = this;
			b.StartBuild();
		}
		//Register a comparison opcode (CMP or CMN) which does not allow the S bit and does
		//not modify a destination register.
		internal void RegisterDataOpAWC_CMP(DataOpAWCOperation opHandler, uint base_opcode, string basename)
		{
			DataOpBuilderAWC b = new DataOpBuilderAWCCMP();
			b.opcode = base_opcode;
			b.name = basename;
			b.opHandler = opHandler;
			b.owner = this;
			b.StartBuild();
		}
		//Register an arithmetic opcode which allows the S bit.
		internal void RegisterDataOpLogical(DataOpLogicalBinary opHandler, uint base_opcode, string basename)
		{
#if DEBUG
			if ((base_opcode & s_mask) != 0)
				throw new Exception("base_opcode and s_mask not disjoint registering " + basename);
#endif
			DataOpBuilderLogical b = new DataOpBuilderLogical();
			b.opcode = base_opcode;
			b.name = basename;
			b.IsUnaryOperation = false;
			b.BinaryOp = opHandler;
			b.owner = this;
			b.StartBuild();
		}
		//Register a unary arithmetic opcode which allows the S bit.
		internal void RegisterDataOpLogical(DataOpLogicalUnary opHandler, uint base_opcode, string basename)
		{
			DataOpBuilderLogical b = new DataOpBuilderLogical();
			b.opcode = base_opcode;
			b.name = basename;
			b.IsUnaryOperation = true;
			b.UnaryOp = opHandler;
			b.owner = this;
			b.StartBuild();
		}
		//Register a logical opcode which does not support the S bit or update a destination
		//register (such as TST or TEQ)
		internal void RegisterDataOpLogical_CMP(DataOpLogicalBinary opHandler, uint base_opcode, string basename)
		{
			DataOpBuilderLogical b = new DataOpBuilderLogicalCMP();
			b.opcode = base_opcode;
			b.name = basename;
			b.IsUnaryOperation = false;
			b.BinaryOp = opHandler;
			b.owner = this;
			b.StartBuild();
		}

		internal void RegisterDataOpMultiply(DataOpMultiplyRaw opHandler, uint base_opcode, bool supports_setflags, string basename)
		{
			RegisterOpcode(base_opcode, 0xf00fff0f, delegate(uint opcode) { return ExecuteDataOpMultiply(opHandler, opcode, false); }, basename);
			if (supports_setflags)
				RegisterOpcode(base_opcode | s_mask, 0xf00fff0f, delegate(uint opcode) { return ExecuteDataOpMultiply(opHandler, opcode, true); }, basename + "S");
		}
		internal void RegisterDataOpMultiply(DataOpMultiplySigned opHandler, uint base_opcode, bool supports_setflags, string basename)
		{
			RegisterDataOpMultiply((UInt32 rdhilo_value, UInt32 rs_value, UInt32 rm_value) => opHandler((Int32)rdhilo_value, (Int32)rs_value, (Int32)rm_value), base_opcode, supports_setflags, basename);
		}

		internal void RegisterDataOpMultiply(DataOpMultiplyLongRaw opHandler, uint base_opcode, bool supports_setflags, string basename)
		{
			RegisterOpcode(base_opcode, 0xf00fff0f, delegate(uint opcode) { return ExecuteDataOpMultiplyLong(opHandler, opcode, false); }, basename);
			if (supports_setflags)
				RegisterOpcode(base_opcode | s_mask, 0xf00fff0f, delegate(uint opcode) { return ExecuteDataOpMultiplyLong(opHandler, opcode, true); }, basename + "S");
		}

		internal void RegisterDataOpMultiply(DataOpMultiplyLongUnsigned opHandler, uint base_opcode, bool supports_setflags, string basename)
		{
			RegisterDataOpMultiply((UInt64 rdhilo_value, UInt32 rs_value, UInt32 rm_value) => opHandler(rdhilo_value, rs_value, rm_value), base_opcode, supports_setflags, basename);
		}
		internal void RegisterDataOpMultiply(DataOpMultiplyLongSigned opHandler, uint base_opcode, bool supports_setflags, string basename)
		{
			RegisterDataOpMultiply((UInt64 rdhilo_value, UInt32 rs_value, UInt32 rm_value) => opHandler((Int64)(int)rdhilo_value, (Int64)(int)rs_value, (Int64)(int)rm_value), base_opcode, supports_setflags, basename);
		}


		/* **************** DATA OP HANDLERS ********************** */

		internal uint swap(uint op_code)
		{
			uint address = get_reg((byte)((op_code & rn_mask) >> 16));
			ARMPluginInterfaces.MemorySize ms = ((op_code & byte_mask) != 0) ? ARMPluginInterfaces.MemorySize.Byte : ARMPluginInterfaces.MemorySize.Word;

			uint data = this.GetMemory(address, ms);
            this.SetMemory(address, ms, get_reg(op_code & rm_mask));
			mGPR[((op_code & rd_mask) >> 12)] = data;
			return 4;
		}

		//Move PSR to general purpose register
		internal uint mrs(uint op_code)
		{
			if ((op_code & 0X00400000) == 0)
			{
				//CPSR register, move into destination reg
				mGPR[((op_code & rd_mask) >> 12)] = this.CPSR.Flags;
			}//if
			else
			{
				//get the SPSR for the current cpu mode and set into destination reg
				mGPR[((op_code & rd_mask) >> 12)] = this.CPSR.SPSR;
			}//else
			return 1;
		}//mrs



		internal uint msr(uint opcode)
		{
			uint mask;
			switch (opcode & 0x00090000)
			{
				//bottom 8 bits are the control bits(I,F,T(ignored), 5 mode bits).
				case 0x00010000:
					mask = 0x000000ff;
					break;

				//top 4 bits are the status bits(N,Z,C,V)
				case 0x00080000:
					mask = 0xf0000000;
					break;

				//combination of top 4 and bottom 8 bits
				case 0x00090000:
					mask = 0xf00000ff;
					break;

				//all other fields are off limits
				default:
					mask = 0;
					break;
			}//switch

			if (CPSR.Mode == CPSR.CPUModeEnum.User)
			{
				//if we are in User mode, non privileged mode, cannot touch the non status bits
				//can only change top 4 bits only
				mask &= 0xf0000000;
			}

			uint source;
			if ((opcode & imm_mask) == 0)           //Test applies for both cases
			{
				//get new value from register
				source = (get_reg(opcode & rm_mask) & mask);
			}
			else
			{
				//otherwise, get new value from immmediate operand in opcode
				uint x = opcode & 0x0ff;                                   //Immediate value
				uint y = (opcode & 0xf00) >> 7;                            //Number of rotates

				bool dummy = false;
				source = lsr(x, y, ref dummy);
				source |= (lsl(x, 32 - y, ref dummy) & mask);

			}//else

			if ((opcode & 0x00400000) == 0)
			{
				//update cpsr.
				CPSR.Flags = (CPSR.Flags & ~mask) | source;
			}//if
			else
			{
				//update spsr.
				CPSR.SPSR = (CPSR.SPSR & ~mask) | source;
			}
			return 1;
		}//msr


		internal uint bx(uint opcode)
		{
			uint Rm = (opcode & rm_mask);
			bool link = ((opcode & 0x00000020) != 0);
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


		//----------------------------------------------------------------------------
		//Count Leading Zeros
		internal uint clz(uint op_code)
		{
			uint j = get_reg((byte)(op_code & rm_mask));
			uint count = 32;
			while (j != 0)
			{
				j = j >> 1;
				count--;
			}//while
			mGPR[((op_code & rd_mask) >> 12)] = count;
			return 1;
		}//clz


		/// <summary>
		/// Logical Shift Left 1 position
		/// </summary>
		/// <param name="val"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint lsl(uint val, ref bool cf)
		{
			cf = Utils.msb(val);
			return val << 1;
		}//lsl

		/// <summary>
		/// Logical Shift Left n positions
		/// </summary>
		/// <param name="val"></param>
		/// <param name="distance"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint lsl(uint val, uint distance, ref bool cf)
		{
			for (uint ii = 0; ii < distance; ii++)
			{
				val = lsl(val, ref cf);
			}
			return val;
		}//lsl


		/// <summary>
		/// Logical Shift Right 1 position
		/// </summary>
		/// <param name="val"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint lsr(uint val, ref bool cf)
		{
			cf = Utils.lsb(val);
			return val >> 1;
		}//lsr

		/// <summary>
		/// Logical Shift Right n positions
		/// </summary>
		/// <param name="val"></param>
		/// <param name="distance"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint lsr(uint val, uint distance, ref bool cf)
		{
			for (uint ii = 0; ii < distance; ii++)
			{
				val = lsr(val, ref cf);
			}
			return val;
		}//lsr

		/// <summary>
		/// Arithmetic Shift Right 1 position
		/// </summary>
		/// <param name="val"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint asr(uint val, ref bool cf)
		{
			uint sign = val & Utils.bit_31;
			return (lsr(val, ref cf) | sign);
		}//asr

		/// <summary>
		/// Arithmetic Shift Right n positions
		/// </summary>
		/// <param name="val"></param>
		/// <param name="distance"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint asr(uint val, uint distance, ref bool cf)
		{
			for (uint ii = 0; ii < distance; ii++)
			{
				val = asr(val, ref cf);
			}
			return val;
		}//asr

		/// <summary>
		/// Rotate Right 1 position
		/// </summary>
		/// <param name="val"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint ror(uint val, ref bool cf)
		{
			cf = Utils.lsb(val);
			uint sign = Utils.lsb(val) ? Utils.bit_31 : 0;
			return (val >> 1) | sign;
		}//ror

		/// <summary>
		/// Rotate Right n positions
		/// </summary>
		/// <param name="val"></param>
		/// <param name="distance"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal static uint ror(uint val, uint distance, ref bool cf)
		{
			for (uint ii = 0; ii < distance; ii++)
			{
				val = ror(val, ref cf);
			}
			return val;
		}//ror

		/// <summary>
		/// shift type: 00 = LSL, 01 = LSR, 10 = ASR, 11 = ROR
		/// </summary>
		/// <param name="op2"></param>
		/// <param name="cf"></param>
		/// <returns></returns>
		internal uint b_reg(uint op2, ref bool cf)
		{
			op2 = op2 & op2_mask;
			uint reg = get_reg((byte)(op2 & 0x00f));    /* Register */
			uint shift_type = (op2 & 0x060) >> 5;       /* Type of shift */
			uint distance = 0;

			if ((op2 & 0x010) == 0)
			{                                           /* Immediate value */
				distance = (op2 & 0Xf80) >> 7;
				if (distance == 0)      /* Special cases */
				{
					if (shift_type == 3)
					{
						shift_type = 4;     /* RRX */
						distance = 1;       /* Something non-zero */
					}//if
					else if (shift_type != 0)
						distance = 32;      /* LSL excluded */
				}//if
			}//if
			else
				distance = (get_reg((byte)((op2 & 0XF00) >> 8)) & 0xff);
			/* Register value */

			uint result = 0;
			cf = (CPSR.cf);                 /* Previous carry */
			switch (shift_type)
			{
				case 0x0: result = lsl(reg, distance, ref cf); break;             /* LSL */
				case 0x1: result = lsr(reg, distance, ref cf); break;             /* LSR */
				case 0x2: result = asr(reg, distance, ref cf); break;             /* ASR */
				case 0x3: result = ror(reg, distance, ref cf); break;             /* ROR */
				case 0x4:                                                         /* RRX #1 */
					result = reg >> 1;
					if (!CPSR.cf)
						result &= ~Utils.bit_31;
					else
						result |= Utils.bit_31;

					cf = Utils.lsb(reg);
					break;
			}

			return result;
		}//b_reg

		//----------------------------------------------------------------------------
		internal uint b_immediate(uint op2, ref bool cf)
		{
			uint x = op2 & 0X0FF;                                          /* Immediate value */
			uint y = (op2 & 0XF00) >> 7;                                 /* Number of rotates */
			if (y == 0)
				cf = (CPSR.cf);                 /* Previous carry */
			else
				lsr(x, y, ref cf);

			bool dummy = false;
			return ror(x, y, ref dummy);                               /* Circular rotation */
		}

	}
}
