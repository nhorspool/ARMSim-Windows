using System;
using System.Collections.Generic;
using System.Text;

namespace ArmAssembly {

public class DisassembleARM {
    //register source/destination masks
    private const uint rn_mask = 0x000f0000;
    private const uint rd_mask = 0x0000f000;
    private const uint rs_mask = 0x00000f00;
    private const uint rm_mask = 0x0000000f;

    private const uint s_mask = 0x00100000;

    private const uint op2_mask = 0x00000fff;
    private const uint up_mask = 0x00800000;		//up,down mask(sbhw opcodes)
    private const uint imm_hw_mask = 0x00400000;	//half word versions

    private const uint pre_mask = 0x01000000;  		// P bit
    private const uint load_mask = 0x00100000;
    private const uint write_back_mask = 0x00200000;  // W bit

    private const uint byte_mask = 0x00400000;
    private const uint mode_mask = 0x0000001f;
    private const uint imm_mask = 0x02000000;

    private const uint link_mask = 0x01000000;
    private const uint branch_field = 0x00ffffff;
    private const uint branch_sign = 0x00800000;

    private const uint undef_mask = 0x0e000010;
    private const uint undef_code = 0x06000010;
    private const uint user_mask = 0x00400000;

	private static string[] condCode =
	    { "EQ", "NE", "CS", "CC", "MI", "PL", "VS", "VC",
	      "HI", "LS", "GE", "LT", "GT", "LE", "",  null };

	private static string[] regName =
	    { "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7",
	      "r8", "r9", "r10", "fp", "ip", "sp", "lr", "pc" };

    private static string[] shiftKind =
    	{ "lsl", "lsr", "asr", "ror" };

	private static string opName, condition, Sbit, operands;
	
	private static uint pc;  // needed for disassembling branch ops

    public static ARMSim.Simulator.AddressLabelPair[] CurrentLabels { get; set; }

    private DisassembleARM() { }  // prevent instantiation of class

    public static string DisassembleARMInstruction(uint opCode, uint npc) {
        condition = condCode[opCode >> 28];
        if (condition == null)
        {
            if ((opCode & 0xFF000000) == 0xFF000000)
                return String.Format("{0,-7} 0X{1:X}", "SWI", opCode&0x00FFFFFF); ;
            return null;  // special instruction?
        }
	    opName = null;
	    pc = npc;
	    operands = Sbit = "";

        switch ((opCode >> 25) & 0x00000007) {
        case 0x00:  data_op(opCode);  break;		//includes load/store hw & sb
        case 0x01:  data_op(opCode);  break;		//data processing & MSR # */
        case 0x02:  transfer(opCode); break;
        case 0x03:  transfer(opCode); break;
        case 0X04:  multiple(opCode); break;
        case 0x05:  branch(opCode);   break;
        case 0X06:  // floating point or co-processor
        			floatop(opCode);  break;
        case 0X07:
                if (((opCode & 0X0F000000) == 0X0E000000)
                    || ((opCode & 0X0F000000) == 0X0C000000)
                    || ((opCode & 0X0F000000) == 0X0D000000))
                {
                    // floating point or co-processor
                    uint coprocessor = opCode & 0x00000f00;
                    if (coprocessor == 0x00000a00 || coprocessor == 0x00000b00)
                        floatop(opCode);
                }
                if (opName != null) break;
                // swi op?
                if ((opCode & 0X0F000000) == 0X0F000000) {
                    opName = "SWI";
                    operands = "0x" + (opCode & 0xFFFFFF).ToString("X");
                }
                break;
        default: break;
        }
        if (opName == null) return null;
        opName += Sbit + condition;
        if (String.IsNullOrEmpty(operands))
            return String.Format("{0,-7}", opName);
        return String.Format("{0,-7} {1}", opName, operands);
    }//DisassembleARMInstruction

    private const uint mul_long_bit = 0x00800000;
    private const uint mul_acc_bit = 0x00200000;

