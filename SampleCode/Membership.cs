using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.GlassImage;

[Serializable]
public class CardItem
{
    public RectTransform RT;
    public CanvasGroup CG;
    public Button BT;
}
[Serializable]
public class CrossItem
{
    public RectTransform RT;
    public CanvasGroup CG;
    public Button BT;
    public Image Img;
}
public class Membership : MonoBehaviour
{
    public CardDrag CD;
    public PinEvent PE;
    [SerializeField] Button BT_MembershipBtnManager;
    public GameObject Cross;
    [SerializeField] CrossItem Item_Cross;

    public RectTransform RT_shadow, RT_memAnCou, RT_Arrow;
    public CanvasGroup CG_shadow, CG_Membership_BlackDim;
    public Button BT_Member, BT_Arrow;
    public float Duration;
    public float Fadeamount;
    public float nowtime;
    public bool IsBlur, IsListClose, IsOpen;
    public int MyIndex;
    public List<GameObject> List_item;
    public List<CardItem> List_Card;
    public List<float> List_mypos;
    public Image IM_Member, IM_Coupon;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        BT_Member.onClick.AddListener(() =>
        {
            if (!CD.istweeening && !CD.isopen)
            {
                mebershipPageOnOff();
            }
            else if (PE.isPinOpen)
            {
                mebershipPageOnOff();
            }
        });
        BT_Arrow.onClick.AddListener(ArrowEvent);
        Item_Cross.BT.onClick.AddListener(() =>
        {
            CloseEvent();
        });

