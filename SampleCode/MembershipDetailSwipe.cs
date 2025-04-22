using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MembershipDetailSwipe : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    float movpos, movpos2;
    public ScrollRect SC_ScrollCantainer;
    public CardDrag CD;
    [SerializeField] bool isTouch;

    public int step = 0;
    [SerializeField] TweenObject Container, DetailTile, Card, DetailPage, TempTitle, BackButton;

    void Start()
    {
        BackButton.Btn.onClick.AddListener(OutMeberShipDetail);
    }
    bool isScrollContactBottom;
    public bool isSnaped;
    float Value;
    float movAmount;
    public void OnPointerDown(PointerEventData e)
    {
        isTouch = true;
    }
    public void AppearCard()
    {
        StopTweening();
        Card.CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        Card.RT.DOScale(new Vector2(1, 1), 0.3f).SetEase(Model.Inst.Ease_Out_100);
        TempTitle.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
    }
    public void DisappearCard()
    {
        StopTweening();
        Card.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        Card.RT.DOScale(new Vector2(0, 0f), 0.3f).SetEase(Model.Inst.Ease_Out_100);

        TempTitle.CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
    }
    public void OnBeginDrag(PointerEventData e)
    {
        if (Container.RT.anchoredPosition.y <= 0)
        {
            movAmount = e.pressPosition.y;
        }
        if (!isSnaped)
        {
            if (e.delta.y > 0)
            {
                DetailTile.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
            }
        }
        else if (isSnaped)
        {
            if (e.delta.y < 0)
            {
                DetailTile.CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
            }
        }
    }
    public void GoToMebershipDetail()
    {
        DetailPage.RT.DOAnchorPosY(0, 0.2f).SetEase(Model.Inst.Ease_Out_100);
        DetailPage.CG.DOFade(1, 0.2f).OnComplete(() =>
        {
            DetailPage.CG.blocksRaycasts = true;
        });
        CD.MyStatus = Status.membershipDetail;
    }
    public void OutMeberShipDetail()
    {
        DetailPage.RT.DOAnchorPosY(-461, 0.2f).SetEase(Model.Inst.Ease_Out_100);
        DetailPage.CG.DOFade(0, 0.2f).OnComplete(() =>
        {
            Container.RT.anchoredPosition = new Vector2(0, 0);
            TempTitle.CG.alpha = 0;
            DetailTile.CG.alpha = 1;
            isSnaped = false;
            movAmount = 0;
            Value = 0;
            Card.RT.localScale = new Vector3(1,1,1);
            Card.CG.alpha = 1;
            step = 0;
        });
        CD.MyStatus = Status.CouponListOpen;
        DetailPage.CG.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData e)
    {
        if (Container.RT.anchoredPosition.y > 0)
        {
            movAmount = e.position.y;
        }
        movpos = -e.position.y + movAmount;

        Value = Mathf.Clamp(movpos / 530, -1, 1);
        if (Container.RT.anchoredPosition.y <= 0)
        {
            if (Value >= 0)
            {
                SC_ScrollCantainer.enabled = false;
            }
            else
                SC_ScrollCantainer.enabled = true;
        }
        else
        {
            ValueDrag(movpos2);
        }
        movpos2 = e.pressPosition.y - e.position.y;

    }

    public void ValueDrag(float _movpos)
    {
        if (!isSnaped)
        {
            float value = Mathf.Clamp(-0.5f - movpos2 / 450, 0, 1);

            Card.CG.alpha = 1 - value;
            Card.RT.localScale = new Vector2(1 - value, 1 - value);
            // CG_DetailTitle.alpha = 1 - value;
            // RT_DetailTitle_content.localScale = new Vector2(1 - 0.85f * value, 1 - 0.85f * value);
            TempTitle.CG.alpha = value;
        }
        else if (isSnaped)
        {
            if (step != 2 && step != 3)
            {
                float value = Mathf.Clamp(-0.5f + movpos2 / 450, 0, 1);
                Card.CG.alpha = value;
                Card.RT.localScale = new Vector2(value, value);
                // CG_DetailTitle.alpha = value;
                // RT_DetailTitle_content.localScale = new Vector2(value, value);
                TempTitle.CG.alpha = 1 - value;
            }

        }
    }


    public void OnEndDrag(PointerEventData e)
    {
    }

    public void OnPointerUp(PointerEventData e)
    {
        SC_ScrollCantainer.enabled = true;

        if (Card.RT.anchoredPosition.y <= 0)
        {

        }
        else
        {
            SnapDetail();
        }

        isTouch = false;

        movpos = 0;
        movpos2 = 0;
        movAmount = 0;
    }
    void StopTweening()
    {
        Card.CG.DOKill();
        Card.RT.DOKill();
        // RT_DetailTitle_content.DOKill();
        // CG_DetailTitle.DOKill();
    }
    void SnapDetail()
    {
        if (step == 0)
        {
            if (movpos2 < 0)
            {
                SC_ScrollCantainer.enabled = false;
                Container.RT.DOAnchorPosY(798, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                {
                    SC_ScrollCantainer.enabled = true;
                });
                DisappearCard();
                isSnaped = true;
                step++;
            }
            else
            {
                SC_ScrollCantainer.enabled = false;
                Container.RT.DOAnchorPosY(0, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                {
                    SC_ScrollCantainer.enabled = true;
                });
                isSnaped = false;
                AppearCard();
            }
        }
        else if (step == 1)
        {
            if (movpos2 < 0)
            {
                step++;
            }
            else
            {
                SC_ScrollCantainer.enabled = false;
                Container.RT.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                {
                    SC_ScrollCantainer.enabled = true;
                });
                AppearCard();
                isSnaped = false;
                step--;
            }
        }
        else
        {
            if (movpos2 > 0)
            {
                SC_ScrollCantainer.enabled = false;
                Container.RT.DOAnchorPosY(798, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                {
                    SC_ScrollCantainer.enabled = true;
                });
                step--;
            }
        }
    }
    bool isTabSettleIn;
    void Update()
    {
    }
}