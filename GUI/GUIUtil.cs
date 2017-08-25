/* GUIUtil.cs
 * 
 * Miscellaneous helpers for the GUI implementation. 
 * 
 * 
 * B. Bird - 08/20/2014
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARMSim.GUI
{
	/* FirstAvailableIndexDispenser
	 * 
	 * A data structure to manage a sequence of indices where
	 * the lowest available index is always assigned first.
	*/

	public class FirstAvailableIndexDispenser
	{
		SortedSet<int> S;
		int takenCount;
		int maxTaken;
		public FirstAvailableIndexDispenser(int startIndex = 0)
		{
			takenCount = startIndex;
			maxTaken = startIndex - 1;
			S = new SortedSet<int>();

		}
		//Get the next available index.
		public int GetIndex()
		{
			if (S.Count == 0)
			{
				takenCount++;
				return ++maxTaken;
			}
			int result = S.Min;
			S.Remove(S.Min);
			return result;
		}
		//Reserve the given index. If the index is taken, the return value will be false.
		//If the index was successfully reserved, the return value will be true.
		public bool ReserveIndex(int index)
		{
			if (index <= maxTaken && !S.Contains(index))
				return false;
			if (index <= maxTaken)
				S.Remove(index);
			else
			{
				for (int i = maxTaken + 1; i < index; i++)
					S.Add(i);
				maxTaken = index;
			}
			takenCount++;
			return true;
		}
		public void ReturnIndex(int index)
		{
			takenCount--;
			if (index == maxTaken)
				maxTaken--;
			else
				S.Add(index);
		}
	}
}