    private static string[] mulops =
        { "MUL", "MLA", "UMAAL", "MLS",
          "UMULL", "UMLAL", "SMULL", "SMLAL"};

    private static void multiply(uint opcode) {
    	// See Table A5-7 Multiply & multiply-accumulate
        uint Rs = ((opcode & rs_mask) >> 8);
        uint Rm =  (opcode & rm_mask);
        uint Rd = ((opcode & rd_mask) >> 12);
        uint Rn = ((opcode & rn_mask) >> 16);

    	opName = mulops[(opcode >> 21) & 0x7];
        if ((opcode & mul_long_bit) == 0) //Normal:mla,mul
            operands = String.Format("{0},{1},{2}", Rd, Rn, Rm);
        else
        	operands = String.Format("{0},{1},{2},{3}", Rd, Rn, Rm, Rs);
        if ((opcode & s_mask) != 0) Sbit = "S";
    }//multiply

    private static bool is_it_sbhw(uint op_code) {
        if (((op_code & 0x0e000090) == 0x00000090) && ((op_code & 0x00000060) != 0x00000000)	//No multiplies
                && ((op_code & 0x00100040) != 0x00000040))										//No signed stores
            return ((op_code & 0x00400000) != 0) || ((op_code & 0x00000f00) == 0);
        return false;
    }

    private static string[] sbhwops =
        { null, null, "STRH", "LDRH", "STRSB", "LDRSB", "STRSH", "LDRSH" };

    private static void transfer_sbhw(uint op_code) {
        // See Table A5-2
        uint op2 = (op_code >> 5) & 0x3;
        uint ix = (op2 << 1) + ((op_code & load_mask) >> 20);
        if (ix < 2) return;

        string Rd = regName[(op_code & rd_mask) >> 12];
        string am = symbolicAddress(op_code);
        if (am == null)
            am = addressingMode(op_code);
        if (am == null) return;
        operands = String.Format("{0}, {1}", Rd, am);
        opName = sbhwops[ix];
    }

    private static void swap(uint op_code) {
    	opName = (op_code & byte_mask) != 0 ? "SWPB" : "SWP";
        operands = String.Format("{0}, {1}, [{2}]",
        	regName[(op_code >> 12) & 0xF], regName[op_code & 0xF],
        	regName[(op_code >> 16) & 0xF]);
    }

    private static void mrs(uint op_code) {
    	opName = "MRS";
    	operands = String.Format("{0}, {1}", regName[(op_code >> 12) & 0xF],
    				(op_code & 0X00400000) == 0? "CPSR" : "SPSR");
    }

    private static void msr(uint opcode) {
        opName = "MSR";
        string spr = (opcode & 0x00400000) == 0? "CPSR" : "SPSR";
        if ((opcode & imm_mask) == 0)
            operands = String.Format("{0}, {1}", spr, regName[opcode & rm_mask]);
        else
            operands = String.Format("{0}, #0x{1}", spr, ARMExpandImm(opcode).ToString("X"));
    }

    private static void clz(uint op_code) {
    	uint Rd = ((op_code & rd_mask) >> 12);
    	uint Rm = op_code & rm_mask;
    	opName = "CLZ";
    	operands = String.Format("{0}, {1}", regName[Rd], regName[Rm]);
    }

	private static string[] normalOps =
		{ "AND", "EOR", "SUB", "RSB", "ADD", "ADC", "SBC", "RSC",
		  "TST", "TEQ", "CMP", "CMN", "ORR", "MOV", "BIC", "MVN" };

