namespace ARMSim.Simulator.VFP
{
    /// <summary>
    /// Part of the FloatingPointProcessor class
    /// Handles all the extension instructions
    /// </summary>
    partial class FloatingPointProcessor
    {
        /// <summary>
        /// Execute a extension instruction. Decode it and execute it.
        /// </summary>
        /// <param name="opCode">opcode of instruction</param>
        /// <param name="Fd">Fd register</param>
        /// <param name="Fm">Fm register</param>
        /// <returns>number of clock cyces used</returns>
        public uint extension_instructions(uint opCode, uint Fd, uint Fm)
        {
            bool singleType = isSingle(opCode);

            uint Fn = (opCode >> 15);
            Fn &= 0x1e;
            Fn |= ((opCode >> 7) & 0x01);

            switch (Fn)
            {
                case 0x00:
                    //fcpys,fcpyd:Fd = Fm
                    if(_FPR.isNaN(Fm,singleType))
                        throw new InvalidOperationFloatingPointException("fcpy", singleType);

                    if (singleType)
                        _FPR.WriteS(Fd,_FPR.ReadS(Fm));
                    else
                        _FPR.WriteD(Fd, _FPR.ReadD(Fm));
                    break;
                case 0x01:
                    //fabss,fabsd:Fd = abs(Fm)
                    if (_FPR.isNaN(Fm, singleType))
                        throw new InvalidOperationFloatingPointException("fabs", singleType);

                    if (singleType)
                        _FPR.WriteS(Fd,System.Math.Abs(_FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, System.Math.Abs(_FPR.ReadD(Fm)));
                    break;
                case 0x02:
                    //fnegs,fnegd:Fd = -Fm
                    if (_FPR.isNaN(Fm, singleType))
                        throw new InvalidOperationFloatingPointException("fneg", singleType);

                    if (singleType)
                        _FPR.WriteS(Fd, -(_FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, -(_FPR.ReadD(Fm)));
                    break;
                case 0x03:
                    //fsqrts,fsqrtd:Fd = sqrt(Fm)
                    if (_FPR.isNaN(Fm, singleType))
                        throw new InvalidOperationFloatingPointException("fsqrt", singleType);

                    if (singleType)
                        _FPR.WriteS(Fd, (float)System.Math.Sqrt(_FPR.ReadS(Fm)));
                    else
                        _FPR.WriteD(Fd, System.Math.Sqrt(_FPR.ReadD(Fm)));
                    break;

                case 0x08:
                    //fcmps,fcmpd:Compare Fd to Fm,no exceptions on Nan
                    if(singleType)
                        compareFPS("fcmps", true, _FPR.ReadS(Fd),_FPR.ReadS(Fm));
                    else
                        compareFPD("fcmps", true, _FPR.ReadD(Fd), _FPR.ReadD(Fm));
                    break;

                case 0x09:
                //fcmpes,fcmped:Compare Fd to Fm,exceptions on Nan
                    if (singleType)
                        compareFPS("fcmpe", false, FPR.ReadS(Fd), _FPR.ReadS(Fm));
                    else
                        compareFPD("fcmpe", false, FPR.ReadD(Fd), _FPR.ReadD(Fm));

                    break;

                case 0x0a:
                    //fcmpzs,fcmpzd:Compare Fd to 0,no exceptions on Nan
                    if (singleType)
                        compareFPS("fcmpzs", true, _FPR.ReadS(Fd), (float)0.0);
                    else
                        compareFPD("fcmpzs", true, _FPR.ReadD(Fd), (double)0.0);
                    break;

                case 0x0b:
                    //fcmpezs,fcmpezd:Compare Fd to 0,exceptions on Nan
                    if (singleType)
                        compareFPS("fcmpez", false, _FPR.ReadS(Fd), (float)0.0);
                    else
                        compareFPD("fcmpez", false, _FPR.ReadD(Fd), (double)0.0);
                    break;

                case 0x0f:
                    //fcvtds,fcvtsd
                    if (singleType)
                    {
                        //fcvtds:single to double
                        Fd = (opCode >> 12) & 0x0f;

                        if (_FPR.isNaN(Fm, true))
                            throw new InvalidOperationFloatingPointException("fcvtd", true);
                        _FPR.WriteD(Fd, (double)_FPR.ReadS(Fm));
                    }
                    else
                    {
                        //fcvtsd:double to single
                        Fd = (opCode >> 11);
                        Fd &= 0x1e;
                        Fd |= ((opCode >> 22) & 0x01);

                        double dvalue = _FPR.ReadD(Fm);
                        if (double.IsNaN(dvalue))
                            throw new InvalidOperationFloatingPointException("fcvts", false);

                        if(dvalue > float.MaxValue)
                            throw new OverflowFloatingPointException("fcvts", false);

                        if (dvalue < float.MinValue)
                            throw new UnderflowFloatingPointException("fcvts", false);

                        _FPR.WriteS(Fd, (float)dvalue);
                    }
                    break;
                case 0x10:
                    {
                        //fuitos,fuitod
                        //Fd = Fm
                        if (_FPR.isNaN(Fm, singleType))
                            throw new InvalidOperationFloatingPointException("fuito", singleType);

                        uint udata = _FPR.ReadRaw(Fm);
                        if (singleType)
                            _FPR.WriteS(Fd,(float)udata);
                        else
                            _FPR.WriteD(Fd,(double)udata);
                    }
                    break;

                case 0x11:
                    //fsitos,fsitod
                    //Fd = Fm
                    {
                        Fm = (opCode << 1);
                        Fm &= 0x1e;
                        Fm |= ((opCode >> 5) & 0x01);
                        
                        int idata = (int)_FPR.ReadRaw(Fm);
                        if (singleType)
                            _FPR.WriteS(Fd, (float)idata);
                        else
                            _FPR.WriteD(Fd, (double)idata);
                    }
                    break;

                case 0x18:
                case 0x19:
                    //ftouis,ftouid
                    //Fd = Fm
                    if (_FPR.isNaN(Fm, singleType))
                        throw new InvalidOperationFloatingPointException("ftoui", singleType);

                    if (singleType)
                        _FPR.WriteRaw(Fd, (uint)_FPR.ReadS(Fm));
                    else
                        _FPR.WriteRaw(Fd, (uint)_FPR.ReadD(Fm));
                    break;

                case 0x1a:
                case 0x1b:
                    {
                        //ftosis,ftosid
                        //Fd = Fm
                        if (_FPR.isNaN(Fm, singleType))
                            throw new InvalidOperationFloatingPointException("ftosi", singleType);

                        Fd = (opCode >> 11);
                        Fd &= 0x1e;
                        Fd |= ((opCode >> 22) & 0x01);

                        if (singleType)
                            _FPR.WriteRaw(Fd, (uint)(int)_FPR.ReadS(Fm));
                        else
                        {
                            int num = (int)_FPR.ReadD(Fm);
                            _FPR.WriteRaw(Fd, (uint)num);
                        }
                    }
                    break;

                default:
                    return 0;

            }//switch
            return 1;
        }//extension_instructions

        /// <summary>
        /// Compare two Single fp numbers.
        /// </summary>
        /// <param name="opcode">opcode of instruction</param>
        /// <param name="quiet">ignore NaN error</param>
        /// <param name="Fd">Fd register</param>
        /// <param name="Fm">Fm register</param>
        private void compareFPS(string opcode,bool quiet,float Fd,float Fm)
        {
            if (float.IsNaN(Fd) || float.IsNaN(Fm))
            {
                if (quiet)
                {
                    _FPSCR.setUnordered();
                    return;
                }
                else
                {
                    throw new InvalidOperationFloatingPointException(opcode, true);
                }
            }//if

            int result = Fd.CompareTo(Fm);
            if (result == 0)
                _FPSCR.setEqual();
            else if (result < 0)
                _FPSCR.setLessThan();
            else
                _FPSCR.setGreaterThan();

        }//compareFPS

        /// <summary>
        /// Compare two Double fp numbers.
        /// </summary>
        /// <param name="opcode">opcode of instruction</param>
        /// <param name="quiet">ignore NaN error</param>
        /// <param name="Fd">Fd register</param>
        /// <param name="Fm">Fm register</param>
        private void compareFPD(string opcode, bool quiet, double Fd, double Fm)
        {
            if (double.IsNaN(Fd) || double.IsNaN(Fm))
            {
                if (quiet)
                {
                    _FPSCR.setUnordered();
                    return;
                }
                else
                {
                    throw new InvalidOperationFloatingPointException(opcode, false);
                }
            }

            int result = Fd.CompareTo(Fm);
            if (result == 0)
                _FPSCR.setEqual();
            else if (result < 0)
                _FPSCR.setLessThan();
            else
                _FPSCR.setGreaterThan();

        }//compareFPD
    }//class FloatingPointProcessor
}
