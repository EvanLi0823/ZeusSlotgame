using System.Collections;
using System.Collections.Generic;
using Classic;
using LuaFramework;

public class BaseEvent
{
	protected bool needParse = true;
	public string EventName{
		get{
			return Utils.Utilities.GetValue<string> (EventDict,GameConstants.Event_Key,"NONE");
		}
	}
	protected object mOwner;
	private Dictionary<string,object> eventDict;
	private Dictionary<string,object> paramDict;
	public Dictionary<string, object> EventDict {
		get {
			return eventDict;
		}
		set {
			eventDict = value;
			needParse = true;
		}
	}

	public BaseEvent(Dictionary<string, object> eventDict, object owner = null)
	{
		EventDict = eventDict;
		mOwner = owner;
	}

	public Dictionary<string,object> ParaDict
	{
		get
		{ 
			if (needParse) {
				needParse = false;
				paramDict= Utils.Utilities.GetValue<Dictionary<string,object>>(EventDict,GameConstants.ParaDict_Key,null);
			} 
			return paramDict;
		}
	}

	public virtual void ExcuteEventAction(Dictionary<string,object> dict){}
	//public virtual void ExcuteEventAction(Dictionary<string,object> dict,ICommandEvent acceptEvent){}


	public virtual void OnShow(Dictionary<string,object> dict){
		BaseGameConsole.ActiveGameConsole().LogBaseEvent("Popup_Open", dict);
	}

	public virtual bool IsValid(){return true;}

	public virtual bool IsReady(){return true;}

    public virtual string GetSlotNameFromEvent(){return string.Empty;}

    public virtual bool IsCanDelete(){return false;}
    public virtual void Do(Dictionary<string, object> dict)
    {
	    ExcuteEventAction(dict);
    }
}