using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class LineTable
    {

		public readonly static string TABLE_KEY = "Table";
		public readonly static string LINE_TABLES_KEY = "LineTables";

		public int MaxValue = -1; //最大的值，为计算画payline准备
		public string Name()
		{
			return name;
		}

		public void SetName(string newName)
		{
			name = newName;
		}

        public int TotalPayLineCount()
        {
            if (payLines == null) {
                return 0;
            }
            return payLines.Count;
        }

        public List<PayLine> PayLines()
        {
            if (payLines == null) {
                return new List<PayLine>();
            }
            return new List<PayLine>(payLines);
        }

        public PayLine GetPayLineAtIndex(int index)
        {
			if (payLines == null || index >= payLines.Count || index<0) {
				return null;
            }

            return payLines[index];
        }

        public int GetRowPosition(int payLineIndex, int reelIndex) {
            return payLines[payLineIndex].RowNumberAt(reelIndex);
        }

        public int RowCountInFrame_deprecated()
        {
            int rowCount = 0;
            foreach(PayLine payLine in payLines) {
                foreach (int row in payLine.RowNumbers()) {
                    if (rowCount <= row) {
                        rowCount = row + 1;
                    }
                }
            }
            return rowCount;
        }

        protected List<PayLine> payLines;

		public void AddPayLine(PayLine payline)
		{
			if (payline == null) {
				throw new ArgumentNullException("payline");
			}
			if (payLines == null) {
				payLines = new List<PayLine>();
			}

			payLines.Add (payline);
		}

		public void SetPayLines(List<PayLine> newPayLines)
		{
			if (newPayLines == null) {
				payLines = new List<PayLine>();
			} else {
				payLines = new List<PayLine>(newPayLines);
			}
		}

		void Init (int[,] lines)
		{
			for (int lineIndex = 0; lineIndex <= lines.GetUpperBound(0); lineIndex++) {
				int[] line = new int[lines.GetUpperBound(1) + 1];
				for (int i = 0; i < line.Length; i++) {
					line[i] = lines[lineIndex,i];
				}
				AddPayLine(new PayLine(line));
			}
		}

		private string name =  "LineTable";


		public static LineTable ParseLineTable (string name, Dictionary<string,object> lineTableDict, object context)
		{
			List<object> lines = lineTableDict [TABLE_KEY] as List<object>;
			LineTable lineTable = new LineTable ();
			List<PayLine> payLines = new List<PayLine> ();
			foreach (object lineObj in lines) {
				string line = (string)lineObj;
				string[] tokens = line.Split (',');
				for(int i = 0; i < tokens.Length;i++)
				{
					tokens[i] = tokens[i].Trim();
				}
				int[] rowNumbers = Array.ConvertAll<string, int> (tokens, int.Parse);
				PayLine payLine = new PayLine (rowNumbers);
				payLines.Add (payLine);

				int _smallPayLineValue = payLine.MaxNumber ();
				if (_smallPayLineValue >lineTable.MaxValue) {
					lineTable.MaxValue = _smallPayLineValue;
				}
			}
			lineTable.SetPayLines (payLines);
			lineTable.SetName (name);
			return lineTable;
		}
    }
}