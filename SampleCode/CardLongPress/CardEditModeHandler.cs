using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardEditModeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Model M;
    public CardDrag CD;
    public LongPressNEditHandler LPE;
    public bool isEditMode;
    public cardItem targetCard;
    public RectTransform targetBox;
    float startPosY;
    Vector2 movedPos;
    public int BarAmount;
    public int Myindex;

    void Start()
    {
        // Init();
    }

    public void OnBeginDrag(PointerEventData e)
    {

    }
    // 드래그 하는동안 startdrag에서 준비된 것들이 작동한다
    // movpos의 거리를 계산하게 되고 dragCard함수가 호출이 되고 
    // 본인이 얼마나 움직였는지를 알 수 있는 함수가 호출이되고 
    // 다른 카드들이 본인의 위치에 따라 어떻게 변하는지를 알 수 있다.
    public void OnDrag(PointerEventData e)
    {
        if (isEditMode)
        {
            movedPos = e.pressPosition - e.position;
            dragCard();
            knowBar(movedPos.y);
            for (int i = 0; i < CD.List_Card.Count; i++)
            {
                CD.List_Card[i].parallax.isParallax = false;
            }
        }
    }
    // movpos가 음수와 양수일때를 기준으로 작동한다
    // 193만큼의 거리를 간격으로 다르게 작동한다. 
    // ondrag에서 작동하기 때문에 단 한번만 호출할 수 있게 해준다
    // idx가 baramount와 다를 때 한번 호출이 되면서 baramount에 idx가 대입이 된다.
    // 그럼 더이상 해당 구역에 있을 때 호출이 안된다.
    void knowBar(float _movepos)
    {

        if (-_movepos > 0) // ----간격 나중에 변경 가능 할 수 있게 변수로 써주기
        {
            int idx = 0;
            for (int i = 0; i < CD.List_Card.Count - Myindex - 1; i++)
            {
                if (-_movepos >= idx * CD.distanceCard2Card && -_movepos <= idx * CD.distanceCard2Card + CD.distanceCard2Card)
                {
                    if (idx != BarAmount)
                    {
                        BarAmount = idx;
                        cardPositioning();
                    }
                }
                idx++;
            }
        }
        else
        {
            int idx = 0;
            for (int i = 0; i <= Myindex; i++)
            {
                if (_movepos >= idx * CD.distanceCard2Card && _movepos <= idx * CD.distanceCard2Card + CD.distanceCard2Card)
                {
                    if (-idx != BarAmount)
                    {
                        BarAmount = -idx;
                        cardPositioning();
                    }
                }
                idx++;
            }
        }


    }
    //float bigHeight = 580, small = 132;
    // 지금 선택된 targetcards는 carddrag 스크립트에 myindex번째 카드다
    // 고로 myindex를 제외한 나머지 카드들만 움직이게 한다.
    // myindes에서 얼마나 움직였는지를 알려주는 baramount를 더한 값이 현재 targetcard의 위치고
    //그 위치를 기준으로 작은 카드들의 자리와 큰 카드들의 자리를 정해준다.
    // knowBar 안에서 한번씩만 동작하게 한다.
    void cardPositioning()
    {
        int cnt = 0;

        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            if (i != Myindex)
            {
                if (cnt >= Myindex + BarAmount)
                {
                    CD.List_Card[i].RT.DOAnchorPosY(CD.List_openpos[cnt] + CD.distanceCard2Card, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
                else
                {
                    CD.List_Card[i].RT.DOAnchorPosY(CD.List_openpos[cnt], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
                cnt++;
            }
        }

    }
    public void OnEndDrag(PointerEventData e)
    {

    }
    // targetcard를 움직인 거리만큼 움직이게 한다.
    void dragCard()
    {
        if (targetCard != null)
        {
            targetCard.RT.anchoredPosition = new Vector2(0, startPosY + 200) - movedPos;
            targetBox.anchoredPosition = new Vector2(0, startPosY + 200) - movedPos;
        }
    }
    // carddrag에 들어가게 될 함수다
    // idx 번째 carditem을 동작시킬 준비를 한다.
    // targetcard와 startpos 변수에 carddrag안에서의 정보들을 넣어준다
    public Vector2 preStartPos;
    public float tweenTime;
    public void StartDrag(int idx)
    { //
        isEditMode = true;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].Img_Shadow.color = Color.black;
            CD.List_Card[i].parallax.isParallax = false;
            CD.List_Card[i].CG_gradient.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        targetCard = CD.List_Card[idx];
        targetBox = CD.List_target[idx];
        startPosY = CD.List_openpos[idx];
        Myindex = idx;
        targetCard.CG_shadow.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);


        // -------------- b1 -----------------------------------------------------------------------------------------------
        if (idx != 0)
        {
            tweenTime = 0.15f;
            CD.smoothcontainer.DOAnchorPosY(CD.RT_Scrollcontainer.anchoredPosition.y, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            targetCard.RT.DOAnchorPos(new Vector2(0, startPosY + 363), 0.15f).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
            {
                CD.List_Card[Myindex].RT.SetAsLastSibling();
                targetCard.RT.DOScale(0.8f, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
                targetCard.RT.DOAnchorPos(new Vector2(0, startPosY + 200), 0.15f).SetEase(Model.Inst.Ease_Out_100);
            });
        }
        else
        {
            tweenTime = 0.2f;
            CD.smoothcontainer.DOAnchorPosY(CD.RT_Scrollcontainer.anchoredPosition.y, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            targetCard.RT.DOAnchorPos(new Vector2(0, startPosY + 200), 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
            CD.List_Card[Myindex].RT.SetAsLastSibling();
            targetCard.RT.DOScale(0.8f, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
        }

        cardPositioning();
    }
    // 카드 드래그가 끝나고 난뒤 targetcard의 list안에서 순서를 바꿔주고 모든 카드들의 onclickEvent를 다시 부여해준다.
    // 그리고 targetcard는 아무것도 안들어있게하고 editmode는 거짓으로 바뀐다.
    public void EndDrag()
    {
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].RT.DOKill();
        }
        targetCard.CG_shadow.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        targetCard.RT.DOScale(1, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);

        Sprite sample = M.ListAllSprite[Myindex];
        M.ListAllSprite.RemoveAt(Myindex);
        M.ListAllSprite.Insert(Myindex + BarAmount, sample);

        CD.List_Card.RemoveAt(Myindex);
        CD.List_target.RemoveAt(Myindex);
        CD.List_Card.Insert(Myindex + BarAmount, targetCard);
        CD.List_target.Insert(Myindex + BarAmount, targetBox);
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].RT.transform.SetParent(transform);
        }
        CD.smoothcontainer.anchoredPosition = Vector2.zero;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            int my = i;
            float smoothSpeed = 0.3f - (0.23f / CD.List_target.Count * i);
            CD.List_Card[i].RT.transform.SetParent(transform.GetChild(1));
            CD.List_target[i].anchoredPosition = new Vector2(0, CD.List_openpos[i]);
            CD.List_Card[i].parallax.Target = CD.List_target[i];
            CD.List_Card[i].parallax.smoothSpeed = smoothSpeed;
            CD.List_Card[i].Idx = my;
            CD.List_Card[i].LongPress.onLongPress.RemoveAllListeners();
            CD.List_Card[i].LongPress.onEndLongPress.RemoveAllListeners();

            CD.List_Card[i].BT.onClick.RemoveAllListeners();
            CD.List_Card[i].RT.SetAsFirstSibling();
            CD.List_Card[i].BT.onClick.AddListener(() =>
            {
                CD.Myindex = my;
                if (!CD.istweeening && !CD.isdetailopen && CD.MyStatus != Status.CardEditMode)
                {
                    CD.OnCardClick();
                }
            });
            CD.List_Card[i].RT.transform.DOMove(CD.List_Card[i].parallax.Target.position, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            if (CD.List_Card[i].RT.tag != "Plcc")
            {
                CD.List_Card[i].LongPress.onLongPress.AddListener(() => StartDrag(my));
                CD.List_Card[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
            }

            if (i != 0)
            {
                if (i == CD.List_Card.Count - 1)
                    CD.List_Card[i].CG_gradient.DOFade(1, 0.6f).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
                    {
                        if (CD.isopen)
                        {
                            for (var y = 0; y < CD.List_Card.Count; y++)
                            {
                                CD.List_Card[y].parallax.isParallax = true;
                            }
                        }
                    });
                else
                    CD.List_Card[i].CG_gradient.DOFade(1, 0.6f).SetEase(Model.Inst.Ease_InOut20_100);
            }
        }
        LPE.EndDrag2(Myindex, Myindex + BarAmount);
        BarAmount = 0;
        targetCard = null;
        targetBox = null;
        // isEditMode = false; //carddrag에서 스냅처리때문에 거기서 호출이 된다.
        CD.MyStatus = Status.CardListOpen;
    }
}