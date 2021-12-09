using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField] private GameObject soundSetting;
    [SerializeField] private GameObject credit;
    [SerializeField] private GameObject reallyExit;

    public void OnClose()
    {
        UIMgr.Instance.CloseWindow();
    }

    public void OnMain()
    {
        UIMgr.Instance.CloseWindow();
        UIMgr.Instance.InitScene();
    }

    public void OnSoundSetting()
    {
        soundSetting.SetActive(true);
    }

    public void OffSoundSetting()
    {
        soundSetting.SetActive(false);
    }

    public void OnCredit()
    {
        credit.SetActive(true);
    }
    public void OffCredit()
    {
        credit.SetActive(false);
    }
    public void OnReallyExit()
    {
        reallyExit.SetActive(true);
    }
    public void OffReallyExit()
    {
        reallyExit.SetActive(false);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
