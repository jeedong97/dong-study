using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BloomHandler : MonoBehaviour
{
    public AnimationCurve SineInOut80;
    public AnimationCurve SineInOut33;
    private Vector3 circlescale;

    RectTransform RT_Circle1, RT_Circle2;
    CanvasGroup CG_Circle1, CG_Circle2;

    // 스크립트가 시작 될 때 RectTransform 변수에 대입시켜준다.
    void Awake()
    {
        circlescale = new Vector3(1.7f,1.7f,1.7f);
        RT_Circle1 = this.transform.GetChild(0).GetComponent<RectTransform>();
        RT_Circle2 = this.transform.GetChild(1).GetComponent<RectTransform>();
        CG_Circle1 = this.transform.GetChild(0).GetComponent<CanvasGroup>();
        CG_Circle2 = this.transform.GetChild(1).GetComponent<CanvasGroup>();
        Init();
    }
    //생성되자마자 bloomingAnimation 함수가 호출이 된다.
    void Start()
    {
        BloomingAnimation();
    }
    // awake에서 미리 넣어진 원들을 동작시킨다.
    public void BloomingAnimation()
    {
        RT_Circle1.DOScale(circlescale ,2.3f).SetEase(SineInOut80);
        CG_Circle1.DOFade(0, 1.1f).SetEase(SineInOut33).SetDelay(1.1f);

        RT_Circle2.DOScale(circlescale ,2.3f).SetEase(SineInOut80).SetDelay(0.4f);;
        CG_Circle2.DOFade(0, 1.1f).SetEase(SineInOut33).SetDelay(1.7f);
    }
    public void Init()
    {
        // 2개의 파장애니메이션이 생성되고 끝나는 시점에 알아서 파괴되기;
        RT_Circle1.transform.localScale = new Vector2(0.0f,0.0f);
        RT_Circle2.transform.localScale = new Vector2(0.0f,0.0f);

    }
}
