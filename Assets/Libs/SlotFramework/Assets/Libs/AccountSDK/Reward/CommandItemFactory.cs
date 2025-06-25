using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class CommandItemFactory  {

		public static BaseCommandItem CreateCommandItem(string cmdType,string cmdToken,Dictionary<string,object> cmdDict)
		{
			if (string.IsNullOrEmpty(cmdType) || cmdDict == null) return null;
			BaseCommandItem commandItem = null;
			
			switch (cmdType)
			{
				#region Cases
			
				case GameConstants.NormalCommand_Key:
					commandItem = new NormalCommandItem(cmdToken,cmdDict);
					break;
				case GameConstants.EventCommand_Key:
					commandItem = new EventCommandItem(cmdToken,cmdDict);
					break;
				case GameConstants.DeletePrefsCommand_key:
					commandItem = new DeletePrefsCommand(cmdToken, cmdDict);
					break;
				default:
					Utils.Utilities.LogPlistError ("Command:" + cmdType + " Parse not right");
					break;
					#endregion
			}
			return commandItem;
		}
	}
}