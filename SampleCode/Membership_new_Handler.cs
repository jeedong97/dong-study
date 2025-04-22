using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class MemebershipCardItem
{
    public CanvasGroup CG_SelectedCard;
    public Button BT_Card;
}

public class Membership_new_Handler : MonoBehaviour
{
    [SerializeField] Button btn_MembershipBtn;
    public AnimationCurve Ease_InOut20_100;
    [SerializeField] float duration;
    public List<MemebershipCardItem> List_MemebershipCardItem;
    public List<RectTransform> Objects;
    public List<float> List_mypos;
    public bool IsOpen = false;
    public CardDrag CD;
    public PinEvent PE;
    private int MyIndex;
    public CanvasGroup CG_pinModeLogo;
    [SerializeField] Image IM_ExpandCard;
    [SerializeField] List<Sprite> List_ExpandCards;
    [SerializeField] RectTransform RT_Contents;
    [SerializeField] CanvasGroup CG_Memberbt, CG_Indicator;
    [SerializeField] Button btn_Close, btn_ExpandCard;
    [SerializeField] Sprite temp_sprite = null;
    [SerializeField] MembershipDetailSwipe memberDetailSwipe;

    void Start()
    {
        btn_MembershipBtn.onClick.AddListener(() =>
        {
            MembershipControl();
        }
        );
        for (int i = 0; i < Objects.Count; i++)
            Objects[i].DOAnchorPosY(List_mypos[i] + 2400, 0);

        MyIndex = 0;
        for (int i = 0; i < List_MemebershipCardItem.Count; i++)
        {
            int CardNum = i;
            List_MemebershipCardItem[i].BT_Card.onClick.AddListener(() => ButtonClick(CardNum));
        }
        btn_Close.onClick.AddListener(MembershipClose);

        btn_ExpandCard.onClick.AddListener(() =>
        {
            if (temp_sprite.name == "membership_CU")
            {
                GoToMebershipDetail();
            }
        });
    }

