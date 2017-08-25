#region Copyright Notice
//Copyright (c) R. Nigel Horspool,  August 2005 - April 2006
#endregion

using System;

namespace ARMSim.Simulator
{
	/// <summary>
	/// Manages a block of memory. Also includes a heap pointer that can be
	/// allocated by a caller via the malloc function
	/// This class shares the same interface as the cache class.
	/// </summary>
    public class MemoryBlock : ARMPluginInterfaces.IMemoryBlock
	{
        //the fill pattern to use when initializing memory
        private uint _fillPattern = 0;

        //the stack growth direction
        private bool _stackGrowsDown = true;

        // text area protected from writes during program execution
        public bool ProtectTextArea { get; set; }

		//the actual memory is an array of 32bit words
		private uint []		        _memory;

		//address of the first byte of memory
		private readonly uint		_memory_start;
        private readonly uint       _memory_end;
        private uint _data_start;
        private uint _data_end;
        private uint _stack_start;
        private uint _stack_end;
        private uint _heapStart;
        private uint _heapEnd;

		//value of the current heap pointer (a byte address)
		private uint		        _heapPointer;

        //delegate to call when memory is written too.
        private MemoryChangedDelegate _memoryChangedHandler;

        /// <summary>
        /// MemoryBlock ctor
        /// construct a memory block given a start address and size in words
        /// sizeWords must be multiple of 1k(bottom 10 bits must be 0)
        /// </summary>
        /// <param name="address">start address of block</param>
        /// <param name="sizeWords">size of block in words</param>
        /// <param name="fillPattern">fill pattern to use</param>
        public MemoryBlock(uint address, uint sizeWords, ARMSim.Preferences.SimulatorPreferences prefs)
		{
            System.Diagnostics.Debug.Assert(Utils.isValidWordAlignedAddress(address));

            //save the fill pattern and the stack growth direction
            _fillPattern = prefs.FillPattern;
            _stackGrowsDown = prefs.StackGrowsDown;
            ProtectTextArea = prefs.ProtectTextArea;

            //allocate the word array
            _memory = new uint[sizeWords];

            //set the start address
            _memory_start = Utils.valid_address(address);
            _data_start = _memory_start;  // a temporary value
            _stack_end = _memory_start; // a temporary value
            _memory_end = (uint)(_memory_start + (sizeWords * sizeof(int)));
            _data_end = _memory_end; // a temporary value
            _stack_start = _memory_end; // a temporary value

            //clear the heap pointer and initialize memory
            this.Reset(_fillPattern,_stackGrowsDown);
		}

        /// <summary>
        /// Set/get the byte address of the start of the data area (if applicable to this region)
        /// </summary>
        public uint DataStart {
            get { return _data_start; }
            set { _data_start = (value + 3) & 0xFFFFFFFC; }
        }

        /// <summary>
        /// Set/get the byte address of the end of the static data area (if applicable to this region)
        /// </summary>
        public uint DataEnd
        {
            get { return _data_end; }
            set { _data_end = (value+3) &0xFFFFFFFC; }
        }

        /// <summary>
        /// Set/get the byte address of the start of the stack (if applicable to this region)
        /// </summary>
        public uint StackStart {
            get { return _stack_start; }
            set {
                _stack_start = (value + 3) & 0xFFFFFFFC;
                HeapClear();
            }
        }

        public uint StackEnd
        {
            get { return _stack_end; }
            set { _stack_end = (value + 3) & 0xFFFFFFFC; }
        }

        public uint HeapStart
        {
            get { return _heapStart; }
            set { _heapStart = (value + 3) & 0xFFFFFFFC; }
        }

        public uint HeapEnd
        {
            get { return _heapEnd; }
            set { _heapEnd = (value + 3) & 0xFFFFFFFC; }
        }

        /// <summary>
        /// Returns the start address
        /// </summary>
		public uint Start{ get{ return _memory_start; } }

