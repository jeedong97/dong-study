using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FollowItemHandler : MonoBehaviour
{
    public CardDrag cardDrag;
    public List<RectTransform> List_RT, List_Target;
    public CanvasGroup CG;
    public float tweenTime, delayTime;
    void Start()
    {
        CG.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0 ; i <List_RT.Count; i ++)
        {
            float idx = i;
            List_RT[i].DOMove(List_Target[i].position, tweenTime).SetDelay(delayTime* idx).SetEase(Ease.Linear).SetId(0);
            // List_RT[i].eulerAngles = List_Target[i].eulerAngles;
            List_RT[i].DOLocalRotate(List_Target[i].eulerAngles, tweenTime).SetDelay(delayTime*idx).SetEase(Ease.Linear).SetId(0);            
        }
    }
}
