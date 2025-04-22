using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;



public class EditNBackController : MonoBehaviour
{
    public CardDrag CardDrag;
    public CanvasGroup CG_BottomTab;
    public RectTransform RT_ScrollContents, RT_EditPage, RT_MainPage;
    public float duration;
    public AnimationCurve Ease_InOut20_100;
    public Sprite[] SR_Bolds, SR_Regular;
    public Image[] IM_Titles;
    public RectTransform RT_MainPageScroll;
    public Button BT_back, BT_cancel, BT_Done; //(1)
    public Button BT_MembershipBtn;
    public LongPressNEditHandler LPE;

    void Start()
    {
        duration = 0.3f;
        IM_Titles[0].sprite = SR_Bolds[0];
        IM_Titles[1].sprite = SR_Regular[1];
        IM_Titles[2].sprite = SR_Regular[2];
        BT_cancel.onClick.AddListener(ClickBackBtn);
        BT_back.onClick.AddListener(ClickBackBtn);
        BT_Done.onClick.AddListener(ClickDoneBackBtn);
    }


    //에딧모드로 가기위한 함수
    public void ClickEditBtn()
    {
        int count = 0;
        for (int i = 0; i < LPE.List_EI.Count; i++)
        {
            if (!LPE.List_EI[i].Toggle.isOn)
                count++;
        }
        LPE.tempCount = count;
        LPE.MemorytheArray();
        duration = 0.5f;
        BT_back.gameObject.SetActive(true);
        RT_EditPage.DOAnchorPosX(0, duration).SetEase(Ease_InOut20_100);
        RT_MainPage.DOAnchorPosX(-1080.5f, duration).SetEase(Ease_InOut20_100).OnComplete(() =>
        {
            CardDrag.BotButtons.CG.DOKill();
            CardDrag.BotButtons.CG.blocksRaycasts = false;
            RT_MainPageScroll.DOAnchorPosY(0, 0);
        });
        Init();
        CG_BottomTab.interactable = false;
        CG_BottomTab.blocksRaycasts = false;
        BT_MembershipBtn.enabled = false;
        CardDrag.MyStatus = Status.EditPage;
    }
    //에딧모드에서 나오기 위한 함수
    public void ClickBackBtn()
    {
        LPE.ResetEditItemArray();
        LPE.ListResortCard.Clear();
        BT_back.gameObject.SetActive(false);
        duration = 0.5f;
        RT_EditPage.DOAnchorPosX(1080.5f, duration).SetEase(Ease_InOut20_100);
        RT_MainPage.DOAnchorPosX(0, duration).SetEase(Ease_InOut20_100).OnComplete(() =>
        {
            Init();
            for (var i = 0; i < CardDrag.List_Card.Count; i++)
            {
                CardDrag.List_Card[i].parallax.isParallax = true;
            }
        });
        BT_MembershipBtn.enabled = true;
        CG_BottomTab.interactable = true;
        // CG_BottomTab.blocksRaycasts = true;
        CardDrag.MyStatus = Status.CardListOpen;
        if (CardDrag.MB.IsOpen)
        {
            CardDrag.RT_MembershipHolder.DOAnchorPosY(0, duration).SetEase(Ease_InOut20_100);
        }
        LPE.tempCount = 0;
    }
    public void ClickDoneBackBtn()
    {
        LPE.List_TempSprites.Clear();
        LPE.List_TempEI.Clear();

        CardDrag.ReCreateCard();
        LPE.ListResortCard.Clear();
        BT_back.gameObject.SetActive(false);
        duration = 0.5f;
        RT_EditPage.DOAnchorPosX(1080.5f, duration).SetEase(Ease_InOut20_100);
        RT_MainPage.DOAnchorPosX(0, duration).SetEase(Ease_InOut20_100).OnComplete(() =>
        {
            Init();
            for (var i = 0; i < CardDrag.List_Card.Count; i++)
            {
                CardDrag.List_Card[i].parallax.isParallax = true;
            }
        });
        BT_MembershipBtn.enabled = true;
        CG_BottomTab.interactable = true;
        // CG_BottomTab.blocksRaycasts = true;

        CardDrag.MyStatus = Status.CardListOpen;
        if (CardDrag.MB.IsOpen)
        {
            CardDrag.RT_MembershipHolder.DOAnchorPosY(0, duration).SetEase(Ease_InOut20_100);
        }
        LPE.tempCount = 0;
    }
    public void Init()
    {
        RT_ScrollContents.DOAnchorPosY(0, 0);
    }

    void Update()
    {

    }
}
