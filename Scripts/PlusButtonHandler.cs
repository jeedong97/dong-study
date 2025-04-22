using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlusButtonHandler : MonoBehaviour
{
    public Button BT_Plus1, BT_Plus2, BackButton;
    [SerializeField] CanvasGroup CG_PlusPage;
    [SerializeField] CardDrag CD;
    [SerializeField] TabBtnController TabContol;
    [SerializeField] RectTransform Content;
    [SerializeField] ScrollRect sc_mask;
    // Start is called before the first frame update
    void Start()
    {
        BT_Plus1.onClick.AddListener(PressPlusButton);
        BT_Plus2.onClick.AddListener(PressPlusButton);
        BackButton.onClick.AddListener(BackPlusPage);
    }

    public void PressPlusButton()
    {
        CG_PlusPage.DOFade(1, 0.2f).OnComplete(() =>
        {
            TabContol.Quick_QuickAcessEvent();
        });
        CD.BotButtons.CG.DOFade(0, 0.2f);
        CD.BotButtons.CG.blocksRaycasts = false;
        CD.MyStatus = Status.PlusPage;
        CG_PlusPage.blocksRaycasts = true;

    }
    public void BackPlusPage()
    {
        CG_PlusPage.DOFade(0, 0.2f);
        CG_PlusPage.blocksRaycasts = false;
        CD.BotButtons.CG.blocksRaycasts = true;
        BT_Plus1.enabled = true;
        CD.BotButtons.CG.DOFade(1, 0.2f).OnComplete(()=>
        {
            sc_mask.enabled = false;
            Content.anchoredPosition = Vector2.zero;
            sc_mask.enabled = true;
        });
        CD.MyStatus = Status.CardListClose;
    }
    void Update()
    {

    }
}
