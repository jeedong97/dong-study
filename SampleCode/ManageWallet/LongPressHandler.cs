using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    public CardDrag CD;
    public ScrollRect SR_Parant;
    public LongPressNEditHandler EditMode;
    bool isLongPress;
    public UnityEvent onLongPress, onEndLongPress;

    // 유니티 엔진을 두개를 만들어 준다 longpress와 onendlongpress
    void Awake()
    {
        onLongPress = new UnityEvent();
        onEndLongPress = new UnityEvent();
    }
    void Start()
    {

    }
    public void OnPointerDown(PointerEventData e) // longpress 시작
    {
        CancelInvoke("StartEditMode");
        Invoke("StartEditMode", 0.45f);
    }
    // 큰 역할은 없고 다른 pointerEventData들을 받아오는 역할을 한다.
    public void OnBeginDrag(PointerEventData e)
    {
        SR_Parant.OnBeginDrag(e);
        EditMode.OnBeginDrag(e);
    }
    // 다른 pointerEventData들을 받아오는 역할을 한다.
    // 만약 드래그 범위가 일정 범위 이상 움직였을 때 starteditmode는 취소가 된다.
    public void OnDrag(PointerEventData e)
    {
        SR_Parant.OnDrag(e);
        EditMode.OnDrag(e);
        if (Mathf.Abs(e.pressPosition.x - e.position.x) > 10 || Mathf.Abs(e.pressPosition.y - e.position.y) > 10)
        {
            CancelInvoke("StartEditMode");
        }
    }
    // 다른 pointerEventData들을 받아오는 역할을 한다.
    public void OnEndDrag(PointerEventData e)
    {
        if (isLongPress)
            EditMode.OnEndDrag(e);
        else
            SR_Parant.OnEndDrag(e);
    }
    public void OnPointerUp(PointerEventData e)
    {
        if (!isLongPress)
        {
            CancelInvoke("StartEditMode");
        }
        if (isLongPress)
        {
            EndEditMode();
        }
    }
    void StartEditMode() // 에딧모드 진입 함수
    {
        SR_Parant.enabled = false;
        isLongPress = true;
        onLongPress.Invoke();
    }
    void EndEditMode() //에딧모드 퇴장 함수
    {
        SR_Parant.enabled = true;
        isLongPress = false;
        onEndLongPress.Invoke();
    }
}
