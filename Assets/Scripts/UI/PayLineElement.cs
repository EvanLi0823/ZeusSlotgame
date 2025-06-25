using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
public class PayLineElement : MonoBehaviour {

    [HideInInspector]
    public Sprite sprite;
    public Text NumberTxt;
    public Image symbol;
    public Image Line;
    List<Image> list = new List<Image>();
    [HideInInspector]
    public PayLineCustomConfig payLineCustomConfig;
    public void InitElements(int width,int height,int number,int[] lineList,bool hasBlank){
        if (NumberTxt != null) NumberTxt.text = number.ToString();
        if (Line!=null) Line.sprite = sprite;
        SetGridLayoutGroup(width, height);
        SetLineAndTextParams(number,width,height);
        if (symbol!=null&&Line!=null&&lineList!=null)
        {
            int num = width * height;
            for (int i = 0; i < num; i++)
            {
                Image go = Instantiate<Image>(symbol);
                go.enabled = false;
                go.transform.SetParent(Line.transform);
                list.Add(go);
            }

            for (int j = 0; j < width; j++)
            {
                int index = 0;
                if (j >= lineList.Length) break;
                if (hasBlank) index = (height - lineList[j]) * width + j;
                else index = (height - 1 - lineList[j]) * width + j;
                if (index < 0){
                    Debug.LogError("LineTable上的每列Symbol数值高于你所设定的重写的Height值");
                    break;
                } 
                if (index<list.Count&&list[index]!=null)
                {
                    Image go = list[index];
                    go.enabled = true;
                }
            }
        }
    }
    GridLayoutGroup gridLayoutGroup;
    void SetGridLayoutGroup(int width, int height)
    {
        gridLayoutGroup = Line.transform.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        if (payLineCustomConfig!=null) SetLayoutGroup_Custom_Params();
        else if (width == 5 && (height == 3 || height == 5)) SetLayoutGroup_3X5_Params();//3row,5column
        else if (width == 3 && (height == 3 || height == 5)) SetLayoutGroup_3X3_Params();
        else if (width == 4 && height == 4) SetLayoutGroup_4X4_Params();

    } 
    void SetLineAndTextParams(int No,int width,int height){
        if(payLineCustomConfig!=null) SetLine_Custom_Params(No);
        else if (width == 5 && (height == 3 || height == 5)) SetLine_3X5_Params(No);//3row,5column
        else if (width == 3 && (height == 3 || height == 5)) SetLine_3X3_Params(No);
        else if (width == 4 && height == 4) SetLine_4X4_Params(No);
    }
    void SetLayoutGroup_3X5_Params(){
        //Debug.Log("SetLayoutGroup_3X5_Params");
        gridLayoutGroup.padding.left = 13;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 12;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(46.4f, 42.73f);
        gridLayoutGroup.spacing = new Vector2(9.7f, 7.7f);
        gridLayoutGroup.constraintCount = 5;
    }

    void SetLayoutGroup_3X3_Params(){
        //Debug.Log("SetLayoutGroup_3X3_Params");
        gridLayoutGroup.padding.left = 13;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 12;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(46.4f, 42.73f);
        gridLayoutGroup.spacing = new Vector2(9.7f, 7.7f);
        gridLayoutGroup.constraintCount = 3;
    }

    void SetLayoutGroup_4X4_Params(){
        //Debug.Log("SetLayoutGroup_4X4_Params");
        gridLayoutGroup.padding.left = 13;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 12;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(46.4f, 42.73f);
        gridLayoutGroup.spacing = new Vector2(9.7f, 7.7f);
        gridLayoutGroup.constraintCount = 4;
    }

    void SetLayoutGroup_Custom_Params(){
        //Debug.Log("SetLayoutGroup_Custom_Params");
        if (payLineCustomConfig == null) return;
        gridLayoutGroup.padding.left = payLineCustomConfig.lineSymbolPaddingLeft;
        gridLayoutGroup.padding.right = payLineCustomConfig.lineSymbolPaddingRight;
        gridLayoutGroup.padding.top = payLineCustomConfig.lineSymbolPaddingTop;
        gridLayoutGroup.padding.bottom = payLineCustomConfig.lineSymbolPaddingBottom;
        gridLayoutGroup.cellSize = payLineCustomConfig.lineSymbolCellSize;
        gridLayoutGroup.spacing = payLineCustomConfig.lineSymbolSpacing;
        gridLayoutGroup.constraintCount = payLineCustomConfig.lineSymbolConstraintCount;
    }

    void SetLine_3X5_Params(int No){
        //Debug.Log("SetLine_3X5_Params:" + No);
        RectTransform rT1= Line.rectTransform;
        rT1.localPosition = new Vector3(213.8f,-83f,0f);
        rT1.sizeDelta = new Vector2(297.7f,166f);
        RectTransform rT2 = NumberTxt.rectTransform;
        if (No<10)
        {
            rT2.localPosition = new Vector3(32.5f, -83f, 0f);
            rT2.sizeDelta = new Vector2(65f, 166f);
        }
        else
        {
            rT2.localPosition = new Vector3(28f, -83f, 0f);
            rT2.sizeDelta = new Vector2(74f, 109f);
        }

    }
    void SetLine_3X3_Params(int No)
    {
        //Debug.Log("SetLine_3X3_Params:" + No);
        RectTransform rT1 = Line.rectTransform;
        rT1.localPosition = new Vector3(157.5f, -83f, 0f);
        rT1.sizeDelta = new Vector2(185f, 166f);
        RectTransform rT2 = NumberTxt.rectTransform;
        if (No < 10)
        {
            rT2.localPosition = new Vector3(32.5f, -83f, 0f);
            rT2.sizeDelta = new Vector2(65f, 166f);
        }
        else
        {
            rT2.localPosition = new Vector3(28f, -83f, 0f);
            rT2.sizeDelta = new Vector2(74f, 109f);
        }
    }

    void SetLine_4X4_Params(int No)
    {
        //Debug.Log("SetLine_4X4_Params:" + No);
        RectTransform rT1 = Line.rectTransform;
        rT1.localPosition = new Vector3(188f, -107f, 0f);
        rT1.sizeDelta = new Vector2(243f, 215f);
        RectTransform rT2 = NumberTxt.rectTransform;
        if (No<10)
        {
            rT2.localPosition = new Vector3(35.4f, -102.7f, 0f);
            rT2.sizeDelta = new Vector2(65f, 166f);
        }
        else
        {
            rT2.localPosition = new Vector3(33f, -98f, 0f);
            rT2.sizeDelta = new Vector2(78f, 111f);
        }
    }

    void SetLine_Custom_Params(int No)
    {
        //Debug.Log("SetLine_Custom_Params:"+No);
        if (payLineCustomConfig == null) return;
        RectTransform rT1 = Line.rectTransform;
        rT1.localPosition = payLineCustomConfig.lineLocalPosition;
        rT1.sizeDelta = payLineCustomConfig.lineSizeDelta;
        RectTransform rT2 = NumberTxt.rectTransform;
        if (No < 10)
        {
            rT2.localPosition = payLineCustomConfig.numberLessTenLocalPosition;
            rT2.sizeDelta = payLineCustomConfig.numberLessTenSizeDelta;
        }
        else
        {
            rT2.localPosition = payLineCustomConfig.numberNotLessTenLocalPosition;
            rT2.sizeDelta = payLineCustomConfig.numberNotLessTenSizeDelta;
        }
    }
}
