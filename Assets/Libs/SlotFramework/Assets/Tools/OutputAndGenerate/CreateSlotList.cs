using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateSlotList : MonoBehaviour {

		//public Dictionary<string,MyItemDatas> m_MyItemDatas_List = new Dictionary<string,MyItemDatas>();
		// Use this for initialization
		public TextAsset mWinInfoTxt;
		void Start () {
			string[] strLine_Array = mWinInfoTxt.text.Split('\n');
			FileStream WriteStream = new FileStream(Application.dataPath + "/CreateReelStrips/SlotListXml.plist",FileMode.Create);
			StreamWriter write  = new StreamWriter(WriteStream);
			write.WriteLine("\t<dict>");
			write.WriteLine("\t\t<key>SlotList</key>");
			write.WriteLine("\t\t<array>");
			for (int i = 0; i < strLine_Array.Length; i++)
			{
				write.WriteLine("\t\t\t\t<string>"+strLine_Array[i]+"</string>");
			}

			write.WriteLine("\t\t</array>");
			write.WriteLine("\t</dict>");
			write.Close ();
			WriteStream.Close ();

			System.Diagnostics.Process.Start(Application.dataPath + "/CreateReelStrips",null);
		}
}
