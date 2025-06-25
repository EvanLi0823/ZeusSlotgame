using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AutoPilotTempData : MonoBehaviour
{
	public TextAsset mFileTxt_1;
	public TextAsset mFileTxt_2;

	// Use this for initialization
	void Start ()
	{
		string[] strLine1_Array = mFileTxt_1.text.Split('\n');
		string[] strLine2_Array = mFileTxt_2.text.Split('\n');

		List<string> TgtList = new List<string>();

		for (int i = 0; i < strLine1_Array.Length; i++)
		{
			string str1 = strLine1_Array[i];
			if (string.IsNullOrEmpty(str1)) continue;
			if (TgtList.Contains(str1)) continue;

			for (int j = 0; j < strLine2_Array.Length; j++)
			{
				if(!str1.Equals(strLine2_Array[j])) continue;
				TgtList.Add(str1);
			}
		}
		
		FileStream WriteStream = new FileStream(Application.dataPath + "/OutputFile/" + mFileTxt_1.name +"_" + mFileTxt_2.name + ".txt",FileMode.Create);
		StreamWriter write  = new StreamWriter(WriteStream);

		for (int i = 0; i < TgtList.Count; i++)
		{
			write.WriteLine(TgtList[i]);
		}


		write.Close ();
		WriteStream.Close ();



		WriteStream = new FileStream(Application.dataPath + "/OutputFile/" + mFileTxt_2.name + "_Only.txt",FileMode.Create);
		write  = new StreamWriter(WriteStream);

		for (int j = 0; j < strLine2_Array.Length; j++)
		{
			string str = strLine2_Array[j];
			if (string.IsNullOrEmpty(str)) continue;
			if (TgtList.Contains(str)) continue;

			write.WriteLine(str);
		}


		write.Close ();
		WriteStream.Close ();

		System.Diagnostics.Process.Start(Application.dataPath + "/OutputFile",null);
	}
}
