using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Classic;
using Core;
using Plugins;
public class PayLinesCreator : MonoBehaviour {

    public Sprite BG_3X3;
    public Sprite BG_3X5;
    public Sprite BG_4X4;
    public PayLineElement payLineElement;
    public bool EnableCustomConfig = false;
    GridLayoutGroup gridLayoutGroup =null;
    LineTable lineTable = null;
    GameConfigs gameConfigs;
    PayLineCustomConfig payLineCustomConfig;
    public bool UsePlistConfig = true;
    public bool OverrideHeight = false;
    public bool OverrideWidth = false;

    public Vector2 WidthHeightElementsNum = new Vector2();
    private const string WILD_DEFAULT_NAME = "GameConfig.plist";
    private const string CLASSIC_DEFAULT_NAME = "Config_classicIOS.plist";
    //private void Awake()
    //{
        //GeneratePayTable();
    //}
    private int width = 0;
    private int height = 0;
    private bool HasBlank = false;
    LineTable GetLineTable(){
        Dictionary<string, object> config = Plugins.Configuration.ReadConfigInUnityEditor(CLASSIC_DEFAULT_NAME);
        if (config == null) config = Plugins.Configuration.ReadConfigInUnityEditor(WILD_DEFAULT_NAME);
        ConfigurationParseResult result = new ConfigurationParseResult();
        result.SetRawConfiguration(config);
        Dictionary<string, object> lineTableListDict = Utils.Utilities.GetValue<Dictionary<string, object>>(config, LineTable.LINE_TABLES_KEY, null);
        Dictionary<string, LineTable> lineTables = ParseDictionary<LineTable>(lineTableListDict,LineTable.ParseLineTable, result);
        string lineTableName = "";
        Dictionary<string,object> PlistDict = SlotMachineConfig.GetMachinePlistDataInEditor(SwapSceneManager.Instance.GetLogicSceneName());
        if (PlistDict.ContainsKey(SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY))
        {
            lineTableName = PlistDict[SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY] as string;
        }
        return lineTables[lineTableName];
    }
    void SetWidthHeightParams(){
        if (UsePlistConfig)
        {
            string[] strs = lineTable.Name().Split('_');
            if (strs != null) {
               string[] nums = strs[0].Split('X');
               if (nums.Length==2)
                {
                    height= Utils.Utilities.CastValueInt(nums[0]);
                    width = Utils.Utilities.CastValueInt(nums[1]);
                    if (height == 5) 
                    {
                        height = (height + 1) / 2;
                        HasBlank = true;
                    }
                    else HasBlank = false;
                }
            }
        }
        else
        {
            gameConfigs = GameObject.Find("Main Camera/UICanvas/UIPanel/MiddlePanel/MachinePanel/GamePanel").transform.GetComponent<GameConfigs>();
            if (gameConfigs == null) Debug.LogError("请检查当前的GameConfig路径命名是否与其一致:Main Camera/UICanvas/UIPanel/MiddlePanel/MachinePanel/GamePanel");
          
            #region WidthAndHeight
            width = gameConfigs.reelConfigs.Length;
            List<int> heights = new List<int>();
            if (gameConfigs.hasBlank)
            {
                for (int i = 0; i < width; i++)
                {
                    heights.Add((gameConfigs.reelConfigs[i].ElementNumbers + 1) / 2);
                    if (height < heights[i])
                    {
                        height = heights[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < width; i++)
                {
                    heights.Add(gameConfigs.reelConfigs[i].ElementNumbers);
                    if (height < heights[i])
                    {
                        height = heights[i];
                    }
                }
            }
            #endregion
            HasBlank = gameConfigs.hasBlank;
        }
    
        if (OverrideWidth)
        {
            width = (int)WidthHeightElementsNum.x;
        }
        if (OverrideHeight)
        {
            height = (int)WidthHeightElementsNum.y;
            if (height == 5) 
            {
                height = (height + 1) / 2;
                HasBlank = true;
            }
        }
       
    }
    protected delegate T ParseDictElement<T>(string name, Dictionary<string, object> dict, object context);
    protected Dictionary<string, T> ParseDictionary<T>(Dictionary<string, object> dictObj, ParseDictElement<T> elementParse, object context)
    {
        Dictionary<string, T> result = new Dictionary<string, T>();
        foreach (string key in dictObj.Keys)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)dictObj[key];
            T tObj = elementParse(key, dict, context);
            if (tObj != null)
            {
                result.Add(key, tObj);
            }
        }
        return result;
    }
    public void ClearObjects(){
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            DestroyImmediate(child.gameObject);
        }
        payLineCustomConfig = null;
        lineTable = null;
        gameConfigs = null;
        gridLayoutGroup = null;
    }
    public void GeneratePayTable(){
        ClearObjects();
        if (payLineElement == null) return;
        gridLayoutGroup = transform.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null) return;
        if (EnableCustomConfig) payLineCustomConfig = transform.GetComponent<PayLineCustomConfig>();

        lineTable = GetLineTable();
        int lineNum = lineTable.PayLines().Count;
        SetWidthHeightParams();
        SetGridLayoutGroup(width, height);
        for (int i = 0; i < lineNum; i++)
        {
            PayLineElement go = Instantiate<PayLineElement>(payLineElement);
            if (EnableCustomConfig) go.payLineCustomConfig = payLineCustomConfig;
            SetPayLineElementBG(width, height, go);
            go.InitElements(width, height, i + 1, lineTable.PayLines()[i].RowNumbers(), HasBlank);
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
        }
    }
    void SetPayLineElementBG(int width, int height,PayLineElement ple){
        if (payLineCustomConfig != null) ple.sprite = payLineCustomConfig.sprite;
        else if (width == 3 && (height == 3 || height == 5) && BG_3X3 != null) ple.sprite = BG_3X3;
        else if (width == 5 && (height == 3 || height == 5) && BG_3X5 != null) ple.sprite = BG_3X5;
        else if (width == 4 && height == 4 && BG_4X4 != null) ple.sprite = BG_4X4;
    }

    void SetGridLayoutGroup(int width,int height){
        gridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        if(payLineCustomConfig!=null) SetLayoutGroup_Custom_Params();
        else if (width == 5 &&(height == 3 || height == 5)) SetLayoutGroup_3X5_Params();//3row,5column
        else if (width == 3 && (height == 3 || height == 5) ) SetLayoutGroup_3X3_Params();
        else if (width == 4 && height == 4) SetLayoutGroup_4X4_Params();

    }

    void SetLayoutGroup_3X3_Params(){
        //Debug.Log("SetLayoutGroup_3X3_Params");
        gridLayoutGroup.padding.left = 110;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 180;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(250, 166);
        gridLayoutGroup.spacing = new Vector2(40, 50);
        gridLayoutGroup.constraintCount = 5;
    }

    void SetLayoutGroup_3X5_Params(){
        //Debug.Log("SetLayoutGroup_3X5_Params");
        gridLayoutGroup.padding.left = 110;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 180;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(360, 166);
        gridLayoutGroup.spacing = new Vector2(150, 50);
        gridLayoutGroup.constraintCount = 3;
    }

    void SetLayoutGroup_4X4_Params(){
        //Debug.Log("SetLayoutGroup_4X4_Params");
        gridLayoutGroup.padding.left = 30;
        gridLayoutGroup.padding.right = 0;
        gridLayoutGroup.padding.top = 180;
        gridLayoutGroup.padding.bottom = 0;
        gridLayoutGroup.cellSize = new Vector2(310, 215);
        gridLayoutGroup.spacing = new Vector2(0, 50);
        gridLayoutGroup.constraintCount = 5;
    }

    void SetLayoutGroup_Custom_Params(){
        //Debug.Log("SetLayoutGroup_Custom_Params");
        if (payLineCustomConfig == null) return;
        gridLayoutGroup.padding.left = payLineCustomConfig.payLinesPaddingLeft;
        gridLayoutGroup.padding.right = payLineCustomConfig.payLinesPaddingRight;
        gridLayoutGroup.padding.top = payLineCustomConfig.payLinesPaddingTop;
        gridLayoutGroup.padding.bottom = payLineCustomConfig.payLinesPaddingBottom;
        gridLayoutGroup.cellSize =payLineCustomConfig.payLinesCellSize;
        gridLayoutGroup.spacing = payLineCustomConfig.payLinesSpacing;
        gridLayoutGroup.constraintCount = payLineCustomConfig.payLinesConstraintCount;
    }
}
