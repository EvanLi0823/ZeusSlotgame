using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classic;
using Core;
using System;

public class BoardStripConfig
{

     public SymbolMap SymbolMap
    {
        get;
        set;
    }

    public ReelStripManager BoardStrips
    {
        get;
        set;
    }

    public ClassicPaytable PayTable
    {
        get;
        set;
    }

    public LineTable LineTable()
    {
        return lineTable;
    }

    public void SetLineTable(LineTable table)
    {
        this.lineTable = table;
    }
    private LineTable lineTable;


    public void InitStripConfig(Dictionary<string, object> plistData,SlotMachineConfig config)
    {   
        if (plistData.ContainsKey(SymbolMap.SYMBOL_MAP))
        {
            this.SymbolMap = new SymbolMap(plistData[SymbolMap.SYMBOL_MAP] as Dictionary<string, object>);
        }
        else throw new Exception(config.Name() + "symbolMap is Empty");


        if(!config.SpinUseNetwork )
        {
            if (plistData.ContainsKey(ReelStripManager.MACHINE_MATH_NODE))
                this.BoardStrips = new ReelStripManager(this.SymbolMap,
                    plistData[ReelStripManager.MACHINE_MATH_NODE] as Dictionary<string, object>);
            else throw new Exception(config.Name() + "ReelStrip is Empty");
        }

        if(!config.SpinUseNetwork )
        {
            if (plistData.ContainsKey(ClassicPaytable.PAY_TABLE))
            {
                this.PayTable = new ClassicPaytable(this.SymbolMap,
                    plistData[ClassicPaytable.PAY_TABLE] as List<object>, null);
            }
            else
            {
                throw new Exception(config.Name() + "PayTable is Empty");
            }
        }

        if (plistData.ContainsKey(SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY))
        {
            string lineTableName = plistData[SlotMachineConfigParse.SLOT_MACHINE_LINE_TABLE_NAME_KEY] as string;

            if(string.IsNullOrEmpty(lineTableName))
            {
                throw new Exception(config.Name() + "lineTable is Empty");
            }

            if(config.LineTableDict.ContainsKey(lineTableName))
            {
                LineTable _lineTable = config.LineTableDict[lineTableName];
                if(_lineTable != null)
                {
                    this.SetLineTable(_lineTable);
                }
            }
            else { 
                Plugins.ConfigurationParseResult plistResult = Plugins.Configuration.GetInstance().ConfigurationParseResult();
                LineTable _lineTable = plistResult.GetLineTable(lineTableName);
                this.SetLineTable(_lineTable);
            }
        }
    }
    
    
    public void Clear()
    {
        this.SymbolMap = null;
        this.BoardStrips = null;
        PayTable = null;
        lineTable = null;
    }
}
