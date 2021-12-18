using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    [SerializeField] private Character character;

    public void OnSelect()
    {
        DataMgr.Instance.CurrentPlayer = character;
        SoundMgr.Instance.OnPlaySFX("character click");

        var start = FindObjectOfType<Button>();
        start.interactable = true;
    }
}