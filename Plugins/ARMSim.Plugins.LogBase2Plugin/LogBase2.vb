Imports ARMPluginInterfaces

'This class implements an ARMSim plugin that extends the ARM instruction set
'by adding a LOG (base 2) instruction.
'It has the form:
'LOG2{<cond>} <Rd>, <Rm>
'where:
'<cond> Is the condition under which the instruction is executed
'<Rd>   Destination register
'<Rm>   Source register
'
'The instruction computes the real log base 2 of the input integer and save the result in
'the destination register as a scaled integer. Since the largest result is 32, the number
'range 0-32 is scaled into the 0 - 2^32
'
'This example is implemented in VB.NET, but can be implemented in any .NET language
'
Public Class LogBase2
    Implements IARMPlugin

    Private mIHost As IARMHost

    '<summary>
    'The init function is called once the plugin has been loaded.
    'From this function you can subscribe to the events the
    'simulator supports.
    '</summary>
    '<param name="IHost">Reference to host ARMSim</param>
    Public Sub init(ByVal IHost As IARMHost) Implements IARMPlugin.init

        ' Save a reference to the host interface
        mIHost = IHost

        'request to be notified once all the plugins have been initialized
        AddHandler mIHost.Load, AddressOf Me.onLoad
        'mIHost.Load += New pluginLoadHandlerDelegate(AddressOf onLoad)

    End Sub

    '<summary>
    'The onLoad function is called after all the plugins have been loaded and their
    'init methods called.
    '</summary>
    Public Sub onLoad()

        'spare opcode close to clz
        'mIHost.RequestOpcodeRange(&H16F0E10, &HFFF0FF0, New pluginInstructionExecuteDelegate(AddressOf onExecute))

        mIHost.RequestOpcodeRange(&HEF000F0, &HFF0FEF0, New pluginInstructionExecuteEventHandler(AddressOf onExecuteInteger))

        'request the LN2 opcode be inserted into the assembler parsing tables
        'the callback is for forming the final opcode
        mIHost.RequestMnemonic("LOG2I", &HEF000F0, "RR", New pluginFormOpcodeEventHandler(AddressOf onFormOpcode))
        mIHost.RequestMnemonic("ALOG2I", &HEF001F0, "RR", New pluginFormOpcodeEventHandler(AddressOf onFormOpcode))

    End Sub

    'This function is called when the LOG2 mnemonic has been parsed by the parser.
    'At this point we will form the final 32 bit opcode and return it to the parser.
    'The registers specified in the code is passed in the array operands.
    Public Function onFormOpcode(ByVal baseCode As UInteger, ByVal ParamArray operands() As UInteger) As UInteger

        'sanity check that we have exactly 2 operands. The parser will only call this function if 2 were
        'found, but check here just in case
        If (operands.Length <> 2) Then
            Return baseCode
        End If

        'Get a copy of the original base code value
        Dim result As UInteger = baseCode

        'Or in the destination register bits into the correct location
        result = result Or (operands(0) << 16)

        'Or in the Source register bits
        result = result Or (operands(1))

        'return the final formed opcode
        Return result

    End Function
    Public Function onExecuteDouble(ByVal opcode As UInteger) As UInteger
        'extract destination(Fd) and source(Fm) registers
        Dim Fm As UInteger = (opcode And &HF)
        Dim Fd As UInteger = (opcode >> 16 And &HF)

        If ((opcode And &HF00) = 2) Then

            Dim result As Double = 0

            'fetch raw source integer from registers. If it is a 0, then return a 0
            Dim sourceInteger As UInteger = mIHost.getReg(Fm)

            If (sourceInteger <> 0) Then

                'compute the log2 of the source integer
                result = Math.Log(sourceInteger) / Math.Log(2.0)

            End If
            mIHost.setFPDoubleReg(Fd, result)

        Else


        End If


        onExecuteDouble = 6
    End Function

    'This function is called when the LOG2 opcode is encountered by the simulator engine.
    'The engine has detected that this plugin will handle this opcode and is passing control
    'to us.
    'The return value is the cycle count of this instruction.
    Public Function onExecuteInteger(ByVal opcode As UInteger) As UInteger

        'extract destination(Fd) and source(Fm) registers
        Dim Fm As UInteger = (opcode And &HF)
        Dim Fd As UInteger = (opcode >> 16 And &HF)

        Dim result As UInteger = 0

        'fetch raw source integer from registers. If it is a 0, then return a 0
        Dim sourceInteger As UInteger = mIHost.getReg(Fm)

        If ((opcode And &HF00) = 0) Then
            'Perform Log2
            If (sourceInteger <> 0) Then

                'compute the log2 of the source integer
                Dim d As Double = Math.Log(sourceInteger) / Math.Log(2.0)

                'scale result up to a 32bit integer spanning 0-32
                d = d * Math.Pow(2.0, 27.0)

                'convert result to an uint
                result = d

            End If
        Else

            Dim d As Double = sourceInteger / Math.Pow(2.0, 27.0)
            d = Math.Pow(2.0, d)

            result = d

        End If

        'and write resulting integer back into destination register
        mIHost.setReg(Fd, result)

        'return true indicating opcode was processed
        onExecuteInteger = 3

    End Function

    'This property is the name of the plugin. Every name in a plugin assembly must be unique.
    Public ReadOnly Property PluginName() As String Implements IARMPlugin.PluginName
        Get
            Return "LogBase2Plugin"
        End Get
    End Property

    'This property is a description string of the plugin
    Public ReadOnly Property PluginDescription() As String Implements IARMPlugin.PluginDescription
        Get
            Return "Computes a scaled log base 2 of an integer."
        End Get
    End Property

End Class
