using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReLoadHandler : MonoBehaviour
{
    Button Btn_Reload;

    void Start()
    {
        Btn_Reload = GetComponent<Button>();
        Btn_Reload.onClick.AddListener(Reload);
    }

    void Reload()
    {
        SceneManager.LoadScene("SWallet");
    }
}
