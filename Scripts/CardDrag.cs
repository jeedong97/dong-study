using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class cardItem
{
    public RectTransform RT;

    public Button BT;
    public Image Img;
    public CanvasGroup CG;
    public int Idx;
    public CardLongPress LongPress;
    public CanvasGroup CG_shadow;
    public Image Img_Shadow;
    public RectTransform RT_Shadow;
    public CanvasGroup CG_gradient;
    public Parallax parallax;
}

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Status MyStatus = Status.CardListClose;
    public CardEditModeHandler cardEditMode;
    public Membership_new_Handler MB;
    public DetailSwipeDown detailSwipeDown;
    public EditNBackController editNBack;
    public float Movpos, Movposx;
    public GameObject GM_prefb, GM_TargetPrefb;
    public List<cardItem> List_Card;
    public List<RectTransform> List_target;
    //-----------------------------------------------------
    public List<float> List_openpos;
    public List<float> List_closepos;
    public TweenObject MemberCoupon, EditWallet, BotButtons, Benefit;
    public RectTransform RT_Scrollcontainer, RT_MembershipHolder;
    public CanvasGroup CG_SamsungTitle, CG_pin, CG_BTPin;
    public Parallax parallax_edit;
    public int Myindex;
    public bool istweeening, isopen, isDrag;
    public ScrollRect Scl;
    public Animator Ani_Controller;
    public bool isTouch, isWave;
    // Start is called before the first frame update

    void Start()
    {
        Application.targetFrameRate = 60;
        Init();
        setCard();
        Scl.enabled = false;
        BT_GoSpareDetail1.onClick.AddListener(goSpareDetail);
        BT_GoSpareDetail2.onClick.AddListener(goSpareDetail);
        BT_DimOutSpare.onClick.AddListener(outSpareDetail);
        BT_OutDetail.onClick.AddListener(OutDetail);
        setStartAnimationPosition();
        Invoke("startAnimation", 0.25f);
    }
    // 처음 시작할 때 카드들 애니메이션 시작 위치를 잡아준다.s
    void setStartAnimationPosition()
    {
        for (int i = 0; i < 3; i++)
        {
            List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[0] + i * 60);
        }
        MemberCoupon.CG.alpha = 0;
        CG_pin.alpha = 0;
    }
    // 처음 시작할때 카들의 애니메이션들의 약간의 시간차 후 실행되게 한다.
    void startAnimation()
    {
        istweeening = true;
        for (int i = 0; i < 3; i++)
        {
            List_Card[i].RT.DOAnchorPosY(List_closepos[i], 0.2f + (0.1f * i)).SetEase(Model.Inst.Ease_InOut20_100);
        }
        MemberCoupon.CG.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_Out_100).SetDelay(0.4f);
        CG_pin.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_Out_100).SetDelay(0.4f).OnComplete(() =>
        {
            istweeening = false;
        });
        BenefitApear(Myindex);
    }
    // 미리 만들어진 카드 게임 오브젝트를 복사하고 붙여넣어준다. 그리고 복사된 카드들이 있어야 하는 위치랑 뎁스랑 사이즈를 정해주고 미리 지정된 sprtie 리스트에 따라 sprite들을 넣어준다. 
    // 만들어진 카드에는 속성들이 있는데 RT, CG, BT, Img, Idx, LongPress 같은 속성들은 부여해준다.
    // 그 외 list open값이랑 list close 값을 지정해준다.
    void Init()
    {
        List_closepos = new List<float>();
        for (int i = 0; i < 6; i++)
        {
            GameObject prefb;
            prefb = Instantiate(GM_prefb);
            prefb.name = "Card" + i.ToString();
            prefb.transform.SetParent(this.transform.GetChild(1));
            prefb.transform.localScale = Vector3.one;
            prefb.transform.localPosition = Vector3.zero;
            prefb.SetActive(true);

            GameObject target;
            target = Instantiate(GM_TargetPrefb);
            target.name = "targetCard" + i.ToString();
            target.transform.SetParent(this.transform.GetChild(0));
            target.transform.localScale = Vector3.one;
            target.transform.localPosition = Vector3.zero;
            target.SetActive(true);

            cardItem item = null;

            item = new cardItem
            {
                RT = prefb.GetComponent<RectTransform>(),
                BT = prefb.GetComponent<Button>(),
                Img = prefb.transform.GetChild(1).GetComponent<Image>(),
                CG = prefb.GetComponent<CanvasGroup>(),
                Idx = i,
                LongPress = prefb.GetComponent<CardLongPress>(),
                CG_shadow = prefb.transform.GetChild(0).GetComponent<CanvasGroup>(),
                Img_Shadow = prefb.transform.GetChild(0).GetComponent<Image>(),
                RT_Shadow = prefb.transform.GetChild(0).GetComponent<RectTransform>(),
                CG_gradient = prefb.transform.GetChild(2).GetComponent<CanvasGroup>(),
                parallax = prefb.GetComponent<Parallax>()
            };

            List_target.Add(target.GetComponent<RectTransform>());
            List_Card.Add(item);
        }
    }
    // 처음 시작할 때 복제된 카드들에다가 미리 만들어놓은 List_sprite 에 있는 sprite들을 넣어준다. 그러면서 생성된 순서대로 카드드을 inspector 차에서 정렬시켜준다.
    void setCard()
    {
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].Img.sprite = Model.Inst.ListAllSprite[i];
        }
        SetPlccCard();
        SetCardPos();
        for (int i = 0; i < List_Card.Count; i++)
        {
            int my = i;
            List_Card[i].BT.onClick.AddListener(() =>
            {
                Myindex = my;
                if (!istweeening && !isdetailopen && MyStatus != Status.CardEditMode)
                {
                    OnCardClick();
                }
            });
            if (List_Card[i].RT.tag != "Plcc")
            {
                List_Card[i].LongPress.onLongPress.AddListener(() =>
                {
                    if (direction == "Ver")
                        cardEditMode.StartDrag(List_Card[my].Idx);
                });
                List_Card[i].LongPress.onEndLongPress.AddListener(() =>
                {
                    if (direction == "Ver")
                    {
                        cardEditMode.EndDrag();
                    }
                });
            }
        }
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[i]);
            List_Card[i].RT.transform.SetAsFirstSibling();
        }
        float smoothSpeed = 0;
        for (int i = 0; i < List_target.Count; i++)
        {
            smoothSpeed = 0.3f - (0.2f / (List_target.Count - 1) * i);
            List_target[i].anchoredPosition = new Vector2(0, List_openpos[i]);
            List_Card[i].parallax.Target = List_target[i];
            List_Card[i].parallax.smoothSpeed = smoothSpeed;
        }
        parallax_edit.smoothSpeed = 0.3f;
    }
    // -------------------------- Resorting Card ---------------------------------
    public List<Sprite> ListReSortCard;
    // 에딧 페이지에 들어간 후 버튼 이벤트들에 따라 "홈" 페이지에 카드들이 생성되고 파괴되는데 그러면서 카들들의 리스트들을 초기화 하고 새로 이벤트들을 부여하고 생성시켜준다.  
    // limitposition 이랑 limitposition2 의 값을 지정하기도 한다.
    public void ReCreateCard() // 재정렬 및 새로 생성
    {
        for (int i = 0; i < List_Card.Count; i++)
        {
            Destroy(List_Card[i].RT.gameObject);
            Destroy(List_target[i].gameObject);
        }
        List_Card.Clear();
        List_target.Clear();
        List_closepos.Clear();
        List_openpos.Clear();
        int count = 1;
        for (int i = 0; i < ListReSortCard.Count; i++)
        {
            GameObject prefb;
            prefb = Instantiate(GM_prefb);
            prefb.name = "Card" + i.ToString();
            prefb.transform.SetParent(this.transform.GetChild(1));
            prefb.transform.localScale = Vector3.one;
            prefb.transform.localPosition = Vector3.zero;
            prefb.SetActive(true);

            GameObject target;
            target = Instantiate(GM_TargetPrefb);
            target.name = "targetCard" + i.ToString();
            target.transform.SetParent(this.transform.GetChild(0));
            target.transform.localScale = Vector3.one;
            target.transform.localPosition = Vector3.zero;
            target.SetActive(true);

            cardItem item = null;

            item = new cardItem
            {
                RT = prefb.GetComponent<RectTransform>(),
                BT = prefb.GetComponent<Button>(),
                Img = prefb.transform.GetChild(1).GetComponent<Image>(),
                CG = prefb.GetComponent<CanvasGroup>(),
                Idx = i,
                LongPress = prefb.GetComponent<CardLongPress>(),
                CG_shadow = prefb.transform.GetChild(0).GetComponent<CanvasGroup>(),
                Img_Shadow = prefb.transform.GetChild(0).GetComponent<Image>(),
                RT_Shadow = prefb.transform.GetChild(0).GetComponent<RectTransform>(),
                CG_gradient = prefb.transform.GetChild(2).GetComponent<CanvasGroup>(),
                parallax = prefb.GetComponent<Parallax>()
            };
            List_Card.Add(item);
            List_target.Add(target.GetComponent<RectTransform>());

            if (List_Card.Count > 8)
            {
                RT_Scrollcontainer.sizeDelta = new Vector2(1080, 2077 + 188 * (count));
                LimitPosition = -186 * count;
                count++;
            }
            else
            {
                RT_Scrollcontainer.sizeDelta = new Vector2(1080, 2077);
                LimitPosition = 1;
            }
            List_Card[i].Img.sprite = ListReSortCard[i];
            //108.5 15
            //-124.5 224
        }
        //--------------followRT---------------
        SetPlccCard();
        SetCardPos();
        for (int i = 0; i < List_Card.Count; i++)
        {
            int my = i;
            List_Card[i].BT.onClick.AddListener(() =>
            {
                Myindex = my;
                if (!istweeening && !isdetailopen && MyStatus != Status.CardEditMode)
                {
                    OnCardClick();
                }
            });
            if (List_Card[i].RT.tag != "Plcc")
            {
                List_Card[i].LongPress.onLongPress.AddListener(() =>
               {
                   if (direction == "Ver")
                       cardEditMode.StartDrag(my);
               });
                List_Card[i].LongPress.onEndLongPress.AddListener(() =>
                {
                    if (direction == "Ver")
                    {
                        cardEditMode.EndDrag();
                    }
                });
            }

            List_Card[i].RT.anchoredPosition = new Vector2(0, List_openpos[i]);
            List_Card[i].RT.transform.SetAsFirstSibling();
        }
        float smoothSpeed = 0;
        for (int i = 0; i < List_target.Count; i++)
        {
            smoothSpeed = 0.3f - (0.2f / (List_target.Count - 1) * i);
            List_target[i].anchoredPosition = new Vector2(0, List_openpos[i]);
            List_Card[i].parallax.Target = List_target[i];
            List_Card[i].parallax.smoothSpeed = smoothSpeed;
        }
        parallax_edit.smoothSpeed = 0.3f;
    }

    public float distanceCard2Card;
    public void SetCardPos()
    {
        int count = 0;
        int LimitpositionCount = 1;
        float closepos;
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (!MB.IsOpen)
            {
                if (i >= 3)
                    closepos = 972f;
                else
                    closepos = 972f + i * 15;
            }
            else
            {
                if (i >= 3)
                    closepos = 1002;
                else
                    closepos = 1002 + i * 15;
            }
            List_closepos.Add(closepos);
        }
        if (List_Card.Count == 1)
        {
            List_openpos.Add(880);
        }
        else if (List_Card.Count == 2)
        {
            distanceCard2Card = 240;
            for (int a = 0; a < List_Card.Count; a++)
            {
                float _pos = 756f + a * distanceCard2Card;
                List_openpos.Add(_pos);
            }
        }
        else if (List_Card.Count == 3)
        {
            distanceCard2Card = 222;
            for (int a = 0; a < List_Card.Count; a++)
            {
                float _pos = 654f + a * distanceCard2Card;
                List_openpos.Add(_pos);
            }
        }
        else if (List_Card.Count == 4)
        {
            distanceCard2Card = 192;
            for (int a = 0; a < List_Card.Count; a++)
            {
                float _pos = 588f + a * distanceCard2Card;
                List_openpos.Add(_pos);
            }
        }
        else if (List_Card.Count == 5)
        {
            distanceCard2Card = 192;
            for (int a = 0; a < List_Card.Count; a++)
            {
                float _pos = 492 + a * distanceCard2Card;
                List_openpos.Add(_pos);
            }
        }
        else
        {
            for (int a = 0; a < List_Card.Count; a++)
            {
                distanceCard2Card = 192;
                float _pos = 429f + a * distanceCard2Card;
                List_openpos.Add(_pos);

                if (a > 7)
                    count++;
            }
        }

        if (List_Card.Count < 9)
        {
            LimitPosition = 1;
        }
        else
        {
            LimitPosition = -191;
            for (int a = 9; a < List_Card.Count; a++)
            {
                LimitPosition = -191 - 192 * LimitpositionCount;
                LimitpositionCount++;
            }
        }
        RT_Scrollcontainer.sizeDelta = new Vector2(1080, 2077 + 192 * count);
    }
    // ---------------------------------------------------------------------------
    [SerializeField] GameObject Plcc;
    public void SetPlccCard()
    {
        GameObject prefb;
        prefb = Instantiate(Plcc);
        prefb.name = "plcc";
        prefb.transform.SetParent(this.transform.GetChild(1));
        prefb.transform.localScale = Vector3.one;
        prefb.transform.localPosition = Vector3.zero;
        prefb.SetActive(true);

        GameObject target;
        target = Instantiate(GM_TargetPrefb);
        target.name = "targetCard" + List_target.Count.ToString();
        target.transform.SetParent(this.transform.GetChild(0));
        target.transform.localScale = Vector3.one;
        target.transform.localPosition = Vector3.zero;
        target.SetActive(true);

        cardItem item = null;
        item = new cardItem
        {
            RT = prefb.GetComponent<RectTransform>(),
            BT = prefb.GetComponent<Button>(),
            Img = prefb.transform.GetChild(1).GetComponent<Image>(),
            CG = prefb.GetComponent<CanvasGroup>(),
            Idx = List_Card.Count,
            LongPress = prefb.GetComponent<CardLongPress>(),
            CG_shadow = prefb.transform.GetChild(0).GetComponent<CanvasGroup>(),
            Img_Shadow = prefb.transform.GetChild(0).GetComponent<Image>(),
            RT_Shadow = prefb.transform.GetChild(0).GetComponent<RectTransform>(),
            CG_gradient = prefb.transform.GetChild(2).GetComponent<CanvasGroup>(),
            parallax = prefb.GetComponent<Parallax>()
        };
        List_target.Add(target.GetComponent<RectTransform>());
        List_Card.Insert(List_Card.Count, item);

        for (int i = List_Card.Count; i < List_Card.Count; i++)
        {
            List_Card[i].Idx += 1;
        }
    }
    // 카드를 클릭했을 때
    public void OnCardClick()
    {
        int index = List_Card.IndexOf(List_Card[Myindex]);
        if (!istweeening && !isdetailopen && !isDrag && MyStatus != Status.CardEditMode)
        {
            if (isopen) // 카드가 열려있을 경우
            {
                cardSelectedToClose();
            }
            else
            {
                if (List_Card[Myindex].Img.sprite.name != "ID" && List_Card[Myindex].RT.tag != "Plcc")
                {
                    gotoDetail();
                }
            }
        }
    }
    // 카드가 눌렸을 때 발생하는 이벤트, isopen = true 일때만 작동하게 되어있음
    // myindex가 0이거나 myindex가 0이 아닐때로 구분이 되어있다.
    void cardSelectedToClose()
    {
        for (var i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.transform.DOKill();
            List_Card[i].CG_gradient.DOKill();
            List_Card[i].CG_gradient.alpha = 0;
        }
        // Myindex = HorMyindex;
        string _name = List_Card[Myindex].Img.sprite.name;
        if (_name != "Vaccine" && _name != "BoardingPass" && _name != "MembershipCard" && _name != "Ticket")
        {
            CG_BTPin.blocksRaycasts = true;
        }
        else if (_name == "Vaccine" || _name == "BoardingPass" || _name == "MembershipCard")
        {
            CG_ShowQR.blocksRaycasts = true;
        }
        else if (_name == "Ticket")
        {
            CG_UseCard.blocksRaycasts = true;
        }
        MyStatus = Status.CardListClose;
        isopen = false;
        Scl.enabled = false;
        istweeening = true;
        EditWallet.CG.blocksRaycasts = false;
        CG_pin.blocksRaycasts = true;
        MemberCoupon.CG.blocksRaycasts = true;
        BotButtons.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        BotButtons.CG.blocksRaycasts = true;
        MemberCoupon.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        CG_Indicator.DOFade(1, 0.2f);
        EditWallet.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
        RT_Scrollcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        smoothcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.go.SetActive(false);
        BenefitApear(Myindex);
        if (!MB.IsOpen)
        {
            CG_SamsungTitle.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            plus.BT_Plus1.gameObject.SetActive(true);
        }
        else
        {
            for (int j = 0; j < MB.Objects.Count; j++)
            {
                MB.Objects[j].DOAnchorPosY(MB.List_mypos[j], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            }
            plus.BT_Plus1.gameObject.SetActive(false);
            MemberCoupon.CG.DOFade(0, tweenTime);
        }
        if (Myindex != 0)
        {
            parallexCardSelct();
        }
        else
        {
            List_Card[Myindex].RT.SetAsLastSibling();
            for (int i = 0; i < List_Card.Count; i++)
            {
                List_Card[i].RT.DOScale(new Vector2(CardSize, CardSize), tweenTime).SetEase(Model.Inst.Ease_Out_100);
                if (i == 0)
                {
                    List_Card[i].RT.DOAnchorPosY(List_closepos[i], tweenTime).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
                    {
                        istweeening = false;
                    });
                }
                else
                {
                    List_Card[i].RT.DOAnchorPosY(List_closepos[i], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
            }
        }
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].CG_gradient.alpha = 0;
            List_Card[i].parallax.isParallax = false;
        }
    }

    // parallex한 동작으로 카드가 움직이는 연출을 시켜준다. 
    // onclick 함수 안에 들어있고 myindex가 0이 아닐때 작동하게 되어있다.
    void parallexCardSelct()
    {
        // int count = 0;
        for (int i = 0; i < Myindex; i++)
        {
            List_Card[i].RT.transform.SetParent(this.transform);
            List_Card[i].RT.DOAnchorPos(new Vector2(horArrival, List_closepos[0]), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            List_Card[i].RT.DOScale(new Vector3(0.8f, 0.8f, 1), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            List_Card[i].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100); //SetDelay(count * 0.01f);

            if (i == Myindex - 1)
                List_Card[i].CG.DOFade(0.45f, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            else
                List_Card[i].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        int count1 = 0;
        for (int i = Myindex; i < List_Card.Count; i++)
        {
            List_Card[i].RT.DOScale(new Vector2(CardSize, CardSize), tweenTime).SetEase(Model.Inst.Ease_Out_100);
            List_Card[i].RT.DOAnchorPosY(List_closepos[count1], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            count1++;
            istweeening = false;
        }
    }

    public RectTransform RT_DetailBG, RT_DetailContainer, RT_DetailContainerBackground, RT_payButton, RT_DetailContentBackground;
    [SerializeField] List<Sprite> List_Text, List_DetailBg;
    [SerializeField] TweenObject Detail_Title, Detail_Contents;
    public CanvasGroup CG_button, CG_PayButton;
    public Image Img_BG, Img_Navi, Img_TilteBackGround;
    public bool isdetailopen;
    public Color ColorDetailBG, ColorBG;
    float tweenTimeDetail = 0.4f;
    // isopen = false일 때 실행되는 함수다. 
    // 뒷배경이 색이 변하며 아랫쪽에서 detail page 레이어가 올라오는 연출과 함께 카드가 뭉쳐지면서 선택된 카드가 커지고
    // 그 뒤로 다른 카드들이 숨는 모습을 한다.
    // detail로 들어왔는지 안왔는지는 isdetailopen의 참거짓값으로 구분이 되며 isdetailopen일때는 ondrag가 작동이 안한다.
    [SerializeField] CanvasGroup CG_Dim, CG_BoardIndicator;
    [SerializeField] TweenObject SpareDetail;
    [SerializeField] Button BT_GoSpareDetail1, BT_GoSpareDetail2, BT_DimOutSpare;
    [SerializeField] Button BT_OutDetail;
    [SerializeField] List<Sprite> list_spare;
    [SerializeField] RectTransform boardingPassDetail;
    [SerializeField] GameObject Key_BG;
    void goSpareDetail()
    {
        MyStatus = Status.SpareDetail;
        string _temp = List_Card[Myindex].Img.sprite.name;
        // if (_temp == "BoardingPass")
        // {
        //     boardingPassDetail.DOAnchorPosY(0, 0.4f).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
        //     {
        //         CG_BoardIndicator.DOFade(1, 0.2f);
        //     });
        // }
        // else
        // {
        SpareDetail.Img.sprite = list_spare.Find(Sprite => Sprite.name == _temp);
        SpareDetail.Img.SetNativeSize();

        SpareDetail.RT.DOAnchorPosY(-102, 0.4f).SetEase(Model.Inst.Ease_InOut20_100);
        // }
        CG_Dim.blocksRaycasts = true;
        CG_Dim.DOFade(1, tweenTime);
        CG_Indicator.DOFade(0, 0.2f);
    }
    public void outSpareDetail()
    {
        if (isdetailopen)
        {
            MyStatus = Status.CardDetail;
        }
        else
        {
            MyStatus = Status.CardListClose;
        }
        string _temp = List_Card[Myindex].Img.sprite.name;
        if (_temp == "BoardingPass")
        {
            SpareDetail.RT.DOAnchorPosY(2100, 0.4f).SetEase(Ease.OutQuart);
        }
        else
        {
            SpareDetail.RT.DOAnchorPosY(1600, 0.4f).SetEase(Ease.OutQuart);
        }
        CG_Dim.blocksRaycasts = false;
        CG_Dim.DOFade(0, tweenTime);
        CG_BoardIndicator.alpha = 0;
        boardingPassDetail.DOAnchorPosY(1600f, 0.4f).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Indicator.DOFade(1, 0.2f);
    }

    public void gotoDetail()
    {
        MyStatus = Status.CardDetail;
        isdetailopen = true;
        plus.BT_Plus1.gameObject.SetActive(false);
        detailSwipeDown.Temp_Title.CG.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_Out_100);
        RT_DetailBG.DOAnchorPosY(2061.5f, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
        Img_BG.DOColor(ColorDetailBG, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
        Img_TilteBackGround.DOColor(ColorDetailBG, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
        CG_button.DOFade(1, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
        BotButtons.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        CG_pin.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        Benefit.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        if (!MB.IsOpen)
        {
            CG_SamsungTitle.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else
        {
            for (int j = 0; j < MB.Objects.Count; j++)
            {
                MB.Objects[j].DOAnchorPosY(MB.List_mypos[j] + 2400, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            }
        }
        RT_DetailContainer.transform.SetParent(RT_DetailBG.transform);
        RT_payButton.transform.SetAsLastSibling();
        Benefit.RT.DOAnchorPosY(-500, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.RT.DOScale(new Vector2(0.8f, 0.8f), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        MemberCoupon.CG.blocksRaycasts = false;

        // ------------------------- detail Matching --------------------------
        string _tempName = List_Card[Myindex].Img.sprite.name;

        setShadowColor();

        if (_tempName == "Genesis")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "DigitalKeyTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "DigitalKey");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 2705.5f);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -838f);
            Model.Inst.DetailLimitPosition = 0;
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            MakeDegitalKey();
        }
        else if (_tempName == "StudentID")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "IDTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "ID");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 2687.5f);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -773);
            Model.Inst.DetailLimitPosition = 0;
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
        }
        else if (_tempName == "Vaccine")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "VaccineTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "VaccineC");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 2794);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -878);
            Model.Inst.DetailLimitPosition = 0;
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeDetailVaccineButton();
        }
        else if (_tempName == "Samsung")
        {
            CG_PayButton.alpha = 1;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "SamsungTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "Samsungs");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 7442);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -844f);
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeSecondSamsungDetailContents();
            Model.Inst.DetailLimitPosition = 0;
        }
        else if (_tempName == "BoardingPass")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "BoardingPassTitle");
            Detail_Contents.Img.sprite = BoardPassImages.Find(Sprite => Sprite.name == "1");
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -784);
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeBoardingPassContents();

        }
        else if (_tempName == "DigitalAsset")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "DigitalAssetTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "DigitalAsset");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 2909.5f);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -976);
            Model.Inst.DetailLimitPosition = 0;
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeDigitalAssetButton();
        }
        else if (_tempName == "Ticket")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "TicketTitle");
            Detail_Contents.Img.sprite = ticketImages.Find(Sprite => Sprite.name == "1");
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -784);
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeTicketContent();
        }
        else if (_tempName == "Yale")
        {
            CG_PayButton.alpha = 0;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "YaleTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "Yale");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 2705.5f);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -838);
            Model.Inst.DetailLimitPosition = 0;
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            MakeDegitalKey();
        }
        else
        {
            CG_PayButton.alpha = 1;
            Detail_Title.RT.anchoredPosition = new Vector2(-418, -412);
            Detail_Title.Img.sprite = List_Text.Find(Sprite => Sprite.name == "normalCardTitle");
            Detail_Contents.Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "normalCard");
            RT_DetailContainer.sizeDelta = new Vector2(1080, 5487);
            Detail_Contents.RT.anchoredPosition = new Vector2(0, -1133);
            Detail_Title.Img.SetNativeSize();
            Detail_Contents.Img.SetNativeSize();
            makeSecondDetailContents();
            Model.Inst.DetailLimitPosition = 0;
        }
        // ---------------------------------------------------------------------

        RT_DetailContainerBackground.anchoredPosition = new Vector2(0, RT_DetailBG.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition.y);
        RT_DetailContainerBackground.sizeDelta = Detail_Contents.RT.sizeDelta;
        RT_DetailContainerBackground.anchoredPosition = Detail_Contents.RT.anchoredPosition;
        // Detail_Title.Img.SetNativeSize();
        CG_button.blocksRaycasts = true;
        MemberCoupon.CG.DOFade(0, tweenTimeDetail / 2).SetEase(Model.Inst.Ease_InOut20_100);
        MemberCoupon.CG.blocksRaycasts = false;
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i < Myindex)
            {
                List_Card[i].RT.DOAnchorPosX(horArrival2, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            }
            else
            {
                if (i == Myindex)
                {
                    List_Card[i].RT.DOAnchorPosY(1502, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT.DOScale(new Vector2(0.85f, 0.85f), tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT_Shadow.DOAnchorPosY(206, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].CG_shadow.DOFade(1, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                }
                else
                {
                    List_Card[i].RT.DOAnchorPosY(1502, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT.DOScale(new Vector2(0.8f, 0.8f), tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].CG.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                }
            }
        }
    }
    void setShadowColor()
    {
        string _tempName = List_Card[Myindex].Img.sprite.name;
        Image _tempColor = List_Card[Myindex].Img_Shadow;

        if (_tempName == "Genesis")
        {
            _tempColor.color = new Color32(15, 15, 15, 255);
        }
        else if (_tempName == "StudentID")
        {
            _tempColor.color = new Color32(16, 32, 105, 255);
        }
        else if (_tempName == "Vaccine")
        {
            _tempColor.color = new Color32(24, 98, 231, 255);
        }
        else if (_tempName == "Samsung")
        {
            _tempColor.color = new Color32(90, 45, 174, 255);
        }
        else if (_tempName == "BoardingPass")
        {
            _tempColor.color = new Color32(1, 48, 134, 255);
        }
        else if (_tempName == "Yale")
        {
            _tempColor.color = new Color32(255, 207, 41, 255);
        }
        else if (_tempName == "DigitalAsset")
        {
            _tempColor.color = new Color32(15, 57, 169, 255);
        }
        else if (_tempName == "Ticket")
        {
            _tempColor.color = new Color32(44, 26, 89, 255);
        }
        else if (_tempName == "Sky")
        {
            _tempColor.color = new Color32(80, 187, 220, 255);
        }
        else if (_tempName == "MembershipCard")
        {
            _tempColor.color = new Color32(177, 8, 33, 255);
        }
        else if (_tempName == "MembershipCard")
        {
            _tempColor.color = new Color32(177, 8, 33, 255);
        }
        else if (_tempName == "RedVisa")
        {
            _tempColor.color = new Color32(177, 8, 33, 255);
        }
        else if (_tempName == "Violet")
        {
            _tempColor.color = new Color32(109, 9, 81, 255);
        }
        else if (_tempName == "Purple")
        {
            _tempColor.color = new Color32(79, 87, 219, 255);
        }
    }
    [SerializeField] List<Sprite> ticketImages, BoardPassImages;
    [SerializeField] GameObject icon_refresh;
    [SerializeField] TweenObject SamsungSecondContainer;
    public void makeBoardingPassContents()
    {
        GameObject checkItem;
        checkItem = GameObject.Find("BoardingPassContainer");
        if (checkItem != null)
        {
            return;
        }
        RectTransform tempRT = null;
        float tempPos = 0;
        GameObject container = new GameObject();
        container.name = "BoardingPassContainer";
        container.transform.SetParent(RT_DetailContainer);
        container.AddComponent<RectTransform>();
        RectTransform rt_container = container.GetComponent<RectTransform>();
        rt_container.sizeDelta = new Vector2(1080, 1080);

        rt_container.pivot = new Vector2(0, 1);
        rt_container.anchorMax = new Vector2(0, 1);
        rt_container.anchorMin = new Vector2(0, 1);
        rt_container.localScale = Vector3.one;
        rt_container.localPosition = Vector3.zero;
        rt_container.anchoredPosition = new Vector2(0, Detail_Contents.RT.anchoredPosition.y - Detail_Contents.RT.sizeDelta.y - 48 - 18);

        GameObject icon = Instantiate(icon_refresh);
        icon.transform.SetParent(CG_button.transform);
        RectTransform RT_icon = icon.GetComponent<RectTransform>();
        icon.name = "icon";
        RT_icon.pivot = new Vector2(1, 1);
        RT_icon.anchorMax = new Vector2(1, 1);
        RT_icon.anchorMin = new Vector2(1, 1);
        RT_icon.localScale = Vector3.one;
        RT_icon.localPosition = Vector3.zero;
        RT_icon.anchoredPosition = new Vector2(-87, 0);


        for (int i = 1; i < BoardPassImages.Count; i++)
        {
            GameObject content = new GameObject();
            content.transform.SetParent(container.transform);
            content.name = "BoardingPass" + i;
            content.AddComponent<Image>();
            Image img = content.GetComponent<Image>();
            RectTransform rt = content.GetComponent<RectTransform>();

            img.sprite = BoardPassImages[i];
            img.SetNativeSize();
            rt.pivot = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.localScale = Vector3.one;
            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = new Vector2(0, tempPos);

            tempRT = rt;
            tempPos -= rt.sizeDelta.y + 48;
        }
        RT_DetailContainer.sizeDelta = new Vector2(1080, 7080);
    }
    public void makeTicketContent()
    {
        GameObject checkItem;
        checkItem = GameObject.Find("TicketContainer");
        if (checkItem != null)
        {
            return;
        }
        RectTransform tempRT = null;
        float tempPos = 0;
        GameObject container = new GameObject();
        container.name = "TicketContainer";
        container.transform.SetParent(RT_DetailContainer);
        container.AddComponent<RectTransform>();
        RectTransform rt_container = container.GetComponent<RectTransform>();
        rt_container.sizeDelta = new Vector2(1080, 1080);

        rt_container.pivot = new Vector2(0, 1);
        rt_container.anchorMax = new Vector2(0, 1);
        rt_container.anchorMin = new Vector2(0, 1);
        rt_container.localScale = Vector3.one;
        rt_container.localPosition = Vector3.zero;
        rt_container.anchoredPosition = new Vector2(0, Detail_Contents.RT.anchoredPosition.y - Detail_Contents.RT.sizeDelta.y - 18);


        for (int i = 1; i < ticketImages.Count; i++)
        {
            GameObject content = new GameObject();
            content.transform.SetParent(container.transform);
            content.name = "Ticket" + i;
            content.AddComponent<Image>();
            Image img = content.GetComponent<Image>();
            RectTransform rt = content.GetComponent<RectTransform>();

            img.sprite = ticketImages[i];
            img.SetNativeSize();
            if (i == ticketImages.Count - 1)
            {
                rt.pivot = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.localScale = Vector3.one;
                rt.localPosition = Vector3.zero;
                rt.anchoredPosition = new Vector2(0, tempPos);
            }
            else
            {
                rt.pivot = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.localScale = Vector3.one;
                rt.localPosition = Vector3.zero;
                rt.anchoredPosition = new Vector2(0, tempPos);
            }

            tempRT = rt;
            tempPos -= rt.sizeDelta.y + 48;
        }
        RT_DetailContainer.sizeDelta = new Vector2(1080, 5721 - 12);
    }

    public void makeSecondSamsungDetailContents() // 디테일 컨텐츠가 깨지는거 때문에 새로운 영역을 만들려고 만들거임
    {

        GameObject checkItem;
        checkItem = GameObject.Find("SecondDetailContents");
        if (checkItem != null)
        {
            return;
        }

        GameObject Detail = Instantiate(SamsungSecondContainer.go);
        Detail.name = "SecondDetailContents";
        RectTransform RT = Detail.GetComponent<RectTransform>();

        RT.pivot = new Vector2(0, 1);
        RT.anchorMax = new Vector2(0, 1);
        RT.anchorMin = new Vector2(0, 1);
        //---------------------------------------------------------------
        GameObject Detail2 = new GameObject();
        Detail2.name = "contents1";
        Detail2.AddComponent<Image>();
        Image Img2 = Detail2.GetComponent<Image>();
        RectTransform RT2 = Detail2.GetComponent<RectTransform>();
        Img2.color = new Color(1, 1, 1, 1);

        RT2.pivot = new Vector2(0, 1);
        RT2.anchorMax = new Vector2(0, 1);
        RT2.anchorMin = new Vector2(0, 1);
        //--------------------------------------------------------

        for (int i = 0; i < 5; i++)
        {
            GameObject Detail_Content = new GameObject();
            Detail_Content.name = "Contents" + i.ToString();
            Detail_Content.AddComponent<Image>();
            Image img = Detail_Content.GetComponent<Image>();
            RectTransform rt = Detail_Content.GetComponent<RectTransform>();
            img.color = new Color(1, 1, 1, 1);

            rt.pivot = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);

            Detail_Content.transform.SetParent(Detail.transform);
            rt.localScale = Vector3.one;
            rt.transform.localPosition = Vector3.zero;

            img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "SamSungs_1");
            rt.anchoredPosition = new Vector2(63, -87 - 423 * 3 - 286 * 3 * i);

            img.SetNativeSize();
        }

        Detail.transform.SetParent(RT_DetailContainer.transform);
        Detail2.transform.SetParent(Detail.transform);

        RT.localScale = Vector3.one;
        RT.transform.localPosition = Vector3.zero;
        RT2.localScale = Vector3.one;
        RT2.transform.localPosition = Vector3.zero;

        RT.anchoredPosition = new Vector2(0, -1570 - 12);
        Img2.sprite = List_DetailBg.Find(Sprite => Sprite.name == "Samsungs2");
        RT2.anchoredPosition = new Vector2(63, -87);

        Img2.SetNativeSize();
    }
    public void MakeDegitalKey() // 디테일 컨텐츠가 깨지는거 때문에 새로운 영역을 만들려고 만들거임
    {

        GameObject checkItem;
        checkItem = GameObject.Find("Blue_text");
        if (checkItem != null)
        {
            return;
        }

        GameObject Keys = Instantiate(Key_BG);
        TypeBHandler TypeBkey = Keys.GetComponent<TypeBHandler>();
        Keys.transform.SetParent(RT_DetailContainer.gameObject.transform);
        Keys.name = "Keys";
        RectTransform Rt_key = Keys.GetComponent<RectTransform>();
        Rt_key.pivot = new Vector2(0, 1);
        Rt_key.anchorMax = new Vector2(0, 1);
        Rt_key.anchorMin = new Vector2(0, 1);
        Rt_key.localScale = Vector3.one;
        Rt_key.transform.localPosition = Vector3.zero;
        Rt_key.anchoredPosition = new Vector2(0, 28);

        GameObject blueText = Keys.transform.GetChild(0).gameObject;
        blueText.transform.SetParent(Detail_Title.RT.gameObject.transform);
        blueText.name = "Blue_text";
        RectTransform Rt_blue = blueText.GetComponent<RectTransform>();
        Rt_blue.pivot = new Vector2(0, 1);
        Rt_blue.anchorMax = new Vector2(0, 1);
        Rt_blue.anchorMin = new Vector2(0, 1);
        Rt_blue.localScale = Vector3.one;
        Rt_blue.transform.localPosition = Vector3.zero;
        Rt_blue.anchoredPosition = new Vector2(0, -78);

        SetScroll(Keys.transform.GetChild(0).gameObject);
        SetScroll(Keys.transform.GetChild(1).gameObject);
        SetScroll(Keys.transform.GetChild(2).gameObject);
        SetScroll(Keys.transform.GetChild(3).gameObject);
        if (List_Card[Myindex].Img.sprite.name == "Yale")
        {
            Keys.transform.GetChild(2).gameObject.SetActive(false);
            Keys.transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    void SetScroll(GameObject _gameObject)
    {
        ScrollButton SCButton = _gameObject.GetComponent<ScrollButton>();

        SCButton.detailSwipeDown = detailSwipeDown;
        SCButton.scrollRect = detailSwipeDown.GetComponent<ScrollRect>();
    }
    public void makeSecondDetailContents() // 디테일 컨텐츠가 깨지는거 때문에 새로운 영역을 만들려고 만들거임
    {

        GameObject checkItem;
        checkItem = GameObject.Find("SecondDetailContents");
        if (checkItem != null)
        {
            return;
        }

        GameObject Detail = new GameObject();
        Detail.name = "SecondDetailContents";
        Detail.AddComponent<Image>();
        Image Img = Detail.GetComponent<Image>();
        RectTransform RT = Detail.GetComponent<RectTransform>();
        Img.color = new Color(1, 1, 1, 1);

        RT.pivot = new Vector2(0, 1);
        RT.anchorMax = new Vector2(0, 1);
        RT.anchorMin = new Vector2(0, 1);
        //---------------------------------------------------------------
        GameObject Detail2 = new GameObject();
        Detail2.name = "contents1";
        Detail2.AddComponent<Image>();
        Image Img2 = Detail2.GetComponent<Image>();
        RectTransform RT2 = Detail2.GetComponent<RectTransform>();
        Img2.color = new Color(1, 1, 1, 1);

        RT2.pivot = new Vector2(0, 1);
        RT2.anchorMax = new Vector2(0, 1);
        RT2.anchorMin = new Vector2(0, 1);
        //--------------------------------------------------------
        GameObject Detail3 = new GameObject();
        Detail3.name = "contents2";
        Detail3.AddComponent<Image>();
        Image Img3 = Detail3.GetComponent<Image>();
        RectTransform RT3 = Detail3.GetComponent<RectTransform>();
        Img3.color = new Color(1, 1, 1, 1);

        RT3.pivot = new Vector2(0, 1);
        RT3.anchorMax = new Vector2(0, 1);
        RT3.anchorMin = new Vector2(0, 1);
        //--------------------------------------------------------

        Detail.transform.SetParent(RT_DetailContainer.transform);
        Detail2.transform.SetParent(Detail.transform);
        Detail3.transform.SetParent(Detail.transform);

        RT.localScale = Vector3.one;
        RT.transform.localPosition = Vector3.zero;
        RT2.localScale = Vector3.one;
        RT2.transform.localPosition = Vector3.zero;
        RT3.localScale = Vector3.one;
        RT3.transform.localPosition = Vector3.zero;

        GameObject tabs = new GameObject();
        tabs.name = "Detail_2Tab";
        tabs.AddComponent<Image>();
        Image Img_Tab = tabs.GetComponent<Image>();
        RectTransform RT_Tab = tabs.GetComponent<RectTransform>();
        Img_Tab.color = new Color(1, 1, 1, 1);

        RT_Tab.pivot = new Vector2(0, 1);
        RT_Tab.anchorMax = new Vector2(0, 1);
        RT_Tab.anchorMin = new Vector2(0, 1);

        tabs.transform.SetParent(Detail.transform);
        RT_Tab.localScale = Vector3.one;
        RT_Tab.transform.localPosition = Vector3.zero;

        Img.sprite = List_DetailBg.Find(Sprite => Sprite.name == "normalCardback");
        RT.anchoredPosition = new Vector2(0, -1661 - 20);
        Img2.sprite = List_DetailBg.Find(Sprite => Sprite.name == "normalCard2");
        Img3.sprite = List_DetailBg.Find(Sprite => Sprite.name == "normalCard3");
        Img_Tab.sprite = List_DetailBg.Find(Sprite => Sprite.name == "2Tabs");
        Img_Tab.SetNativeSize();

        RT.sizeDelta = new Vector2(1080, 3100 - 20);
        RT2.anchoredPosition = new Vector2(60, -294);
        RT3.anchoredPosition = new Vector2(60, -2140);

        if (detailSwipeDown.RT_Detail_2Tab == null)
        {
            detailSwipeDown.RT_Detail_2Tab = RT_Tab;
            detailSwipeDown.SecondDetailContent = Detail;
        }

        Img.SetNativeSize();
        Img2.SetNativeSize();
        Img3.SetNativeSize();
    }
    GameObject DetailVaccineButton;
    public void makeDetailVaccineButton()
    {
        GameObject checkItem;
        checkItem = GameObject.Find("VaccineDetailButton");
        if (checkItem != null)
        {
            return;
        }
        DetailVaccineButton = new GameObject();
        DetailVaccineButton.name = "VaccineDetailButton";
        DetailVaccineButton.AddComponent<Image>();
        DetailVaccineButton.AddComponent<ScrollButton>();
        DetailVaccineButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        RectTransform RT = DetailVaccineButton.GetComponent<RectTransform>();

        DetailVaccineButton.transform.SetParent(Detail_Contents.RT);
        RT.localScale = Vector3.one;
        RT.transform.localPosition = Vector3.zero;
        RT.anchoredPosition = new Vector3(0, 734.5f);
        RT.sizeDelta = new Vector2(366, 366);

        DetailVaccineButton.GetComponent<ScrollButton>().onClick.AddListener(goSpareDetail);
    }
    public void makeDigitalAssetButton()
    {
        GameObject checkItem;
        checkItem = GameObject.Find("CoinBaseButton");
        if (checkItem != null)
        {
            return;
        }
        GameObject DigitalButton;
        DigitalButton = new GameObject();
        DigitalButton.name = "CoinBaseButton";
        DigitalButton.AddComponent<Image>();
        DigitalButton.AddComponent<ScrollButton>();
        DigitalButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        RectTransform RT = DigitalButton.GetComponent<RectTransform>();

        DigitalButton.transform.SetParent(Detail_Contents.RT);
        RT.localScale = Vector3.one;
        RT.transform.localPosition = Vector3.zero;
        RT.anchoredPosition = new Vector3(0, 257);
        RT.sizeDelta = new Vector2(1080, 210);

        DigitalButton.GetComponent<ScrollButton>().onClick.AddListener(goCoinBase);
    }
    [SerializeField] CanvasGroup cg_coinbase;
    void goCoinBase()
    {
        MyStatus = Status.Coibase;
        cg_coinbase.DOFade(1, 0.3f);
        cg_coinbase.blocksRaycasts = true;
    }
    public void OutCoinBase()
    {
        MyStatus = Status.CardDetail;
        cg_coinbase.DOFade(0, 0.3f);
        cg_coinbase.blocksRaycasts = false;
    }
    public void DragOutDetail(float _value)
    {
        string _name = List_Card[Myindex].Img.sprite.name;
        Detail_Title.CG.alpha = 1;
        detailSwipeDown.RT_DetailTitle_content.localScale = Vector2.one;
        float color = (246 + 6 * _value) / 255;
        RT_DetailBG.anchoredPosition = new Vector2(0, 2061.5f - 2061.5f * _value);
        List_Card[Myindex].CG.alpha = 1;
        Img_BG.color = new Color(color, color, color);
        Img_TilteBackGround.color = new Color(color, color, color);
        CG_button.alpha = 1 - _value * 5;
        if (_name != "Vaccine" && _name != "BoardingPass" && _name != "Ticket" && _name != "MembershipCard" && _name != "Genesis" && _name != "Yale")
            CG_pin.alpha = -4 + _value * 5;
        if (!MB.IsOpen)
        {
            CG_SamsungTitle.alpha = _value;
            plus.BT_Plus1.gameObject.SetActive(true);
        }
        else
        {
            for (int j = 0; j < MB.Objects.Count; j++)
            {
                MB.Objects[j].anchoredPosition = new Vector2(MB.Objects[j].anchoredPosition.x, MB.List_mypos[j] + 2400 - 2400 * _value);
            }
            plus.BT_Plus1.gameObject.SetActive(false);
        }
        Benefit.RT.anchoredPosition = new Vector2(0, -500 - 690 * _value);
        Benefit.RT.localScale = new Vector2(0.8f + 0.2f * _value, 0.8f + 0.2f * _value);
        int count = 0;
        float SizeGap = (CardSize - 0.85f) * _value;
        float SizeGap2 = (CardSize - 0.8f) * _value;
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i >= Myindex)
            {
                List_Card[i].RT.anchoredPosition = new Vector2(0, 1502 - (1502 - List_closepos[count]) * _value);
                if (i == Myindex)
                {
                    List_Card[i].RT.localScale = new Vector2(0.85f + SizeGap, 0.85f + SizeGap);
                    List_Card[i].CG_shadow.alpha = 1 - _value;
                    List_Card[i].RT_Shadow.anchoredPosition = new Vector2(0, 206 + 24 * _value);
                }
                else
                    List_Card[i].RT.localScale = new Vector2(0.8f + SizeGap2, 0.8f + SizeGap2);
                count++;
            }
        }
    }

    // isdetailopen= true일때 false로 만드러주며 detail 뎁스에서 홈으로 나오게 한다.
    [SerializeField] PlusButtonHandler plus;
    public void OutDetail()
    {
        if (isopen || isdetailopen)
        {
            MyStatus = Status.CardListClose;
            isdetailopen = false;
            detailSwipeDown.isSnaped = false;
            detailSwipeDown.step = 0;
            detailSwipeDown.Temp_Title.CG.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_Out_100);
            RT_DetailBG.DOAnchorPosY(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            Img_BG.DOColor(ColorBG, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            Img_TilteBackGround.DOColor(ColorBG, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            CG_button.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            BotButtons.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
            Benefit.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
            Benefit.RT.DOScale(new Vector2(CardSize, CardSize), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            RT_Scrollcontainer.DOAnchorPosY(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            smoothcontainer.DOAnchorPosY(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            RT_DetailContainer.DOAnchorPosY(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            Detail_Title.CG.alpha = 1;
            detailSwipeDown.RT_DetailTitle_content.localScale = Vector2.one;
            if (!MB.IsOpen)
            {
                CG_SamsungTitle.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                plus.BT_Plus1.gameObject.SetActive(true);
            }
            else
            {
                for (int j = 0; j < MB.Objects.Count; j++)
                {
                    MB.Objects[j].DOAnchorPosY(MB.List_mypos[j], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
                plus.BT_Plus1.gameObject.SetActive(false);
            }
            EditWallet.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
            MemberCoupon.CG.blocksRaycasts = true;
            Scl.enabled = false;
            EditWallet.RT.DOAnchorPosY(-93.5f, tweenTime).SetEase(Model.Inst.Ease_Out_100);

            BenefitApear(Myindex);

            CG_button.blocksRaycasts = false;
            int count = 0;
            if (!MB.IsOpen)
            {
                MemberCoupon.CG.DOFade(1, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100).SetDelay(tweenTime);
            }
            else
            {
                MemberCoupon.CG.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
            }
            MemberCoupon.CG.blocksRaycasts = true;
            for (int i = 0; i < List_Card.Count; i++)
            {
                if (i < Myindex)
                {
                    List_Card[i].RT.DOAnchorPosX(horArrival, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    if (Myindex - 1 > i)
                    {
                        List_Card[i].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                    }
                    else
                    {
                        List_Card[i].CG.DOFade(0.45f, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                    }
                }
                else
                {
                    List_Card[i].CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                    List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT.DOScale(new Vector2(CardSize, CardSize), tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT_Shadow.DOAnchorPosY(230, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].CG_shadow.DOFade(0, tweenTimeDetail).SetEase(Model.Inst.Ease_InOut20_100);
                    count++;
                }
            }
            if (isopen)
                Myindex = 0;
            isopen = false;
            Destroy(DetailVaccineButton);
            GameObject checkItem = GameObject.Find("SecondDetailContents");
            GameObject checkItem2 = GameObject.Find("Detail_2Tab");
            GameObject checkItem3 = GameObject.Find("BoardingPassContainer");
            GameObject checkItem4 = GameObject.Find("TicketContainer");
            GameObject checkItem5 = GameObject.Find("CoinBaseButton");
            GameObject checkItem6 = GameObject.Find("Blue_text");
            GameObject checkItem7 = GameObject.Find("Keys");
            GameObject checkItem8 = GameObject.Find("icon");
            {
                Destroy(checkItem3);
                Destroy(checkItem2);
                Destroy(checkItem);
                Destroy(checkItem4);
                Destroy(checkItem5);
                Destroy(checkItem6);
                Destroy(checkItem7);
                Destroy(checkItem8);
            }
        }
    }

    public string direction = " ";

    // onbegindrag에 들어가있는 함수다. 드래그가 시작되는 동시에 카드들은 cardContainer안으로 들어오게 되고 
    // 카드리스트에 순서대로 ispector 순서도 바꿔준다. 
    void reSortingCard()
    {
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.transform.SetParent(this.transform.GetChild(1));
            List_Card[i].RT.SetAsFirstSibling();
        }
    }

    void SetHorSwipeForIsopen()
    {
        Scl.enabled = false;
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].parallax.isParallax = false;
        }
    }
    float movamount;
    // 세로로 스크롤하는 동안 어느 지점부터 movePos 값을 리셋해주기 위한 bool
    [SerializeField] bool isOnCheckPointBot = false;
    [SerializeField] bool isOnCheckPointTop = false;

    [SerializeField] float LimitPosition = 1;

    //!!!!!!!!========================================================================================================
    float time;
    public void OnBeginDrag(PointerEventData e)
    {
        isTouch = true;
        time = 0;
        if (!isdetailopen && !isopen)
            if (e.pressPosition.y - e.position.y > 0)
                reSortingCard();

        if (MyStatus != Status.CardEditMode)
        {
            if (Mathf.Abs(e.delta.x) < Mathf.Abs(e.delta.y))
                direction = "Ver";
            else
                direction = "Hor";
        }

        if (!isdetailopen && !istweeening && MyStatus != Status.CardEditMode)
        {
            if (direction == "Ver")
            {
                isWave = false;
                if (!isopen)
                    beforeOpenCardReset();
                else
                {
                    for (int i = 0; i < List_Card.Count; i++)
                    {
                        List_Card[i].RT.DOKill();
                    }
                    for (int i = 0; i < List_Card.Count; i++)
                    {
                        List_Card[i].RT.transform.position = List_Card[i].parallax.Target.position;
                        List_Card[i].CG.alpha = 1;
                        List_Card[i].RT.localScale = Vector3.one;
                        List_Card[i].parallax.isParallax = true;
                    }
                    if (RT_Scrollcontainer.anchoredPosition.y > -1f)
                        waveAnimation(e.pressPosition.y - e.position.y);
                }

                if (RT_Scrollcontainer.anchoredPosition.y > -1f || RT_Scrollcontainer.anchoredPosition.y < LimitPosition)
                    movamount = e.pressPosition.y;
            }
            else
            {
                if (isopen)
                {
                    SetHorSwipeForIsopen();
                }
                else
                {
                    if (e.pressPosition.x - e.position.x < 0)
                    {
                        // 오른쪽으로 스와프 동작 시 바로 앞카드의 알파값을 1로 만들어줌.
                        if (Myindex != 0)
                            List_Card[Myindex - 1].CG.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_Out_100);
                    }

                }
            }
        }
    }
    bool isDirectionSelected = false;
    public void OnDrag(PointerEventData e)
    {
        if (!isdetailopen && MyStatus != Status.CardEditMode)
        {
            if (1f < Mathf.Abs(e.delta.y) || 1f < Mathf.Abs(e.delta.x))
                isDrag = true;

            if (direction == "Ver")// && !MB.isOpen)
            {
                if (!istweeening)
                {
                    if (isopen)
                    {
                        if (List_Card.Count > 7) //일반적인 엔드 이펙트
                        {
                            if (!isOnCheckPointTop && !isOnCheckPointBot)
                                movamount = e.position.y;

                            //! === 세로로 스크롤하는 동안 어느 지점부터 movePos 값을 리셋해주기 위한 동작 시작점
                            if (RT_Scrollcontainer.anchoredPosition.y > -1f)
                            {
                                if (e.delta.y >= 0)
                                {
                                    isOnCheckPointBot = true;
                                    isOnCheckPointTop = false;
                                }
                            }
                            else
                                isOnCheckPointBot = false;

                            if (RT_Scrollcontainer.anchoredPosition.y < LimitPosition)
                            {
                                if (e.delta.y <= 0)
                                {
                                    isOnCheckPointTop = true;
                                    isOnCheckPointBot = false;
                                }
                            }
                            else
                                isOnCheckPointTop = false;
                        }
                        else if (List_Card.Count <= 7) // 스크롤 컨테이너랑 컨텐츠 길이가 같을때 아래 엔드이펙트랑 위쪽 엔드 이펙트가 겹치는 경우
                        {
                            if (!isOnCheckPointTop && !isOnCheckPointBot)
                                movamount = e.position.y;

                            //! === 세로로 스크롤하는 동안 어느 지점부터 movePos 값을 리셋해주기 위한 동작 시작점
                            if (RT_Scrollcontainer.anchoredPosition.y > -1f)
                            {
                                if (e.delta.y >= 0)
                                {
                                    if (!isDirectionSelected)
                                    {
                                        isOnCheckPointBot = true;
                                        isOnCheckPointTop = false;
                                    }
                                    isDirectionSelected = true;
                                }
                            }
                            if (RT_Scrollcontainer.anchoredPosition.y < LimitPosition)
                            {
                                if (e.delta.y <= 0)
                                {
                                    if (!isDirectionSelected)
                                    {
                                        isOnCheckPointTop = true;
                                        isOnCheckPointBot = false;
                                    }
                                    isDirectionSelected = true;
                                }
                            }
                        }
                        //! === 요기까지 ================================================
                        if (isOnCheckPointBot)
                        {
                            if (e.pressPosition.y - e.position.y < 0)
                                Movpos = e.position.y - movamount;

                            if (dragValue(Movpos) < 0.5f)
                            {
                                //열려있을 때 닫히기 시작되는 드래그 밸류 부분!!!
                                openToCloseValueTween();
                            }
                            else
                            {
                                CloseCard();
                            }
                        }
                        else if (isOnCheckPointTop)
                        {
                            if (e.pressPosition.y - e.position.y > 0)
                                Movpos = e.position.y - movamount;
                            // for (int i = 0; i < List_Card.Count; i++)
                            // {
                            //     List_Card[i].RT.transform.rotation = Quaternion.Euler(-38 * dragValue(-Movpos), 0, 0);
                            // }
                            int count = 0;
                            for (int i = 1; i < List_Card.Count; i++)
                            {
                                List_target[i].anchoredPosition = new Vector2(0, List_openpos[i] - (List_openpos[i] - 579 - (168 + 18) * count) * dragValueEndEffect(-Movpos));
                                count++;
                            }
                        }
                    }
                    else
                    {
                        Movpos = e.pressPosition.y - e.position.y;
                        cardOpeningDrag(dragValue(Movpos));
                        if (!MB.IsOpen)
                        {
                            CG_SamsungTitle.alpha = 1 - dragValue(Movpos) * 5;
                            MemberCoupon.CG.alpha = 1 - dragValue(Movpos * 5);
                        }
                        else
                        {
                            for (int j = 0; j < MB.Objects.Count; j++)
                            {
                                if (j == 0)
                                    MB.Objects[j].anchoredPosition = new Vector2(81, MB.List_mypos[j] + 800 * dragValue(Movpos * 4.2f));
                                else
                                    MB.Objects[j].anchoredPosition = new Vector2(95, MB.List_mypos[j] + 800 * dragValue(Movpos * 4.2f));
                            }
                        }
                        string _name = List_Card[Myindex].Img.sprite.name;
                        if (_name != "Vaccine" && _name != "BoardingPass" && _name != "Ticket" && _name != "StudentID" && _name != "MembershipCard" && _name != "DigitalAsset" && _name != "Genesis" && _name != "Yale" && _name != "PLCC")
                        {
                            CG_pin.alpha = 1 - dragValue(Movpos * 5);
                        }
                        else if (_name == "Vaccine" || _name == "BoardingPass" || _name == "MembershipCard")
                        {
                            CG_ShowQR.alpha = 1 - dragValue(Movpos * 5);
                        }
                        else if (_name == "Genesis" || _name == "Yale")
                        {
                            CG_Door.alpha = 1 - dragValue(Movpos * 5);
                        }
                        else if (_name == "Ticket")
                        {
                            CG_UseCard.alpha = 1 - dragValue(Movpos * 5);
                        }
                        EditWallet.CG.alpha = dragValue(Movpos);
                        BotButtons.CG.alpha = 1 - dragValue(Movpos);
                    }
                }
            }
            else
            {
                Movposx = e.pressPosition.x - e.position.x;
                if (!isopen)
                {
                    horDrag(Movposx);
                }
            }
        }
    }
    public void OnEndDrag(PointerEventData e)
    {
        if (!isdetailopen && MyStatus != Status.CardEditMode)
        {
            if (direction == "Ver")
            {
                if (isopen)
                {
                    if (dragValue(Movpos) <= 0.5f)
                        cardNotMove();

                    float mov4snap = (e.position.y - e.pressPosition.y);
                    if (!cardEditMode.isEditMode)
                    {
                        if (mov4snap > 0)
                        {
                            Scl.enabled = false;
                            RT_Scrollcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                            {
                                Scl.enabled = true;
                            });
                            if (List_Card.Count > 8)
                            {
                                parallax_edit.isParallax = false;
                                int editcount = 0;
                                for (int i = 0; i < List_Card.Count; i++)
                                {
                                    List_Card[i].parallax.isParallax = false;
                                    if (i != 0)
                                    {
                                        List_Card[i].RT.DOAnchorPosY(429f + 192 * i, 0.6f - 0.3f / List_Card.Count * i).SetEase(Model.Inst.Ease_Out_100);
                                    }
                                    else
                                    {
                                        List_Card[i].RT.DOAnchorPosY(429f + 192 * i, 0.6f - 0.3f / List_Card.Count * i).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                                        {
                                            float smoothSpeed = 0;
                                            for (int v = 0; v < List_Card.Count; v++)
                                            {
                                                smoothSpeed = 0.3f - (0.2f / (List_target.Count - 1) * v);
                                                List_Card[v].parallax.smoothSpeed = smoothSpeed;
                                                List_Card[v].parallax.isParallax = true;
                                            }
                                            parallax_edit.isParallax = true;
                                            parallax_edit.smoothSpeed = 0.3f;
                                        });
                                    }
                                    editcount++;
                                }
                                EditWallet.RT.DOAnchorPosY(-93.5f, 0.6f).SetEase(Model.Inst.Ease_Out_100);
                            }
                        }
                        else if (mov4snap < 0)
                        {
                            Scl.enabled = false;
                            RT_Scrollcontainer.DOAnchorPosY(LimitPosition, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                            {
                                Scl.enabled = true;
                            });
                            if (List_Card.Count > 8)
                            {
                                parallax_edit.isParallax = false;
                                int count = List_Card.Count - 1;
                                for (int i = 0; i < List_Card.Count; i++)
                                {
                                    List_Card[count].parallax.isParallax = false;
                                    if (i != 0)
                                    {
                                        List_Card[count].RT.DOAnchorPosY(1774 - 192 * i, 0.6f - 0.3f / List_Card.Count * i).SetEase(Model.Inst.Ease_Out_100);
                                    }
                                    else
                                    {
                                        List_Card[count].RT.DOAnchorPosY(1774 - 192 * i, 0.6f - 0.3f / List_Card.Count * i).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                                        {
                                            float smoothSpeed = 0;
                                            int count1 = List_target.Count - 1;
                                            for (int v = 0; v < List_Card.Count; v++)
                                            {
                                                List_Card[v].parallax.isParallax = true;

                                                smoothSpeed = 0.3f - (0.2f / (List_target.Count - 1) * count1);
                                                List_Card[v].parallax.smoothSpeed = smoothSpeed;
                                                count1--;
                                            }
                                            parallax_edit.isParallax = true;
                                            parallax_edit.smoothSpeed = 0.1f;
                                        });
                                    }
                                    count--;
                                }
                                EditWallet.RT.DOAnchorPosY(-93.5f - 192 * (List_Card.Count - 8), 0.6f).SetEase(Model.Inst.Ease_Out_100);
                            }
                        }
                    }
                }
                else
                {
                    if (dragValue(Movpos) > 0.5f)
                        openCard();
                    else
                        cardNotMove();
                }
                Movpos = 0;
                isTouch = false;
                movamount = 0;
            }
            else
                horCardSnap();
        }
        isDrag = false;
        cardEditMode.isEditMode = false;
        isDirectionSelected = false;
    }

    //!!!!!!========================================================================================================

    // 열려있다가 닫히는 과정을 보여준다. isopen 일때 카드들이 list_openpos[본인순서]에 있다가 list_closepos[본인순서]로 이동하게 되는데 이는 dragValue(movpos)에
    //값에 따라 달라진다. 
    void openToCloseValueTween()
    {
        // for (int i = 0; i < List_Card.Count; i++)
        //     List_Card[i].RT.transform.rotation = Quaternion.Euler(60 * dragValue(Movpos), 0, 0);
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_target[i].anchoredPosition = new Vector2(0, List_openpos[i] - (List_openpos[i] - List_closepos[i]) * dragValue(Movpos));
        }
        float sizeGap = (1 - CardSize) * dragValue(Movpos);
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_target[i].localScale = new Vector3(1, 1, 1);
            List_Card[i].RT.localScale = new Vector3(1 - sizeGap, 1 - sizeGap, 1 - sizeGap);
        }
        // CG_pin.alpha = dragValue(Movpos);
        BotButtons.CG.alpha = dragValue(Movpos);
        EditWallet.CG.alpha = 1 - dragValue(Movpos * 5);
    }
    // 카드들의 움직임을 멈춰주고 onbegindrag에서 실행되어 상태가 달라져 있던 카드들을 알파값이랑 스케일 값을 돌려놓는다.
    void beforeOpenCardReset()
    {
        //?? 카드가 열리기 직전에 좌측에 빠져있는 카드들의 사이즈와 알파값을 초기화 
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.DOKill();
            List_Card[i].RT.localScale = new Vector2(CardSize, CardSize);
            List_Card[i].CG.alpha = 1;
        }
    }
    // isopen = false 일때 카듣들이 닫혀있는 상태에서 _Movepos값에 따라 List_closepos[본인순서]에서 List_openpos[본인순서]로 움직인다.
    // myindex가 0일때랑 0이 아닐때로 구분되어있으며 그 외 다른 gameobect들의 alpha값이나 anchoredposition, deltasize를 movpos에 따라 바꿔준다
    // movpos는 0~1의 숫자로 대응이 된다.
    void cardOpeningDrag(float _Movepos)
    {
        if (Myindex != 0)
        {
            int count = 0;
            int count1 = 0;
            for (int i = 0; i < List_Card.Count; i++)
            {
                if (Myindex <= i)
                {
                    if (i < Myindex + 5)
                    {
                        if (List_Card.Count - Myindex < 6)
                        {
                            count = 6 - (List_Card.Count - Myindex);
                        }
                        List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count1] + ((429 + (distanceCard2Card / 2) * count + distanceCard2Card * count1 - List_closepos[count1]) * _Movepos));
                        count1++;
                    }
                    else
                    {
                        List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count1] + ((429 + (distanceCard2Card / 2) * count + distanceCard2Card * count1 - List_closepos[count1]) * _Movepos));
                    }
                }
                else
                {
                    List_Card[i].RT.anchoredPosition = new Vector2(0, -700);
                }
            }
        }
        else
        {
            int count = 0;
            int count1 = 0;
            if (List_Card.Count - Myindex < 6)
            {
                count = 6 - (List_Card.Count - Myindex);
            }
            for (int i = 0; i < List_Card.Count; i++)
            {
                if (i < Myindex + 5)
                {
                    List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count1] + ((List_openpos[0] + distanceCard2Card * count1 - List_closepos[count1]) * _Movepos));
                    count1++;
                }
                else
                {
                    List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count1] + ((List_openpos[0] + distanceCard2Card * count1 - List_closepos[count1]) * _Movepos));
                }
            }
        }
        float sizeGap = (1 - CardSize) * _Movepos;
        for (int i = 0; i < List_Card.Count; i++)
        {
            List_target[i].localScale = new Vector3(1, 1, 1);
            List_Card[i].RT.localScale = new Vector3(CardSize + sizeGap, CardSize + sizeGap, CardSize + sizeGap);
        }
    }
    // begindrag에서 서로 절대값 e.delta.x 가 e.delta.y보다 클경우 실행된다. 카드는 드래그하는동안 가로로 움직이는 연출만 되고 
    // _Hormovamount 값에따라 카드들의 위치가 달라진다. isopen= false일때만 작동한다.
    // 가로로 움직이는 동안 anchoredposition이랑 alpha값이 달라진다.
    void horDrag(float _Hormovamount)
    {
        if (!istweeening)
        {
            if (!isopen)
            {
                if (_Hormovamount >= 0)
                {
                    float Value = 0;
                    float Value2 = 0;
                    Value = Mathf.Clamp(_Hormovamount / -horArrival, 0, 1);
                    Value2 = Mathf.Clamp(_Hormovamount / -horTilting, 0, 1);
                    List_Card[Myindex].RT.transform.SetAsLastSibling();

                    float sizingGap = CardSize - 0.8f;
                    if (Myindex != List_Card.Count - 1)
                    {
                        List_Card[Myindex].RT.anchoredPosition = new Vector2(0 + horArrival * Value, List_closepos[0]);
                        List_Card[Myindex].RT.transform.localScale = new Vector2(CardSize - sizingGap * Value, CardSize - sizingGap * Value);
                        //List_Card[Myindex].CG.alpha = 1 - Value * 0.6f;
                        int count = 0;
                        for (int i = 0; i < List_Card.Count; i++)
                        {
                            if (i > Myindex)
                            {
                                List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count + 1] - (List_closepos[count + 1] - List_closepos[count]) * Value);
                                count++;
                            }
                        }
                        if (Myindex != 0)
                        {
                            List_Card[Myindex - 1].CG.alpha = 0.45f - 0.45f * Value;
                        }
                    }
                    else
                    {
                        List_Card[Myindex].RT.transform.rotation = Quaternion.Euler(0, 18 * Value2, 0);
                    }
                }
                else
                {
                    float Value = 0;
                    float Value2 = 0;
                    Value = Mathf.Clamp(-_Hormovamount / -horArrival, 0, 1);
                    Value2 = Mathf.Clamp(-_Hormovamount / -horTilting, 0, 1);
                    float sizingGap = CardSize - 0.8f;
                    if (Myindex != 0)
                    {
                        List_Card[Myindex - 1].RT.transform.SetAsLastSibling();
                        List_Card[Myindex - 1].RT.anchoredPosition = new Vector2(horArrival + (-horArrival * Value), List_closepos[0]);
                        List_Card[Myindex - 1].RT.transform.localScale = new Vector2(0.8f + sizingGap * Value, 0.8f + sizingGap * Value);
                        int count = 0;
                        for (int i = 0; i < List_Card.Count; i++)
                        {
                            if (i >= Myindex)
                            {
                                List_Card[i].RT.anchoredPosition = new Vector2(0, List_closepos[count] + (List_closepos[count + 1] - List_closepos[count]) * Value);
                                count++;
                            }
                        }
                        if (Myindex > 1)
                        {
                            List_Card[Myindex - 2].CG.alpha = 0.45f * Value;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < List_Card.Count; i++)
                        {
                            List_Card[i].RT.transform.rotation = Quaternion.Euler(0, -18 * Value2, 0);
                        }
                    }
                }
                // }
            }
        }
    }
    // 카드를 드래그 하는동안 처음지점에서 끝지점을 0~1이라는 숫자로 치환해주는 함수다
    float dragValue(float _value)
    {
        float Value = 0;
        if (isopen)
        {
            Value = Mathf.Clamp(_value / 540, 0f, 1f);
        }
        else
        {
            Value = Mathf.Clamp(_value / 402, 0f, 1f);
        }

        return Value;
    }
    float dragValueEndEffect(float _value)
    {
        float Value = 0;
        Value = Mathf.Clamp(_value / 324, 0f, 1f);
        return Value;
    }

    public float tweenTime;
    // 카드드래그가 끝났을 때 카드가 실행되는 함수다. 카드들이 Listopenpos에서 Listclosepos로 움직이게 하면 그 외 다른 gameobejct들도
    // alpha값이랑 deltasize anchoredposition 등등을 닫혀있는 상태로 바꿔준다.
    // isopen값이 true에서 false로 바꿔준다
    public void CloseCard()
    {

        MyStatus = Status.CardListClose;
        if (!MB.IsOpen)
        {
            plus.BT_Plus1.gameObject.SetActive(true);
            CG_SamsungTitle.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            MemberCoupon.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        }
        else
        {
            plus.BT_Plus1.gameObject.SetActive(false);
            for (int j = 0; j < MB.Objects.Count; j++)
            {
                MB.Objects[j].DOAnchorPosY(MB.List_mypos[j], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            }
        }

        RT_Scrollcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        smoothcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        EditWallet.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        BotButtons.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        CG_Indicator.DOFade(1, 0.2f);
        BotButtons.CG.blocksRaycasts = true;

        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.DORotate(Vector3.zero, tweenTime).SetEase(Model.Inst.Ease_Out_100); //지우기
            List_Card[i].RT.DOAnchorPosY(List_closepos[i], tweenTime).SetEase(Model.Inst.Ease_Out_100);
            List_Card[i].RT.DOScale(new Vector2(CardSize, CardSize), tweenTime).SetEase(Model.Inst.Ease_Out_100);
            List_Card[i].CG_gradient.DOFade(0, 0.3f);
            List_Card[i].parallax.isParallax = false;
        }
        Scl.enabled = false;
        isopen = false;
        EditWallet.CG.blocksRaycasts = false;
        CG_pin.blocksRaycasts = true;
        CG_BTPin.blocksRaycasts = true;
        MemberCoupon.CG.blocksRaycasts = true;

        Myindex = 0;
        EditWallet.RT.DOAnchorPosY(-93.5f, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        BenefitApear(Myindex);
        Movpos = 0;
    }
    // 카드드래그가 끝났을 때 카드가 실행되는 함수다. 카드들이 Listclosepos에서 Listopenpos로 움직이게 하면 그 외 다른 gameobejct들도
    // alpha값이랑 deltasize anchoredposition 등등을 열려있는 상태로 바꿔준다.
    // isopen값이 false에서 true로 바꿔준다
    void openCard()
    {
        MyStatus = Status.CardListOpen;
        plus.BT_Plus1.gameObject.SetActive(false);
        if (Myindex != 0)
        {
            int count1 = List_Card.Count - 1;
            float _goal = (RT_Scrollcontainer.sizeDelta.y - 2077) / (List_Card.Count - 1) * Myindex;
            for (int i = 0; i < List_Card.Count; i++)
            {
                float _tmpTime1 = Mathf.Clamp(tweenTime - tweenTime * dragValue(Movpos), 0.2f + (0.03f * count1), 0.5f);
                float _tmpTime = Mathf.Clamp(tweenTime - tweenTime * dragValue(Movpos), 0.2f, 0.5f);
                List_Card[i].CG.DOFade(1, _tmpTime).SetEase(Model.Inst.Ease_Out_100);
                if (i != List_Card.Count - 1)
                {
                    List_Card[i].RT.DOAnchorPos(new Vector2(0, List_openpos[i] - _goal), _tmpTime1).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        for (int y = 0; y < List_Card.Count; y++)
                        {
                            List_Card[y].parallax.isParallax = true;
                        }
                    });
                }
                else
                {
                    List_Card[i].RT.DOAnchorPos(new Vector2(0, List_openpos[i] - _goal), _tmpTime1).SetEase(Model.Inst.Ease_Out_100); //패럴렉스
                }
                count1--;
            }

            float _Time = Mathf.Clamp(tweenTime - tweenTime * dragValue(Movpos), 0.2f, 0.5f);
            if (List_Card.Count > 7)
            {
                RT_Scrollcontainer.DOAnchorPosY(-_goal, _Time).SetEase(Model.Inst.Ease_Out_100);
            }
            else
            {
                RT_Scrollcontainer.DOAnchorPosY(LimitPosition, _Time).SetEase(Model.Inst.Ease_Out_100);
            }
        }
        else
        {
            for (int i = 0; i < List_Card.Count; i++)
            {
                float _tmpTime1 = Mathf.Clamp(tweenTime - tweenTime * dragValue(Movpos), 0.2f + (0.03f * i), 0.5f);
                float _tmpTime = Mathf.Clamp(tweenTime - tweenTime * dragValue(Movpos), 0.2f, 0.5f);
                if (i == List_Card.Count - 1)
                {
                    List_Card[i].RT.DOAnchorPos(new Vector2(0, List_openpos[i]), _tmpTime1).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                   {
                       for (int y = 0; y < List_Card.Count; y++)
                       {
                           List_Card[y].parallax.isParallax = true;
                       }
                   });
                }
                else
                {
                    List_Card[i].RT.DOAnchorPos(new Vector2(0, List_openpos[i]), _tmpTime1).SetEase(Model.Inst.Ease_Out_100); //패럴렉스
                }
            }
        }
        if (!MB.IsOpen)
            CG_SamsungTitle.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        else
        {
            for (int j = 0; j < MB.Objects.Count; j++)
            {
                MB.Objects[j].DOAnchorPosY(MB.List_mypos[j] + 2400, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            }
        }

        EditWallet.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        BotButtons.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
        BotButtons.CG.blocksRaycasts = false;


        for (int i = 0; i < List_Card.Count; i++)
        {
            List_Card[i].RT.DOScale(Vector2.one, tweenTime).SetEase(Model.Inst.Ease_Out_100);
            List_target[i].anchoredPosition = new Vector2(0, List_openpos[i]);
            List_Card[i].BT.enabled = true;
            if (i != 0)
            {
                List_Card[i].CG_gradient.DOFade(1, 0.3f);
            }
        }
        Benefit.RT.anchoredPosition = new Vector2(0, BeneClose);
        Benefit.go.SetActive(false);
        CG_pin.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        CG_ShowQR.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        CG_UseCard.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        CG_Indicator.DOFade(0, 0.2f);
        EditWallet.CG.blocksRaycasts = true;
        CG_BTPin.blocksRaycasts = false;
        MemberCoupon.CG.blocksRaycasts = false;
        CG_ShowQR.blocksRaycasts = false;
        CG_UseCard.blocksRaycasts = false;
        isopen = true;
        Scl.enabled = true;
    }
    // 카드드래그가 끝났을 때 카드가 실행되는 함수다. 
    // 카드를 횡방향으로 드래그 했을 때 실행된다. 
    // 좌/우로 스와이프 했을 대 myindex값이 ++ / -- 된다.
    //----------------------------------------horDirection-------------------------------------------------
    float horArrival = -871, horArrival2 = -900, horTilting = -435;

    //-----------------------------------------------------------------------------------------------------
    void horCardSnap()
    {
        if (!istweeening)
        {
            if (!isopen)
            {
                istweeening = true;
                if (Movposx > 0)
                {
                    if (Myindex != List_Card.Count - 1)
                    {
                        List_Card[Myindex].RT.DOScale(new Vector2(0.8f, 0.8f), tweenTime).SetEase(Model.Inst.Ease_Out_100);
                        int count = 0;
                        for (int i = Myindex + 1; i < List_Card.Count; i++)
                        {
                            List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTime).SetEase(Model.Inst.Ease_Out_100);
                            count++;
                        }
                        if (Myindex != 0)
                            List_Card[Myindex - 1].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                        List_Card[Myindex].RT.DOAnchorPosX(horArrival, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                        {
                            List_Card[Myindex].CG.DOFade(0.45f, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                            istweeening = false;
                            Myindex++;
                        });
                        BenefitApear(Myindex + 1);
                    }
                    else
                    {
                        List_Card[Myindex].RT.DORotate(Vector2.zero, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                        {
                            istweeening = false;
                        });
                        BenefitApear(Myindex);
                    }
                }
                else if (Movposx < 0)
                {
                    if (Myindex != 0)
                    {
                        int count = 1;
                        for (int i = Myindex; i < List_Card.Count; i++)
                        {
                            List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTime).SetEase(Model.Inst.Ease_Out_100);
                            count++;
                        }
                        List_Card[Myindex - 1].CG.DOFade(1f, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                        if (Myindex > 1)
                            List_Card[Myindex - 2].CG.DOFade(0.45f, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                        List_Card[Myindex - 1].RT.DOScale(CardSize, tweenTime).SetEase(Model.Inst.Ease_Out_100);
                        List_Card[Myindex - 1].RT.DOAnchorPosX(0, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                        {
                            istweeening = false;
                            Myindex--;
                        });
                        BenefitApear(Myindex - 1);
                    }
                    else
                    {
                        for (int i = 0; i < List_Card.Count; i++)
                        {
                            List_Card[i].RT.DORotate(Vector2.zero, tweenTime).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                            {
                                istweeening = false;
                            });
                            BenefitApear(Myindex);
                        }
                    }
                }

            }
        }
    }

    public CanvasGroup CG_ShowQR, CG_UseCard, CG_Door;
    [SerializeField] List<Sprite> list_imgBenefit;

    [HideInInspector] public float BeneClose = -1040, BeneOpen = -1318;
    [HideInInspector]
    public float CardSize = 1;


    void BenefitApear(int _myindex)
    {
        //-benefit---
        string _name = List_Card[_myindex].Img.sprite.name;
        if (_name == "Genesis")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else if (_name == "Yale")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else if (_name == "DigitalAsset")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else if (_name == "Samsung")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else if (_name == "Sky")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else if (_name == "RedVisa")
        {
            Benefit.Img.sprite = list_imgBenefit.Find(Sprite => Sprite.name == _name);
            Benefit.go.SetActive(true);
            Benefit.RT.DOAnchorPosY(BeneOpen, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else
        {
            Benefit.RT.DOAnchorPosY(BeneClose, tweenTime).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
            {
                Benefit.go.SetActive(false);
            });
        }
        Benefit.Img.SetNativeSize();
        //--pin
        if (_name == "Vaccine" || _name == "MembershipCard" || _name == "BoardingPass" || _name == "Ticket"
             || _name == "PLCC" || _name == "ID" || _name == "StudentID" || _name == "DigitalAsset" || _name == "Genesis" || _name == "Yale")
        {
            CG_pin.DOFade(0, tweenTime);
            CG_pin.blocksRaycasts = false;
            CG_BTPin.blocksRaycasts = false;
        }
        else
        {
            CG_pin.DOFade(1, tweenTime);
            CG_pin.blocksRaycasts = true;
            CG_BTPin.blocksRaycasts = true;

            CG_ShowQR.DOFade(0, tweenTime);
            CG_ShowQR.blocksRaycasts = false;
            CG_UseCard.DOFade(0, tweenTime);
            CG_UseCard.blocksRaycasts = false;
        }
        //---showQR, useCard
        if (_name == "Vaccine" || _name == "MembershipCard" || _name == "BoardingPass")
        {
            CG_ShowQR.DOFade(1, tweenTime);
            CG_ShowQR.blocksRaycasts = true;
            CG_UseCard.DOFade(0, tweenTime);
            CG_UseCard.blocksRaycasts = false;
            CG_Door.DOFade(0, tweenTime);
        }
        else if (_name == "Ticket")
        {
            CG_UseCard.DOFade(1, tweenTime);
            CG_UseCard.blocksRaycasts = true;
            CG_ShowQR.DOFade(0, tweenTime);
            CG_ShowQR.blocksRaycasts = false;
            CG_Door.DOFade(0, tweenTime);
        }
        else if (_name == "Genesis" || _name == "Yale")
        {
            CG_UseCard.DOFade(0, tweenTime);
            CG_UseCard.blocksRaycasts = false;
            CG_ShowQR.DOFade(0, tweenTime);
            CG_ShowQR.blocksRaycasts = false;
            CG_Door.DOFade(1, tweenTime);
        }
        else
        {
            CG_UseCard.DOFade(0, tweenTime);
            CG_UseCard.blocksRaycasts = false;
            CG_ShowQR.DOFade(0, tweenTime);
            CG_ShowQR.blocksRaycasts = false;
            CG_Door.DOFade(0, tweenTime);
        }

        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i != _myindex)
            {
                List_Card[i].BT.enabled = false;
            }
            else
            {
                List_Card[i].BT.enabled = true;
            }
        }
    }
    //----------------edit 색 변하게 해주는 기능
    [SerializeField] Image Img_Edit;
    // 토글형태의 함수이며 isopen의 참거짓을 기준을 삼고있다.
    // dragValue(Movepos)이 0.5이상이 안되었을 때 작동하게 한다
    // 드래그 하는도중에 다시 원래위치로 돌아가게 하는 함수다.
    void cardNotMove()
    {
        if (isopen)
        {
            for (int i = 0; i < List_Card.Count; i++)
            {
                List_Card[i].RT.DORotate(Vector2.zero, tweenTime);
                List_Card[i].RT.SetAsFirstSibling();
                List_target[i].anchoredPosition = new Vector2(0, List_openpos[i]);
            }
            if (!MB.IsOpen)
                CG_SamsungTitle.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            else
                RT_MembershipHolder.DOAnchorPosY(667, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            EditWallet.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            BotButtons.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
            BotButtons.CG.blocksRaycasts = false;

            CG_pin.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
        }
        else
        {
            int count = 0;
            for (int i = 0; i < List_Card.Count; i++)
            {
                int idx = i;
                if (i >= Myindex)
                {
                    List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    count++;
                }
                else
                {
                    List_Card[i].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT.DOAnchorPosY(-700, tweenTime).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
                    {
                        List_Card[idx].RT.anchoredPosition = new Vector2(horArrival2, List_closepos[0]);
                        List_Card[idx].RT.localScale = new Vector2(0.8f, 0.8f);
                        List_Card[idx].CG.alpha = 0;

                        if (Myindex - 1 >= 0)
                        {
                            List_Card[Myindex - 1].CG.alpha = 0.45f;
                        }
                        List_Card[idx].RT.DOAnchorPosX(horArrival, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    });
                }
            }
            if (!MB.IsOpen)
            {
                CG_SamsungTitle.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                MemberCoupon.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_Out_100);
            }
            else
            {
                for (int j = 0; j < MB.Objects.Count; j++)
                {
                    MB.Objects[j].DOAnchorPosY(MB.List_mypos[j], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
            }
            EditWallet.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
            BotButtons.CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);

            BenefitApear(Myindex);
            RT_Scrollcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            smoothcontainer.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        }
    }
    void waveAnimation(float _mov)
    {
        if (isopen)
        {
            if (_mov < 0 && isTouch)
            {
                Ani_Controller.Play("Anim", 0, 0);
                isWave = true;
            }
        }
    }
    //애니메이션을 실행시켜준다
    void waveAnimation1()
    {
        if (isopen)
        {
            Ani_Controller.Play("Anim", 0, 0);
            isWave = true;
        }
    }

    //--------------------------------------------------------------------------------------------------------------

    // Pay모드로 들어갈때 홈 화면에서 바뀌어야 하는 것들이 바뀌게 해주는 스크립트다. 실제로 이 스크립트가 실행되는 곳을 PinEvent안에 있는 스크립트에서 작동된다.
    public void ToPayMode()
    {
        istweeening = true;
        plus.BT_Plus1.gameObject.SetActive(false);
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i != Myindex)
            {
                if (i > Myindex)
                {
                    if (!MB.IsOpen)
                    {
                        List_Card[i].RT.DOAnchorPosY(List_closepos[0] - 120, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    }
                    else
                    {
                        List_Card[i].RT.DOAnchorPosY(784, tweenTime).SetEase(Ease.InOutQuart);
                    }
                    List_Card[i].RT.DORotate(new Vector3(0, 0, -90), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
                else
                {
                    List_Card[i].RT.DOAnchorPosX(horArrival2, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
            }
            else
            {
                List_Card[i].RT.DORotate(new Vector3(0, 0, -90), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                if (!MB.IsOpen)
                {
                    List_Card[i].RT.DOAnchorPosY(List_closepos[0] - 120, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                    List_Card[i].RT.DOScale(new Vector2(1.115f, 1.115f), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                }
                else
                {
                    List_Card[i].RT.DOScale(Vector2.one, tweenTime).SetEase(Ease.InOutQuart);
                    List_Card[i].RT.DOAnchorPosY(784, tweenTime).SetEase(Ease.InOutQuart);
                }
            }
        }
        if (!MB.IsOpen)
            CG_SamsungTitle.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        else
        {
            MemberCoupon.CG.DOFade(0, tweenTime);
        }
        MemberCoupon.RT.DOAnchorPos(new Vector2(7.5f, -657), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        MemberCoupon.RT.transform.GetChild(0).GetComponent<Image>().DOColor(Color.white, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_pin.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        BotButtons.CG.DOFade(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        BotButtons.CG.blocksRaycasts = false;
        BotButtons.RT.DOAnchorPosY(-354, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Img_BG.DOColor(Color.black, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Img_Navi.DOColor(Color.white, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.RT.DOAnchorPosY(BeneClose, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.go.SetActive(false);
    }
    [SerializeField] Color colorWhite;
    [SerializeField] Color colorGray;
    [SerializeField] Color colormemberGray;
    // pay 모드에서 다시 홈을 돌아갈 때 카드들과 다른 오브젝트들이 원래 자리로 돌아가게 해주는 함수다. 
    // 실제로 이 함수가 실행되는 곳으 PinEvent안에서 작동된다.
    public void BacktoHome()
    {
        int count = 0;
        for (int i = 0; i < List_Card.Count; i++)
        {
            if (i >= Myindex)
            {
                List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                List_Card[i].RT.DORotate(new Vector3(0, 0, 0), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                List_Card[i].RT.DOScale(new Vector2(CardSize, CardSize), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                List_Card[i].CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                count++;
            }
            else
            {
                List_Card[i].RT.DOAnchorPosX(horArrival, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
                List_Card[i].RT.DOAnchorPosY(List_closepos[count], tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            }
        }
        if (!MB.IsOpen)
        {
            CG_SamsungTitle.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            plus.BT_Plus1.gameObject.SetActive(true);
        }
        else
        {
            RT_MembershipHolder.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
            plus.BT_Plus1.gameObject.SetActive(false);
        }
        istweeening = false;
        MemberCoupon.RT.DOAnchorPos(new Vector2(99f, -713), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        MemberCoupon.RT.transform.GetChild(0).GetComponent<Image>().DOColor(colormemberGray, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.RT.DORotate(new Vector3(0, 0, 0), tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        BotButtons.CG.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        BotButtons.CG.blocksRaycasts = true;
        BotButtons.RT.DOAnchorPosY(0, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Img_BG.DOColor(colorWhite, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Detail_Title.Img.DOColor(colorWhite, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Img_Navi.DOColor(colorGray, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        istweeening = false;
        MB.CG_pinModeLogo.DOFade(1, tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        Benefit.RT.DORotate(new Vector3(0, 0, 0), tweenTime).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
        {
            Benefit.go.SetActive(true);
            BenefitApear(Myindex);
        });
    }

    // Update is called once per frame
    float timesss = 0;
    void Update()
    {
        if (!isTouch && !isWave)
        {
            if (RT_Scrollcontainer.anchoredPosition.y > 0f)
            {
                waveAnimation1();
            }
        }
        if (isdetailopen && MemberCoupon.CG.alpha != 0)
        {
            MemberCoupon.CG.alpha = 0;
        }

        if (isopen && MemberCoupon.CG.alpha != 0)
        {
            MemberCoupon.CG.alpha = 0;
        }
        if (isTouch)
            time = time + Time.deltaTime;
    }
    public RectTransform smoothcontainer;
    [SerializeField] CanvasGroup CG_Indicator;
}