using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RowItemInfo
{
    public string RowItem_Name;
    public string strSymbolCount;
    public string strCoin;
}

public class CSymbolItemInfo
{
    public string strSymbolName;
    public List<RowItemInfo> mRowInfo_List;

    public CSymbolItemInfo(){
        mRowInfo_List = new List<RowItemInfo>();
    }
}

public class CreatePayTable : MonoBehaviour 
{   
    public TextAsset mPayTableTxt;
    public int mPayTable_MaxCount;
    public int mpayTable_MinCount;

    MyReelStrip[] mReelStrips;

    List<CSymbolItemInfo> mSymbolItemInfo_List = new List<CSymbolItemInfo>();
	// Use this for initialization
	void Start () {
        
        //if (mPayTableTxt != null)
        {
            string[] strLine_Array = mPayTableTxt.text.Split('\n');
           
            int nCount = mPayTable_MaxCount - mpayTable_MinCount + 1;
            for (int i = 0; i < strLine_Array.Length; i = i + nCount)
            {
                CSymbolItemInfo symbolItemInfo = new CSymbolItemInfo();
                for (int j = 0; j < nCount; j++)
                {
                    string strline = strLine_Array[i + j];

                    RowItemInfo rowItemInfo = new RowItemInfo();
                    string[] attribute_ArrayList = strline.Split('\t');
                    string strName_Count = attribute_ArrayList[0];
                    rowItemInfo.strCoin = attribute_ArrayList[1];

                    string[] attribute_ArrayList1 = strName_Count.Split('_');

                    if (j == 0)
                    {
                        symbolItemInfo.strSymbolName = attribute_ArrayList1[0];
                    }
                    rowItemInfo.strSymbolCount = attribute_ArrayList1[1];

                    rowItemInfo.RowItem_Name = strName_Count;
                    symbolItemInfo.mRowInfo_List.Add(rowItemInfo);
                }

                mSymbolItemInfo_List.Add(symbolItemInfo);
            }

            FileStream WriteStream = new FileStream(Application.dataPath + "/CreateReelStrips/PayTableXml.plist", FileMode.Create);
            StreamWriter write = new StreamWriter(WriteStream);
            write.WriteLine("\t<dict>");
            write.WriteLine("\t\t<key>PayTable</key>");

            //for (int i = 0; i < mSymbolItemInfo_List.Count; i++)
            //{
            write.WriteLine("\t\t<array>");
            for (int i = 0; i < mSymbolItemInfo_List.Count; i++)
            {
                write.WriteLine("\t\t\t<dict>");

                write.WriteLine("\t\t\t\t<key>IsContinuity</key>");
                write.WriteLine("\t\t\t\t<true/>");


                write.WriteLine("\t\t\t\t<key>AwardMap</key>");
                write.WriteLine("\t\t\t\t<dict>");

                for (int j = 0; j < mSymbolItemInfo_List[i].mRowInfo_List.Count; j++)
                {
                    write.WriteLine("\t\t\t\t\t<key>" + mSymbolItemInfo_List[i].mRowInfo_List[j].strSymbolCount + "</key>");
                    write.WriteLine("\t\t\t\t\t<integer>" + mSymbolItemInfo_List[i].mRowInfo_List[j].strCoin + "</integer>");
                }
                write.WriteLine("\t\t\t\t</dict>");




                write.WriteLine("\t\t\t\t<key>AwardName</key>");
                write.WriteLine("\t\t\t\t<dict>");

                for (int j = 0; j < mSymbolItemInfo_List[i].mRowInfo_List.Count; j++)
                {
                    write.WriteLine("\t\t\t\t\t<key>" + mSymbolItemInfo_List[i].mRowInfo_List[j].strSymbolCount + "</key>");
                    write.WriteLine("\t\t\t\t\t<string>" + mSymbolItemInfo_List[i].mRowInfo_List[j].RowItem_Name + "</string>");
                }
                write.WriteLine("\t\t\t\t</dict>");



                write.WriteLine("\t\t\t\t<key>SymbolList</key>");
                write.WriteLine("\t\t\t\t<array>");

                write.WriteLine("\t\t\t\t\t<string>" + mSymbolItemInfo_List[i].strSymbolName + "</string>");
                write.WriteLine("\t\t\t\t</array>");

                write.WriteLine("\t\t\t</dict>");
            }
            write.WriteLine("\t\t</array>");
            //}
            write.WriteLine("\t</dict>");
            write.Close();
            WriteStream.Close();
        }


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
