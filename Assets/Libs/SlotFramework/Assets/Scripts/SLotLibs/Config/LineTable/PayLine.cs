using System;

namespace Core
{
    public class PayLine
    {
       

        public PayLine(int[] rowNumbers)
        {
            if (rowNumbers == null) {
                throw new System.ArgumentNullException("rowNumbers");
            }
            this.rowNumbers = rowNumbers;
        }

        public int[] RowNumbers()
        {
            if (rowNumbers == null) {
                return new int[0];
            }
            return rowNumbers;
        }

        public int RowNumberAt(int reelIndex) 
        {
            return rowNumbers[reelIndex];
        }

        public int GetSize(){
            if(rowNumbers==null){
                return 0;
            }
            return rowNumbers.Length;
        }

        public override string ToString ()
        {
            return string.Join(",", Array.ConvertAll<int, string>(rowNumbers, Convert.ToString));
        }

        public Boolean startWithSameLine(PayLine payLine){
          return  isSameAtIndex(payLine,0);
        }

        public Boolean isSameAtIndex(PayLine payLine,int index){
            int start = index;
            int end = index+1;
            if(index<0 || end>=payLine.GetSize()||end>=payLine.GetSize() ) {
                return false;
            }
            return (RowNumberAt(start)== payLine.RowNumberAt(start)) && (RowNumberAt(end)== payLine.RowNumberAt(end));
        }

		/// <summary>
		/// 
		/// Max of the number in the array. for  payline picture
		/// </summary>
		/// <returns>The number.</returns>
		public int MaxNumber()
		{
			int result = -1;
			for (int i = 0; i < GetSize (); i++) {
				if (rowNumbers [i] > result) {
					result = rowNumbers[i];
				}
			}
			return result;
		}

		public int GetLastNumber()
		{
			if (rowNumbers.Length <= 0) {
				Log.Error ("payline number length wrong");
				return 0;
			} else {
				return rowNumbers [rowNumbers.Length - 1];
			}
		}

        protected int[] rowNumbers;
    }
}