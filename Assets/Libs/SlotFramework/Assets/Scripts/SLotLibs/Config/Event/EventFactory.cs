using System.Collections;
using System.Collections.Generic;
namespace Classic{
	public class EventFactory  {
		public const string REWARD_SPIN_EVENT_KEY = "RewardSpinEvent";
		public static BaseEvent CreateEvent(Dictionary<string,object> eventDict,object owner)
        {
            string eventName = Utils.Utilities.GetValue<string>(eventDict, GameConstants.Event_Key, string.Empty);
			if (string.IsNullOrEmpty(eventName)) return null;
			
			BaseEvent ec = null;
			switch (eventName)
			{
				case REWARD_SPIN_EVENT_KEY:
					ec = new RewardSpinEvent (eventDict, owner);
					break;
				default:
					break;
			}
			return ec;
		}
	}
}