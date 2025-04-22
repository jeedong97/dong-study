using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BoardingPassSwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int MyIndex = 0;
    public List<RectTransform> List_RT;
    public List<float> List_Pos;
    public RectTransform Indicator;
    float movPos;
    void Start()
    {
        for (var i = 0; i < 3; i++)
        {
            List_Pos.Add(i * 1035);
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {

    }
    public void OnDrag(PointerEventData e)
    {
        movPos = e.pressPosition.x - e.position.x;
        Debug.Log(movPos);
        for (int i = 0; i < List_RT.Count; i++)
        {
            List_RT[i].anchoredPosition = new Vector2(List_Pos[i] - 1035 * MyIndex - movPos/2, -30);
        }
    }
    public void OnEndDrag(PointerEventData e)
    {
        if (movPos > 0)
        {
            if (MyIndex != List_RT.Count - 1)
            {
                MyIndex++;
            }
        }
        else
        {
            if (MyIndex != 0)
            {
                MyIndex--;
            }
        }
        for (int i = 0; i < List_RT.Count; i++)
        {
            List_RT[i].DOAnchorPosX(List_Pos[i] - 1035 * MyIndex, 0.4f).SetEase(Ease.OutQuart);
        }

        Indicator.anchoredPosition = new Vector2(63*MyIndex , 0);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
