using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] public int hp { get; private set; }        // 체력   
    [SerializeField] public int hpMax { get; private set; }     // 체력   
    [SerializeField] public int mp { get; private set; }        // 마나
    [SerializeField] public int mpMax { get; private set; }     // 최대 마나
    [SerializeField] public int mpRemain { get; private set; }          // 남게되는 마나
    [SerializeField] public int addAtk { get; private set; }    // 추가 데미지
    [SerializeField] public int defense { get; private set; }   // 방어력. 받는 데미지 가/감
    [SerializeField] public UnitAnim unitanim;  // 애니메이션
    [SerializeField] private Slider hpSlider;   // hp바
    [SerializeField] private Slider mpSlider;   // mp바
    [SerializeField] private Text hpText;   // Hp바
    [SerializeField] private Text mpText;   // mp바

    public bool isInArea;                   // 캐릭터가 스킬 범위안에 있는지 확인
    //public bool isMove = false;
    //
    //public void OnMoveEnter()
    //{
    //    isMove = true;
    //}
    //public void OnMoveExit()
    //{
    //    isMove = false;
    //}
    //private void Awake()
    //{
    //    InitUnit();
    //}

    public void AddHP(int value)
    {
        hp = Mathf.Clamp(hp + value, 0, 100);
        hpSlider.value = hp;
        hpText.text = string.Format("HP {0}", hp);
    }

    public void AddMP(int value)
    {
        mp = Mathf.Clamp(mp + value, 0, 100);
        mpSlider.value = mp;
        SetRemainCost(mp);
        mpText.text = string.Format("MP {0}", mp);
    }

    public void AddAtk(int value)
    {
        addAtk = value;
    }

    public void AddDefense(int value)
    {
        defense = value;
    }

    public void SetRemainCost(int value)
    {
        mpRemain = value;
    }

    public void InitUnit()
    {
        hp = 100;
        hpMax = 100;
        mp = 100;
        mpMax = 100;
        mpRemain = 100;
        addAtk = 0;
        defense = 0;
        hpSlider.value = hp;
        mpSlider.value = mp;
        hpText.text = string.Format("HP {0}", hp);
        mpText.text = string.Format("MP {0}", mp);
    }

    public Vector2 GetUnitPos()
    {
        return transform.localPosition;
    }
}
