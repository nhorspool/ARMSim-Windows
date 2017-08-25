using System;
using System.Collections.Generic;
using System.Text;

using CSML;
using ARMSim.Plugins.UIControls;

namespace ARMSim.Plugins.MatrixOperationsPlugin
{
    public class FixedMatrixStack
    {
        private readonly int mMaxSize;
        private List<Matrix> mStack = new List<Matrix>();
        public FixedMatrixStack(int size)
        {
            mMaxSize = size; 
        }

        public void Push(Matrix m)
        {
            if(mStack.Count >= mMaxSize)
                mStack.RemoveAt(mMaxSize-1);

            mStack.Insert(0, m);
        }

        public Matrix Pop()
        {
            Matrix m = null;
            if(mStack.Count > 0)
            {
                m = mStack[0];
                mStack.RemoveAt(0);
            }
            return m;
        }//Pop

        public void Clear()
        {
            mStack.Clear();
        }

        public MatrixStackDisplay.MatrixDef[] ToArray()
        {
            MatrixStackDisplay.MatrixDef[] matrixes = new MatrixStackDisplay.MatrixDef[mStack.Count];
            for (int ii = 0; ii < mStack.Count; ii++)
            {
                matrixes[ii] = new MatrixStackDisplay.MatrixDef();
                matrixes[ii].rows = mStack[ii].RowCount;
                matrixes[ii].cols = mStack[ii].ColumnCount;
                matrixes[ii].elements = new double[matrixes[ii].rows * matrixes[ii].cols];

                int index = 0;
                for (int row = 1; row <= matrixes[ii].rows; row++)
                {
                    for (int col = 1; col <= matrixes[ii].cols; col++)
                    {
                        matrixes[ii].elements[index++] = mStack[ii][row, col].Re;
                    }
                }
            }
            return matrixes;
        }

    }//class FixedMatrixStack
}
