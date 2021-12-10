using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public GameObject OptionObject;
    [SerializeField] private GameObject soundSetting;
    [SerializeField] private GameObject credit;
    [SerializeField] private GameObject reallyExit;

    public void OnClose()
    {
        UIMgr.Instance.CloseOption();
    }

    public void OnMain()
    {
        UIMgr.Instance.CloseOption();
        GameMgr.Instance.ResetGame();
        SoundMgr.Instance.ToMain();
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

    // 옵션 창의 상태를 초기 상태로 되돌린다.
    public void ResetOption()
    {
        soundSetting.SetActive(false);
        credit.SetActive(false);
        reallyExit.SetActive(false);
        OptionObject.SetActive(false);
    }
}
