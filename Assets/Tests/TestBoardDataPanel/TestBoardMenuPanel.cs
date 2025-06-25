using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TestBoardMenuPanel : MonoBehaviour,IDragHandler,IPointerDownHandler
{
    private Vector2 OffsetPos;
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = eventData.position - OffsetPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OffsetPos = eventData.position - (Vector2) transform.localPosition;
    }
}
