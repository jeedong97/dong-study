using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BotBackkeyHandler : MonoBehaviour
{
    [SerializeField] Button Btn_Back;
    public PinEvent PinEvent;
    public Membership_new_Handler Membership;
    public CardDrag CardDrag;
    public EditNBackController editNBackController;
    public TabBtnController tabBtnController;
    public PlusButtonHandler plusButtonHandler;
    public MembershipDetailSwipe membershipDetailSwipe;

    void Start()
    {
        Btn_Back.onClick.AddListener(onClickBackButton);
    }
    //CardDrag에서 지정한 enum에 따라 back버튼은 눌렀을 때 이벤트가 달라진다.
    void onClickBackButton()
    {
        if (CardDrag.MyStatus == Status.CardListOpen)
        {
            CardDrag.OnCardClick();
        }
        else if (CardDrag.MyStatus == Status.KeyPad)
        {
            PinEvent.DisappearKeyPad();
        }
        else if (CardDrag.MyStatus == Status.PayMode)
        {
            PinEvent.BacktoHome();
        }
        else if (CardDrag.MyStatus == Status.CouponListOpen)
        {
            Membership.MembershipClose();
            if (PinEvent.isPinOpen)
            {
                CardDrag.MyStatus = Status.PayMode;
                PinEvent.AbleToCount = true;
            }
            else
            {
                CardDrag.MyStatus = Status.CardListClose;
            }
        }
        else if (CardDrag.MyStatus == Status.CardDetail)
        {
            CardDrag.OutDetail();
        }
        else if(CardDrag.MyStatus == Status.SpareDetail)
        {
            CardDrag.outSpareDetail();
        }
        else if(CardDrag.MyStatus == Status.EditPage)
        {
            editNBackController.ClickBackBtn();
            Debug.Log("hello");
        }
        else if(CardDrag.MyStatus == Status.MenuPage)
        {
            tabBtnController.QuickAccessEvent();
        }
        else if(CardDrag.MyStatus == Status.MenuDepth2)
        {
            tabBtnController.BackButton();
        }
        else if(CardDrag.MyStatus == Status.PlusPage)
        {
            plusButtonHandler.BackPlusPage();
        }
        else if(CardDrag.MyStatus == Status.Coibase)
        {
            CardDrag.OutCoinBase();
        }
        else if(CardDrag.MyStatus == Status.membershipDetail)
        {
            membershipDetailSwipe.OutMeberShipDetail();
        }
    }
}
