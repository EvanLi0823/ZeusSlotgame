using System;
using System.Collections;
using System.Threading;

using App;
using Core;
using Plugins;

public class FreespinRecord{
    public class RecordElement
    {
        public string Name;
        public long Value;
        
        public RecordElement (string name)
        {
            this.Name = name;
			this.Value = SharedPlayerPrefs.LoadPlayerPrefsLong (name, 0L);
        }
        
        public void Save (long value)
        {
            this.Value = value;
            Save ();
        }
        
        public void Save ()
        {
			SharedPlayerPrefs.SavePlayerPrefsLong (Name, Value);
        }
    }

    public FreespinRecord (string preFixed)
    {
        this.preFixed = preFixed;
        LeftTimes = new RecordElement (this.preFixed + LEFT_TIMES);
        TotalBonus = new RecordElement (this.preFixed + TOTAL_BONUS);
        IsActive = new RecordElement (this.preFixed + IS_ACTIVE);
    }
	

    public RecordElement LeftTimes;
    public RecordElement TotalBonus;
    public RecordElement IsActive; //value: 0,disable;1 active

    private static readonly string LEFT_TIMES = "LeftTimesFreespin";
    private static readonly string TOTAL_BONUS = "TotalBonusFreespin";
    private static readonly string IS_ACTIVE = "IsActiveFreespin";
    private string preFixed;


    public bool IsEffective ()
    {
        return IsActive.Value == 1L && LeftTimes.Value>0L ;
    }


    public void DisActive(){
        LeftTimes.Save (0L);
        TotalBonus.Save (0L);
        IsActive.Save (0L);
    }


    public void SaveInfo(long leftTimes,long totalBonus){
        LeftTimes.Save (leftTimes);
        TotalBonus.Save (totalBonus);
        IsActive.Save (1L);
    }
}
