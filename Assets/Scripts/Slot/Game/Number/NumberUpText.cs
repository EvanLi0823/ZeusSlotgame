using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Libs;
using DG.Tweening;
using Classic;
/// <summary>
/// 此控件全部是采用相对布局实现，自动实现位数扩充，目前考虑只支持整形数字或者小数部分位数固定的数值
///Number up text.
/// </summary>
public class NumberUpText : MonoBehaviour {

    public int initTextZoneShowCharCount;
    public float initTextXMinAnchor;
    public float initTextXMaxAnchor;
    public float initTextYMinAnchor;
    public float initTextYMaxAnchor;
    public float commaMultiplier = 0.5f;
    public string initNumberText = "$0";
    public List<GameObject> NumberBitPrefabList;
    protected int preNumber= 0;
    [Serializable]
    public class NumberBitPropertyData{
       public float speed;
       public bool loop;
    }
    protected bool finished = true;
    public List<GameObject> numberBitList = new List<GameObject>();
    protected RectTransform mTrans;
    void Start(){
        InitParameterData ();
    }
    protected Coroutine currentCo= null;
    public void SetNumberUpText(string text,bool enableAnimation= true){
        if (TestController.Instance) {
            return;
        }
        int target = ConvertToInt32 (text);
        int pre = ConvertToInt32 (preTargetText);
        winStep = target>pre?(target-pre)/3:3;
        winStep = winStep > 0 ? winStep : 3;
        if (enableAnimation) {
            if (finished) {
                currentCo = StartCoroutine(IncreaseNum(text));
            } else {
                StopCoroutine (currentCo);//目前由于协程终止的不确定性，导致程序在任意时刻都有可能被终止，所以需要刷新一次。
                SetStepText (preTargetText, true);
                currentCo = StartCoroutine(IncreaseNum(text));
            }

        } else {
            if (!finished) {
                StopCoroutine (currentCo);
                SetStepText (preTargetText, true);
            }
            SetStepText (text, false);
        }
        preTargetText = text;
    }
    protected int  winStep = 3;
    protected string preTargetText = "";
    IEnumerator IncreaseNum(string text){
        int to = ConvertToInt32 (text);
        int from = preNumber;
        finished = false;
        while (from<to) {
            yield return new WaitForSeconds (0.5f);
            from += winStep;
            if (from>=to) {
                from = to;
            }
            SetStepText("$"+Utils.Utilities.ThousandSeparatorNumber (from));
        }
        finished = true;
    }

    private void SetStepText(string text,bool enableAni= true){
        AdjustNumberUpText (text.Length - numberBitList.Count,text,enableAni);
    }
    private float anchorWidth = 0f;
    private float bitAnchorWidth = 0f;
    private void InitParameterData(){
        mTrans = transform as RectTransform;
        anchorWidth = initTextXMaxAnchor - initTextXMinAnchor;
        bitAnchorWidth = anchorWidth / initTextZoneShowCharCount;
    }

    #region 第一步相关方法

    private void AdjustNumberUpText(int spawnBitNum,string text,bool enableAnimation = true){
        //LQ 第一步处理布局，并生成所需字符位数
        if (spawnBitNum!= 0) {
            ChangeCharBitNum (spawnBitNum,text);
            ChangeTextLayout (text);
        }
        //LQ 第二步，根据文本内容判定是数字还是非数字，以此来判定当前位是否需要执行动画
        CheckEnableCharBitAnimation (text);
        //LQ 第三步，控制每位的动画播放节奏
        PlayNumberTextAnimation (text);      
        InitNumberBitSequenceData (text,enableAnimation);
    } 