    private static void normal_data_op(uint op_code, uint operation) {
        uint Rd = ((op_code & rd_mask) >> 12);
        uint Rn = ((op_code & rn_mask) >> 16);
        if ((op_code & s_mask) != 0) Sbit = "S";
        opName = normalOps[operation];
        switch (operation)
        {
            case 0x08: // TST
            case 0x09: // TEQ
            case 0x0A: // CMP
            case 0x0B: // CMN  ... only 2 operands for these instructions
                Sbit = "";  // and S bit is never needed in the opcode
                operands = String.Format("{0}, {1}",
                    regName[Rn], shifterOperand(op_code));
                break;
            case 0x0D: // MOV
            case 0x0F: // MVN  ... only 2 operands for these instructions
                operands = String.Format("{0}, {1}",
                    regName[Rd], shifterOperand(op_code));
                break;
            default:   // 3 operands
                operands = String.Format("{0}, {1}, {2}",
                    regName[Rd], regName[Rn], shifterOperand(op_code));
                break;
        }
    }

    private static void bx(uint Rm, bool link) {
    	opName = link? "BLX" : "BX";
        operands = regName[Rm];
    }

    private static void data_op(uint op_code) {
        //check if Arithmetic instruction extension
        //first test is for:mul,muls,mla,mlas
        //second test:umull,umulls,umlal,umlals,smull,smulls,smlal,smlals
        if (((op_code & 0x0fc000f0) == 0x00000090) || ((op_code & 0x0f8000f0) == 0x00800090))
        {
            multiply(op_code);  return;
        }
        if (is_it_sbhw(op_code))
        {
        	transfer_sbhw(op_code);  return;
        }
        if ((op_code & 0x0fb00ff0) == 0x01000090) {
            swap(op_code); return;
        }

        //TST, TEQ, CMP, CMN - all lie in following range, but have S set
        if ((op_code & 0x01900000) == 0x01000000) { //PSR transfers OR BX
            if ((op_code & 0x0fbf0fff) == 0x010f0000)
                mrs(op_code);
            else if (((op_code & 0x0db0f000) == 0x0120f000) && ((op_code & 0x02000010) != 0x00000010))
            	msr(op_code);
            else if ((op_code & 0x0fffffd0) == 0x012fff10)
                bx((op_code & rm_mask), ((op_code & 0x00000020) != 0));
            else if ((op_code & 0x0fff0ff0) == 0x016f0f10)
                clz(op_code);
        } else { //All data processing operations
            uint operation = (op_code & 0x01e00000) >> 21;
            normal_data_op(op_code, operation);
        }
    }

    private static void transfer(uint op_code) {
        if ((op_code & undef_mask) == undef_code) return;
        uint Rd = (op_code & rd_mask) >> 12;
       // uint Rn = (op_code & rn_mask) >> 16;

        string am = symbolicAddress(op_code);
        if (am == null)
            am = addressingMode(op_code);
        if (am == null)
            return;
        operands = String.Format("{0}, {1}", regName[Rd], am);
        opName = (op_code & load_mask) == 0? "STR" : "LDR";
		if ((op_code & byte_mask) != 0) opName += "B";
    }

    private static void branch(uint op_code) {
        if (((op_code & link_mask) != 0) || ((op_code & 0xf0000000) == 0xf0000000))
            opName = "BL";
        else
            opName = "B";

        uint dest = (op_code & branch_field) << 2;
        if ((op_code & branch_sign) != 0)
            dest |= 0xfc000000;
        dest += (uint)(pc + 8);
        string target = SymbolicOffset((int)dest);
        operands = String.Format(
            (target == null) ? "0x{0:X}" : "{1}  @ 0x{0:X}",
            dest, target);
    }


    // private static string[] modeName = { "DA", "IA", "DB", "IB" }; generic names
    private static string[] stmModes = { "ED", "EA", "FD", "FA" };
    private static string[] ldmModes = { "FA", "FD", "EA", "ED" };

