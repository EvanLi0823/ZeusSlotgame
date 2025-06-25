using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonDragScale : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler{
	public delegate void ClickDelegate ();
	public ClickDelegate onClick;
	public void OnPointerDown (PointerEventData eventData)
	{
		(transform as RectTransform).DOScale(0.9f, 0.3f);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		(transform as RectTransform).DOScale(1f, 0.3f);
	}
	public void OnPointerClick (PointerEventData eventData)
	{
		if (onClick != null) {
			onClick ();
		}
	}
}
