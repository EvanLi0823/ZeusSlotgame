using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class CreateStackData : MonoBehaviour {

	//public Dictionary<string,MyItemDatas> m_MyItemDatas_List = new Dictionary<string,MyItemDatas>();
	// Use this for initialization
	public TextAsset mWinInfoTxt;
	void Start () {
		string[] strLine_Array = mWinInfoTxt.text.Split('\n');

		FileStream WriteStream = new FileStream(Application.dataPath + "/CreateReelStrips/StackData.plist",FileMode.Create);
		StreamWriter write  = new StreamWriter(WriteStream);
		write.WriteLine("<dict>");
		write.WriteLine("\t<key>BonusGameStack</key>");
		write.WriteLine ("\t<array>");

		for (int i = 0; i < strLine_Array.Length; i++)
		{
			write.WriteLine("\t\t<dict>");
			write.WriteLine("\t\t\t<key>Symbol</key>");
			string strline = strLine_Array[i];
			string[] attribute_ArrayList = strline.Split('\t');

			CSymbolInfo symbolInfo = new CSymbolInfo();
			symbolInfo.Name = attribute_ArrayList[0];
			for (int j = 1; j < attribute_ArrayList.Length-1; j = j + 1)
			{
				symbolInfo.Name = symbolInfo.Name+","+attribute_ArrayList[j];
			}
			symbolInfo.Weight = attribute_ArrayList[attribute_ArrayList.Length-1];
			write.WriteLine("\t\t\t<string>"+symbolInfo.Name+"</string>");
			write.WriteLine("\t\t\t<key>Weights</key>");
			write.WriteLine("\t\t\t<integer>"+symbolInfo.Weight+"</integer>");
			write.WriteLine("\t\t</dict>");
		}
		write.WriteLine("\t</array>");
		write.WriteLine("</dict>");

		write.Close ();
		WriteStream.Close ();

		System.Diagnostics.Process.Start(Application.dataPath + "/CreateStackData",null);
	}
}
