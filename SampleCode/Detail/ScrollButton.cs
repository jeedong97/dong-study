using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollButton : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IDragHandler, IPointerUpHandler
{
    public DetailSwipeDown detailSwipeDown;
    public ScrollRect scrollRect;
    public UnityEvent onClick;
    Vector2 movpos;
    private void Awake()
    {
        onClick = new UnityEvent();
    }
    void Start()
    {
        detailSwipeDown = GameObject.Find("*DetailBg_container").GetComponent<DetailSwipeDown>();
        scrollRect = detailSwipeDown.SC_ScrollCantainer;
    }

    public void OnPointerDown(PointerEventData e)
    {
        detailSwipeDown.OnPointerDown(e);
        scrollRect.OnBeginDrag(e);
    }
    public void OnBeginDrag(PointerEventData e)
    {
        detailSwipeDown.OnBeginDrag(e);
    }
    public void OnDrag(PointerEventData e)
    {
        movpos = e.pressPosition - e.position;
        detailSwipeDown.OnDrag(e);
        scrollRect.OnDrag(e);
    }
    public void OnPointerUp(PointerEventData e)
    {
        if (movpos.magnitude < 50)
        {
            onClick.Invoke();
            movpos = Vector2.zero;
            return;
        }
        movpos = Vector2.zero;
        scrollRect.OnEndDrag(e);
        detailSwipeDown.OnPointerUp(e);
    }
    void Update()
    {

    }
}
