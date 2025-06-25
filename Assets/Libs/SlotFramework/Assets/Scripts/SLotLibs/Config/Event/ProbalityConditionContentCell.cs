using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
//因为概率这些东西，在大量重复实验下，较精准，实际个例样本较小相差很大，所以我们在概率范围值较低的时候，使用shuffle,在低概率下，精准
public class ProbalityConditionContentCell : ConditionContentCell {
	public List<int> shuffleList;
	public const int SHUFFLE_LIMIT = 100;
	public bool enableShuffle= false;
	public ProbalityConditionContentCell(object info){
		try {
			string[] temp = info.ToString().Split (',');
			min = Utils.Utilities.CastValueInt(temp[0]);
			max = Utils.Utilities.CastValueInt (temp[1]);
			if (max<=SHUFFLE_LIMIT) {
				enableShuffle = true;
			}
		} catch (System.Exception ex) {
			Utils.Utilities.LogPlistError ("ProbalityConditionContentCell Config Error:"+info.ToString());
		}

	}
	public override bool ConditionIsOK(){
		if (enableShuffle) {
			if (ShuffleResultData()<min) {
				return true;
			}
		} 
		else {
			if (Random.Range(0,max)<min) {
				return true;
			}
		}

		return false;
	}

	private int ShuffleResultData(){
		if (shuffleList==null) {
			shuffleList = new List<int> ();
		}
		if (shuffleList.Count==0) {
			for (int i = 0; i < max; i++) {
				shuffleList.Add (i);
			}
		}
		int idx = Random.Range(0,shuffleList.Count);
		int result = shuffleList [idx];
		shuffleList.RemoveAt(idx);
		return result;
	}
}