    int Count;
    public float DelayDuration;
    void MembershipControl()
    {
        Count = Objects.Count - 1;

        if (!IsOpen)
        {
            MembershipOpen();
        }
        else
        {
            MembershipClose();
        }
    }
    void GoToMebershipDetail()
    {
        memberDetailSwipe.GoToMebershipDetail();
    }
    void MembershipOpen()
    {
        for (int i = 0; i < Objects.Count; i++)
            Objects[i].DOAnchorPosY(List_mypos[i], (duration + Count * 0.02f)).SetEase(Ease_InOut20_100);
        if (PE.isPinOpen)
        {
            CG_pinModeLogo.DOFade(0, duration).SetEase(Ease_InOut20_100);
            RT_Contents.DOAnchorPosX(-205, 0.5f).SetEase(Ease_InOut20_100).SetDelay(DelayDuration);
        }
        else
        {
            CD.CG_SamsungTitle.DOFade(0, duration).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
            // CD.RT_advertise.DOSizeDelta(new Vector2(1080, 0), duration).SetEase(Ease_InOut20_100).OnComplete(() =>
            {
                RT_Contents.DOAnchorPosX(-205, 0.5f).SetEase(Ease_InOut20_100).SetDelay(DelayDuration);
            });
        }
        float closepos;
        CD.List_closepos.Clear();
        CD.BeneOpen = -1253;
        CD.CardSize = 0.85f;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            if (i >= 3)
            {
                closepos = 1002;
            }
            else
            {
                closepos = 1002f + i * 15;
            }
            CD.List_closepos.Add(closepos);
        }
        if (!PE.isPinOpen)
            goListClosePos();
        else
        {
            for (int i = CD.Myindex; i < CD.List_Card.Count; i++)
            {
                CD.List_Card[i].RT.DOScale(Vector2.one, duration).SetEase(Ease.InOutQuart);
                CD.List_Card[i].RT.DOAnchorPosY(784, duration).SetEase(Ease.InOutQuart);
            }
            CD.Benefit.RT.DOScale(CD.CardSize, duration).SetEase(Ease_InOut20_100);
        }

        CG_Indicator.DOFade(0, duration).SetEase(Ease_InOut20_100);
        CG_Memberbt.DOFade(0, duration).SetEase(Ease_InOut20_100);
        btn_MembershipBtn.interactable = false;
        ButtonClick(MyIndex);
        CD.MyStatus = Status.CouponListOpen;
        IsOpen = true;
    }

    public void MembershipClose()
    {
        for (int i = 0; i < Objects.Count; i++)
            Objects[i].DOAnchorPosY(List_mypos[i] + 2400, duration + Count * 0.02f).SetEase(Ease_InOut20_100);
        if (PE.isPinOpen)
        {
            CG_pinModeLogo.DOFade(1, duration).SetEase(Ease_InOut20_100);
            RT_Contents.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            CD.CG_SamsungTitle.DOFade(1, duration).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
            // CD.RT_advertise.DOSizeDelta(new Vector2(1080, 567), duration).SetEase(Ease_InOut20_100).OnComplete(() =>
            {
                RT_Contents.anchoredPosition = new Vector2(0, 0);
            });
        }
        float closepos;
        CD.List_closepos.Clear();
        CD.BeneOpen = -1318;
        CD.CardSize = 1;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            if (i >= 3)
            {
                closepos = 972;
            }
            else
            {
                closepos = 972 + i * 15;
            }
            CD.List_closepos.Add(closepos);
        }
        if (!PE.isPinOpen)
            goListClosePos();
        else
        {
            for (int i = CD.Myindex; i < CD.List_Card.Count; i++)
            {
                CD.List_Card[i].RT.DOScale(new Vector2(1.115f, 1.115f), duration).SetEase(Ease.InOutQuart);
                CD.List_Card[i].RT.DOAnchorPosY(852, duration).SetEase(Ease.InOutQuart);
            }
            CD.Benefit.RT.DOScale(CD.CardSize, duration).SetEase(Ease_InOut20_100);
        }
        CG_Indicator.DOFade(1, duration).SetEase(Ease_InOut20_100);
        CG_Memberbt.DOFade(1, duration).SetEase(Ease_InOut20_100);
        btn_MembershipBtn.interactable = true;
        IsOpen = false;
        CD.MyStatus = Status.CardListClose;
    }

    void goListClosePos()
    {
        int count = 0;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].RT.DOAnchorPosY(CD.List_closepos[count], duration).SetEase(Ease_InOut20_100);
            if (i >= CD.Myindex)
                count++;
        }
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            if (CD.Myindex <= i)
            {
                CD.List_Card[i].RT.DOScale(CD.CardSize, duration).SetEase(Ease_InOut20_100);

            }
            else
                CD.List_Card[i].RT.DOScale(0.8f, duration).SetEase(Ease_InOut20_100);
        }
        string _name = CD.List_Card[CD.Myindex].Img.sprite.name;
        if (_name == "Genesis" || _name == "Samsung" || _name == "Yale" || _name == "Sky" || _name == "RedVisa" || _name == "DigitalAsset")
        {
            CD.Benefit.RT.DOAnchorPosY(CD.BeneOpen, duration).SetEase(Ease_InOut20_100);
        }
        else
        {
            CD.Benefit.RT.DOAnchorPosY(CD.BeneClose, duration).SetEase(Ease_InOut20_100);
        }
        CD.Benefit.RT.DOScale(CD.CardSize, duration).SetEase(Ease_InOut20_100);
    }

    void ButtonClick(int CardNum)
    {
        for (int i = 0; i < List_MemebershipCardItem.Count; i++)
        {
            if (i != CardNum)
            {
                List_MemebershipCardItem[i].CG_SelectedCard.DOFade(0, duration).SetEase(Ease_InOut20_100);
            }
            else
            {
                List_MemebershipCardItem[i].CG_SelectedCard.DOFade(1, duration).SetEase(Ease_InOut20_100);
                IM_ExpandCard.sprite = List_ExpandCards[i];
                temp_sprite = List_ExpandCards[i];
            }
        }
    }
}