    /// <summary>
    /// LDM/STM instruction.
    /// </summary>
    /// <param name="opcode"></param>
    private static void multiple(uint opcode) {
        uint mode = (opcode & 0x01800000) >> 23;            //isolate PU bits
        uint Rn = (opcode & rn_mask) >> 16;                 //get base register
        uint reg_list = opcode & 0x0000ffff;                //get the register list
        bool writeback = (opcode & write_back_mask) != 0;   //determine writeback flag

        if ((opcode & load_mask) == 0)
            opName = "STM" + stmModes[mode];
        else
            opName = "LDM" + ldmModes[mode];
		
		operands = String.Format("{0}{1}, {2}", regName[Rn],
					writeback?"!":"", addressingMode4(reg_list));
    }//multiple

    private static void floatop(uint opcode) {
        bool singleType = ((opcode & 0x00000f00) == 0x00000a00);
        //one of these 3 classes of instructions
        if ((opcode & 0x0f000010) == 0x0e000000)
            fp_data_processing(opcode, singleType);
        else if ((opcode & 0x0e000000) == 0x0c000000)
            fp_load_store(opcode, singleType);
        else if ((opcode & 0x0f000070) == 0x0e000010)
            fp_register_transfer(opcode, singleType);
    }

    private static string[] fpdpops =
        { "FMAC", "FNMAC", "FMSC", "FNMSC", "FMUL", "FNMUL", "FADD", "FSUB", "FDIV" };

    private static void fp_data_processing(uint opcode, bool singleType) {
        uint Fn = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackFn(opcode);
        uint Fd = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackFd(opcode);
        uint Fm = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackFm(opcode);
        uint pqrs = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackPQRS(opcode);
        //pqrs flags determine instruction
        if (pqrs > 8)
            return; // 15 = extension instruction; others are undefined
        opName = fpdpops[pqrs] + (singleType? "S" : "D");
        operands = String.Format(
            singleType ? "f{0}, f{1}, f{2}" : "d{0}, d{1}, d{2}", Fd, Fn, Fm);
    }

    private static void fp_load_store(uint opcode, bool singleType) {
        uint Rn = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackRn(opcode);
        uint Rd = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackRd(opcode);
        uint Fm = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackFm(opcode);
        bool LBit = ((opcode & 0x00100000) != 0);

        //Check if ARMv5TE extension instructions
        if ((opcode & 0x0fe000d0) == 0x0c400010) {
            if (!singleType)
            {
                opName = LBit?  "FMRRD" : "FMDRR";
                operands = String.Format(
                    LBit? "{1}, {2}, d{0}" : "d{0}, {1}, {2}", Fm, regName[Rd], regName[Rn]);
            }
            else
            {
                opName = LBit ? "FMRRS" : "FMSRR";
                operands = String.Format(
                    LBit ? "{1}, {2}, s{0}" : "s{0}, {1}, {2}", Fm, regName[Rd], regName[Rn]);
            }
            return;
        }//if

        uint puw = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackPUW(opcode);
        //bool WBit = ((opcode & 0x00200000) != 0);
        //bool UBit = ((opcode & 0x00800000) != 0);
        //uint Fd = ARMSim.Simulator.VFP.FloatingPointProcessor.UnpackFd(opcode);

        //uint start_reg = singleType ? Fd : Fd * 2;

        //uint count = singleType ? (opcode & 0x0f) : (opcode & 0x0e);
        //uint offset = ((opcode & 0xff) * 4);

        switch (puw)
        {
            case 0:
            case 1:
            case 7:
                //invalid, undefined
                return;
        };
  
    }

    private static void fp_register_transfer(uint opcode, bool singleType)
    {
    }

    private static uint ARMExpandImm(uint opcode) {
        uint val = opcode & 0xFF;
        uint rot = (opcode & 0xF00) >> 7;
        int m = (int)(rot & 0x7);  // mod 8
        return (val >> m) | (val << (12 - m));
    }

//----------------------------------------------------------------------------
// Code copied from ArmInstruction.cs
//----------------------------------------------------------------------------
    
