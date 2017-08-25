// File LiteralSet.cs
//
// Copyright (c) R. Nigel Horspool,  August 2005 - March 2010


using System;
using System.Collections;
using System.Collections.Generic;


namespace ArmAssembler {

// Implements a set of literal constants
// This is currently implemented very inefficiently, requiring
// linear searches.
// Future work : replace this with a hashtable organization

public class LiteralSet : IEnumerable<AsmLiteral> {
    static string findErrorMsg = "failure in Find, value={0}, position={1}";
    List<AsmLiteral> theList;

    // constructor
    public LiteralSet() {
        theList = new List<AsmLiteral>();
    }

    // It *may* be better to check whether the literal is
    // already present in the literal pool (and in the same
    // text subsection and not yet placed by an ltorg) 
    public void Add( AsmLiteral val ) {
        theList.Add(val);
    }

    public int Count { get{ return theList.Count; } }

    public int Find( int v, int position ) {
        foreach( AsmLiteral r in theList) {
            IntLiteral ir = r as IntLiteral;
            if (ir == null || ir.IntValue != v) continue;
            if (ir.Offset > position) return ir.Offset;
        }
        for( int i=0;  i<theList.Count;  i++ ) {
            AsmLiteral r = theList[i];
            if (r is DelayedIntLiteral && r.Offset > position) {
                IntLiteral ir = new IntLiteral(v,0);
                ir.Offset = r.Offset;
                ir.LtorgNumber = r.LtorgNumber;  // bug fix, 9 March 10 (NH)
                theList[i] = ir;
                return ir.Offset;
            }
        }
        throw new AsmException(findErrorMsg, v, position);
    }
    
    public int Find( string v, int position ) {
        foreach( AsmLiteral r in theList) {
            StringLiteral ir = r as StringLiteral;
            if (ir == null || ir.StringValue != v) continue;
            if (ir.Offset > position) return ir.Offset;
        }
        throw new AsmException(findErrorMsg, v, position);
    }

    public int Find( float v, int position ) {
        foreach( AsmLiteral r in theList) {
            FloatLiteral ir = r as FloatLiteral;
            if (ir == null || ir.FloatValue != v) continue;
            if (ir.Offset > position) return ir.Offset;
        }
        throw new AsmException(findErrorMsg, v, position);
    }

    public int Find(double v, int position) {
        foreach (AsmLiteral r in theList) {
            DoubleLiteral ir = r as DoubleLiteral;
            if (ir == null || ir.DoubleValue != v) continue;
            if (ir.Offset > position) return ir.Offset;
        }
        throw new AsmException(findErrorMsg, v, position);
    }

    public void ApplyLtorg( AsmFileInfo fileInfo ) {
        int currSubsection = fileInfo.CurrSubSection;
        List<AsmLiteral> align4 = new List<AsmLiteral>();
        List<AsmLiteral> align2 = new List<AsmLiteral>();
        List<AsmLiteral> align1 = new List<AsmLiteral>();
        // scan to find unplaced constants in current subsection
        for( int i = 0;  i < theList.Count;  i++ ) {
            AsmLiteral lit = theList[i];
            if (lit.Subsection != currSubsection) continue;
            if (lit.Offset >= 0) continue;  // already placed
            // lit.LtorgNumber = fileInfo.LtorgIdentifier;
            AsmLiteral dup;
            switch(lit.Align()) {
            case 4:
                dup = findDuplicate(align4,lit);
                if (dup == null)
                    align4.Add(lit);
                else
                    theList[i] = dup;
                break;
            case 2:
                dup = findDuplicate(align2,lit);
                if (dup == null)
                    align2.Add(lit);
                else
                    theList[i] = dup;
                break;
            case 1:
                dup = findDuplicate(align1,lit);
                if (dup == null)
                    align1.Add(lit);
                else
                    theList[i] = dup;
                break;
            }
        }
        foreach( AsmLiteral lit in align4 )
            lit.Offset = fileInfo.AdvancePosition(lit.Size(), 4);
        foreach( AsmLiteral lit in align2 )
            lit.Offset = fileInfo.AdvancePosition(lit.Size(), 2);
        foreach( AsmLiteral lit in align1 )
            lit.Offset = fileInfo.AdvancePosition(lit.Size(), 1);

    }

    private AsmLiteral findDuplicate( List<AsmLiteral> al, AsmLiteral v ) {
        foreach( AsmLiteral lit in al ) {
            if (lit.Equals(v)) return lit;
        }
        return null;
    }

    public void RemoveDuplicates() {
        foreach( AsmLiteral pi in theList )
            pi.Subsection = 1;
        List<AsmLiteral> newList = new List<AsmLiteral>();
        foreach( AsmLiteral pi in theList ) {
            if (pi.Subsection == 0) continue;  // already seen
            pi.Subsection = 0;
            newList.Add(pi);
        }
        theList = newList;
    }

    public void CopyLiterals( AsmFileInfo fileInfo ) {
        for (int copyIndex = 0; copyIndex < theList.Count; copyIndex++)
        {
            AsmLiteral lit = theList[copyIndex];
            if (lit.LtorgNumber == fileInfo.LtorgIdentifier)
                lit.AddTo(fileInfo);
        }
    }

    public AsmLiteral this[ int pos ] {
        get{ return theList[pos]; }
    }

    // Implement the IEnumerable.GetEnumerator() method:
    IEnumerator IEnumerable.GetEnumerator() {
        return new MyEnumerator(this);
    }

    // Implement the IEnumerable<AsmLiteral>.GetEnumerator() method:
    IEnumerator<AsmLiteral> IEnumerable<AsmLiteral>.GetEnumerator() {
        return new MyEnumerator(this);
    }

    // Declare the enumerator and implement the IEnumerator interface:
    public class MyEnumerator: IEnumerator<AsmLiteral>  {
        int nIndex;
        LiteralSet collection;

        public MyEnumerator(LiteralSet coll) {
            collection = coll;
            nIndex = -1;
        }

        public void Reset() {
            nIndex = -1;
        }

        public void Dispose() {
            collection = null;
            nIndex = -1;
        }

        public bool MoveNext() {
            nIndex++;
            return(nIndex < collection.theList.Count);
        }

        public AsmLiteral Current {
            get{ return( collection.theList[nIndex]); } }
        
        // The Current property for the IEnumerator<AsmLiteral> interface:
        AsmLiteral IEnumerator<AsmLiteral>.Current { get{ return(Current); } }

        // The Current property for the IEnumerator interface:
        object IEnumerator.Current { get{ return(Current); } }

    }
}

} // end of namespace ArmAssembler
