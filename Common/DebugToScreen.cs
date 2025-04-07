using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace com.hnine.framework
{
    public class DebugToScreen : MonoBehaviour
    {
        string myLog;
        Queue myLogQueue = new Queue();
        [SerializeField] Text T_DebugText;
        [SerializeField] CanvasGroup cG_Debug;
        [SerializeField] Button btn_Debug;

        void Awake()
        {
            Application.logMessageReceived += HandleLog;
            if (btn_Debug != null)
            {
                btn_Debug.onClick.AddListener(Clear);
            }
            Debug.Log("Debug init");
        }
        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                T_DebugText.text += "\n<color=red>[From Unity] " + logString + "</color>";
            }
            else
            {
                T_DebugText.text += "\n[From Unity] " + logString + "";
            }
            // AddMessageUI(logString);
        }

        void AddMessageUI(string msg)
        {
            T_DebugText.text += "\n[From Unity] " + msg;
        }

        public void OnOff()
        {
            if (cG_Debug.alpha == 1)
            {
                cG_Debug.blocksRaycasts = false;
                cG_Debug.alpha = 0;
            }
            else
            {
                cG_Debug.blocksRaycasts = true;
                cG_Debug.alpha = 1;
            }
        }

        public void Clear()
        {
            T_DebugText.text = "Clear.";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnOff();
            }
        }
    }
}