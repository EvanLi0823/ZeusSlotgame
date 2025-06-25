using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CSymbolInfo{
    public string Name;
    public string Weight;
}

public class MyReelStrip{
    public List<CSymbolInfo> mSymbol_List;

    public MyReelStrip(){
        mSymbol_List = new List<CSymbolInfo>();
    }
}
public class CreateReelStrips : MonoBehaviour {
    public TextAsset mReelStripsTxt;
    MyReelStrip[] mReelStrips;
    public int nReelCount;

	// Use this for initialization
	void Start () {

        string[] strLine_Array = mReelStripsTxt.text.Split('\n');
        mReelStrips = new MyReelStrip[nReelCount];
        for (int i = 0; i < nReelCount; i++)
        {
            mReelStrips[i] = new MyReelStrip();
        }

        for (int i = 0; i < strLine_Array.Length; i++)
        {
            string strline = strLine_Array[i];
            string[] attribute_ArrayList = strline.Split('\t');


            for (int j = 0; j < attribute_ArrayList.Length; j = j + 2)
            {
                if (attribute_ArrayList[j].Length > 0)
                {
                    CSymbolInfo symbolInfo = new CSymbolInfo();
                    symbolInfo.Name = attribute_ArrayList[j];
					//symbolInfo.Name = symbolInfo.Name.ToUpper();

                    symbolInfo.Weight = attribute_ArrayList[j + 1];
                
                    mReelStrips[j / 2].mSymbol_List.Add(symbolInfo);
                }
                else
                {
                    print("AAAAAA");
                }
            }
        }

        FileStream WriteStream = new FileStream(Application.dataPath + "/CreateReelStrips/ReelStrips.plist",FileMode.Create);
        StreamWriter write  = new StreamWriter(WriteStream);
        write.WriteLine("\t<dict>");
        write.WriteLine("\t\t<key>ClassicReelStripData</key>");

        write.WriteLine("\t\t<array>");
        for (int i = 0; i < mReelStrips.Length; i++)
        {
            write.WriteLine("\t\t<array>");
            for (int j = 0; j < mReelStrips[i].mSymbol_List.Count; j++)
            {
                write.WriteLine("\t\t\t<dict>");

                write.WriteLine("\t\t\t\t<key>Symbol</key>");
                write.WriteLine("\t\t\t\t<string>" + mReelStrips[i].mSymbol_List[j].Name + "</string>");
                write.WriteLine("\t\t\t\t<key>Weights</key>");
                write.WriteLine("\t\t\t\t<integer>" + mReelStrips[i].mSymbol_List[j].Weight + "</integer>");

                write.WriteLine("\t\t\t</dict>");
            }
            write.WriteLine("\t\t</array>");
        }

        write.WriteLine("\t\t</array>");
        write.WriteLine("\t</dict>");
        write.Close ();
        WriteStream.Close ();

        System.Diagnostics.Process.Start(Application.dataPath + "/CreateReelStrips",null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        //GUILayout.Label(mReelStripsTxt.text);
    }
}