        /// <summary>
        /// Returns the size of allocated memory in bytes
        /// </summary>
        public uint Size { get { return (uint)(_memory.Length * sizeof(int)); } }

        ///// <summary>
        ///// Returns the address of the byte past the last valid byte
        ///// </summary>
        //public uint End { get { return (uint)(_memory_start + (_memory.Length * sizeof(int))); } }

        /// <summary>
        /// Get the word at the specified address. Bottom 2 bits are ignored.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>word at address</returns>
		private uint getmem32(uint address)
		{
            return _memory[(address - _memory_start) >> 2];
		}//getmem32

        /// <summary>
        /// Get the half-word at the specified address. Bottom 1 bit is ignored.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>half word at address</returns>
		private uint getmem16(uint address)
		{
            uint data = getmem32(address);
			return (Utils.llsb(address) ? data>>16 : data&0x0000ffff);
		}//getmem16

        /// <summary>
        /// Get the byte at the specified address.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <returns>byte read</returns>
		private uint getmem8(uint address)
		{
			uint data=getmem16(address);
			return (Utils.lsb(address) ? data>>8 : data&0x00ff);
		}//getmem8

        /// <summary>
        /// Get a memory value at the specified address
        /// Checks for a valid address within the memory block limits.
        /// </summary>
        /// <param name="address">address to read</param>
        /// <param name="ms">size of memory to read</param>
        /// <returns>value read at address</returns>
        public uint GetMemory(uint address, ARMPluginInterfaces.MemorySize ms)
		{
            //if its out of range, abort
            if (!InRange(address, ms))
                throw new ARMPluginInterfaces.MemoryAccessException(address, "Out of range");

            //otherwise call the specific memget based on type
			switch(ms)
			{
                case ARMPluginInterfaces.MemorySize.Byte: return getmem8(address);
                case ARMPluginInterfaces.MemorySize.HalfWord: return getmem16(address);
                case ARMPluginInterfaces.MemorySize.Word: return getmem32(address);
				default:
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Bad memory size");

			}//switvh
        }//GetMemory

        /// <summary>
        /// Set the word at the specified address. Bottom 2 bits are ignored.
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="data">word to write</param>
		private void setmem32(uint address,uint data)
		{
			_memory[(address-_memory_start)>>2]=data;
		}//setmem32

        /// <summary>
        /// Set the half-word at the specified address. Bottom 1 bit is ignored.
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="data">half word to write</param>
        private void setmem16(uint address, uint data)
		{
			uint mem_data = getmem32(address);
			uint wdata = (data & 0x0000ffff);

			if (Utils.llsb(address))
			{
				mem_data=(mem_data&0x0000ffff) | (uint)wdata<<16;
			}
			else
			{
				mem_data=(mem_data&0xffff0000) | wdata;
			}
			setmem32(address,mem_data);
		}//setmem16

        /// <summary>
        /// Set the byte at the specified address.
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="data">byte to write</param>
        private void setmem8(uint address, uint data)
		{
			uint mem_data=getmem16(address);
			uint wdata = (data & 0x000000ff);

			if (Utils.lsb(address))
			{
				mem_data=(uint)((mem_data&0x00ff) | (uint)wdata<<8);
			}
			else
			{
				mem_data=(uint)((mem_data&0xff00)|(uint)wdata);
			}
			setmem16(address,mem_data);
		}//setmem8

