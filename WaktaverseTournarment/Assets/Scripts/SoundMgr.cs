using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SoundMgr : MonoBehaviour
{
    #region instance
    private static SoundMgr instance = null;
    public static SoundMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance�� ���� ������Ʈ�� �����
        instance = this;

        // Scene �̵� �� ���� ���� �ʵ��� ó��
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private Dictionary<string, AudioClip> audioDictionary;   // ���ҽ��� �ҷ��� ����� Ŭ���� ������ ��ųʸ�
    [SerializeField] private AudioSource BGM;    // BGM
    [SerializeField] private AudioSource SFX;    // SFX

    //void Start()
    //{
    //    LoadAudio();
    //}

    public void LoadAudio()
    {
        audioDictionary = DataMgr.Instance.SetDictionary<AudioClip>("Audios");
    }

    public void SetBGMVolume(float value)
    {
        BGM.volume = value;
    }

    public void OnPlayBGM(string key)
    {
        //���� ���� ����
        StopBGM();
        BGM.clip = audioDictionary[key];
        BGM.Play();
    }

    public void StopBGM()
    {
        BGM.Stop();
    }
}
