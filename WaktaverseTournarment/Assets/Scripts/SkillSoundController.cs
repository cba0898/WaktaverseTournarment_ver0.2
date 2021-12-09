using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private void OnEnable()
    {
        if (audioSource)
        {       
            audioSource.mute = SoundMgr.Instance.SFXSoundMute;
            audioSource.volume = SoundMgr.Instance.SFXSoundVolume;
        }
    }
}
