using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MultipleBoardFGProgressData : SceneProgressDataBase
{
    public int BoardNum = 0;

    public List<string> FreespinMultiBoardResults = new List<string>();

    public List<List<List<int>>> GetMultiBoardResultList()
    {
        List<List<List<int>>> result = new List<List<List<int>>>();
        if (FreespinMultiBoardResults.Count > 0)
        {
            for (int i = 0; i < FreespinMultiBoardResults.Count; i++)
            {
                List<List<int>> element = Utils.Utilities.ConvertStringToList<int>(FreespinMultiBoardResults[i]);
                result.Add(element);
            }

            //return Utils.Utilities.ConvertStringToList<int>(BaseGameResultString);
        }
        return result;

    }

    public void AddMultiBoardFreespinResult(List<List<List<int>>>  freeSpinResults)
    {
        for(int i = 0; i < freeSpinResults.Count;i++){
            List<List<int>> freeSpinResult = freeSpinResults[i];
            string result = Utils.Utilities.ConvertListToString<int>(freeSpinResult);
            FreespinMultiBoardResults.Add(result);
        }

    }

}
