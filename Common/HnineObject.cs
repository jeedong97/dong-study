/*
    ? 사용하는 방법은?
    ? 상단에 요렇게 게임오브젝트만 선언 해 주고,
    public GameObject objImage;
        
    void Start()
    {
        ? 해당 오브젝트에서 hnine()호출 뒤에 Btn, Cg, Rt, Img, Mask 등등 접근하면 됨!!

        objImage.hnine().Btn.onClick.AddListener(()=>{
            Debug.Log("click");
        });
        objImage.hnine().Cg.alpha = 0.5f;
        objImage.hnine().Rt.anchoredPosition = Vector2.zero;
    }
*/


using UnityEngine;
using UnityEngine.UI;

public class HnineObject : MonoBehaviour
{
    public GameObject obj;
    Image _img;
    public Image Img
    {
        get
        {
            if (_img == null)
            {
                _img = obj.IMG();
            }
            return _img;
        }
    }
    CanvasGroup _cg;
    public CanvasGroup Cg
    {
        get
        {
            if (_cg == null)
            {
                _cg = obj.CG();
            }
            return _cg;
        }
    }
    Text _txt;
    public Text Txt
    {
        get
        {
            if (_txt == null)
            {
                _txt = obj.TXT();
            }
            return _txt;
        }
    }
    Button _btn;
    public Button Btn
    {
        get
        {
            if (_btn == null)
            {
                _btn = obj.BTN();
            }
            return _btn;
        }
    }
    RectTransform _rt;
    public RectTransform Rt
    {
        get
        {
            if (_rt == null)
            {
                _rt = obj.RT();
            }
            return _rt;
        }
    }
    Mask _mask;
    public Mask Mask
    {
        get
        {
            if (_mask == null)
            {
                _mask = obj.Mask();
            }
            return _mask;
        }
    }
}

public static class GameObjectExtension
{
    public static HnineObject hnine(this GameObject obj)
    {
        var hnineObj = obj.GetComponent<HnineObject>();
        if (hnineObj == null)
            hnineObj = obj.AddComponent<HnineObject>();
        hnineObj.obj = obj;
        return hnineObj;
    }
}