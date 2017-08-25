using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ARMSim.CommandLine;
using ARMSim.Simulator;
using ARMSim.Preferences;


namespace ARMSim.Batch
{
    static class BatchMode
    {
        static public void Run(ARMSimArguments parsedArgs)
        {
            //create a new instance of the simulator engine
            ApplicationJimulator jm = new ApplicationJimulator(parsedArgs);
            jm.ARMPreferences = new ARMPreferences();
            jm.ARMPreferences.PluginPreferences.EnableSWIExtendedInstructions();

            ARMSim.Preferences.PluginPreferences pref = new ARMSim.Preferences.PluginPreferences();
            pref.EnableSWIExtendedInstructions(parsedArgs.SWI);

            jm.InitPlugins(null, null);

            jm.Load(parsedArgs.Files);

            if (jm.ErrorReports.Count > 0)
            {
                Console.Error.WriteLine();
                foreach (string fileName in jm.ErrorReports.ErrorLists.Keys)
                {
                    foreach (ArmAssembly.ErrorReport er in jm.ErrorReports.GetErrorsList(fileName))
                    {
                        Console.Error.WriteLine("File:{0} Line:{1} Column:{2} {3}", fileName, er.Line, er.Col, er.ErrorMsg);
                    }
                }
            }

            if (jm.ValidLoadedProgram)
            {
                int xlimit = parsedArgs.ExecLimit;
                jm.StartSimulation();
                while (!jm.isCurrentOpCodeSWIExit())
                {
                    jm.Execute();
                    if (--xlimit <= 0)
                    {
                        Console.Error.WriteLine("\n\n** Limit of {0} instructions have been executed\n",
                            parsedArgs.ExecLimit);
                        break;
                    }
                }

                printMemory(jm, parsedArgs.PrintMemory);

                jm.HaltSimulation();
            }

            //close all user and standard file handles
            jm.Shutdown();

        }//Run

        static void printMemory(ApplicationJimulator jm, string[] areas)
        {
            foreach (string area in areas)
            {
                printMemoryArea(jm, area);
            }
        }

        static void printMemoryArea(ApplicationJimulator jm, string area)
        {
            string start, end;
            string format, addrs;
            Console.WriteLine("\nContents of memory: {0}", area);
            int slashpos = area.IndexOf('/');
            if (slashpos > 0)
            {
                format = area.Substring(slashpos + 1);
                if (format.Length == 0) format = "X";
                addrs = area.Substring(0, slashpos);
            }
            else
            {
                format = "X";
                addrs = area;
            }
            int colonpos = addrs.IndexOf(':');
            if (colonpos > 0)
            {
                start = addrs.Substring(0, colonpos);
                end = addrs.Substring(colonpos + 1);
            }
            else
            {
                start = addrs;
                end = null;
            }
            int startAddr = parseAddress(jm, start);
            int endAddr;
            if (startAddr < 0)
            {
                Console.Error.WriteLine("\nPrintMemory: cannot decode address: {0}", start);
                return;
            }
            if (string.IsNullOrEmpty(end))
                endAddr = 0;
            else
            {
                endAddr = parseAddress(jm, end);
                if (endAddr < 0)
                {
                    Console.Error.WriteLine("\nPrintMemory: cannot decode address: {0}", end);
                    return;
                }

                if (endAddr == startAddr)
                    endAddr += 4;
            }
            // Valid formats are these:
            //    X X4 X2 X1 D D4 D2 D1 H B F F8 F4
            if (format.Length > 2) format = format.Substring(0,2);
        Next: 
            switch (format)
            {
                // 4-byte integer formats
                case "X": case "X4": case "D": case "D4":                       
                    startAddr = (int)((uint)startAddr & 0xfffffffc);
                    if (endAddr % 4 != 0)
                        endAddr = (int)((uint)(endAddr + 3) & 0xfffffffc);
                    if (endAddr <= startAddr)
                        endAddr = startAddr + 4;
                    printWords(jm, (uint)startAddr, (uint)endAddr, format[0]);
                    break;
                // 2-byte integer formats
                case "X2": case "D2": case "H":
                    startAddr = (int)((uint)startAddr & 0xfffffffe);
                    if (endAddr % 2 != 0)
                        endAddr = (int)((uint)(endAddr + 1) & 0xfffffffe);
                    if (endAddr <= startAddr)
                        endAddr = startAddr + 2;
                    printHWords(jm, (uint)startAddr, (uint)endAddr, format[0]);
                    break;
                // 1-byte integer formats (+ char automatically)
                case "X1": case "D1": case "B":
                    if (endAddr <= startAddr)
                        endAddr = startAddr + 1;
                    printBytes(jm, (uint)startAddr, (uint)endAddr, format[0]);
                    break;
                // 8-byte double format
                case "F8":
                    startAddr = (int)((uint)startAddr & 0xfffffff8);
                    if (endAddr % 8 != 0)
                        endAddr = (int)((uint)(endAddr + 7) & 0xfffffff8);
                    if (endAddr <= startAddr)
                        endAddr = startAddr + 8;
                    printDoubles(jm, (uint)startAddr, (uint)endAddr, format[0]);
                    break;
                // 4-byte float format
                case "F4": case "F":
                    startAddr = (int)((uint)startAddr & 0xfffffffc);
                    if (endAddr % 4 != 0)
                        endAddr = (int)((uint)(endAddr + 3) & 0xfffffffc);
                    if (endAddr <= startAddr)
                        endAddr = startAddr + 4;
                    printFloats(jm, (uint)startAddr, (uint)endAddr, format[0]);
                    break;
                default:
                    Console.WriteLine("* unrecognized format {0}, using X4 format", format);
                    format = "X";
                    goto Next;
            }
        }

