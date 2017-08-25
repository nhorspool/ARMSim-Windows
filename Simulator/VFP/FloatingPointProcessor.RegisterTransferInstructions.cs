namespace ARMSim.Simulator.VFP
{
    /// <summary>
    /// Class handles the register tansfer instructions
    /// Moving a fp register to a general purpose register and back
    /// </summary>
    partial class FloatingPointProcessor
    {
        /// <summary>
        /// Transfer a register from/to general purpose registers and fp registers
        /// </summary>
        /// <param name="opCode">opcode of instruction</param>
        /// <returns>clock cycles used</returns>
        public uint register_transfer(uint opCode)
        {
            System.Diagnostics.Debug.Assert((opCode & 0x0f000070) == 0x0e000010);

            uint Fn = UnpackFn(opCode);                     //get source register
            uint Rd = UnpackRd(opCode);                     //get destination register
            bool Lbit = ((opCode & 0x00100000) != 0);       //L - 1 means load, l - 0 means store
            bool singleType = isSingle(opCode);             //get single/double instruction

            switch ((opCode >> 21) & 0x07)
            {
                case 0x0:
                    if (singleType)
                    {
                        if (!Lbit)
                            _FPR.WriteRaw(Fn, _jm.GPR[Rd]);//fmsr:Fn = Rd
                        else
                            _jm.GPR[Rd] = _FPR.ReadRaw(Fn);//fmrs:Rd = Fn
                    }
                    else
                    {
                        if (!Lbit)
                            _FPR.WriteRaw(Fn, _jm.GPR[Rd], true);//fmdlr:Fn[0:31] = Rd
                        else
                            _jm.GPR[Rd] = _FPR.ReadRaw(Fn, true);//fmrdl:Rd = Fn[0:31]
                    } break;

                case 0x1:
                    if (!Lbit)
                    {
                        _FPR.WriteRaw(Fn, _jm.GPR[Rd], false);//fmdhr:Fn[32:63] = Rd
                    }
                    else
                    {
                        _jm.GPR[Rd] = _FPR.ReadRaw(Fn, false);//fmrdh:Rd = Fn[32:63]
                    } break;
                case 0x7:
                    if (!singleType) break;

                    if (Fn == 2)
                    {
                        //FPSCR
                        if (!Lbit)
                            this.FPSCR.Flags = _jm.GPR[Rd];//fmxr:SystemReg(Fn)=Rd
                        else
                        {
                            if (Rd == 15)
                            {
                                //special case if r15 is destination
                                //it is fmstat instruction, transfer FPSCR to CPU CPSR register
                                uint flags = _jm.CPSR.Flags & 0x0fffffff;
                                flags |= (this.FPSCR.Flags & 0xf0000000);
                                _jm.CPSR.Flags = flags;
                            }
                            else
                            {
                                _jm.GPR[Rd] = this.FPSCR.Flags;//fmrx:Rd=SystemReg(Fn)
                            }
                        }
                    }
                    else if ( (Fn == 0) && Lbit)
                    {
                        //FPSID:Read Only
                        //ToDo-need to get FPSID value from real VFP
                        _jm.GPR[Rd] = 0x12345678;
                    }
                    break;
                default: return 0;
            }//switch
            //todo - must calculate correct clock cycles
            return 1;
        }//register_transfer
    }//class FloatingPointProcessor
}
