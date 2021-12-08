using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] public int hp { get; private set; }        // ü��   
    [SerializeField] public int hpMax { get; private set; }     // ü��   
    [SerializeField] public int mp { get; private set; }        // ����
    [SerializeField] public int mpMax { get; private set; }     // �ִ� ����
    [SerializeField] public int mpRemain { get; private set; }          // ���ԵǴ� ����
    [SerializeField] public int addAtk { get; private set; }    // �߰� ������
    [SerializeField] public int defense { get; private set; }   // ����. �޴� ������ ��/��
    [SerializeField] public UnitAnim unitanim;  // �ִϸ��̼�
    [SerializeField] private Slider hpSlider;   // hp��
    [SerializeField] private Slider mpSlider;   // mp��
    [SerializeField] private Text hpText;   // Hp��
    [SerializeField] private Text mpText;   // mp��

    public bool isInArea;                   // ĳ���Ͱ� ��ų �����ȿ� �ִ��� Ȯ��
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
