using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;


[Serializable]
public class EditItem
{
    public RectTransform RT;
    public int Idx;
    public LongPressHandler LongPress;
    public CanvasGroup CG_shadow;
    public CanvasGroup CG;
    public Toggle Toggle;
}
public class LongPressNEditHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Model model;
    [SerializeField] CardDrag cardDrag;
    [SerializeField] List<GameObject> gm_lists;
    public List<EditItem> List_EI;
    [SerializeField] Vector2 movpos;
    [SerializeField] RectTransform RT_Divider;
    public EditItem target_Item;
    public List<Sprite> ListResortCard;
    private float startPosY;
    [SerializeField] int BarAmount, Myindex;
    [SerializeField] Text CardCount;

    public List<EditItem> List_TempEI;
    public List<Sprite> List_TempSprites;
    public int tempCount;
    bool isEditMode;
    void Start()
    {
        ListResortCard = cardDrag.ListReSortCard;
        Init();
        for (int i = cardDrag.List_Card.Count - 1; i < List_EI.Count; i++)
        {
            List_EI[i].Toggle.isOn = true;
        }
        Init2();
    }
    public void Init()
    {
        for (var i = 0; i < gm_lists.Count; i++)
        {
            int my = i;
            EditItem prefb = new EditItem
            {
                RT = gm_lists[i].GetComponent<RectTransform>(),
                Idx = my,
                LongPress = gm_lists[i].GetComponent<LongPressHandler>(),
                CG_shadow = gm_lists[i].transform.GetChild(0).GetComponent<CanvasGroup>(),
                CG = gm_lists[i].GetComponent<CanvasGroup>(),
                Toggle = gm_lists[i].transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).GetComponent<Toggle>()
            };
            List_EI.Add(prefb);
        }
        for (int i = 0; i < List_EI.Count; i++)
        {
            int my = i;
            List_EI[i].LongPress.onLongPress.AddListener(() => startDrag(List_EI[my].Idx));
            List_EI[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
        }
    }
    public void Init2()
    {
        for (int i = 0; i < List_EI.Count; i++)
        {
            List_EI[i].Toggle.onValueChanged.RemoveAllListeners();
            int idx = i;
            List_EI[i].Toggle.onValueChanged.AddListener((bool isbool) =>
            {
                ToggleEvent(List_EI[idx].Toggle.isOn, idx); // 카드를 옮겨주는 역할
                Init2();
            });
            List_EI[i].Toggle.onValueChanged.AddListener(ExamineCardResorting);
        }
        int number = 0;
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (!List_EI[i].Toggle.isOn)
            {
                number++;
            }
        }
        CardCount.text = "(" + number + ")";
    }
    public void ToggleEvent(bool _is, int _idx)
    {
        int count = 0;
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (!List_EI[i].Toggle.isOn)
                count++;
        }
        EditItem prefb = List_EI[_idx];
        Sprite img = model.ListAllSprite[_idx];

        List_EI.RemoveAt(_idx);
        model.ListAllSprite.RemoveAt(_idx);
        if (!_is)
        {
            List_EI.Insert(count - 1, prefb);
            model.ListAllSprite.Insert(count - 1, img);
        }
        else
        {
            List_EI.Insert(List_EI.Count, prefb);
            model.ListAllSprite.Insert(model.ListAllSprite.Count, img);
        }
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (i < count)
                List_EI[i].RT.DOAnchorPosY(1333 - 288 * i, 0.2f).SetEase(Ease.InOutQuad);
            else
                List_EI[i].RT.DOAnchorPosY(1333 - 288 * i - 171, 0.2f).SetEase(Ease.InOutQuad);
            List_EI[i].Toggle.onValueChanged.RemoveAllListeners();
        }
        RT_Divider.DOAnchorPosY(1333 - 288 * (count - 1) - 248, 0.2f).SetEase(Ease.InOutQuad);

        for (int i = 0; i < List_EI.Count; i++)
        {
            int my = i;
            List_EI[i].Idx = my;

            List_EI[i].LongPress.onLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onLongPress.AddListener(() => startDrag(List_EI[my].Idx));
            List_EI[i].LongPress.onEndLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
        }
        prefb = null;
    }
    void ExamineCardResorting(bool _value)
    {
        ListResortCard.Clear();
        int idx = 0;
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (!List_EI[i].Toggle.isOn)
            {
                idx++;
            }
        }
        for (int i = 0; i < idx; i++)
        {
            ListResortCard.Add(Model.Inst.ListAllSprite[i]);
        }
    }
    public void OnBeginDrag(PointerEventData e)
    {

    }
    public void OnDrag(PointerEventData e)
    {
        if (isEditMode)
        {
            movpos = e.pressPosition - e.position;
            dragCard();
            knowBar(movpos.y);
        }
    }
    public void OnEndDrag(PointerEventData e)
    {
        if (isEditMode)
        {
            EndDrag();
        }
    }
    [SerializeField] int _Count;
    void knowBar(float _movpos) // 지정한 카드가 원래 지점에서 몇 *칸* 을 이동했는지 알아보기 
    {
        if (-_movpos > 0)
        {
            int idx = 0;
            for (int i = 0; i < Myindex + 1; i++)
            {
                if (-_movpos >= idx * 288 && -_movpos <= idx * 288 + 144)
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
        else
        {
            int idx = 0;
            for (int i = 0; i < _Count - Myindex; i++)
            {
                if (_movpos >= idx * 288 && _movpos <= idx * 288 + 144)
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
    }
    void dragCard() // 지정한 카드 움직이게 하기
    {
        if (target_Item != null)
        {
            target_Item.RT.anchoredPosition = new Vector2(0, startPosY) - movpos;
        }
    }
    void cardPositioning() //지정한 카드가 움직이는 동안 다른 카드들이 자기 자리 가게 하기
    {
        int cnt = 0;
        for (int i = 0; i < _Count; i++)
        {
            if (i != Myindex)
            {
                if (cnt < Myindex + BarAmount)
                {
                    List_EI[i].RT.DOAnchorPosY(1333 - 288 * cnt, 0.2f).SetEase(model.Ease_InOut20_100);
                }
                else
                {
                    List_EI[i].RT.DOAnchorPosY(1333 - 288 * cnt - 288, 0.2f).SetEase(model.Ease_InOut20_100);
                }
                cnt++;
            }
        }
    }
    void startDrag(int idx)// 카드를 오래누르면 생기는 이벤트 시작하기
    {
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (!List_EI[i].Toggle.isOn)
                _Count++;
        }

        isEditMode = true;
        target_Item = List_EI[idx];
        target_Item.RT.transform.SetAsLastSibling();
        startPosY = 1333 - 288 * idx;
        Myindex = idx;
        target_Item.CG_shadow.DOFade(1, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);

        cardPositioning();
    }
    void EndDrag() //카드 놨을 때
    {
        target_Item.CG_shadow.DOFade(0, 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
        List_EI.RemoveAt(Myindex);
        List_EI.Insert(Myindex + BarAmount, target_Item);

        Sprite sample = cardDrag.ListReSortCard[Myindex];
        
        model.ListAllSprite.RemoveAt(Myindex);
        model.ListAllSprite.Insert(Myindex + BarAmount, sample);

        cardDrag.ListReSortCard.RemoveAt(Myindex);
        cardDrag.ListReSortCard.Insert(Myindex + BarAmount, sample);

        for (int i = 0; i < List_EI.Count; i++)
        {
            int my = i;
            if (i < _Count)
                List_EI[i].RT.DOAnchorPos(new Vector2(0, 1333 - 288 * i), 0.2f).SetEase(Model.Inst.Ease_InOut20_100);
            List_EI[i].Idx = my;

            List_EI[i].LongPress.onLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onLongPress.AddListener(() => startDrag(my));
            List_EI[i].LongPress.onEndLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
        }

        Init2();
        target_Item = null;
        isEditMode = false;
        BarAmount = 0;
        _Count = 0;
    }
    public void MemorytheArray() //(1)
    {
        int count = 0;
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (!List_EI[i].Toggle.isOn)
                count++;
        }
        for (int i = 0; i < count; i++)
        {
            ListResortCard.Add(Model.Inst.ListAllSprite[i]);
        }
        for (int i = 0; i < Model.Inst.ListAllSprite.Count; i++)
        {
            List_TempSprites.Add(Model.Inst.ListAllSprite[i]);
        }

        for (int i = 0; i < List_EI.Count; i++)
        {
            List_TempEI.Add(List_EI[i]);
        }
    }
    public void EndDrag2(int _idx, int _idx2) //카드 놨을 때 mainpage에서 쓰임
    {
        EditItem prefb = List_EI[_idx];
        List_EI.RemoveAt(_idx);
        List_EI.Insert(_idx2, prefb);

        for (int i = 0; i < List_EI.Count; i++)
        {
            int my = i;
            List_EI[i].RT.anchoredPosition = new Vector2(0, 1333 - 288 * i);
            List_EI[i].Idx = my;

            List_EI[i].LongPress.onLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onLongPress.AddListener(() => startDrag(List_EI[my].Idx));
            List_EI[i].LongPress.onEndLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
        }
        Init2();
        prefb = null;
        BarAmount = 0;
    }
    public void ResetEditItemArray()
    {
        List_EI.Clear();
        model.ListAllSprite.Clear();
        for (int i = 0; i < List_TempEI.Count; i++)
        {
            List_EI.Add(List_TempEI[i]);
        }
        for (int i = 0; i < List_TempSprites.Count; i++)
        {
            model.ListAllSprite.Add(List_TempSprites[i]);
        }
        for (int i = 0; i < List_EI.Count; i++)
        {
            List_EI[i].Toggle.onValueChanged.RemoveAllListeners();
            if (i < tempCount)
                List_EI[i].Toggle.isOn = false;
            else
                List_EI[i].Toggle.isOn = true;
        }
        for (int i = 0; i < List_EI.Count; i++)
        {
            if (i < tempCount)
                List_EI[i].RT.anchoredPosition = new Vector2(0, 1333 - 288 * i);
            else
                List_EI[i].RT.anchoredPosition = new Vector2(0, 1333 - 288 * i - 171);
        }
        RT_Divider.anchoredPosition = new Vector2(0, 1333 - 288 * (tempCount - 1) - 248);

        Init2();
        for (int i = 0; i < List_EI.Count; i++)
        {
            int my = i;
            List_EI[i].Idx = my;

            List_EI[i].LongPress.onLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onLongPress.AddListener(() => startDrag(List_EI[my].Idx));
            List_EI[i].LongPress.onEndLongPress.RemoveAllListeners();
            List_EI[i].LongPress.onEndLongPress.AddListener(() => EndDrag());
        }
        List_TempEI.Clear();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
