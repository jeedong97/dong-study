using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DetailSwipeDown : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    float movpos, movpos2;
    [SerializeField] RectTransform RT_ScrollContainer;
    public ScrollRect SC_ScrollCantainer;
    [SerializeField] CardDrag CD;
    [SerializeField] bool isTouch;

    public int step = 0;

    public TweenObject Temp_Title;
    [SerializeField] CanvasGroup CG_DetailTitle;
    public RectTransform RT_DetailTitle_content;
    public RectTransform RT_Detail_2Tab;
    public GameObject SecondDetailContent;

    [SerializeField] List<Sprite> List_TempTitle;

    void Start()
    {

    }
    bool isScrollContactBottom;
    public bool isSnaped;
    float Value;
    float movAmount;
    public void OnPointerDown(PointerEventData e)
    {
        isTouch = true;
        string _name = CD.List_Card[CD.Myindex].Img.sprite.name;
        if (CD.isdetailopen)
        {
            Sprite temp_img = List_TempTitle.Find(Sprite => Sprite.name == "Tmp_" + _name);
            if (temp_img != null)
            {
                Temp_Title.Img.sprite = temp_img;
            }
            else
            {
                Temp_Title.Img.sprite = List_TempTitle.Find(Sprite => Sprite.name == "Tmp_normalCard");
            }
            Temp_Title.Img.SetNativeSize();
        }
    }
    public void AppearCard()
    {
        StopTweening();
        CD.List_Card[CD.Myindex].CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        CD.List_Card[CD.Myindex].RT.DOScale(new Vector2(0.85f, 0.85f), 0.3f).SetEase(Model.Inst.Ease_Out_100);

        // CG_DetailTitle.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        // RT_DetailTitle_content.DOScale(new Vector2(1, 1f), 0.3f).SetEase(Model.Inst.Ease_Out_100);
        Temp_Title.CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
    }
    public void DisappearCard()
    {
        StopTweening();
        CD.List_Card[CD.Myindex].CG.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        CD.List_Card[CD.Myindex].RT.DOScale(new Vector2(0, 0f), 0.3f).SetEase(Model.Inst.Ease_Out_100);

        // CG_DetailTitle.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
        // RT_DetailTitle_content.DOScale(new Vector2(0, 0), 0.3f).SetEase(Model.Inst.Ease_Out_100);
        Temp_Title.CG.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
    }
    public void OnBeginDrag(PointerEventData e)
    {
        if (CD.isdetailopen)
        {
            if (RT_ScrollContainer.anchoredPosition.y <= Model.Inst.DetailLimitPosition)
            {
                movAmount = e.pressPosition.y;
            }
            if (!isSnaped)
            {
                if (e.delta.y > 0)
                {
                    CG_DetailTitle.DOFade(0, 0.3f).SetEase(Model.Inst.Ease_Out_100);
                }
            }
            else if (isSnaped)
            {
                if (e.delta.y < 0)
                {
                    CG_DetailTitle.DOFade(1, 0.3f).SetEase(Model.Inst.Ease_Out_100);
                }
            }
        }
    }
    public void OnDrag(PointerEventData e)
    {
        if (CD.isdetailopen)
        {
            if (RT_ScrollContainer.anchoredPosition.y > Model.Inst.DetailLimitPosition)
            {
                movAmount = e.position.y;
            }
            movpos = -e.position.y + movAmount;

            Value = Mathf.Clamp(movpos / 530, -1, 1);
            if (RT_ScrollContainer.anchoredPosition.y <= Model.Inst.DetailLimitPosition)
            {
                if (Value >= 0)
                {
                    CD.DragOutDetail(Value);
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
    }

    public void ValueDrag(float _movpos)
    {
        if (!isSnaped)
        {
            float value = Mathf.Clamp(-0.5f - movpos2 / 450, 0, 1);

            CD.List_Card[CD.Myindex].CG.alpha = 1 - value;
            CD.List_Card[CD.Myindex].RT.localScale = new Vector2(0.85f - 0.85f * value, 0.85f - 0.85f * value);
            // CG_DetailTitle.alpha = 1 - value;
            // RT_DetailTitle_content.localScale = new Vector2(1 - 0.85f * value, 1 - 0.85f * value);
            Temp_Title.CG.alpha = value;
        }
        else if (isSnaped)
        {
            if (step != 2 && step != 3)
            {
                float value = Mathf.Clamp(-0.5f + movpos2 / 450, 0, 1);
                CD.List_Card[CD.Myindex].CG.alpha = value;
                CD.List_Card[CD.Myindex].RT.localScale = new Vector2(0.85f * value, 0.85f * value);
                // CG_DetailTitle.alpha = value;
                // RT_DetailTitle_content.localScale = new Vector2(value, value);
                Temp_Title.CG.alpha = 1 - value;
            }

        }
    }


    public void OnEndDrag(PointerEventData e)
    {
    }

    public void OnPointerUp(PointerEventData e)
    {
        SC_ScrollCantainer.enabled = true;
        if (CD.isdetailopen)
        {
            if (RT_ScrollContainer.anchoredPosition.y <= Model.Inst.DetailLimitPosition)
            {
                if (Value > 0.3f)
                    CD.OutDetail();
                else
                    CD.gotoDetail();
            }
            else
            {
                SnapDetail();
            }
        }
        isTouch = false;

        movpos = 0;
        movpos2 = 0;
        movAmount = 0;
    }
    void StopTweening()
    {
        CD.List_Card[CD.Myindex].CG.DOKill();
        CD.List_Card[CD.Myindex].RT.DOKill();
        // RT_DetailTitle_content.DOKill();
        // CG_DetailTitle.DOKill();
    }
    void SnapDetail()
    {
        if (!CD.isdetailopen)
            return;

        string _name = CD.List_Card[CD.Myindex].Img.sprite.name;
        if (_name == "Samsung")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(843, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    RT_ScrollContainer.DOAnchorPosY(0, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1579, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    RT_ScrollContainer.DOAnchorPosY(843, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
        }
        else if (_name == "StudentID")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(770, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    DisappearCard();
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                }
            }
        }
        else if (_name == "Vaccine")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(876, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    DisappearCard();
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                }
            }
        }
        else if (_name == "Genesis")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(788, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    DisappearCard();
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                }
            }
        }
        else if (_name == "Yale")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(788, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    DisappearCard();
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                }
            }
        }
        else if (_name == "DigitalAsset")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(992, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = true;
                    DisappearCard();
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                }
            }
        }
        else if (_name == "BoardingPass")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(806, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    RT_ScrollContainer.DOAnchorPosY(0, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(2048, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    AppearCard();
                    isSnaped = false;
                    step--;
                }
            }
            else if (step == 2)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(3207, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(806, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
            else if (step == 3)
            {
                if (movpos2 < 0)
                {
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(2048, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
            else
            {
                if (movpos2 > 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(3207, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
        }
        else if (_name == "Ticket")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    if (movpos2 < -826)
                    {
                        RT_ScrollContainer.DOAnchorPosY(1228, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                        {
                            SC_ScrollCantainer.enabled = true;
                        });
                        step++;
                    }
                    else
                    {
                        RT_ScrollContainer.DOAnchorPosY(804, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                       {
                           SC_ScrollCantainer.enabled = true;
                       });
                    }
                    DisappearCard();
                    isSnaped = true;
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1228, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    AppearCard();
                    isSnaped = false;
                    step--;
                }
            }
            else if (step == 2)
            {
                if (movpos2 < 0)
                {
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(804, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
            else
            {
                if (movpos2 > 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1228, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
        }
        else if (_name != "Samsung" && _name != "Vaccine" && _name != "StudentID" && _name != "Genesis")
        {
            if (step == 0)
            {
                if (movpos2 < 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1139, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    RT_ScrollContainer.DOAnchorPosY(0, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
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
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1680, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step++;
                }
                else
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(0f, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    isSnaped = false;
                    AppearCard();
                    step--;
                }
            }
            else if (step == 2)
            {
                if (movpos2 < 0)
                {
                    step++;
                }
                else
                {
                    if (movpos2 > 0)
                    {
                        SC_ScrollCantainer.enabled = false;
                        RT_ScrollContainer.DOAnchorPosY(1139, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                        {
                            SC_ScrollCantainer.enabled = true;
                        });
                        step--;
                    }
                }
            }
            else
            {
                if (movpos2 > 0)
                {
                    SC_ScrollCantainer.enabled = false;
                    RT_ScrollContainer.DOAnchorPosY(1680, 0.3f).SetEase(Model.Inst.Ease_Out_100).OnComplete(() =>
                    {
                        SC_ScrollCantainer.enabled = true;
                    });
                    step--;
                }
            }
        }
    }
    bool isTabSettleIn;
    void Update()
    {
        if (CD.isdetailopen)
        {
            string _name = CD.List_Card[CD.Myindex].Img.sprite.name;

            if (_name != "Samsung" && _name != "Vaccine" && _name != "StudentID" && _name != "Genesis" &&
                _name != "Ticket"  && _name != "BoardingPass")
            {
                if (!isTabSettleIn && RT_ScrollContainer.anchoredPosition.y >= 1680)
                {
                    isTabSettleIn = true;
                    RT_Detail_2Tab.transform.SetParent(CD.RT_DetailBG.transform);
                    RT_Detail_2Tab.anchoredPosition = Vector2.zero;
                }
                else if (isTabSettleIn && RT_ScrollContainer.anchoredPosition.y < 1680)
                {
                    isTabSettleIn = false;
                    RT_Detail_2Tab.transform.SetParent(SecondDetailContent.transform);
                    RT_Detail_2Tab.anchoredPosition = Vector2.zero;
                }
            }
        }
    }
}