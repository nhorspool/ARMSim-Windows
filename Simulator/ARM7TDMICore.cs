using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARMSim.Simulator
{



	//An InstructionFunc handler takes an opcode, executes it, then returns the number
	//of cycles used.
	public delegate uint InstructionFunc(uint opCode);
	public abstract class InstructionRegistry
	{

		//Generates all bitstrings of the form (base_string | x) where (x & ~wildcard_mask) == 0
		//(i.e. the 1 bits of x are a subset of the 1 bits of wildcard_mask)
		public static IEnumerable<uint> BitStringWildcard(uint base_string, uint wildcard_mask){
			if ((base_string & wildcard_mask) != 0)
			{
				throw new InstructionRegistryException("base_string and mask are not disjoint");
			}
			uint popcount = wildcard_mask & 1;
			uint x;
			
			x = wildcard_mask;
			while (x != 0)
				popcount += (x >>= 1)&1;

			int[] map = new int[popcount];
			int pos = 0;
			for (int i = 0; i < popcount; i++)
			{
				while ((wildcard_mask & (1 << pos)) == 0)
					pos++;
				map[i] = pos;
				pos++;
			}
			for (int i = 0; i < (1 << (int)popcount); i++)
			{
				x = 0;
				for (int j = 0; j < popcount; j++)
					x |= (uint)(((i >> j) & 1) << map[j]);
				yield return (x|base_string);
			}

		}

		[Serializable]
        public class InstructionRegistryException : Exception
		{
			public InstructionRegistryException(string s): base(s){}
		}
		private InstructionFunc[] registry;
		private string[] registeredNames;
		private InstructionFunc defaultInstruction;

		protected InstructionRegistry(int width, InstructionFunc defaultInstruction)
		{
			registry = new InstructionFunc[1 << width];
			registeredNames = new string[1 << width];
			this.defaultInstruction = defaultInstruction;
			for (int i = 0; i < registry.Length; i++)
			{
				registry[i] = defaultInstruction;
				registeredNames[i] = "(none)";
			}
		}
		//This function converts an opcode to a bitstring of the width of the
		//registry.
		protected abstract uint Narrow(uint opCode);
		public InstructionFunc this[uint opCode]
		{
			get { return registry[Narrow(opCode)]; }
		}

		//Register a range of opcodes to be handled by a specific handler.
		//The basecode gives the first opcode in the range and mask contains the
		//set of free bits in the range (whose value can be either 0 or 1). Note
		//that basecode and mask should be relative to the narrowed range (i.e. the width
		//of the registry), not actual opcodes)


		private void RegisterNarrow(uint basecode, uint wildcard_mask, InstructionFunc handler, string name = "")
		{
			RegisterNarrow(BitStringWildcard(basecode, wildcard_mask), handler, name);
		}
		
		private void RegisterNarrow(IEnumerable<uint> narrow_opcodes, InstructionFunc handler, string name = "")
		{
			foreach (uint i in narrow_opcodes)
				RegisterNarrow(i, handler, name);
		}
		private void RegisterNarrow(uint basecode, InstructionFunc handler, string name = "")
		{
			if (registry[basecode] != defaultInstruction)
				System.Diagnostics.Debug.Assert(registry[basecode] == defaultInstruction, "Attempting to register " + name + " into slot taken by " + registeredNames[basecode]);
			registry[basecode] = handler;
			registeredNames[basecode] = name;
		}
		public void RegisterOpCode(uint opCode, uint wildcard_mask, InstructionFunc handler, string name = "")
		{
			RegisterNarrow(Narrow(opCode), Narrow(wildcard_mask), handler,name);
		}
		public void RegisterOpCode(uint opCode, InstructionFunc handler, string name = "")
		{
			RegisterNarrow(Narrow(opCode), handler, name);
		}
		public void RegisterOpCode(IEnumerable<uint> opCodes, InstructionFunc handler, string name = "")
		{
			foreach (uint i in opCodes)
				RegisterNarrow(Narrow(i), handler, name);
		}
		public InstructionFunc UnregisterNarrow(uint basecode)
		{
			InstructionFunc f = registry[basecode];
			registry[basecode] = defaultInstruction;
			return f;
		}
		public InstructionFunc UnRegister(uint opCode)
		{
			return UnregisterNarrow(Narrow(opCode));
		}
	}

	public abstract partial class ARM7TDMICore: BaseARMCore
	{

		protected ARM7TDMICore()
		{
			RegisterARMInstructions();
			RegisterThumbInstructions();
		}

		/// <summary>
		/// Helper function to return the contents of a cpu register (r0 - r15)
		/// Note that when requesting r15 (the PC) a constant value of the current word size
		/// is added. (4 bytes in ARM mode, 2 bytes in Thumb mode).
		/// </summary>
		/// <param name="reg"></param>
		/// <returns></returns>
		private uint get_reg(uint reg)
		{
			if (reg == GeneralPurposeRegisters.PCRegisterIndex)
				return (mGPR.PC + (CPSR.tf ? (uint)2 : (uint)4));

			return mGPR[reg];
		}
		
		protected static uint DefaultInvalidOpCodeHandler(uint opCode)
		{
			return 0;
		}

		public override uint ExecuteInstruction(uint opCode, out bool swiInstruction)
		{
			if (this.CPSR.tf)
                return ExecuteThumbInstruction(opCode, out swiInstruction);
			else
                return ExecuteARMInstruction(opCode, out swiInstruction);
		}


	}
}
