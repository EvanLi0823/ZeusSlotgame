using UnityEngine;
using System.Text;

public class StaticString
{
	public static StaticString DefaultStaticStr = new StaticString(50);
	public enum CharAlignment
	{
		Left = 0,
		Right = 1
	}

	#region Fields

	private static System.Reflection.FieldInfo _sb_str_info = typeof(StringBuilder).GetField("_str",
		System.Reflection.BindingFlags.NonPublic |
		System.Reflection.BindingFlags.Instance);
	private StringBuilder _sb;

	#endregion

	#region CONSTRUCTOR

	public StaticString(int size)
	{
		_sb = new StringBuilder(new string(' ', size), 0, size, size);
	}
	#endregion

	#region Properties

	public CharAlignment Alignment
	{
		get;
		set;
	}

	public string Value
	{
		get { return _sb_str_info.GetValue(_sb) as string; }
	}

	#endregion

	#region Methods

	public void Set(long value)
	{
		const int CHAR_0 = (int)'0';
		_sb.Length = 0;
		Alignment = CharAlignment.Left;

		bool isNeg = value < 0;
		value = System.Math.Abs(value);
		int cap = _sb.Capacity;
		int log = (int)System.Math.Floor(System.Math.Log10(value));
		int charCnt = log + ((isNeg) ? 2 : 1);
		int blankCnt = cap - charCnt;

		switch (this.Alignment)
		{
		case CharAlignment.Left:
			{
				if (isNeg) _sb.Append('-');
				int min = System.Math.Max(charCnt - cap, 0);
				for(int i = log; i >= min; i--)
				{
					long pow = (long)System.Math.Pow(10, i);
					long digit = (value / pow) % 10;
					_sb.Append((char)(digit + CHAR_0));
				}

				for (int i = 0; i < blankCnt; i++)
				{
					_sb.Append(' ');
				}
			}
			break;
		case CharAlignment.Right:
			{
				for (int i = 0; i < blankCnt; i++)
				{
					_sb.Append(' ');
				}

				if (isNeg) _sb.Append('-');
				int min = System.Math.Max(charCnt - cap, 0);
				for (int i = log; i >= min; i--)
				{
					long pow = (int)System.Math.Pow(10, i);
					long digit = (value / pow) % 10;
					_sb.Append((char)(digit + CHAR_0));
				}
			}
			break;
		}
	}

	public string SetThousand(long value)
	{
		const int CHAR_0 = (int)'0';
		_sb.Length = 0;

		Alignment = CharAlignment.Left;

		bool isNeg = value < 0;
		value = System.Math.Abs(value);
		int cap = _sb.Capacity;
		int log = (int)System.Math.Floor(System.Math.Log10(value));
		int charCnt = log + ((isNeg) ? 2 : 1);
		int blankCnt = cap - charCnt;

		switch (this.Alignment)
		{
		case CharAlignment.Left:
			{
				if (isNeg) _sb.Append('-');
				int min = System.Math.Max(charCnt - cap, 0);

				for(int i = log; i >= min; i--)
				{
					long pow = (long)System.Math.Pow(10, i);
					long digit = (value / pow) % 10;
					_sb.Append((char)(digit + CHAR_0));
				
					if ( i != min && i!=0 && (i % 3) == 0) {
						_sb.Append (",");
					}
				}

//				for (int i = 0; i < blankCnt; i++)
//				{
//					_sb.Append(' ');
//				}
			}
			break;
		case CharAlignment.Right:
			{
				for (int i = 0; i < blankCnt; i++)
				{
					_sb.Append(' ');
				}

				if (isNeg) _sb.Append('-');
				int min = System.Math.Max(charCnt - cap, 0);
				for (int i = log; i >= min; i--)
				{
					long pow = (int)System.Math.Pow(10, i);
					long digit = (value / pow) % 10;
					_sb.Append((char)(digit + CHAR_0));
				}
			}
			break;
		}
//		return _sb_str_info.GetValue(_sb) as string;
		return _sb.ToString();
	}

	#endregion
}
