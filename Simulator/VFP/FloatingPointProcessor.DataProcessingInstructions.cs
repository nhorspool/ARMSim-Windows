namespace ARMSim.Simulator.VFP
{
    /// <summary>
    /// Part of the FloatingPointProcessor class
    /// Handles all the data processing instructions
    /// </summary>
    partial class FloatingPointProcessor
    {
        /// <summary>
        /// Execute a data processing instruction. Decode it and execute it.
        /// </summary>
        /// <param name="opCode">opcode of instruction</param>
        /// <param name="singleType">true if single precision</param>
        /// <returns>clock cycles used</returns>
        public uint data_processing(uint opCode, bool singleType)
        {
            uint Fn = UnpackFn(opCode);
            uint Fd = UnpackFd(opCode);
            uint Fm = UnpackFm(opCode);
            uint pqrs = UnpackPQRS(opCode);

            //pqrs flags determine instruction
            switch (pqrs)
            {
                case 0x0:
                    //fmacs,fmacd
                    if (singleType)
                        _FPR.WriteS(Fd,_FPR.ReadS(Fd) + (_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, _FPR.ReadD(Fd) + (_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x1:
                    //fnmacs,fnmacd
                    if (singleType)
                        _FPR.WriteS(Fd, _FPR.ReadS(Fd) - (_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, _FPR.ReadD(Fd) - (_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x2:
                    //fmscs,fmscd
                    if (singleType)
                        _FPR.WriteS(Fd, -(_FPR.ReadS(Fd)) + (_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, -(_FPR.ReadD(Fd)) + (_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x3:
                    //fnmscs,fnmscd
                    if (singleType)
                        _FPR.WriteS(Fd, -(_FPR.ReadS(Fd)) - (_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, -(_FPR.ReadD(Fd)) - (_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x4:
                    //fmuls,fmuld
                    if (singleType)
                        _FPR.WriteS(Fd, (_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, (_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x5:
                    //fnmuls,fnmuld
                    if (singleType)
                        _FPR.WriteS(Fd, -(_FPR.ReadS(Fn) * _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, -(_FPR.ReadD(Fn) * _FPR.ReadD(Fm)));
                    break;
                case 0x6:
                    //fadds,faddd
                    if (singleType)
                        _FPR.WriteS(Fd, (_FPR.ReadS(Fn) + _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, (_FPR.ReadD(Fn) + _FPR.ReadD(Fm)));
                    break;
                case 0x7:
                    //fsubs,fsubd
                    if (singleType)
                        _FPR.WriteS(Fd, (_FPR.ReadS(Fn) - _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, (_FPR.ReadD(Fn) - _FPR.ReadD(Fm)));
                    break;
                case 0x8:
                    //fdivs,fdivd
                    if (singleType)
                        _FPR.WriteS(Fd, (_FPR.ReadS(Fn) / _FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, (_FPR.ReadD(Fn) / _FPR.ReadD(Fm)));
                    break;
                case 0xf:
                    return extension_instructions(opCode, Fd, Fm);
                default: return 0;
            }//switch

            //todo - must calculate correct clock cycles
            return 1;
        }//data_processing

        //public double onAddD(double op1, double op2) { return op1 + op2; }
        //public float onAddS(float op1, float op2) { return op1 + op2; }

    }//class FloatingPointProcessor
}