    private static string shifterOperand(uint val) {
        if ((val & 0x02000000) != 0) {
            // it's a 32-bit immediate operand
            int shift = (int)((val & 0xf00) >> 7);
            val &= 0xff;
            // rotate val right by shift places
            while (shift > 0) {
                if ((val & 0x1) != 0)
                    val = (val >> 1) | 0x80000000;
                else
                    val >>= 1;
                shift--;
            }
            return String.Format("#{0}", val);
        }
        string Rm = regName[(int)(val & 0x0f)];
        if ((val & 0xff0) == 0)  // direct register operand
            return Rm;
        string sk = shiftKind[(int)((val >> 5) & 0x3)];
        if ((val & 0x10) == 0) {
            // it's an immediate shift
            int shiftImm = (int)((val & 0xf80) >> 7);
            if (shiftImm == 0) {
                if ((val & 0x60) == 0x60)
                    return String.Format("{0}, rrx", Rm);
                shiftImm = 32;
            }
            return String.Format("{0}, {1} #{2}", Rm, sk, shiftImm);
        } else {
            // it's a register shift
            string Rs = regName[(int)((val >> 8) & 0xf)];
            return String.Format("{0}, {1} {2}", Rm, sk, Rs);
        }
    }

    // Decodes Addressing Modes 2 and 3
    private static string addressingMode(uint val) {
        string pm = ((val & 0x800000) == 0) ? "-" : "";  // U bit
        string Rn = regName[(int)((val >> 16) & 0xf)];
        string wb = ((val & 0x200000) == 0) ? "" : "!";  // W bit
        string Rm = regName[(int)(val & 0xf)];
        int offset = (int)(val & 0xfff);  // for addressing mode 2
        int shiftImm = (int)((val >> 7) & 0x1f);        // ditto
        string sk = shiftKind[(int)((val >> 5) & 0x3)]; // ditto

        // We will select an appropriate format string for the
        // String.Format function.  In this format, we will assume
        // the necessary arguments have been set, assuming
        //    {0} == Rn
        //    {1} == pm == +/- (sign of the additive offset / reg)
        //    {2} == offset
        //    {3} == wb == ! or nothing (for writeback)
        //    {4} == Rm
        //    {5} == sk == shiftkind (one of lsl, lsr ... etc)
        //    {6} == shiftImm == shift immediate

        string fmt = null;

        // assume addressing mode 2 for now
        if ((val & 0x2000000) == 0) {
            // immediate offset/index
            fmt = ((val & 0x1000000) != 0) ?
                "[{0}, #{1}{2}]{3}" : "[{0}], #{1}{2}";
        }

        if (fmt == null && (val & 0xff0) == 0) {
            // register offset/index
            fmt = ((val & 0x1000000) != 0) ?
                "[{0}, {1}{4}]{3}" : "[{0}], {1}{4}";
        }

        if (fmt == null && (val & 0x10) == 0) {
            // scaled register offset/index
            if ((val & 0x1000000) != 0) {
                fmt = "[{0}, {1}{4}, {5} #{6}]{3}";
                if (shiftImm == 0) {
                    if ((val & 0x60) == 0x60)
                        fmt = "[{0}, {1}{4} RRX]{3}";
                    else
                        shiftImm = 32;
                }
            } else {
                fmt = "[{0}], {1}{4}, {5}{6}";
                if (shiftImm == 0) {
                    if ((val & 0x60) == 0x60)
                        fmt = "[{0}], {1}{4}, RRX";
                    else
                        shiftImm = 32;
                }
            }
        }

        // Addressing mode 3
        if (fmt == null) {
            if ((val & 0x60) == 0x0 || (val & 0x100040) == 0x04)
                return null;	// extended instruction space
            if ((val & 0x400000) != 0) {
                offset = (int)((val >> 4) & 0xf0 | (val & 0x0f));
                fmt = ((val & 0x1000000) != 0) ?
                     "[{0}, #{1}{2}]{3}" : "[{0}], #{1}{2}";
            } else
                fmt = ((val & 0x1000000) != 0) ?
                    "[{0}, {1}{4}]{3}" : "[{0}], {1}{4}";
        }
        if (fmt != null)
            return String.Format(fmt, Rn, pm, offset, wb, Rm, sk, shiftImm);
        return null;
    }

