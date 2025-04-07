/*
Hierachy 구조

- PageHolder
-- HidePage
-- ShowPage

*/


using UnityEngine;

namespace com.hnine.framework
{
    public enum PageSwitchType
    {
        FromLeftToRight,
        FromRightToLeft,
        FromLeftToBack, // disappearing screen disappears as it shrinks back to 90%.
        FromRightToBack,
        FromLeftToRightCross,
        FromRightToLeftCross,
        AppOpen,
        AppClose,
        FromDownToUp,
        FromUpToDown
    }

    public class PageChanger : MonoBehaviour
    {
        private static PageChanger _instance;
        public static PageChanger Instance
        {
            get
            {
                if (_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(PageChanger)) as PageChanger[];
                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                    }
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(PageChanger).Name + " in the scene.");
                    }
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("_{0}", typeof(PageChanger).Name);
                        _instance = obj.AddComponent<PageChanger>();
                    }
                }
                return _instance;
            }
        }

        bool isTween, isComplete;
        float tweenTime, delayTime;
        float delayTimer, tweenTimer;
        RectTransform hidePage, showPage;
        CanvasGroup showPageCG, hidePageCG;
        PageSwitchType switchType;
        UnityEngine.Events.UnityAction onComplete;
        AnimationCurve tweenEase;
        float upToDownShowPagePosY;

        public void ScreenChange(RectTransform _hidePage, RectTransform _showPage, PageSwitchType _switchType, float _delayTime, float _tweenTime, AnimationCurve _animCurve, UnityEngine.Events.UnityAction _onComplete,float showPagePosY = 0f)
        {
            if (!isTween)
            {
                _showPage.gameObject.SetActive(true);
                tweenTimer = delayTimer = 0f;
                tweenEase = _animCurve;
                hidePage = _hidePage;
                showPage = _showPage;
                switchType = _switchType;
                delayTime = _delayTime;
                tweenTime = _tweenTime;
                onComplete = _onComplete;
                hidePageCG = _hidePage.gameObject.CG();
                showPageCG = _showPage.gameObject.CG();
                showPage.localScale = Vector2.one;

                switch (_switchType)
                {
                    case PageSwitchType.FromLeftToRight:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(-getScreenWidth(), 0, 0);
                        break;
                    case PageSwitchType.FromRightToLeft:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(getScreenWidth(), 0, 0);
                        break;
                    case PageSwitchType.FromLeftToBack:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(-getScreenWidth(), 0, 0);
                        showPage.localScale = Vector3.one;
                        break;
                    case PageSwitchType.FromRightToBack:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(-getScreenWidth(), 0, 0);
                        showPage.localScale = Vector3.one;
                        break;
                    case PageSwitchType.FromLeftToRightCross:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(-getScreenWidth(), 0, 0);
                        break;
                    case PageSwitchType.FromRightToLeftCross:
                        RTDim.SetParent(hidePage.parent);
                        RTDim.localScale = Vector3.one;
                        RTDim.localPosition = Vector2.zero;
                        CGDim.alpha = 0;
                        RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPage.localPosition = new Vector3(getScreenWidth(), 0, 0);
                        break;
                    case PageSwitchType.FromDownToUp:
                        // RTDim.SetParent(hidePage.parent);
                        // RTDim.localScale = Vector3.one;
                        // RTDim.localPosition = Vector2.zero;
                        // CGDim.alpha = 0;
                        // RTDim.SetAsLastSibling();
                        showPage.SetAsLastSibling();
                        showPageCG.alpha = 0;
                        showPage.localPosition = new Vector3(-34f, 0, 0);
                        break;
                    case PageSwitchType.FromUpToDown:
                        // RTDim.SetParent(hidePage.parent);
                        // RTDim.localScale = Vector3.one;
                        // RTDim.localPosition = Vector2.zero;
                        // CGDim.alpha = 0;
                        // RTDim.SetAsLastSibling();
                        upToDownShowPagePosY = showPagePosY;
                        Debug.Log("UpToDown");
                        showPage.SetAsLastSibling();
                        showPageCG.alpha = 0;
                        showPage.localPosition = new Vector3(0, 0, 0);
                        break;
                }

                isComplete = false;
                isTween = true;
            }
        }


        void Update()
        {
            if (isTween)
            {
                // if (hidePage == null || showPage == null)
                //     return;

                if (delayTimer <= delayTime)
                {
                    delayTimer += Time.deltaTime;
                    return;
                }

                if (tweenTimer <= tweenTime)
                {
                    switch (switchType)
                    {
                        case PageSwitchType.FromLeftToRight:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(getScreenWidth(), 0, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(-getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                        case PageSwitchType.FromRightToLeft:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(-getScreenWidth(), 0, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                        case PageSwitchType.FromLeftToBack:
                            if (hidePage != null) hidePage.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.9f, 0.9f, 0.9f), tweenEase.Evaluate(tweenTimer / tweenTime));
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(-getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                        case PageSwitchType.FromRightToBack:
                            if (hidePage != null) hidePage.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.9f, 0.9f, 0.9f), tweenEase.Evaluate(tweenTimer / tweenTime));
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                        case PageSwitchType.FromLeftToRightCross:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(getScreenWidth() / 2f, 0, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(-getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                        case PageSwitchType.FromRightToLeftCross:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(-getScreenWidth() / 2f, 0, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(getScreenWidth(), 0, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;

                        case PageSwitchType.FromDownToUp:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, 16f, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            if (hidePage != null) hidePageCG.alpha = 1 - tweenEase.Evaluate(tweenTimer / tweenTime);
                            if (showPage != null) showPageCG.alpha = tweenEase.Evaluate(tweenTimer / tweenTime);
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(0, -34f, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;

                        case PageSwitchType.FromUpToDown:
                            if (hidePage != null) hidePage.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, -16f, 0), tweenEase.Evaluate(tweenTimer / tweenTime));
                            if (hidePage != null) hidePageCG.alpha = 1 - tweenEase.Evaluate(tweenTimer / tweenTime);
                            if (showPage != null) showPageCG.alpha = tweenEase.Evaluate(tweenTimer / tweenTime);
                            // CGDim.alpha = tweenEase.Evaluate(tweenTimer / tweenTime) * 0.5f;
                            if (showPage != null) showPage.localPosition = Vector3.Lerp(new Vector3(0, upToDownShowPagePosY, 0), Vector3.zero, tweenEase.Evaluate(tweenTimer / tweenTime));
                            break;
                    }
                    tweenTimer += Time.deltaTime;
                }
                else if (tweenTimer > tweenTime && !isComplete)
                {
                    if (onComplete != null) onComplete.Invoke();
                    isTween = false;
                    isComplete = true;
                }
            }
        }

        float getScreenWidth()
        {
            var canvas = FindObjectOfType<UnityEngine.UI.CanvasScaler>();
            return canvas.referenceResolution.x;
        }
        float getScreeHeight()
        {
            return Screen.height * (getScreenWidth() / Screen.width);
        }

        static float calNum = Mathf.PI / 180;
        public AnimationCurve SineInOut90 = new AnimationCurve(
            new Keyframe(0, 0, Mathf.Tan(5 * calNum), Mathf.Tan(5 * calNum)),
            new Keyframe(0.12f, 0.06f, Mathf.Tan(52 * calNum), Mathf.Tan(52 * calNum)),
            new Keyframe(0.4f, 0.72f, Mathf.Tan(50 * calNum), Mathf.Tan(50 * calNum)),
            new Keyframe(1f, 1f, Mathf.Tan(1 * calNum), Mathf.Tan(1 * calNum))
        );
        CanvasGroup _cgDim;
        RectTransform _rtDim;
        GameObject _goDim;
        GameObject GODim
        {
            get
            {
                if (_goDim == null)
                {
                    GameObject go = new GameObject("_Dim");
                    go.transform.SetParent(hidePage);
                    go.transform.localPosition = Vector2.zero;
                    go.transform.localScale = Vector2.one;
                    _goDim = go;
                }
                return _goDim;
            }
        }
        RectTransform RTDim
        {
            get
            {
                if (_rtDim == null)
                {
                    _rtDim = GODim.GetComponent<RectTransform>();
                }
                if (_rtDim == null)
                {
                    _rtDim = GODim.AddComponent<RectTransform>();
                }
                return _rtDim;
            }
        }

        CanvasGroup CGDim
        {
            get
            {
                if (_cgDim == null)
                {
                    RTDim.anchorMin = new Vector2(0.5f, 0.5f);
                    RTDim.anchorMax = new Vector2(0.5f, 0.5f);
                    RTDim.anchoredPosition = Vector2.zero;
                    RTDim.sizeDelta = new Vector2(getScreenWidth(), getScreeHeight());
                    GODim.AddComponent<UnityEngine.UI.Image>().color = Color.black;
                    _cgDim = GODim.AddComponent<CanvasGroup>();
                }
                return _cgDim;
            }
        }
    }
}