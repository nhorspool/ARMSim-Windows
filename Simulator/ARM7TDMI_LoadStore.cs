using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARMSim.Simulator
{

	public partial class ARM7TDMICore
	{

		//There are two broad families of single element load/store instructions in ARM:
		//"Normal" loads and stores, such as STR and LDR, and "Halfword" loads and stores
		//such as LDRH. The normal family uses an encoding derived from the STR/LDR encoding
		//and the Halfword family uses an encoding similar to LDRH and STRH. The different
		//types of loads and stores are distributed strangely between the two groups, however.
		//
		// Normal family:
		//   - Load/Store words: LDR, STR
		//   - Load/Store bytes: LDRB, STRB.
		// Halfword family: (Even though not all instructions deal with halfwords, the code below refers to all of them as halfword instructions):
		//   - Load/Store halfwords: LDRH, STRH
		//   - Load signed halfwords: LDRSH
		//   - Load signed bytes: LDRSB


		internal delegate uint ComputeOffsetFunc(uint opcode);
		internal delegate uint ComputeOffsetAddressFunc(uint base_address, uint offset);
		internal delegate uint ComputeAddressFunc(uint base_address, uint offset_address);
		internal delegate void WritebackAddressFunc(uint Rn, uint offset_address);
		internal delegate uint LoadStoreMemoryFunc(uint address, uint Rd, ARMPluginInterfaces.MemorySize memorySize);


		internal uint ComputeOffsetRegister(uint opcode) { bool cf = false; return b_reg(opcode & op2_mask, ref cf); }
		internal uint ComputeOffsetImmediate(uint opcode) { return opcode&0x00000fff; }

		//For the halfword load/store functions (e.g. STRH), the syntax for both types of operand is radically different.
		//If registers are used with HW instructions, no shifts are allowed (just an offset consisting of the register value Rm)
		//The register index is contained in the last 4 bits (which is used for operands called Rm everywhere in the specification)
		internal uint ComputeOffsetRegisterHW(uint opcode) { return get_reg(opcode&rm_mask); }
		//If an immediate constant is given for the offset of an halfword instruction, it is an 8 bit value split over bits 11 - 8 and 3 - 0
		internal uint ComputeOffsetImmediateHW(uint opcode) { return ((opcode&0x00000f00)>>4)| (opcode&0x0000000f); }

		internal uint ComputeOffsetAddressAdd(uint base_address, uint offset) { return base_address + offset; }
		internal uint ComputeOffsetAddressSubtract(uint base_address, uint offset) { return base_address - offset; }


		//These functions return the address to access under each indexing mode (so under pre-indexing, we use the offset address,
		//but under post-indexing, we use the original address since the offset address is only used afterwards).
		internal uint ComputeAddressPreIndex(uint base_address, uint offset_address) { return offset_address; }
		internal uint ComputeAddressPostIndex(uint base_address, uint offset_address) { return base_address; }

		internal void WritebackAddressDisabled(uint Rn, uint offset_address) { return; }
		internal void WritebackAddressEnabled(uint Rn, uint offset_address) { mGPR[Rn] = offset_address; }




		//The cycle counts in the below functions (i.e. 5 or 3 for loads, 2 for stores) were in the original code.
		//I have no idea where they came from originally - BB
		//TODO 10/16/2014 - The old code for LDRH contained some cryptic handling code for aligning the value
		//and toggling Thumb mode when loading into the PC. That code has been omitted here because I can't find
		//any explanation for it anywhere in the ARM documentation and it does not match the word load/store pattern...
		internal uint LoadMemory(uint address, uint Rd, ARMPluginInterfaces.MemorySize memorySize) { 
			mGPR[Rd] = GetMemory(address, memorySize); 
			return (Rd == GeneralPurposeRegisters.PCRegisterIndex) ? (uint)5 : (uint)3;
		}
		internal uint LoadMemorySignedHalfword(uint address, uint Rd, ARMPluginInterfaces.MemorySize memorySize)
		{
			System.Diagnostics.Debug.Assert(memorySize == ARMPluginInterfaces.MemorySize.HalfWord);
			uint value = GetMemory(address, memorySize); 
			//Sign-extend the loaded halfword to a word.
			mGPR[Rd] = (uint)(Int16)(value & 0xffff);
			return (Rd == GeneralPurposeRegisters.PCRegisterIndex) ? (uint)5 : (uint)3;
		}
		internal uint LoadMemorySignedByte(uint address, uint Rd, ARMPluginInterfaces.MemorySize memorySize)
		{
			System.Diagnostics.Debug.Assert(memorySize == ARMPluginInterfaces.MemorySize.Byte);
			uint value = GetMemory(address, memorySize);
			//Sign-extend the loaded byte to a word.
			mGPR[Rd] = (uint)(SByte)(value & 0xff);
			return (Rd == GeneralPurposeRegisters.PCRegisterIndex) ? (uint)5 : (uint)3;
		}

		internal uint StoreMemory(uint address, uint Rd, ARMPluginInterfaces.MemorySize memorySize) {
            SetMemory(address, memorySize, get_reg(Rd));
            return 2;
        }



		internal InstructionFunc GenerateLoadStoreInstruction(ComputeOffsetFunc ComputeOffset, ComputeOffsetAddressFunc ComputeOffsetAddress, ComputeAddressFunc ComputeFinalAddress,
														 WritebackAddressFunc WritebackAddress, ARMPluginInterfaces.MemorySize memorySize, LoadStoreMemoryFunc LoadStoreMemory)
		{
			return delegate(uint opcode)
			{
				uint Rd = (opcode&rd_mask)>>12;
				uint Rn = (opcode&rn_mask)>>16;
				uint base_address = get_reg(Rn);
				uint offset = ComputeOffset(opcode);
				uint offset_address = ComputeOffsetAddress(base_address,offset);
				uint address = ComputeFinalAddress(base_address,offset_address);
				if (TrapUnalignedMemoryAccess){
					if ((address & (uint)(memorySize-1)) != 0)
						throw new UnalignedAccessException(address);
				}
				//Writeback the address (if pre- or post-indexing is not being used,
				//the function will do nothing).
				WritebackAddress(Rn, offset_address);

				//Visit memory and perform whatever operation we came for.
				return LoadStoreMemory(address, Rd, memorySize);
			};
		}


		internal const uint loadstore_preindex_bit = 0x01000000; //This is bit 'P' in the ARM documentation. If set to 1, pre-indexing is used. If set to 0, post-indexing (with writeback) is used.
		internal const uint offset_add_bit = 0x00800000; //This is bit 'U' in the ARM documentation. If 1, the offset is added to the addresss. If 0, the offset is subtracted.
		internal const uint writeback_bit = 0x00200000; //This is bit 'W' in the ARM documentation. If 1, the offset address is written back. If 0, it is not. 
		//If both W and P are 1, the instruction is the 'T' variation, which is not supported by ARMv4T (and not supported here either).

		//For Word/Byte instructions, loadstore_register_bit determines whether offset is a register or immediate constant
		//For Halfword instructions, loadstore_immediate_bit_hw is used instead, and has the opposite meaning.

		internal const uint loadstore_register_bit = 0x02000000; //If this bit is 1, the offset operand is given by a (shifted) register. If 0, the offset is an immediate constant. (This is the opposite of the usual imm_mask convention).
		internal const uint loadstore_immediate_bit_hw = 0x00400000; //If this bit is 0, the offset operand is given by a (shifted) register. If 1, the offset is an immediate constant. (This is the opposite of the usual imm_mask convention).
		internal const uint loadstore_byte_bit = 0x00400000; //If this bit is 1, a byte is loaded/stored. If it is 0, a word is loaded/stored. Note that halfword instructions like STRH have a different encoding.

		/**** FUNCTIONS TO CREATE AND REGISTER THE "NORMAL" (BYTE/WORD) LOAD AND STORE INSTRUCTIONS ****/

		//TODO LoadStoreBuilder may be better off as a struct
		internal class LoadStoreBuilder
		{
			public ComputeOffsetFunc ComputeOffset;
			public ComputeOffsetAddressFunc ComputeOffsetAddress;
			public ComputeAddressFunc ComputeFinalAddress;
			public WritebackAddressFunc WritebackAddress;
			public ARMPluginInterfaces.MemorySize memorySize;
			public LoadStoreMemoryFunc LoadStoreMemory;
			public uint opcode;
			public uint wildcard_mask;
			public string name;
		}
		internal void FinishBuildingLoadStoreOp(LoadStoreBuilder b)
		{
			InstructionFunc handler = GenerateLoadStoreInstruction(b.ComputeOffset, b.ComputeOffsetAddress, b.ComputeFinalAddress, b.WritebackAddress, b.memorySize, b.LoadStoreMemory);
			RegisterOpcode(b.opcode, b.wildcard_mask, handler, b.name);
		}

		void BuildLoadStore_IndexWriteBack(LoadStoreBuilder b)
		{
			//Phase 3 - Set indexing (pre or post) and writeback behavior
			//Even though these are theoretically independent options, there is a special case
			//So we handle them all here.

			//Clear relevant flags:
			b.opcode &= ~(loadstore_preindex_bit | writeback_bit);

			//Case 1: Post-indexing (index_bit = 0) with writeback (by default) and writeback_bit = 0
			//Note that post-indexing is supposed to always have writeback, so the correct
			//protocol is for W = 0 when index_bit = 0.
			b.ComputeFinalAddress = ComputeAddressPostIndex;
			b.WritebackAddress = WritebackAddressEnabled;
			FinishBuildingLoadStoreOp(b);
			b.opcode &= ~(loadstore_preindex_bit | writeback_bit);

			//Case 2: Post-indexing (index_bit = 0) with writeback (by default) and writeback_bit = 1
			//This actually corresponds to a special type of instruction with a T suffix (e.g. STRT)
			//cf. ARMv7 A8.8.92
			//For now, we ignore it.
			//TODO stop ignoring it.


			//Case 3: Pre-indexing (index_bit = 1) with no writeback (writeback_bit = 0)
			b.opcode |= loadstore_preindex_bit;
			b.ComputeFinalAddress = ComputeAddressPreIndex;
			b.WritebackAddress = WritebackAddressDisabled;
			FinishBuildingLoadStoreOp(b);
			b.opcode &= ~(loadstore_preindex_bit | writeback_bit);

			//Case 3: Pre-indexing (index_bit = 1) with writeback (writeback_bit = 1)
			b.opcode |= loadstore_preindex_bit | writeback_bit;
			b.ComputeFinalAddress = ComputeAddressPreIndex;
			b.WritebackAddress = WritebackAddressEnabled;
			FinishBuildingLoadStoreOp(b);

			//Clear relevant flags:
			b.opcode &= ~(loadstore_preindex_bit | writeback_bit);
		}
		void BuildLoadStore_OffsetAddress(LoadStoreBuilder b)
		{
			//Phase 2 - Set add/subtract behavior (i.e. is offset_address = "address + offset" or "address - offset"

			//Clear add/subtract flag (U bit)
			b.opcode &= ~offset_add_bit;

			//Subtract (U bit = 0)
			b.ComputeOffsetAddress = ComputeOffsetAddressSubtract;
			BuildLoadStore_IndexWriteBack(b);

			//Add (U bit = 1)
			b.opcode |= offset_add_bit;
			b.ComputeOffsetAddress = ComputeOffsetAddressAdd;
			BuildLoadStore_IndexWriteBack(b);

			//Clear add/subtract flag
			b.opcode &= ~offset_add_bit;
		}
		//This version applies to Word or Byte versions
		void BuildLoadStore_Offset(LoadStoreBuilder b)
		{
			//Phase 1 - Set operand type (register/immediate)

			//Clear register flag
			b.opcode &= ~loadstore_register_bit;

			//Immediate Offset (register flag 0)
			b.ComputeOffset = ComputeOffsetImmediate;
			BuildLoadStore_OffsetAddress(b);

			//Register Offset (register flag 1)
			b.opcode |= loadstore_register_bit;
			b.ComputeOffset = ComputeOffsetRegister;
			BuildLoadStore_OffsetAddress(b);

			//Clear register flag
			b.opcode &= ~loadstore_register_bit;
		}
		//This version applies to Halfword/Signed Halfword/Signed Byte versions
		void BuildLoadStore_Offset_SignedByte_HalfWord(LoadStoreBuilder b)
		{
			//Phase 1 - Set operand type (register/immediate)

			//Clear register flag
			b.opcode &= ~loadstore_immediate_bit_hw;


			//Register Offset (immediate flag 0)
			b.ComputeOffset = ComputeOffsetRegisterHW;
			BuildLoadStore_OffsetAddress(b);

			//Immediate Offset (immediate flag 1)
			b.opcode |= loadstore_immediate_bit_hw;
			b.ComputeOffset = ComputeOffsetImmediateHW;
			BuildLoadStore_OffsetAddress(b);

			//Clear register flag
			b.opcode &= ~loadstore_immediate_bit_hw;
		}

		//This step is only used by the Word/Byte versions
		void BuildLoadStore_WordAndByte(LoadStoreBuilder b)
		{
			//Phase 0 - Choose memory size (word/byte)
			string originalName = b.name;

			b.opcode &= ~loadstore_byte_bit;
			//Case 1: Word
			b.memorySize = ARMPluginInterfaces.MemorySize.Word;
			BuildLoadStore_Offset(b);

			//Case 2: Byte
			b.name += "B";
			b.opcode |= loadstore_byte_bit;
			b.memorySize = ARMPluginInterfaces.MemorySize.Byte;
			BuildLoadStore_Offset(b);

			b.name = originalName;
			b.opcode &= ~loadstore_byte_bit;

		}

		internal void RegisterARMLoadStoreOps()
		{
			uint base_opcode;
			uint wildcard_mask; //The values of bits 20-27 vary between branches. The bit B (bit 4) in the specification is irrelevant here.
			LoadStoreBuilder b = new LoadStoreBuilder();

			//Create variants of STR and STRB
			base_opcode = 0x04000000;
			wildcard_mask = 0xf00fffff; //The values of bits 20-27 vary between branches. The bit B (bit 4) in the specification is irrelevant here.
			b.name = "STR";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = StoreMemory;
			BuildLoadStore_WordAndByte(b);

			//Create variants of LDR and LDRB
			base_opcode = 0x04100000;
			wildcard_mask = 0xf00fffff;
			b.name = "LDR";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = LoadMemory;
			BuildLoadStore_WordAndByte(b);

			//Create variants of STRH
			base_opcode = 0x000000b0;
			wildcard_mask = 0xf00fff0f;
			b.name = "STRH";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = StoreMemory;
			b.memorySize = ARMPluginInterfaces.MemorySize.HalfWord;
			BuildLoadStore_Offset_SignedByte_HalfWord(b);

			//Create variants of LDRH
			base_opcode = 0x001000b0;
			wildcard_mask = 0xf00fff0f;
			b.name = "LDRH";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = LoadMemory;
			b.memorySize = ARMPluginInterfaces.MemorySize.HalfWord;
			BuildLoadStore_Offset_SignedByte_HalfWord(b);

			//Create variants of LDRSB (signed byte)
			base_opcode = 0x001000d0;
			wildcard_mask = 0xf00fff0f;
			b.name = "LDRSB";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = LoadMemorySignedByte;
			b.memorySize = ARMPluginInterfaces.MemorySize.Byte;
			BuildLoadStore_Offset_SignedByte_HalfWord(b);

			//Create variants of LDRSH (signed halfword)
			base_opcode = 0x001000f0;
			wildcard_mask = 0xf00fff0f;
			b.name = "LDRSB";
			b.opcode = base_opcode;
			b.wildcard_mask = wildcard_mask;
			b.LoadStoreMemory = LoadMemorySignedHalfword;
			b.memorySize = ARMPluginInterfaces.MemorySize.HalfWord;
			BuildLoadStore_Offset_SignedByte_HalfWord(b);


			//LDRD and STRD are load/store "dual" instructions which load or store two 
			//adjacent words to two registers.

			//LDRD is v5TE only
			//RegisterOpcode(0x000000d0, 0xf1afff0f, null, "LDRD"); //LDRD (Register)
			//RegisterOpcode(0x004000d0, 0xf1afff0f, null, "LDRD"); //LDRD (immediate)
			//STRD is v5 and up only
			//RegisterOpcode(0x000000f0, 0xf1afff0f, null, "STRD"); //STRD (Register)
			//RegisterOpcode(0x004000f0, 0xf1afff0f, null, "STRD"); //STRD (immediate)
		}
	}
}
