using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CItemInfo{
	public List<string> m_Str_List = new List<string>();
    public string Weight;
}

public class MyItemDatas{
	public List<CItemInfo> mSymbol_List = new List<CItemInfo>();
}
public class CreateWeightedResultDataInfo : MonoBehaviour {
    public TextAsset mWinInfoTxt;
	string curKey;
	public Dictionary<string,MyItemDatas> m_MyItemDatas_List = new Dictionary<string,MyItemDatas>();
	// Use this for initialization
	void Start () {

		string[] strLine_Array = mWinInfoTxt.text.Split('\n');

        for (int i = 0; i < strLine_Array.Length; i++)
        {
            string strline = strLine_Array[i];
            string[] attribute_ArrayList = strline.Split('\t');
			if (attribute_ArrayList.Length == 1)
			{
				MyItemDatas itemdatas = new MyItemDatas();
				curKey = strline;
				m_MyItemDatas_List.Add(curKey, itemdatas);
			}
			else
			{
				MyItemDatas itemdatas = m_MyItemDatas_List[curKey];
				CItemInfo itemInfo = new CItemInfo();
				for (int j = 0; j < attribute_ArrayList.Length - 1; j++)
				{
					itemInfo.m_Str_List.Add(attribute_ArrayList[j]);
				}
				itemInfo.Weight = attribute_ArrayList[attribute_ArrayList.Length - 1];
				itemdatas.mSymbol_List.Add(itemInfo);
			}
        }

        FileStream WriteStream = new FileStream(Application.dataPath + "/CreateReelStrips/WinInfoXml.plist",FileMode.Create);
        StreamWriter write  = new StreamWriter(WriteStream);
        write.WriteLine("\t<dict>");
		write.WriteLine("\t\t<key>WeightedResultData</key>");
        write.WriteLine("\t\t<dict>");
		foreach (var pair in m_MyItemDatas_List)
		{
			write.WriteLine("\t\t<key>" + pair.Key +  "</key>");
            write.WriteLine("\t\t<array>");
			MyItemDatas itemdatas = pair.Value;
			for (int i = 0; i < itemdatas.mSymbol_List.Count; i++)
            {
				write.WriteLine("\t\t\t<dict>");
				write.WriteLine("\t\t\t\t<key>ResultData</key>");

				write.WriteLine("\t\t\t\t<array>");
				for (int j = 0; j < itemdatas.mSymbol_List[i].m_Str_List.Count; j++)
				{
					write.WriteLine("\t\t\t\t\t<string>" + itemdatas.mSymbol_List[i].m_Str_List[j] + "</string>");
				}
				write.WriteLine("\t\t\t\t</array>");

				write.WriteLine("\t\t\t\t<key>Weights</key>");
				write.WriteLine("\t\t\t\t<integer>" + itemdatas.mSymbol_List[i].Weight + "</integer>");
				write.WriteLine("\t\t\t</dict>");
            }
            write.WriteLine("\t\t</array>");
        }

		write.WriteLine("\t\t</dict>");
        write.WriteLine("\t</dict>");
        write.Close ();
        WriteStream.Close ();

        System.Diagnostics.Process.Start(Application.dataPath + "/CreateReelStrips",null);
	}
}