        /// <summary>
        /// Set a memory value at the specified address
        /// Checks for a valid address within the memory block limits.
        /// If the memory changed handler is set, call it with the write info.
        /// </summary>
        /// <param name="address">address to write</param>
        /// <param name="ms">size of memory to write</param>
        /// <param name="data">value to write</param>
        public void SetMemory(uint address, ARMPluginInterfaces.MemorySize ms, uint data)
		{
            if (ProtectTextArea) {
                if (!InDataRange(address,ms))
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Out of range");
            } else {
                if (!InRange(address, ms))
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Out of range");
            }

            //if changed handler set, call it
            if (_memoryChangedHandler != null)
                _memoryChangedHandler(address, ms, GetMemory(address, ms), data);

            //call the write function based on the data type
			switch(ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte:
					setmem8(address,data);
					break;
                case ARMPluginInterfaces.MemorySize.HalfWord:
					setmem16(address,data);
					break;
                case ARMPluginInterfaces.MemorySize.Word:
					setmem32(address,data);
					break;
				default:
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Bad memory size");
			}//switch
        }//SetMemory

        /// <summary>
        /// Safely checks if the specified address and size are within the bounds of memory.
        /// </summary>
        /// <param name="address">address to check</param>
        /// <param name="ms">size of memory operation to check</param>
        /// <returns>true if in range</returns>
        public bool InRange(uint address, ARMPluginInterfaces.MemorySize ms)
        {
			if(address < _memory_start) return false;
			switch(ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte:
                    return (address < _memory_end);
                case ARMPluginInterfaces.MemorySize.HalfWord:
                    return (address < (_memory_end - 1));
                case ARMPluginInterfaces.MemorySize.Word:
                    return (address < (_memory_end - 3));
				default:
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Bad memory size");
			}//switch
		}//InRange

        /// <summary>
        /// Safely checks if the specified address and size are in the address range for data.
        /// </summary>
        /// <param name="address">address to check</param>
        /// <param name="ms">size of memory operation to check</param>
        /// <returns>true if in range</returns>
        public bool InDataRange(uint address, ARMPluginInterfaces.MemorySize ms)
        {
			if(address < _data_start) return false;
			switch(ms)
            {
                case ARMPluginInterfaces.MemorySize.Byte:
                    return (address < _memory_end);
                case ARMPluginInterfaces.MemorySize.HalfWord:
                    return (address < (_memory_end - 1));
                case ARMPluginInterfaces.MemorySize.Word:
                    return (address < (_memory_end - 3));
				default:
                    throw new ARMPluginInterfaces.MemoryAccessException(address, "Bad memory size");
			}//switch
        }

        /// <summary>
        /// Clear the heap of all allocated memory
        /// </summary>
        public void HeapClear()
        {
            _heapPointer = _heapStart;
        }

        /// <summary>
        /// Reset the memory block. Clear heap and set all memory to fill pattern
        /// </summary>
        /// <param name="fillPattern"></param>
        public void Reset(uint fillPattern, bool stackGrowsDown)
		{
            _fillPattern = fillPattern;
            _stackGrowsDown = stackGrowsDown;

            for (int ii = 0; ii < _memory.Length; ii++)
            {
                _memory[ii] = _fillPattern;
            }
            this.HeapClear();
		}

        /// <summary>
        /// Allocate some memory from the heap. The return address is word aligned.
        /// </summary>
        /// <param name="size">number of bytes to allocate</param>
        /// <returns>address of memory block.</returns>
        /// The result is 0 if a stack/heap collision occurs (out of memory)
		public uint malloc(uint size)
		{
            //calculate number of words to allocate. Round up to nearest word
            uint numBytes = (size + 3) & 0xFFFFFFFC;
            uint result;

            if (_stackGrowsDown)  // heap grows up
            {
                result = _heapPointer;
                if ((result + numBytes) > _stack_end) return 0;
                _heapPointer = result + numBytes;
            }
            else  // heap grows down
            {
                result = _heapPointer - numBytes;
                if (result < _stack_end) return 0;
                _heapPointer = result;
            }

            //and return the address of the allocated block
            return result;
		}

        /// <summary>
        /// The memorychangedhandler property
        /// </summary>
        public MemoryChangedDelegate MemoryChangedHandler
		{
			set{_memoryChangedHandler=value;}
        }//MemoryChangedDelegate

    }//class MemoryBlock
}