    private void ChangeCharBitNum(int spawnBitNum,string text){
        int firstNumberBit = FindFirstNumberBitIndex (text);
        if (spawnBitNum>0) {
            for (int i = 0; i < spawnBitNum; i++) {
                int index = i + numberBitList.Count;
                if (index >= NumberBitPrefabList.Count) {
                    index = NumberBitPrefabList.Count - 1;
                }
                GameObject go = Instantiate (NumberBitPrefabList[index]) as GameObject;//初始化时，数值为空
                if (firstNumberBit<numberBitList.Count) {
                    numberBitList.Insert (firstNumberBit,go);
                } else {
                    numberBitList.Add(go);
                }

            }
        }
        else if (spawnBitNum<0) {
            for (int i = 0; i < Mathf.Abs(spawnBitNum); i++) {
                GameObject go = numberBitList [firstNumberBit];
                numberBitList.RemoveAt (firstNumberBit);
                Destroy (go);
            }
        }
    }
    private int FindFirstNumberBitIndex(string text){
        char[] array = text.ToCharArray ();
        for (int i = 0; i < array.Length; i++) {
            if (char.IsDigit(array[i])) {
                return i;
            }
        }
        return 0;
    }
    //LQ 居中布局处理
    private void ChangeTextLayout(string text){
        //LQ 1 调整最外层显示区的节点布局 --- 规则为先处理Anchor在对rect数据清空
        float ratio = (float)numberBitList.Count/initTextZoneShowCharCount;
        float diffRatio = ratio > 1 ? ratio - 1 : 1 - ratio;
        float diff = diffRatio * initTextZoneShowCharCount*bitAnchorWidth;
        float currentAnchorMinX = 0f;
        float currentAnchorMaxX = 0f;
        if (ratio>1) {
            currentAnchorMaxX = initTextXMaxAnchor + diff / 2;
            currentAnchorMinX = initTextXMinAnchor - diff / 2;
        } else {
            currentAnchorMaxX = initTextXMaxAnchor - diff / 2;
            currentAnchorMinX = initTextXMinAnchor + diff / 2;
        }
        mTrans.anchorMin = new Vector2 (currentAnchorMinX,initTextYMinAnchor);
        mTrans.anchorMax = new Vector2 (currentAnchorMaxX,initTextYMaxAnchor);
        mTrans.offsetMax = Vector2.zero;
        mTrans.offsetMin = Vector2.zero;
        //LQ 2 调整内层字符节点布局 --- 规则为先处理Anchor在对rect数据清空
        float bitWidth = 1.0f/numberBitList.Count;

        float commaBitWidth = bitWidth * commaMultiplier;
        char[] chacArray = text.ToCharArray ();
        List<int> commaIndexList = new List<int> ();
        for (int i = 0; i < chacArray.Length; i++) {
            if (chacArray[i].CompareTo(',')==0) {
                commaIndexList.Add (i);
            }
        }
        float leftStartOffset = ((bitWidth - commaBitWidth) * commaIndexList.Count) / 2;
        RectTransform tempTrans = null;
        for (int i = 0; i < numberBitList.Count; i++) {
           
            tempTrans = (numberBitList [i].transform as RectTransform);
           
            tempTrans.SetParent(this.transform);
            tempTrans.localScale = Vector3.one;
           
            //tempTrans.anchorMin = new Vector2 (bitWidth*i,mTrans.anchorMin.y);
           // tempTrans.anchorMax = new Vector2 (bitWidth*(i+1),mTrans.anchorMax.y);
            tempTrans.anchorMin = new Vector2 (leftStartOffset,mTrans.anchorMin.y);
            if (commaIndexList.Contains(i)) {
                leftStartOffset += commaBitWidth;
            } 
            else {
                leftStartOffset += bitWidth;
            }
            tempTrans.anchorMax = new Vector2 (leftStartOffset,mTrans.anchorMax.y);


            tempTrans.localPosition = Vector3.zero;
            tempTrans.offsetMin = Vector2.zero;
            tempTrans.offsetMax = Vector2.zero;
        }
    }

    private int ConvertToInt32(string text){
        char[] array = text.ToCharArray ();
        string result = "";
        if (text.Equals("")) {
            return 0;
        }
        for (int i = 0; i < array.Length; i++) {
            if (char.IsDigit(array[i])) {
                result += array [i].ToString ();
            }
        }
        return Convert.ToInt32 (result);
    }
    private void InitNumberBitSequenceData (string text,bool enableAnimation){
        
        int currentNumber = ConvertToInt32(text);
        int currentTempNumberBit = currentNumber;
        int preTempNumberBit = preNumber;
        int numberbitListSize = numberBitList.Count;
        for (int i = 0; currentTempNumberBit > 0; i++) {
            int idx = i / 3 + i;//LQ 考虑分隔符，逢三进一
            NumberBitControl nBC = numberBitList [numberbitListSize-1-idx].GetComponentInChildren<NumberBitControl> ();
            List<int> number_Bit = new List<int> ();
            int currentBit = currentTempNumberBit % 10;
            number_Bit.Add (currentBit);
            if (enableAnimation) {
                nBC.InitNumberBitData (number_Bit,currentTempNumberBit!=preTempNumberBit);
            } else {
                nBC.SetUnNumberText (Convert.ToChar(currentBit.ToString().Substring(0,1)));
            }
            currentTempNumberBit = currentTempNumberBit / 10;
            preTempNumberBit = preTempNumberBit / 10;
            if (i > 0 && i%3==0) {
                NumberBitControl nBC_Comma = numberBitList [numberbitListSize-(4*(i/3))].GetComponentInChildren<NumberBitControl> ();
                nBC_Comma.SetUnNumberText (',');
            }
        }
        if (currentNumber==0) {
            NumberBitControl nBC = numberBitList [1].GetComponentInChildren<NumberBitControl> ();
            if (enableAnimation) {
                List<int> number_Bit = new List<int> ();
                number_Bit.Add (0);
                nBC.InitNumberBitData (number_Bit,true);
            } else {
                nBC.SetUnNumberText ('0');
            }
        }
        NumberBitControl nBC_Prefix = numberBitList [0].GetComponentInChildren<NumberBitControl> ();
        nBC_Prefix.SetUnNumberText (Convert.ToChar(text.Substring(0,1)));
        preNumber = currentNumber;
    }
    #endregion
   
    #region 第二步相关方法
    private void CheckEnableCharBitAnimation(string text){
        char[] temp = text.ToCharArray ();
        Animator animator = null;
        for (int i = 0; i < temp.Length; i++) {
            animator = numberBitList [i].GetComponent<Animator> ();
            if (char.IsDigit(temp[i])) {
                if (animator) {
                    animator.enabled = true;
                }
            } else {
                if (animator) {
                    animator.enabled = false;
                }
            }
        }
    }
    #endregion

    #region 第三步相关方法
  
    //LQ 动画事件驱动
    private void PlayNumberTextAnimation(string text){
        char[] temp = text.ToCharArray ();
        for (int i = temp.Length-1; i >=0 ; i--) {
            Animator animator = numberBitList [i].GetComponentInChildren<Animator> ();
            if (char.IsDigit(temp[i])) {
                if (animator.GetInteger("state")!=1) {
                    animator.SetInteger("state",1);
                }
            } 
            else {
                animator.SetInteger("state",0);
            }
        }
    }
    #endregion
}
