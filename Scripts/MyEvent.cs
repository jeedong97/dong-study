using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.hnine
{
    //!=== 이벤트 타입 설정 =========

    [Serializable] public class DragEvent : UnityEvent<Vector2> { }
    [Serializable] public class TouchStartEvent : UnityEvent<Vector2> { }
    [Serializable] public class TouchEndEvent : UnityEvent<Vector2> { }
    [Serializable] public class ButtonClickEvent : UnityEvent<string> { }
    [Serializable] public class LongPressEevent : UnityEvent<float> { }


    //!=== 싱글톤으로 설정 =========
    public class MyEvent : MonoBehaviour
    {
        private static MyEvent instance = null;
        public DragEvent OnDrag;
        public TouchStartEvent OnTouchStart;
        public TouchEndEvent OnTouchEnd;
        public ButtonClickEvent OnClick;
        public LongPressEevent OnLongPress;


        void Awake()
        {
            if (instance == null)
            {
                OnDrag = new DragEvent();
                var me = new GameObject("Event");
                me.name = "Event";
                
                instance = me.AddComponent<MyEvent>();

                DontDestroyOnLoad(this.gameObject);
            }
        }
        public static MyEvent Inst
        {
            get
            {
                if (instance == null)
                    return null;
                return instance;
            }
        }
    }
}