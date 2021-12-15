using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DamageTextType
{
    Attack,
    Defense,
    HpHeal,
    MpHeal
}


public class DamageText : MonoBehaviour
{
    [SerializeField] private Text damageText;
    [SerializeField] private Animation anim;

    //[SerializeField] private  damageText;

    public void SetDamage(int value, DamageTextType type, Vector3 pos)
    {
        transform.position = pos;

        switch(type)
        {
            case DamageTextType.Attack:
                damageText.text = string.Format("-{0:D}", value);
                damageText.color = Color.red;
                break;
            case DamageTextType.Defense:
                damageText.text = string.Format("+{0:D}", value);
                damageText.color = Color.green;
                break;
            case DamageTextType.HpHeal:
                damageText.text = string.Format("+{0:D}", value);
                damageText.color = Color.green;
                break;
            case DamageTextType.MpHeal:
                damageText.text = string.Format("+{0:D}", value);
                damageText.color = Color.blue;
                break;
        }

        gameObject.SetActive(true);
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        while(anim.isPlaying)
        {
            yield return null;
        }

        //---
        gameObject.SetActive(false);
    }
}
