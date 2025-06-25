using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace UI.UIComponent.ScrollList
{
		public class SkyElementBase : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
		{

				protected SkyScrollPanel MySkyScrollPanel;
			
				public virtual bool Init (int index, SkyScrollPanel mySkyScrollPanel)
				{
						this.MySkyScrollPanel = mySkyScrollPanel;
                        return true;
				}

				public virtual void OnPointerDown (PointerEventData eventData)
				{
						MySkyScrollPanel.OnSubPointDown ();
				}

				public virtual void OnPointerUp (PointerEventData eventData)
				{
						MySkyScrollPanel.OnSubPointUp ();
				}

				public virtual void OnPointerClick (PointerEventData eventData)
				{
				}
		}
}
