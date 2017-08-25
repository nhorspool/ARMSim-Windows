// File AsmLiteral.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005


using System;

namespace ArmAssembler {

public enum AsmLiteralKind { UNKNOWN, INT, STRING, FLOAT, DOUBLE, DELAYEDINT };

public abstract class AsmLiteral : IComparable {
	protected int offset = -1;	// offset in text segment (-1 == unassigned)
	protected int subsection;	// text subsection number
	protected int ltorgNumber;	// which ltorg does it belong to?

	public AsmLiteral( int subsection ) {
		this.subsection = subsection;
	}

	public abstract AsmLiteralKind ConstantType();

    public int CompareTo(object obj) {
        if(obj is AsmLiteral) {
            AsmLiteral t = (AsmLiteral) obj;
            return offset.CompareTo(t.offset);
        }
        throw new ArgumentException("object is not a AsmLiteral");    
    }

	public int Offset {
		get{ return offset; }
		set{ offset = value; }
	}

	public int Subsection {
		get{ return subsection; }
		set{ subsection = value; }
	}

	public int LtorgNumber {
		get{ return ltorgNumber; }
		set{ ltorgNumber = value; }
	}

	public abstract int Align();

	public abstract int Size();

	//public abstract void AddTo(AsmFileInfo fileInfo);

	public abstract bool Equals(AsmLiteral v);

	public abstract override string ToString();
}

public class IntLiteral : AsmLiteral {
	int ival;

	public IntLiteral( int ival, int subsection ): base(subsection) {
		this.ival = ival;
	}

	public override AsmLiteralKind ConstantType() { return AsmLiteralKind.INT; }

	public int IntValue { get{ return ival; } }

	public override int Align() { return 4; }

	public override int Size() { return 4; }

	public override void AddTo(AsmFileInfo fileInfo) { fileInfo.Add(this); }

	public override bool Equals(AsmLiteral v) {
		if (v.ConstantType() != AsmLiteralKind.INT) return false;
		IntLiteral iv = v as IntLiteral;
		return ival == iv.IntValue; 
	}

	public override string ToString() {
		return String.Format("integer literal = {0} at offset {1}; ltorg# = {2}",
			ival, offset, ltorgNumber);
	}
}

public class FloatLiteral : AsmLiteral {
	float fval;

	public FloatLiteral( float fval, int subsection ): base(subsection) {
		this.fval = fval;
	}

	public override AsmLiteralKind ConstantType() { return AsmLiteralKind.FLOAT; }

	public float FloatValue { get{ return fval; } }

	public override int Align() { return 4; }

	public override int Size() { return 4; }

	//public override void AddTo(AsmFileInfo fileInfo) { fileInfo.Add(this); }
	
	public override bool Equals(AsmLiteral v) {
		if (v.ConstantType() != AsmLiteralKind.FLOAT) return false;
		FloatLiteral fv = v as FloatLiteral;
		return fval == fv.FloatValue; 
	}

	public override string ToString() {
		return String.Format("float literal = {0} at offset {1}; ltorg# = {2}",
			fval, offset, ltorgNumber);
	}
}

public class DoubleLiteral : AsmLiteral {
        double dval;

    public DoubleLiteral(double dval, int subsection)
            : base(subsection)
        {
            this.dval = dval;
        }

    public override AsmLiteralKind ConstantType() { return AsmLiteralKind.DOUBLE; }

    public double DoubleValue { get { return dval; } }

    public override int Align() { return 4; }

    public override int Size() { return 8; }

    //public override void AddTo(AsmFileInfo fileInfo) { fileInfo.Add(this); }

    public override bool Equals(AsmLiteral v) {
        if (v.ConstantType() != AsmLiteralKind.DOUBLE) return false;
        DoubleLiteral dv = v as DoubleLiteral;
        return dval == dv.DoubleValue;
    }

    public override string ToString() {
        return String.Format("double literal = {0} at offset {1}; ltorg# = {2}",
            dval, offset, ltorgNumber);
    }
}

public class StringLiteral : AsmLiteral {
	string sval;
	int size;
	
	public StringLiteral( string sval, int subsection ): base(subsection) {
		this.sval = sval;
		size = sval.Length;
	}

	public override AsmLiteralKind ConstantType() { return AsmLiteralKind.STRING; }

	public string StringValue { get{ return (string)sval; } }

	public override int Align() { return 1; }

	public override int Size() { return size; }

	public override void AddTo(AsmFileInfo fileInfo) { fileInfo.Add(this); }

	public override bool Equals(AsmLiteral v) {
		if (v.ConstantType() != AsmLiteralKind.STRING) return false;
		StringLiteral sv = v as StringLiteral;
		return sval == sv.StringValue; 
	}

	public override string ToString() {
		return String.Format("string literal = \"{0}\" at offset {1}; ltorg# = {2}",
			sval, offset, ltorgNumber);
	}
}

public class DelayedIntLiteral  : AsmLiteral {
    public DelayedIntLiteral(int subsection, int ltorgNumber) : base(subsection) {
        this.ltorgNumber = ltorgNumber;
    }

	public override AsmLiteralKind ConstantType() { return AsmLiteralKind.DELAYEDINT; }

	public override int Align() { return 4; }

	public override int Size() { return 4; }

	//public override void AddTo(AsmFileInfo fileInfo) { fileInfo.Add(this); }

	public override bool Equals(AsmLiteral v) {
		return false; 
	}

	public override string ToString() {
		return "delayed int constant";
	}
}

} // end of namespace ArmAssembler