        static void printWords(ApplicationJimulator jm, uint a1, uint a2, char format)
        {
            string template; uint stride; bool dec;
            if (format == 'X')
            {
                template = " {0:X8}"; dec = false; stride = 32;
            }
            else
            {
                template = " {0:D11}"; dec = true; stride = 16;
            }
            while (a1 < a2)
            {
                Console.Write("{0:X6}:", a1);
                uint stop = Math.Min(a2, a1 + stride);
                while (a1 < stop)
                {
                    if (jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.Word)) {
                        uint mem = jm.MainMemory.GetMemory(a1, ARMPluginInterfaces.MemorySize.Word);
                        if (dec)
                            Console.Write(((int)mem).ToString("D").PadLeft(12,' '));
                        else
                            Console.Write(template, mem);
                    } else
                        Console.Write(dec? " ???????????" : " ????????");
                    a1 += 4;
                }
                Console.WriteLine();
            }
        }

        static void printHWords(ApplicationJimulator jm, uint a1, uint a2, char format)
        {
            string template; uint stride; bool dec;
            if (format == 'D')
            {
                template = " {0:D}"; dec = true;  stride = 16;
            }
            else
            {
                template = " {0:X4}"; dec = false;  stride = 24;
            }
            while (a1 < a2)
            {
                Console.Write("{0:X6}:", a1);
                uint stop = Math.Min(a2, a1 + stride);
                while (a1 < stop)
                {
                    if (jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.HalfWord)) {
                        uint mem = jm.MainMemory.GetMemory(a1, ARMPluginInterfaces.MemorySize.HalfWord);
                        if (dec)
                            Console.Write(((int)mem).ToString("D").PadLeft(8,' '));
                        else
                            Console.Write(template, mem);
                    } else
                        Console.Write(dec? " ???????" : " ????");
                    a1 += 2;
                }
                Console.WriteLine();
            }
        }

        static void printBytes(ApplicationJimulator jm, uint a1, uint a2, char format)
        {
            string template; uint stride; bool dec;
            if (format == 'D')
            {
                template = " {0:D}"; dec = true;  stride = 12;
            } else
            {
                template = " {0:X2}"; dec = false;  stride = 16;
            }
            StringBuilder sb = new StringBuilder();
            while (a1 < a2)
            {
                if (!jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.Byte))
                    break;
                Console.Write("{0:X6}:", a1);
                uint stop = a1 + stride;
                while (a1 < stop)
                {
                    if (a1 >= a2 || !jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.Byte))
                        Console.Write(dec? "    " : "   ");
                    else
                    {
                        uint mem = jm.MainMemory.GetMemory(a1, ARMPluginInterfaces.MemorySize.Byte);
                        if (dec)
                            Console.Write(mem.ToString("D").PadLeft(4,' '));
                        else
                            Console.Write(template, mem);
                        char c = (char)mem;
                        if (mem < 0x20) c = '.';
                        sb.Append(c);
                    }
                    a1 += 1;
                }
                Console.WriteLine("   \"{0}\"", sb.ToString());
                sb.Clear();
            }
        }

        static void printFloats(ApplicationJimulator jm, uint a1, uint a2, char format)
        {
            // format is currently unused, should be used to specify precision
            string template = " {0:G12}"; uint stride = 16;
            while (a1 < a2)
            {
                Console.Write("{0:X6}:", a1);
                uint stop = Math.Min(a2, a1 + stride);
                while (a1 < stop)
                {
                    if (jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.Word)) {
                        uint mem = jm.MainMemory.GetMemory(a1, ARMPluginInterfaces.MemorySize.Word);
                        float f = BitConverter.ToSingle(BitConverter.GetBytes(mem), 0);
                        Console.Write(template, f);
                    } else
                        Console.Write(" ????????????");
                    a1 += 4;
                }
                Console.WriteLine();
            }
        }

        static void printDoubles(ApplicationJimulator jm, uint a1, uint a2, char format)
        {
            // format is currently unused, should be used to specify precision
            string template = " {0:G16}"; uint stride = 16;
            while (a1 < a2)
            {
                Console.Write("{0:X6}:", a1);
                uint stop = Math.Min(a2, a1 + stride);
                while (a1 < stop)
                {
                    bool fail = false;
                    uint mem1 = 0, mem2 = 0;
                    if (jm.MainMemory.InRange(a1, ARMPluginInterfaces.MemorySize.Word))
                        mem1 = jm.MainMemory.GetMemory(a1, ARMPluginInterfaces.MemorySize.Word);
                    else
                        fail = true;
                    if (jm.MainMemory.InRange(a1+4, ARMPluginInterfaces.MemorySize.Word))
                        mem2 = jm.MainMemory.GetMemory(a1+4, ARMPluginInterfaces.MemorySize.Word);
                    else
                        fail = true;
                    if (fail)
                        Console.Write(" ????????????????");
                    else {
                        Int64 mem = (Int64)((mem1 << 32) | mem2);
                        Console.Write(template, BitConverter.Int64BitsToDouble(mem));
                    }
                    a1 += 8;
                }
                Console.WriteLine();
            }
        }

        static int parseAddress(ApplicationJimulator jm, string addr)
        {
            if (string.IsNullOrEmpty(addr)) return -1;
            uint result = 0;
            if (!char.IsDigit(addr[0]))
            {
                int endPos = addr.IndexOfAny(new char[] { '+', '-' });
                string label;
                if (endPos >= 0)
                {
                    label = addr.Substring(0, endPos);
                    addr = addr.Substring(endPos);
                }
                else
                {
                    label = addr;
                    addr = "";
                }
                if (!jm.CodeLabels.LabelToAddress(label, ref result))
                    return -1;
            }
            if (addr.Length > 0)
            {
                bool add = true;
                if (addr[0] == '+')
                    addr = addr.Substring(1);
                else if (addr[0] == '-')
                {
                    addr = addr.Substring(1);
                    add = false;
                }
                if (addr.Length == 0 || !char.IsDigit(addr[0])) return -1;
                // decimal or hexadecimal number?
                int num = 0;
                if (addr.StartsWith("0x") || addr.StartsWith("0X"))
                {
                    try
                    {
                        num = Int32.Parse(addr.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
                else
                {
                    if (!Int32.TryParse(addr, out num))
                        return -1;
                }
                result = add ? result + (uint)num : result - (uint)num;
            }
            return (int)result;
        }

    }//class BatchExecute
}
