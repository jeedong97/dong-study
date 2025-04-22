using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TabBtnController : MonoBehaviour
{
    [SerializeField] CardDrag cardDrag;
    [SerializeField] List<CanvasGroup> list_QuickAccess;
    [SerializeField] List<CanvasGroup> list_Menu;
    [SerializeField] Button Menu, QuickAccess, pay, account, membership;
    [SerializeField] Button bt_pay, bt_account, bt_membership, bt_back;
    [SerializeField] RectTransform Underline, menuPageScroll;
    [SerializeField] CanvasGroup Title, MenuPage, payPage, accountPage, membershipPage;
    [SerializeField] ScrollRect sc_menu;
    [SerializeField] List<ScrollRect> List_Sc;
    [SerializeField] List<RectTransform> scrolls;

    void Start()
    {
        Menu.onClick.AddListener(MenuEvent);
        QuickAccess.onClick.AddListener(QuickAccessEvent);
        bt_pay.onClick.AddListener(goPaypage);
        bt_account.onClick.AddListener(goAccountPage);
        bt_membership.onClick.AddListener(goMembershipPage);
        bt_back.onClick.AddListener(BackButton);
        bt_back.gameObject.SetActive(false);
    }
    public void QuickAccessEvent() // 메뉴가 내려간다
    {
        if (cardDrag.MyStatus == Status.MenuPage)
        {
            for (int i = 0; i < 2; i++)
            {
                list_QuickAccess[i].DOFade(1, 0.3f);
                list_Menu[i].DOFade(0, 0.3f);
            }

            Underline.DOAnchorPosX(-250, 0.3f).SetEase(Ease.InOutQuart);
            Underline.DOSizeDelta(new Vector2(264, 6), 0.3f).SetEase(Ease.InOutQuart);

            MenuPage.DOFade(0, 0.2f).OnComplete(() =>
            {
                sc_menu.enabled = false;
                menuPageScroll.anchoredPosition = Vector2.zero;
                sc_menu.enabled = true;
            });
            MenuPage.blocksRaycasts = false;

            cardDrag.MemberCoupon.CG.DOFade(1, 0.2f);
            string _name = cardDrag.List_Card[cardDrag.Myindex].Img.sprite.name;
            if (_name != "Vaccine" && _name != "BoardingPass" && _name != "Ticket1" && _name != "Ticket2" && _name != "MembershipCard")
                cardDrag.CG_pin.DOFade(1, 0.2f);
            else if (_name == "Vaccine" || _name == "BoardingPass" || _name == "MembershipCard")
            {
                cardDrag.CG_ShowQR.DOFade(1, 0);
            }
            else if (_name == "Ticket1" || _name == "Ticket2")
            {
                cardDrag.CG_UseCard.DOFade(1, 0);
            }

            cardDrag.Img_BG.DOColor(cardDrag.ColorBG, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
            for (int i = 0; i < cardDrag.List_Card.Count; i++)
            {
                if (i < cardDrag.Myindex)
                {
                    cardDrag.List_Card[i].CG.DOFade(0.45f, 0.2f);
                }
                else
                {
                    cardDrag.List_Card[i].CG.DOFade(1, 0.2f);
                }
            }
            cardDrag.CG_SamsungTitle.DOFade(1, 0.2f);
            Title.DOFade(0, 0.2f);
            cardDrag.Benefit.RT.gameObject.SetActive(true);
            cardDrag.MyStatus = Status.CardListClose;
        }
    }
    public void Quick_QuickAcessEvent()
    {
        for (int i = 0; i < 2; i++)
        {
            list_QuickAccess[i].alpha = 1;
            list_Menu[i].alpha = 0;
        }

        Underline.DOAnchorPosX(-250, 0);
        Underline.DOSizeDelta(new Vector2(264, 6), 0);


        MenuPage.DOFade(0, 0).OnComplete(() =>
        {
            sc_menu.enabled = false;
            menuPageScroll.anchoredPosition = Vector2.zero;
            sc_menu.enabled = true;
        });
        MenuPage.blocksRaycasts = false;

        cardDrag.MemberCoupon.CG.DOFade(1, 0);
        string _name = cardDrag.List_Card[cardDrag.Myindex].Img.sprite.name;
        if (_name != "Vaccine" && _name != "BoardingPass" && _name != "Ticket1" && _name != "Ticket2" && _name != "MembershipCard")
            cardDrag.CG_pin.DOFade(1, 0);
        else if (_name == "Vaccine" || _name == "BoardingPass" || _name == "MembershipCard")
        {
            cardDrag.CG_ShowQR.DOFade(1, 0);
        }
        else if (_name == "Ticket1" || _name == "Ticket2")
        {
            cardDrag.CG_UseCard.DOFade(1, 0);
        }

        cardDrag.Img_BG.DOColor(cardDrag.ColorBG, 0).SetEase(Model.Inst.Ease_InOut20_100);
        for (int i = 0; i < cardDrag.List_Card.Count; i++)
        {
            if (i < cardDrag.Myindex)
            {
                cardDrag.List_Card[i].CG.DOFade(0.45f, 0);
            }
            else
            {
                cardDrag.List_Card[i].CG.DOFade(1, 0);
            }
        }
        cardDrag.CG_SamsungTitle.DOFade(1, 0);
        Title.DOFade(0, 0);
        cardDrag.Benefit.RT.gameObject.SetActive(true);
    }
    void MenuEvent() // 메뉴가 올라온다
    {
        if (cardDrag.MyStatus == Status.CardListClose)
        {
            for (int i = 0; i < 2; i++)
            {
                list_QuickAccess[i].DOFade(0, 0.3f);
                list_Menu[i].DOFade(1, 0.3f);
            }

            Underline.DOAnchorPosX(242, 0.3f).SetEase(Ease.InOutQuart);
            Underline.DOSizeDelta(new Vector2(108, 6), 0.3f).SetEase(Ease.InOutQuart);

            MenuPage.DOFade(1, 0.2f);
            MenuPage.blocksRaycasts = true;

            cardDrag.Img_BG.DOColor(cardDrag.ColorDetailBG, 0.3f).SetEase(Ease.InOutQuart);
            cardDrag.MemberCoupon.CG.DOFade(0, 0.2f);
            cardDrag.CG_pin.DOFade(0, 0.2f);
            cardDrag.CG_ShowQR.DOFade(0, 0.2f);
            cardDrag.CG_UseCard.DOFade(0, 0.2f);

            for (int i = 0; i < cardDrag.List_Card.Count; i++)
            {
                if (i != cardDrag.Myindex)
                {
                    cardDrag.List_Card[i].CG.alpha = 0;
                }
                else
                {
                    cardDrag.List_Card[i].CG.DOFade(0, 0.2f);
                }
            }

            Title.DOFade(1, 0.2f);
            cardDrag.CG_SamsungTitle.DOFade(0, 0.2f);
            cardDrag.Benefit.RT.gameObject.SetActive(false);
            cardDrag.MyStatus = Status.MenuPage;
        }
    }
    void goPaypage()
    {
        menuPageScroll.DOAnchorPosY(0, 0.3f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.RT.DOAnchorPosY(-354, 0.2f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.CG.DOFade(0, 0.2f);
        cardDrag.BotButtons.CG.blocksRaycasts = false;
        bt_back.gameObject.SetActive(true);
        payPage.DOFade(1, 0.2f);
        payPage.blocksRaycasts = true;
        cardDrag.MyStatus = Status.MenuDepth2;
    }
    void goAccountPage()
    {
        menuPageScroll.DOAnchorPosY(0, 0.3f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.RT.DOAnchorPosY(-354, 0.2f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.CG.DOFade(0, 0.2f);
        bt_back.gameObject.SetActive(true);
        accountPage.DOFade(1, 0.2f);
        accountPage.blocksRaycasts = true;
        cardDrag.BotButtons.CG.blocksRaycasts = false;
        cardDrag.MyStatus = Status.MenuDepth2;
    }
    void goMembershipPage()
    {
        menuPageScroll.DOAnchorPosY(0, 0.3f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.RT.DOAnchorPosY(-354, 0.2f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.CG.DOFade(0, 0.2f);
        bt_back.gameObject.SetActive(true);
        membershipPage.DOFade(1, 0.2f);
        membershipPage.blocksRaycasts = true;
        cardDrag.BotButtons.CG.blocksRaycasts = false;
        cardDrag.MyStatus = Status.MenuDepth2;
    }

    public void BackButton()
    {
        for (int i = 0; i < scrolls.Count; i++)
        {
            scrolls[i].anchoredPosition = Vector2.zero;
        }
        cardDrag.BotButtons.RT.DOAnchorPosY(0, 0.2f).SetEase(Ease.InOutQuart);
        cardDrag.BotButtons.CG.DOFade(1, 0.2f).OnComplete(() =>
        {
            for (int i = 0; i < scrolls.Count; i++)
            {
                List_Sc[i].enabled = false;
                scrolls[i].anchoredPosition = Vector2.zero;
                List_Sc[i].enabled = true;
            }
        });
        cardDrag.BotButtons.CG.blocksRaycasts = true;
        bt_back.gameObject.SetActive(false);
        payPage.DOFade(0, 0.2f);
        payPage.blocksRaycasts = false;
        accountPage.DOFade(0, 0.2f);
        accountPage.blocksRaycasts = false;
        membershipPage.DOFade(0, 0.2f);
        membershipPage.blocksRaycasts = false;
        cardDrag.MyStatus = Status.MenuPage;
    }
}
