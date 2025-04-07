/*

m_RectTransform 기반으로 targetRectTransform의 사이즈를 자동 조절하도록 처리

*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Com.Hnine
{

    public enum MySizingType
    {
        XY,
        X,
        Y
    }

    [System.Serializable]
    public class SizeChangeEvent : UnityEvent<Vector2> { }

    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class TargetAutoSizingHandler : UIBehaviour
    {
        RectTransform m_RectTransform;
        [SerializeField] MySizingType sizingType;
        [SerializeField] Vector2 margin; // left, top, right, bottom
        public RectTransform targetRectTransform;
        Vector2 size;

        private void Update()
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
                size = m_RectTransform.sizeDelta;
            }
            if (size != m_RectTransform.sizeDelta)
            {
                // Debug.Log(m_RectTransform.sizeDelta);
                size = m_RectTransform.sizeDelta;

                if (targetRectTransform != null) // target Resize
                {
                    SetSize();
                }
            }
        }

        private void FixedUpdate()
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = GetComponent<RectTransform>();
                size = m_RectTransform.sizeDelta;
            }
            if (size != m_RectTransform.sizeDelta)
            {
                // Debug.Log(m_RectTransform.sizeDelta);
                size = m_RectTransform.sizeDelta;

                if (targetRectTransform != null) // target Resize
                {
                    SetSize();
                }
            }
        }

        // void OnValidate()
        // {
        //     // Debug.Log("인스팩터가 바꾸었어요~!!");
        //     SetSize();
        // }

        protected override void Start()
        {
            SetSize();
        }

        public void SetSize()
        {
            if (sizingType == MySizingType.XY)
                targetRectTransform.sizeDelta = new Vector2(margin.x + size.x, margin.y + size.y);
            else if (sizingType == MySizingType.X)
                targetRectTransform.sizeDelta = new Vector2(margin.x + size.x, targetRectTransform.sizeDelta.y);
            else if (sizingType == MySizingType.Y)
                targetRectTransform.sizeDelta = new Vector2(targetRectTransform.sizeDelta.x, margin.y + size.y);

        }
    }
}