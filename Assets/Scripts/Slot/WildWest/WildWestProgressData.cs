using System.Collections.Generic;

public class WildWestProgressData : SceneProgressDataBase
{
    #region 用户平均下注
    public string averageBetTable = string.Empty;
    public long triggerBetting;
    public Dictionary<double, double> userUseBet
    {
		get
        { 
			if (!string.IsNullOrEmpty (averageBetTable)) 
            {
				return StringToDictionary<double, double>(averageBetTable);
			}
			return new Dictionary<double, double>();
		}
	}

    public Dictionary<T1, T2> StringToDictionary<T1, T2>(string str)
    {
        Dictionary<T1, T2> table = new  Dictionary<T1, T2> ();
        string[] itemStr = str.Split (';');
        foreach (var item in itemStr)
        {
            string[] value = item.Split (',');
            if (typeof(T1)==typeof(double) && typeof(T2)==typeof(double))
            {
                table.Add((T1)(object)double.Parse(value[0]), (T2)(object)double.Parse(value[1]));
            }			
        }
        return table;
    }

    public string DictionaryToString<T1, T2>(Dictionary<T1, T2> tabel)
    {
        if(tabel.Count == 0) return string.Empty;
        string str = "";

        List<T1> tableKeys = new List<T1>(tabel.Keys);

        for (int i = 0; i < tableKeys.Count; i++)
        {
            string itemStr = tableKeys[i].ToString() + "," + tabel[tableKeys[i]].ToString();
            if(i == tableKeys.Count - 1)
            {
                str += itemStr;
            }else
            {
                str += itemStr + ";";
            }
        }

        return str;
    }
    #endregion


    #region FreeGame保存数据
    public string betlevel = string.Empty;
    public string freeType = string.Empty;
    public string featuremul = string.Empty;
    public string initLevel = string.Empty;
    public string freeLevel = string.Empty;
    public string featureItem = string.Empty;//spin出现B01(金牛)个数
    public string infreePendant = string.Empty;
    public string freespinPendant = string.Empty;
    public string equalTable = string.Empty;
    public string freespinTable = string.Empty;

    public string Ispick = string.Empty;
    #endregion
}
