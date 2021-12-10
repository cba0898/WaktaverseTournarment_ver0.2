using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SoundMgr : MonoBehaviour
{
    #region instance
    private static SoundMgr instance = null;
    public static SoundMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance를 유일 오브젝트로 만든다
        instance = this;

        // Scene 이동 시 삭제 되지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private Dictionary<string, AudioClip> audioDictionary;   // 리소스에 불러온 오디오 클립을 저장할 딕셔너리
    [SerializeField] private AudioSource BGM;    // BGM
    [SerializeField] private AudioSource SFX;    // SFX

    // bgm key 문자열 변수들
    public string keyMain = "Main";
    public string keyCardSet = "CardSet";
    public string keyBattle = "Battle";
    public string keyWin = "Win";
    public string keyLose = "Lose";
    public string keyEnding = "Ending";

    //void Start()
    //{
    //    LoadAudio();
    private bool masterMute = false;
    private float masterVolume = 1;

    private bool SFXMute = false;
    private float SFXVolume = 1;

    private bool BGMMute = false;
    private float BGMVolume = 1;

    public bool SFXSoundMute { get { return (masterMute || SFXMute); } }
    public float SFXSoundVolume { get { return (SFXVolume * masterVolume); } }

    public bool BGMSoundMute { get { return (masterMute || BGMMute); } }
    public float BGMSoundVolume { get { return (BGMVolume * masterVolume); } }

    public void ChangeMasterVolume(Slider slider)
    {
        masterVolume = slider.value;
        ApplyBGMVolume();
        ApplySFXVolume();
    }
    public void MuteMasterVolume(Toggle toggle)
    {
        masterMute = !toggle.isOn;
        ApplyBGMVolume();
        ApplySFXVolume();
    }
    public void ChangeSFXVolume(Slider slider)
    {
        SFXVolume = slider.value;
        ApplySFXVolume();
    }
    public void MuteSFXVolume(Toggle toggle)
    {
        SFXMute = !toggle.isOn;
        ApplySFXVolume();
    }
    public void ChangeBGMVolume(Slider slider)
    {
        BGMVolume = slider.value;
        ApplyBGMVolume();
    }
    public void MuteBGMVolume(Toggle toggle)
    {
        BGMMute = !toggle.isOn;
        ApplyBGMVolume();
    }

    public void LoadAudio()
    {
        audioDictionary = DataMgr.Instance.SetDictionary<AudioClip>("Audios/BGM");
        BGM.loop = true;
        SFX.loop = false;
    }

    public void ApplyBGMVolume()
    {
        BGM.volume = BGMSoundVolume;
        BGM.mute = BGMSoundMute;
    }
    public void ApplySFXVolume()
    {
        SFX.volume = SFXSoundVolume;
        SFX.mute = SFXSoundMute;
    }

    public void OnPlayBGM(string key)
    {
        //기존 음악 정지
        StopBGM();

        BGM.clip = audioDictionary[key];
        // 배경음이 없을 경우에만 재생
        if (!BGM.isPlaying) BGM.Play();
    }

    public void StopBGM()
    {
        BGM.Stop();
    }

    public void ToMain()
    {
        // 기존 재생 사운드 정지
        BGM.Stop();
        SFX.Stop();
        // 메인화면 BGM 재생
        BGM.clip = audioDictionary[keyMain];
    }
}
