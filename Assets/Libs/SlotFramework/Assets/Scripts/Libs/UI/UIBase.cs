using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Libs{
	public class UIBase : UIBehaviour 
	{
	    protected RectTransform m_Transform;
	    public RectTransform Transfm { get { if (m_Transform == null)m_Transform = transform as RectTransform; return m_Transform; } }
	    public object m_Data;
		public string CurPrefabName { set; get;}

		protected override void Awake()
		{
	        m_Transform = transform as RectTransform;
	        
	    }

		public virtual void Init () {

		}
		public virtual void SetData(object data,bool needRefresh = false)
	    {
	        m_Data = data;
			if (needRefresh) {
				this.Refresh ();
			}
//			this.Refresh ();
	    }

		public virtual void Refresh(){

		}

	    public virtual void OnButtonClickHandler(GameObject go)
	    {
//			Debug.Log ("OnButtonClickHandler:"+go.name);
	        //to be override
	        //用于接收组件子集中按钮的点击事件，可以统一处理多个按钮事件
			CoroutineUtil.Instance.StartCoroutine(waitForNextClick());
	    }

		private IEnumerator waitForNextClick()
		{
			Libs.EventSystemUtils.DisableEventSystem();
			yield return new WaitForSecondsRealtime(0.5f);
			Libs.EventSystemUtils.EnableEventSystem();
		}

		public virtual void onToggleSelect(GameObject go,bool isSelect)
		{
//			Debug.Log ("OnButtonClickHandler:"+go.name);
		}

		protected override void Start ()
		{

		}
		protected override void OnDestroy()
		{
			
		}
	}
}