using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.hnine.framework
{
    [Serializable]
    public class TweenObject
    {
        public GameObject _go;
        RectTransform _rt;
        CanvasGroup _cg;
        Button _btn;
        Image _img;
        InputField _inputfield;
        Text _text;
        Animator _animator;
        Mask _mask;


        public InputField InputField
        {
            get
            {
                if (_inputfield == null) _inputfield = _go.GetComponent<InputField>();
                if (_inputfield == null) _inputfield = _go.AddComponent<InputField>();
                return _inputfield;
            }
        }

        public Text Txt
        {
            get
            {
                if (_text == null) _text = _go.GetComponent<Text>();
                if (_text == null) _text = _go.AddComponent<Text>();
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public RectTransform RT
        {
            get
            {
                if (_rt == null) _rt = _go.GetComponent<RectTransform>();
                if (_rt == null) _rt = _go.AddComponent<RectTransform>();
                return _rt;
            }
            set
            {
                _rt = value;
            }
        }
        public CanvasGroup CG
        {
            get
            {
                if (_cg == null) _cg = _go.GetComponent<CanvasGroup>();
                if (_cg == null) _cg = _go.AddComponent<CanvasGroup>();
                return _cg;
            }
        }
        public Button Btn
        {
            get
            {
                if (_btn == null) _btn = _go.GetComponent<Button>();
                if (_btn == null) _btn = _go.AddComponent<Button>();
                return _btn;
            }
        }
        public Image Img
        {
            get
            {
                if (_img == null) _img = _go.GetComponent<Image>();
                if (_img == null) _img = _go.AddComponent<Image>();
                return _img;
            }
        }
        public Animator Anim
        {
            get
            {
                if (_animator == null) _animator = _go.GetComponent<Animator>();
                if (_animator == null) _animator = _go.AddComponent<Animator>();
                return _animator;
            }
        }

        public Mask Mask
        {
            get
            {
                if (_mask == null) _mask = _go.GetComponent<Mask>();
                if (_mask == null) _mask = _go.AddComponent<Mask>();
                return _mask;
            }
        }


        public void Log(string msg)
        {
            Debug.Log(msg);
        }
    }
}