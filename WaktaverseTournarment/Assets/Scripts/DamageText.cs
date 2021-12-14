using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private Text damageText;
    //[SerializeField] private  damageText;

    public void SetDamage(int value, bool isAttack)
    {
        damageText.text = value.ToString();
        damageText.color = isAttack ? Color.red : Color.green;
    }
}
