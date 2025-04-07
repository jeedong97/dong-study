using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hnine.framework
{
    public class FrameChecker : MonoBehaviour
    {
        [Range(1, 200)]
        public int fFont_Size;
        [Range(0, 1080)]
        public float xPos;
        [Range(0, 2400)]
        public float yPos;
        [Range(0, 1)]
        public float Red, Green, Blue;

        float deltaTime = 0.0f;

        private void Start()
        {
            fFont_Size = fFont_Size == 0 ? 10 : fFont_Size;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(xPos, yPos, w / 2, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / fFont_Size;
            style.normal.textColor = new Color(Red, Green, Blue, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}