using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Utils;
using System.IO;

namespace UI.Utils
{
    /**
     *
     * Construct a path name to traverse a transform's hierarchy.
     * 
     */
    public class PathNameBuilder {


        public static PathNameBuilder CreateNamePathBuilder(string initalName)
        {
            PathNameBuilder builder = new PathNameBuilder();
            builder.AddName(initalName);
            return builder;
        }

        public PathNameBuilder()
        {
            strBuilder = new StringBuilder();

        }

        public PathNameBuilder AddName(string name)
        {
            if (strBuilder.Length == 0) {
                strBuilder.Append (name);
            } else {
                strBuilder.Append ("/").Append(name);
            }
            return this;
        }

        public string PathName()
        {
            return strBuilder.ToString();
        }

        private StringBuilder strBuilder;
    }

    public class DelayAction {

        public float DelayTime { get;set;}
        public Action DelayedAction {get; set;}
        public IEnumerator DelayedRoutine { get {return delayedRoutine;}}

        public IEnumerator CreateDelayedRoutine() 
        {
            delayedRoutine =  UIUtil.DelayAction(DelayTime,DelayedAction);
            return delayedRoutine;
        }

        private IEnumerator delayedRoutine;
    }

    public class UIUtil
    {
        public UIUtil ()
        {
        }

        public static Vector3 LocalPositionInCanvasToScreenPoint (Transform trans)
        {
            Vector3 localPosition = Vector3.zero;
            Transform t = trans;
            
            while (t != null && t.GetComponent<Canvas>() == null) {
                localPosition += t.localPosition;
                t = t.parent;
            }
            return localPosition;
        }

        public static void ShowGameObject(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        public static void HideGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        public static Canvas ParentCanvas(Text text)
        {
            Transform t = text.transform;
            while (t != null) {
                if (t.GetComponent<Canvas>() != null) {
                    return t.GetComponent<Canvas>();
                }
                t = t.parent;
            }
            return null;
        }

        public static Canvas ParentCanvas(GameObject gameObject)
        {
            Transform t = gameObject.transform;
            while (t != null) {
                if (t.GetComponent<Canvas>() != null) {
                    return t.GetComponent<Canvas>();
                }
                t = t.parent;
            }
            return null;
        }

		public static bool SetTextLabelValueIn(Transform parent, string labelName, string text)
        {
            Text uiText = TextIn (parent,labelName);
            if (uiText != null) {
                uiText.text = text;
                return true;
            }
            return false;
        }

        public static string TextValueIn(Transform parent, string labelName)
        {
            Text text = TextIn (parent,labelName);
            if (text != null) {
                return text.text;
            }
            return null;
        }

        public static Text TextIn(Transform parent, string labelName)
        {
            Transform titleLabelTr = parent.Find (labelName);
            
            if (titleLabelTr != null) {
                Text titleLabel = titleLabelTr.GetComponent<Text>();
                if (titleLabel != null) {
                    return titleLabel;
                }
            }
            return null;
        }

        public static EventTrigger.Entry AddEventTrigger(Selectable button,EventTriggerType triggerType, UnityAction action)
		{
			UnityAction<BaseEventData> actionProxy = null;
			if (action != null) {
				actionProxy = delegate(BaseEventData data) {
					action();
				};
			}

			return AddEventTrigger(button,triggerType,actionProxy);
		}

		public static EventTrigger.Entry AddEventTrigger(Selectable button,EventTriggerType triggerType, UnityAction<BaseEventData> action)
		{
			if (button == null) {
				logger.LogWarning("found an unexpected null button to setup pointerDown event,ignore it");
				return null;
			}
			
			EventTrigger eventTrigger = button.GetComponent<EventTrigger> ();
			if (eventTrigger == null) {
				eventTrigger = button.gameObject.AddComponent<EventTrigger>();
                if (eventTrigger.triggers == null) {
    				eventTrigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
                }
			}
			
			
			EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent ();
			trigger.AddListener ((eventData) => action (eventData));
			
			EventTrigger.Entry entry = new EventTrigger.Entry () {
				callback = trigger,eventID = triggerType
			};
			eventTrigger.triggers.Add (entry);
			
			return entry;
		}

        public static void RemovePointerEventTrigger(Selectable button, EventTrigger.Entry entry)
        {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger> ();
            if (eventTrigger != null && eventTrigger.triggers != null) {
                if (eventTrigger.triggers.Contains(entry)) {
                    eventTrigger.triggers.Remove(entry);
                }
            }
            
           
        }

        public static void MoveVertically(RectTransform uiRectTrans, float offset)
        {
            Vector2 offsetMax = uiRectTrans.offsetMax;
            offsetMax.y += offset;
            Vector2 offsetMin = uiRectTrans.offsetMin;
            offsetMin.y += offset;
            uiRectTrans.offsetMax = offsetMax;
            uiRectTrans.offsetMin = offsetMin;
        }

        public static IEnumerator DelayAction (float dTime, System.Action callback)
        {
            yield return new WaitForSeconds (dTime);
            callback ();
        }

		public static IEnumerator DelayRealTimeAction (float dTime, System.Action callback)
		{
			yield return new WaitForSecondsRealtime (dTime);
			callback ();
		}

		public delegate bool BoolExpr ();
		
		public delegate void RepeatMethod ();

		public static IEnumerator RepeatInvocation (BoolExpr shouldFinish, RepeatMethod repeatMethod, float interval)
		{
			
			while (!shouldFinish ()) {
				repeatMethod ();
				yield return new WaitForSeconds (interval);
			}
			
		}
		
		public static IEnumerator RepeatInvocation(RepeatMethod repeatMethod, float interval)
		{
			return RepeatInvocation (delegate() {
				return false;
			}, repeatMethod, interval);
		}


        public static void CheckNotNull(object obj, string objname)
        {
            if (obj == null) {
                throw new System.ArgumentNullException(objname);
            }
        }

		public static string ReplaceLastZeroBeforeCommaWithZeroStarImage (string coinsCount)
		{
			if (coinsCount.LastIndexOf(',') > 0 && coinsCount.Substring(coinsCount.LastIndexOf(',') - 1,1).Equals("0")) {
				StringBuilder sb = new StringBuilder(coinsCount);
				sb[coinsCount.LastIndexOf(',') - 1] = '$';
				coinsCount = sb.ToString();
			}
			return coinsCount;
		}

		public static void SaveDataToFile (string filePath, byte[]data)
		{
			try{
				string path = Path.GetDirectoryName (filePath);
				if (!Directory.Exists (path)) {
					Directory.CreateDirectory (path);
				}
				File.WriteAllBytes (filePath, data);
			}
			catch(Exception e){
				string msg = e.Message;
				if (string.IsNullOrEmpty(msg)) msg = string.Empty;
				Utilities.LogError(string.Format("SaveDataToFile :{0} {1}",filePath,msg));
			}
		}

		public static Texture2D LoadPNG(string filePath) {
			
			Texture2D tex = null;
			byte[] fileData;
			
			if (File.Exists(filePath))     {
				fileData = File.ReadAllBytes(filePath);
				tex = new Texture2D(2, 2);
				tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			}
			return tex;
		}
        public static global::Utils.Logger logger = global::Utils.Logger.GetUnityDebugLogger(typeof(UIUtil),false);
        private static bool IsRealPicture(string path)
        {
            if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".bmp"))
            {
                return true;
            }
            return false;
        }
    }
}

