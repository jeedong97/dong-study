using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinEvent : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [Header("페이모드를 위한 세팅-------------")]
    [SerializeField] float repeatTime;
    public bool AbleToCount, isPinOpen;
    public float TimeCount;
    public int Count, EndCount;
    public CardDrag CD;
    public Membership_new_Handler MB;
    public CanvasGroup CG_Pinanim;
    public GameObject GO_BloomItemPref;
    public GameObject PinContainer;
    public Text TX_count;
    public RectTransform RT_memberButton;
    public CanvasGroup CG_Arrow, CG_Cross;
    List<BloomHandler> List_bloom;
    public CanvasGroup CG_CompleteBtn, CG_CompleteText, CG_TutorialText, CG_CountBox;

    void Start()
    {
        List_bloom = new List<BloomHandler>();
        giveKeyPadEvent();
        BT_PinButton.onClick.AddListener(AppearKeyPad);
    }
    // pin을 0.5초 이상 눌렀을 때 bTPinEvent가 실행이 된다.

    public void OnPointerDown(PointerEventData e)
    {
        CancelInvoke("bTPinEventToPayMode");
        if (CD.List_Card[CD.Myindex].Img.sprite.name != "ID" && CD.List_Card[CD.Myindex].Img.sprite.name != "Genesis" && CD.List_Card[CD.Myindex].Img.sprite.name != "StudentID" && CD.List_Card[CD.Myindex].Img.sprite.name != "Vaccine")
            Invoke("bTPinEventToPayMode", 0.5f);
    }

    public void OnBeginDrag(PointerEventData e)
    {

    }
    // 약간 움직이는 것까지는 허용하지만 어느 일정 이상 움직임이 감지가 되었다면 0.5초후에 실행될 btpinEvent는 실행이 취소가 된다. 

    public void OnDrag(PointerEventData e)
    {
        if ((Mathf.Abs(e.pressPosition.x - e.position.x) > 100) || (Mathf.Abs(e.pressPosition.y - e.position.y) > 100))
        {
            CancelInvoke("bTPinEventToPayMode");
        }
    }

    public void OnEndDrag(PointerEventData e)
    {

    }
    // 0.5초가 되기전에 손가락을 pin에서 떼어내게 된다면 bTPinEvent는 실행이 취소가 된다.

    public void OnPointerUp(PointerEventData e)
    {
        CancelInvoke("bTPinEventToPayMode");
    }
    String PassWord = "000000";
    [SerializeField] RectTransform RT_KeyPad, RT_DotContainer;
    [SerializeField] CanvasGroup CG_PassWordPage, CG_Text, CG_TextWhenWrong;
    [SerializeField] List<Image> List_DotColor;
    [SerializeField] List<Button> List_KeyPads;
    [SerializeField] Button BT_KeyBack, BT_PinButton;
    [SerializeField] Color Typed, noTyped;
    int passWordIdx = 0;
    List<string> typing = new List<string>();
    void giveKeyPadEvent()
    {

        for (int i = 0; i < List_KeyPads.Count; i++)
        {
            int idx;
            idx = i;
            List_KeyPads[i].onClick.AddListener(() =>
            {
                KeyPadEvent(idx);
            });
        }
        BT_KeyBack.onClick.AddListener(() =>
        {
            if (passWordIdx != 0)
                passWordIdx--;

            for (int i = passWordIdx; i < List_DotColor.Count; i++)
            {
                List_DotColor[i].DOColor(noTyped, 0.1f);
            }
            if (typing.Count != 0)
                typing.RemoveAt(typing.Count - 1);
        });
    }
    void KeyPadEvent(int _idx)
    {
        passWordIdx = Mathf.Clamp(passWordIdx, 0, 5);
        List_DotColor[passWordIdx].DOColor(Typed, 0.2f).OnComplete(() =>
       {
           typing.Add(_idx.ToString());
           if (typing.Count >= 6)
           {
               string _password = "";
               for (int i = 0; i < typing.Count; i++)
               {
                   _password += typing[i];
               }
               if (_password == PassWord)
               {
                   CG_Text.DOFade(1, 0.1f);
                   CG_TextWhenWrong.DOFade(0, 0.1f);
                   CG_PassWordPage.DOFade(0, 0.2f);
                   CG_PassWordPage.blocksRaycasts = false;
                   RT_KeyPad.DOAnchorPosY(-1710, 0.2f).SetEase(Model.Inst.Ease_Out_100);
                   for (int i = 0; i < List_DotColor.Count; i++)
                   {
                       List_DotColor[i].DOKill();
                       List_DotColor[i].color = noTyped;
                   }
                   bTPinEventToPayMode();
                   Debug.Log("Access");
               }
               else
               {
                   DotAnimation(150, 0.07f);
                   CG_Text.DOFade(0, 0.1f);
                   CG_TextWhenWrong.DOFade(1, 0.1f);
                   Debug.Log("Wrong");
               }

               passWordIdx = 0;
               typing.Clear();
           }
       });
        passWordIdx++;
    }
    void DotAnimation(float _pos, float _tween)
    {
        RT_DotContainer.DOAnchorPosX(-_pos, _tween).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            RT_DotContainer.DOAnchorPosX(_pos - (_pos / 6), _tween).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        RT_DotContainer.DOAnchorPosX(-_pos + (_pos / 6) * 2, _tween*1.01f).SetEase(Ease.InOutSine).OnComplete(() =>
                            {
                                RT_DotContainer.DOAnchorPosX(_pos - (_pos / 6) * 4f, _tween*1.2f).SetEase(Ease.InOutSine).OnComplete(() =>
                                   {
                                       RT_DotContainer.DOAnchorPosX(-_pos + (_pos / 6) * 5f, _tween*1.8f).SetEase(Ease.InOutSine).OnComplete(() =>
                                    {
                                        RT_DotContainer.DOAnchorPosX(0, _tween*2f).SetEase(Ease.InOutSine);
                                    });
                                   });
                            });
                    });
        });
        int count = 0;
        for (int i = List_DotColor.Count - 1; i >= 0; i--)
        {
            List_DotColor[i].DOKill();
            List_DotColor[i].DOColor(noTyped, 0.08f).SetDelay(0.02f * count);
            count++;
        }
    }
    void AppearKeyPad()
    {
        if (CD.List_Card[CD.Myindex].Img.sprite.name != "ID" && CD.List_Card[CD.Myindex].Img.sprite.name != "Genesis" && CD.List_Card[CD.Myindex].Img.sprite.name != "StudentID" && CD.List_Card[CD.Myindex].Img.sprite.name != "Vaccine")
        {
            CD.MyStatus = Status.KeyPad;
            CG_PassWordPage.DOFade(1, 0.2f);
            CG_PassWordPage.blocksRaycasts = true;
            RT_KeyPad.DOAnchorPosY(-694.5f, 0.2f).SetEase(Model.Inst.Ease_Out_100);
        }
    }
    public void DisappearKeyPad()
    {
        CD.MyStatus = Status.CardListClose;
        CG_TextWhenWrong.DOFade(0, 0.1f);
        CG_Text.DOFade(1, 0.1f);
        CG_PassWordPage.DOFade(0, 0.2f);
        RT_KeyPad.DOAnchorPosY(-1710f, 0.2f).SetEase(Model.Inst.Ease_Out_100);
        CG_PassWordPage.blocksRaycasts = false;
        for (int i = 0; i < List_DotColor.Count; i++)
        {
            List_DotColor[i].DOKill();
            List_DotColor[i].color = noTyped;
        }
        passWordIdx = 0;
        typing.Clear();
    }

    // 홈상태에서 pay모드로 진입하게 해주는 이벤트다. 0.4초후에 뒤에서 파장이 발생하게 해주며 CardDrag 스크립트에 있는 함수를 가져와서 실행시킨다.
    void bTPinEventToPayMode()
    {
        CD.MyStatus = Status.PayMode;
        AbleToCount = true;
        isPinOpen = true;
        CD.ToPayMode();
        if (MB.IsOpen)
            MB.CG_pinModeLogo.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CancelInvoke("bloomAnimation");
        Invoke("bloomAnimation", 0.4f);
        RT_memberButton.DOAnchorPos(new Vector2(0, -627), CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Pinanim.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Arrow.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Cross.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CD.enabled = false;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].LongPress.enabled = false;
        }
    }

    // BloomHandler라는 스크립트를 가지고 있는 게임오브젝트를 Instantiate 공정을 거쳐주고 item이라는 변수에 넣어준다
    // item은 생성되자마자 본인이 가지고 있는 스크립트에 Start()함수가 실행이 된다. 그리곤 list_bloom이라는 리스트에 들어가게 된다.
    // 동시에 update쪽에서 시간이 흐르면서 카운트 다운이 시작되고 bloomanimation이 지금 시간의 여부에 따라 실행되고 안되고를 결정한다.
    // 시간이 다되면 backhome 함수가 실행이 되며 pay모드에서 home모드로 돌아가게 된다.
    void bloomAnimation()
    {
        if (EndCount < Count)
        {
            BloomHandler item = Instantiate(GO_BloomItemPref).GetComponent<BloomHandler>();
            item.transform.SetParent(PinContainer.transform);
            item.gameObject.SetActive(true);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 592);
            item.Init();

            List_bloom.Add(item);
            CancelInvoke("bloomAnimation");
            Invoke("bloomAnimation", repeatTime);

        }
    }

    // 결제가 완료되는 시간에 맞춰 홈으로 가기 위한 준비동작.
    void payModeToHome()
    {
        CG_CompleteBtn.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
        CG_CompleteText.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
        CG_TutorialText.DOFade(0, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
        CG_CountBox.DOFade(0, 0.2f).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
        {
            CG_CompleteBtn.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100).SetDelay(2.3f);
            CG_CompleteText.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100).SetDelay(2.3f);
        });
        CD.enabled = true;
        Invoke("BacktoHome", 2.6f + 0.2f);
    }

    // 홈으로 돌아가는 모션 및 리셋.
    public void BacktoHome()
    {
        CancelInvoke("bloomAnimation");

        Count = EndCount;
        AbleToCount = false;

        CG_CompleteBtn.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
        CG_CompleteText.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_InOut20_100);
        isPinOpen = false;
        CG_Pinanim.DOFade(0, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        RT_memberButton.DOAnchorPos(new Vector2(91.5f, -789), CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Arrow.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100);
        CG_Cross.DOFade(1, CD.tweenTime).SetEase(Model.Inst.Ease_InOut20_100).OnComplete(() =>
        {
            CG_TutorialText.DOFade(1, 0);
            CG_CountBox.DOFade(1, 0);
            CG_CompleteBtn.DOFade(0, 0);
            CG_CompleteText.DOFade(0, 0);
            CG_CompleteBtn.transform.gameObject.SetActive(true);
            CG_CompleteText.transform.gameObject.SetActive(true);
        });
        CD.BacktoHome();
        CD.enabled = true;
        for (int i = 0; i < CD.List_Card.Count; i++)
        {
            CD.List_Card[i].LongPress.enabled = true;
        }
        TX_count.text = "20";
        if (MB.IsOpen)
            CD.MyStatus = Status.CouponListOpen;
        else
            CD.MyStatus = Status.CardListClose;
        Count = 20;
        TimeCount = 0;
        for (int i = 0; i < List_bloom.Count; i++)
        {
            Destroy(List_bloom[i].gameObject);
        }
        List_bloom.Clear();
    }
    void Update()
    {
        if (AbleToCount)
        {
            TimeCount += Time.deltaTime;
            if (TimeCount >= 1)
            {
                Count--;
                TX_count.text = Count.ToString();
                TimeCount = 0;
            }
            if (EndCount == Count)
            {
                AbleToCount = false;
                payModeToHome();
            }
        }
    }
}