        buttonEvent();
    }

    // 멤버십 컨테이너가 나타나고 없어지게 하는 함수
    // 블러처리와 함께 실행이 된다.
    void mebershipPageOnOff()
    {
        IsOpen = true;
        if (!PE.isPinOpen)
        {
            IM_Coupon.color = new Color(37 / 255f, 37 / 255f, 37 / 255f, 0.5f);
            IM_Member.color = new Color(0, 0, 0, 1);

        }
        else
        {
            CG_Membership_BlackDim.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            IM_Coupon.color = new Color(1, 1, 1, 0.5f);
            IM_Member.color = new Color(1, 1, 1, 1);

        }


        if (IsBlur)
        {
            IsBlur = false;
            MyIndex = 0;
            CD.istweeening = false;
            for (int i = 0; i < List_Card.Count; i++)
            {
                List_Card[i].RT.DOAnchorPosY(List_mypos[i] + 2400, Duration + i * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
            }
            RT_Arrow.DOAnchorPosY(-616f + 2400, Duration).SetEase(Model.Inst.Ease_InOut20_100);
            RT_memAnCou.DOAnchorPosY(-581 + 2400, Duration).SetEase(Model.Inst.Ease_InOut20_100);

            if (PE.isPinOpen)
                PE.AbleToCount = true;
        }
        else
        {
            IsBlur = true;
            int Count = List_Card.Count - 1;
            for (int i = 0; i < List_Card.Count; i++)
            {
                if (i > 0)
                {
                    List_Card[i].RT.DOAnchorPosY(List_mypos[i], Duration + (Count - 1) * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
                }
                else
                {
                    List_Card[i].RT.DOAnchorPosY(List_mypos[i], Duration + Count * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
                    RT_Arrow.DOAnchorPosY(-616f, Duration + Count * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
                    RT_memAnCou.DOAnchorPosY(-581, Duration + Count * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
                }
                Count--;
            }
            Item_Cross.RT.anchoredPosition = new Vector2(918, -153);
            CD.MemberCoupon.CG.blocksRaycasts = true;
            Item_Cross.CG.blocksRaycasts = true;
            // CD.istweeening = true;
            if (PE.isPinOpen)
                PE.AbleToCount = false;
            CD.MyStatus = Status.CouponListOpen;
        }

        CD.MemberCoupon.CG.alpha = 0;
    }
    public CanvasGroup CG_SamsungLogo;

    // 멤버십 컨테이너에 화살표를 눌렀을 때 발생하는 이벤트
    public void ArrowEvent()
    {
        if (!IsListClose)
        {
            CG_SamsungLogo.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
            ListClose();
            if (!PE.isPinOpen)
            {
                IM_Coupon.color = new Color(37 / 255f, 37 / 255f, 37 / 255f, 0.5f);
                IM_Member.color = new Color(0, 0, 0, 1);
            }
            else
            {
                IM_Coupon.color = new Color(1, 1, 1, 0.5f);
                IM_Member.color = new Color(1, 1, 1, 1);
            }
        }
        else
        {
            CG_SamsungLogo.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
            listOpen();
            if (!PE.isPinOpen)
            {
                IM_Coupon.color = new Color(37 / 255f, 37 / 255f, 37 / 255f, 0.5f);
                IM_Member.color = new Color(0, 0, 0, 1);
            }
            else
            {
                IM_Coupon.color = new Color(1, 1, 1, 0.5f);
                IM_Member.color = new Color(1, 1, 1, 1);
            }
        }
    }
    // isOpen = false 일때 멤버십 리스트가 열리게해준다
    void listOpen()
    {
        print("Open");
        CD.MyStatus = Status.CouponListOpen;
        DOTween.Kill(1);
        if (PE.isPinOpen)
        {
            CG_Membership_BlackDim.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        int Count = List_Card.Count - 1;
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i > 0)
            {
                List_Card[i].RT.DOAnchorPosY(List_mypos[i], Duration + (Count - 1) * 0.02f).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            }
            else
            {
                RT_memAnCou.DOAnchorPosY(-581, Duration + Count * 0.02f).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            }
            Count--;
        }
        RT_Arrow.DORotate(new Vector3(0, 0, 0), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        if (!PE.isPinOpen)
        {
            // CD.RT_advertise.DOSizeDelta(new Vector2(1080, 567), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            CD.CG_SamsungTitle.DOFade(1,Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            
        }
        BT_MembershipBtnManager.enabled = false;

        Item_Cross.CG.DOFade(0, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        RT_Arrow.transform.SetAsLastSibling();
        nowtime = 0;
        IsBlur = true;
        IsListClose = false;
        if (PE.isPinOpen)
            PE.AbleToCount = false;
    }
    // isOpen = true 일때 멤버십 리스트가 닫히게
    public void ListClose()
    {
        DOTween.Kill(1);
        CG_Membership_BlackDim.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i > 0)
            {
                List_Card[i].RT.DOAnchorPosY(List_mypos[0], Duration + i * 0.02f).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            }
            else
            {
                List_Card[i].RT.transform.SetAsLastSibling();
                RT_memAnCou.DOAnchorPosY(List_mypos[0], Duration + i * 0.02f).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
            }
        }
        RT_Arrow.DORotate(new Vector3(0, 0, -180), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        RT_Arrow.transform.SetAsLastSibling();
        Item_Cross.RT.transform.SetAsLastSibling();
        // CD.RT_advertise.DOSizeDelta(new Vector2(1080, 0), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        CD.CG_SamsungTitle.DOFade(0,Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        Item_Cross.CG.DOFade(1, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);

        CD.MemberCoupon.CG.DOFade(0, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        CD.MemberCoupon.CG.blocksRaycasts = false;
        CD.MemberCoupon.CG.interactable = false;
        BT_MembershipBtnManager.enabled = false;

        nowtime = Duration;
        Item_Cross.CG.blocksRaycasts = true;

        IsBlur = false;
        IsListClose = true;

        isbuttonanimating = false;
        if (PE.isPinOpen)
        {
            PE.AbleToCount = true;
        }
        if (IsOpen)
        {
            CG_SamsungLogo.DOKill();
            CG_SamsungLogo.DOFade(0, 0.3f);
        }
    }

    //멤버십 버튼은 닫아주는 함수다 x에 붙어있다.
    public void CloseEvent()
    {
        IsOpen = false;
        CG_Membership_BlackDim.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Item_Cross.CG.DOFade(0, Duration).SetEase(Model.Inst.Ease_InOut20_100);
        Item_Cross.RT.DOAnchorPosY(-153 + 2400, Duration).SetEase(Model.Inst.Ease_InOut20_100);
        Item_Cross.CG.blocksRaycasts = false;
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.DOAnchorPosY(List_mypos[0] + 2400, Duration + i * 0.02f).SetEase(Model.Inst.Ease_InOut20_100);
        }
        if (!PE.isPinOpen)
        {
            // CD.RT_advertise.DOSizeDelta(new Vector2(1080, 567), Duration).SetEase(Model.Inst.Ease_InOut20_100);
            CD.CG_SamsungTitle.DOFade(1,Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(1);
        }
        else
        {
            CD.MyStatus = Status.PayMode;
        }
        RT_Arrow.DOAnchorPosY(-616f + 2400, Duration).SetEase(Model.Inst.Ease_InOut20_100);
        RT_memAnCou.DOAnchorPosY(-581 + 2400, Duration).SetEase(Model.Inst.Ease_InOut20_100);
        RT_Arrow.DORotate(new Vector3(0, 0, 0), Duration).SetEase(Model.Inst.Ease_InOut20_100);
        CD.MemberCoupon.CG.DOFade(1, Duration).SetEase(Model.Inst.Ease_InOut20_100);
        CD.MemberCoupon.CG.blocksRaycasts = true;
        CD.MemberCoupon.CG.interactable = true;
        BT_MembershipBtnManager.enabled = true;
        if (!PE.isPinOpen)
        {
            CD.istweeening = false;
        }
        else
            PE.AbleToCount = true;
        // else
        //     CD.istweeening = true;

        //CG_SamsungLogo.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);

        IsBlur = false;
        IsListClose = false;

        // print("closeBtn CLick");
        // print(CD.istweeening);
    }
    // 블러의 농도를 정해주는 함수다
    // for문을 통해 membershipcard들에게 속성을 부여해주는 함수다
    // 그 외 카드들이 있어야 하는 위치값도 정해주며 마지막으로 카드에 달려있는 
    // backtohome 버튼에 대한 속성들도 부여해준다.
    void Init()
    {
        for (int i = 0; i < List_item.Count; i++)
        {
            CardItem prefb;
            prefb = new CardItem
            {
                RT = List_item[i].GetComponent<RectTransform>(),
                CG = List_item[i].transform.GetChild(1).GetComponent<CanvasGroup>(),
                BT = List_item[i].GetComponent<Button>()
            };
            List_Card.Add(prefb);

            if (i == 0)
            {
                List_mypos.Add(-90);
            }
            else
            {
                List_mypos.Add(-693 - (i - 1) * 171);
            }
        }
        Item_Cross = new CrossItem
        {
            RT = Cross.GetComponent<RectTransform>(),
            CG = Cross.GetComponent<CanvasGroup>(),
            BT = Cross.transform.GetChild(0).transform.GetChild(0).GetComponent<Button>(),
            Img = Cross.GetComponent<Image>()
        };

    }
    bool isbuttonanimating = false;
    // 카드들에게 부여된 버튼을 눌렀을 때 발생할 이벤트들이다
    // 카드가 움직이고 있지 않는동안 작동하며 누른 카드의 순서를 알아낸다음 
    // 눌러진 카드와 첫번째(가장위에 있는 카드)의 위치를 바꿔주고 List안에서도 재정렬 해준다  
    //그리고 마지막을 카드들의 버튼 이벤트들을 전체에다가 다시 부여한다.
    void buttonEvent()
    {
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].BT.onClick.RemoveAllListeners();
            if (i != 0)
            {
                int my = i;
                List_Card[i].BT.onClick.AddListener(() =>
                {
                    if (!isbuttonanimating)
                    {
                        isbuttonanimating = true;
                        // CD.istweeening = true;
                        DOTween.Kill(0);
                        MyIndex = my;
                        List_Card[0].RT.transform.SetAsLastSibling();
                        RT_shadow.transform.SetAsLastSibling();
                        List_Card[MyIndex].RT.transform.SetAsLastSibling();
                        List_Card[MyIndex].CG.DOFade(0, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                        List_Card[MyIndex].CG.transform.GetComponent<RectTransform>().DOAnchorPosY(-69, 0.2f).SetEase(Ease.OutQuad);

                        RT_shadow.anchoredPosition = new Vector2(0, List_mypos[MyIndex] - 12);
                        RT_shadow.sizeDelta = new Vector2(870, 141);
                        CG_shadow.alpha = 0;

                        RT_shadow.DOAnchorPosY(List_mypos[MyIndex] + 111 - 12, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                        RT_shadow.DOSizeDelta(new Vector2(870, 444), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                        CG_shadow.DOFade(Fadeamount, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                        List_Card[MyIndex].RT.DOAnchorPosY(List_mypos[MyIndex] + 111, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                        List_Card[MyIndex].RT.DOSizeDelta(new Vector2(888, 444), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0).OnComplete(() =>
                        {
                            RT_shadow.DOAnchorPosY(List_mypos[0] - 12, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                            RT_shadow.DOSizeDelta(new Vector2(870, 101), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                            CG_shadow.DOFade(0, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);

                            List_Card[MyIndex].RT.DOAnchorPosY(List_mypos[0], Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                            List_Card[0].RT.DOSizeDelta(new Vector2(888, 141), Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                            List_Card[0].RT.DOAnchorPosY(List_mypos[MyIndex], Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);
                            List_Card[0].CG.DOFade(1, Duration).SetEase(Model.Inst.Ease_InOut20_100).SetId(0);

                            List_Card[0].CG.transform.GetComponent<RectTransform>().DOAnchorPosY(0, 0.2f).SetEase(Ease.OutQuad);

                            List_Card.Insert(0, List_Card[MyIndex]);
                            List_Card.RemoveAt(MyIndex + 1);
                            List_Card.Insert(MyIndex + 1, List_Card[1]);
                            List_Card.RemoveAt(1);
                            ListClose();
                            buttonEvent();
                        });
                    }
                });
            }
        }
    }


    void Update()
    {
        
    }
}
