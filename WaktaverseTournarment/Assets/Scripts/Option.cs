using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    //[SerializeField] private Button soundSettingButton;
    //[SerializeField] private Button exitButton;

    public void OnClose()
    {
        UIMgr.Instance.CloseWindow();
    }

    public void OnMain()
    {
        UIMgr.Instance.CloseWindow();
        UIMgr.Instance.InitScene();
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
