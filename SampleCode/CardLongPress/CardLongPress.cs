using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardLongPress : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    public CardDrag CD;
    public ScrollRect SR_Parant;
    public CardEditModeHandler CardEditMode;
    public bool isLongPlress;
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
    // 먼저 pointerdown을 했을 때 starteditmode가 작동이 된다.
    Vector2 startPos;
    public void OnPointerDown(PointerEventData e)
    {
        CancelInvoke("StartEditMode");
        Invoke("StartEditMode", 0.45f);
        startPos = e.pressPosition;
    }
    // 큰 역할은 없고 다른 pointerEventData들을 받아오는 역할을 한다.
    public void OnBeginDrag(PointerEventData e)
    {
        CD.OnBeginDrag(e);
        SR_Parant.OnBeginDrag(e);
        CardEditMode.OnBeginDrag(e);

    }
    // 다른 pointerEventData들을 받아오는 역할을 한다.
    // 만약 드래그 범위가 일정 범위 이상 움직였을 때 starteditmode는 취소가 된다.
    public void OnDrag(PointerEventData e)
    {
        CD.OnDrag(e);
        SR_Parant.OnDrag(e);
        CardEditMode.OnDrag(e);

        if (Mathf.Abs(e.pressPosition.x - e.position.x) > 10 || Mathf.Abs(e.pressPosition.y - e.position.y) > 10)
        {
            CancelInvoke("StartEditMode");
        }
    }
    // 다른 pointerEventData들을 받아오는 역할을 한다.
    public void OnEndDrag(PointerEventData e)
    {
        CD.OnEndDrag(e);
        SR_Parant.OnEndDrag(e);
        CardEditMode.OnEndDrag(e);
    }
    //starteditmode가 호출이 되면 islongpress의 참거짓이 달라진다
    // 실행되기전에 손가락을 때면 starteditmode가 취소 되고 
    // 그외에는 endeditmode라는 함수가 호출이 된다.
    public void OnPointerUp(PointerEventData e)
    {
        if (!isLongPlress)
        {
            CancelInvoke("StartEditMode");
        }
        if (isLongPlress)
        {
            EndEditMode();
        }
    }
    // 오랫동안 누르고 있으면 실행이 됨
    // 오랫동안 눌렀다는건 onlongpress임
    // onlongpress는 button.onclick이랑 구분이 됨
    //onlongpress가 호출이 될때 carddrag에서 이벤트가 실행된다고 지정해준다.
    void StartEditMode()
    {
        if (CD.MyStatus == Status.CardListOpen && CD.direction =="Ver")
        {
            CD.Scl.enabled = false;
            isLongPlress = true;
            CD.MyStatus = Status.CardEditMode;
            CardEditMode.preStartPos = startPos;
            onLongPress.Invoke();
        }
    }
    //오래누르고 있다가 때는 상황이다
    //그냥 enddrag나 onpointerup이랑 구분이 된다
    //오래눌르고 있다가 땠다는건 onendlongpress라는 뜻임
    //carddrag에서 onendlongpress일대 실행되는 이벤트를 지정한다.
    void EndEditMode()
    {
        CD.isDrag = true;
        CD.Scl.enabled = true;
        isLongPlress = false;
        onEndLongPress.Invoke();
    }
}