    private static string addressingMode4(uint val) {
        uint rb = 0x1;
        int rn = 0;
        StringBuilder sb = new StringBuilder(20);
        string sep = "{";
        val |= 0x50000;  // sentinel value
        val &= 0x5ffff;
        for ( ; ; ) {
            // find first reg in range
            while ((val & rb) == 0) {
                rb <<= 1; rn++;
            }
            if (rn > 15) break;
            int rm = rn;
            // find last reg in range
            while ((val & rb) != 0) {
                rb <<= 1; rn++;
            }
            if (rn > 15) rn = 16;
            sb.Append(sep);
            sb.Append(regName[rm]);
            if ((rn - rm) > 1) {
                sb.Append((rn - rm) > 2 ? "-" : ",");
                sb.Append(regName[rn - 1]);
            }
            sep = ",";
        }
        if (sb.Length == 0)
            return "{}";
        sb.Append("}");
        return sb.ToString();
    }

    // Given an address in memory, find the nearest preceding label
    // and returns a symbolic address relative to that label
    private static string SymbolicOffset(int dest) {
        if (CurrentLabels == null) {   // no labels anywhere, use dot notation
            int diff = dest - (int)pc;
            if (diff < 0)
                return String.Format(".-{0}", -diff);
            return String.Format(".+{0}", diff);
        }
        int len = CurrentLabels.Length;
        uint d = (uint)dest;
        int last = -1;  // index of the nearest label so far
        int min = 0;
        // binary search of the list of labels
        while (min < len) {
            int mid = (min + len) >> 1;
            uint midval = CurrentLabels[mid].Address;
            if (midval > d) {
                len = mid;
                continue;
            }
            if (midval == d)
                return bestName(mid);
            last = mid;
            min = mid + 1;
        }
        if (last < 0) return null;
        if ((uint)pc > CurrentLabels[last].Address && pc <= dest)
            return String.Format(".+0x{0:X}", dest - pc);
        return String.Format("{0}+0x{1:X}", bestName(last),
        	d - CurrentLabels[last].Address);
    }

    // given a particular label in the list, this checks if other labels have the
    // same value and chooses one whose name does not begin with '$'
    private static string bestName(int ix) {
        uint aval = CurrentLabels[ix].Address;
        int i = ix;
        while (i >= 0 && CurrentLabels[i].Address == aval)
        {
            string s = CurrentLabels[i].Label;
            if (s[0] != '$') return s;
            i--;
        }
        i = ix + 1;
        while (i < CurrentLabels.Length && CurrentLabels[i].Address == aval) {
            string s = CurrentLabels[i].Label;
            if (s[0] != '$') return s;
            i++;
        }
        return CurrentLabels[ix].Label;
    }


    // Decodes Addressing Modes 2 and 3 when address is relative to pc
    private static string symbolicAddress(uint val) {
        int rn = (int)((val >> 16) & 0xf);
        if (rn != 15) return null;
        bool isNeg = (val & 0x800000) == 0;
        int offset;

        if ((val & 0x0E0F0000) == 0x040F0000) {
            // Addressing mode 2, immediate offset/index
            offset = (int)(val & 0xfff);
        } else if ((val & 0x0E4F0090) == 0x004F0090) {
            // Addressing mode 3, immediate offset/index
            offset = (int)((val >> 4) & 0xf0 | (val & 0x0f));
        } else
            return null;
        return SymbolicOffset((int)(isNeg ? pc + 8 - offset : pc + 8 + offset));
    }

} // end of class DisassembleARM

} // end of namespace ArmAssembly
