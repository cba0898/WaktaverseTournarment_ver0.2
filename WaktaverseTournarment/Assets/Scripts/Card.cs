using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image skillImg;            // ��ų �̹���
    [SerializeField] private GameObject[] attackArea;   // ��ų ��ȿ ���� �̹���
    [SerializeField] private GameObject movePos;        // �̵� ���� �̹���
    [SerializeField] private GameObject dashPos;        // ��� ���� �̹���
    [SerializeField] private GameObject hpBuff;         // ü�� ���� ���� �̹���
    [SerializeField] private GameObject mpBuff;         // ���� ���� ���� �̹���
    [SerializeField] private GameObject atkBuff;        // ���ݷ� ���� ���� �̹���
    [SerializeField] private Text skillName;            // ī�� ����
    [SerializeField] private Text discription;          // ī�� ����
    [SerializeField] private Text value;                // ī�� ������, ȸ���� �� value
    [SerializeField] private Text cost;                 // ī�� �Ҹ�
    [SerializeField] private GameObject back;           // ī�� �޸�
    [SerializeField] private GameObject disable;        // ī�� ��� �Ұ� ȿ��
    [SerializeField] private Animation anim;        // ī�� �ִϸ��̼�

    public bool isPlayAnimation { get { return (anim) ? anim.isPlaying : false; } }

    public Normal skillData { get; private set; }
    public Vector3 OriginPos { get; private set; }  // ī���� ���� ��ġ
    public bool IsSelect { get; private set; }  //ī�� ���� ����
    public bool isDisable { get; private set; } // ��Ȱ��ȭ ����

    public void SetData(Normal data, Vector2 pos)
    {
        SetLocalPos(pos);
        InitOriginPos();
        SetData(data);
    }
    public void SetData(Normal data)
    {
        skillData = data;
        IsSelect = false;
        discription.text = skillData.discription;
        skillName.text = skillData.skillName;

        switch (skillData.movePos)
        {
            case LOCATION.CENTER_TOP:   // Up
                SetMoveDirection(90);
                break;
            case LOCATION.CENTER_BOTTOM:    // Down
                SetMoveDirection(-90);
                break;
            case LOCATION.LEFT: // Left
                SetMoveDirection(180);
                break;
            case LOCATION.RIGHT:    // Right
                SetMoveDirection(0);
                break;
        }
        switch(skillData.target)
        {
            // Ÿ���� �÷��̾��� ���
            case TARGET.PLAYER:
                break;
            // Ÿ���� ������ ���
            case TARGET.ENEMY:
                break;
        }
        if (skillData is Attack)
        {
            //thisAction = Action.ATTACK;

            var atk = skillData as Attack;
            var nullvec = new Vector2(2, 2);
            Vector2[] arrSkillRange = SkillMgr.Instance.GetLOCATION(skillData.range, skillData);
            for (int i = 0; i < 9; i++)
            {
                if(arrSkillRange[i] != nullvec)
                    attackArea[i].SetActive(true);
            }
            value.text = string.Format("DM  {0:D2}", atk.value * atk.applyCount);
            cost.text = string.Format("MP  {0:D2}", atk.cost);
            return;
        }
        if (skillData is Utility)
        {
            var util = skillData as Utility;
            // ������ �ִ� ��� Ȯ��
            switch (util.condition)
            {
                // HP�� ������ �ִ� ���
                case INFLUENCE.HP:
                    // ȸ��
                    value.text = string.Format("HP  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    hpBuff.SetActive(true);
                    break;
                // MP�� ������ �ִ� ���
                case INFLUENCE.MP:
                    // �Ѹ�
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP   {0:D2}", util.cost);

                    mpBuff.SetActive(true);
                    break;
                // ���ݷ¿� ������ �ִ� ���
                case INFLUENCE.ATK:
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    atkBuff.SetActive(true);
                    break;
                // ���¿� ������ �ִ� ���
                case INFLUENCE.DEF:
                    // ����
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    hpBuff.SetActive(true);
                    break;
                // �����̻��� ���
                case INFLUENCE.STURN:
                    break;
            }
        }
        //thisAction = Action.MOVE;
    }

    public void InitBattleCard(Normal data)
    {
        gameObject.SetActive(false);
        SetOriginPos();
        ResetCardUI();
        SetData(data);
        BackCard();
        gameObject.SetActive(true);
    }
     
    // ī�� UI�� ����
    public void ResetCardUI()
    {
        value.text = "DM 00";
        cost.text = "MP 00";
        for (int i = 0; i < 9; i++)
        {
            attackArea[i].SetActive(false);
        }
        hpBuff.SetActive(false);
        mpBuff.SetActive(false);
        atkBuff.SetActive(false);
        movePos.SetActive(false);
    }

    public void SetLocalPos(Vector2 pos)
    {
        transform.localPosition = pos;
    }
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }

    // �̵� ī���� ���ù��⿡ ���� ȭ��ǥ�� ������ ����
    private void SetMoveDirection(int indexZ)
    {
        movePos.transform.rotation = Quaternion.Euler(0, 0, indexZ);

        movePos.SetActive(true);
    }

    public void OnSelectCard()
    {
        IsSelect = !IsSelect;

        UIMgr.Instance.OnSlot(this);

        //Debug.Log(DataMgr.Instance.IsOnCardList(this));

    }

    public void InitOriginPos()
    {
        OriginPos = transform.position;
    }
    public void SetOriginPos()
    {
        SetPos(OriginPos);
        IsSelect = false;
    }


    // ī�� �޸� Ȱ��ȭ
    public void BackCard()
    {
        back.SetActive(true);
    }
    public void FrontCard()
    {
        back.SetActive(false);
    }

    // ī�� ���Ұ� ȿ�� Ȱ��ȭ
    public void DisableCard()
    {
        isDisable = true;
        disable.SetActive(true);
    }
    public void AbleCard()
    {
        isDisable = false;
        disable.SetActive(false);
    }
    //public void SetCard(Card card)
    //{
    //    card.skillImg = this.skillImg;
    //    card.movePos = this.movePos;
    //    card.dashPos = this.dashPos;
    //    card.buff = this.buff;
    //    card.skillName = this.skillName;
    //    card.discription = this.discription;
    //    card.value = this.value;
    //    card.cost = this.cost;
    //}


    public void CardOpen()
    {
        anim.Play();
    }

    public bool IsCardOpened()
    {
        if (!anim) return false;
        return anim.isPlaying;
    }

    // �ڽ�Ʈ�� ��ġ���� Ȯ���ϴ� �Լ�
    public void CheckOverCost(int remainMp)
    {    
        // �ش� ī���� �ڽ�Ʈ�� ����
        if (skillData is Utility)
        {
            Utility util = skillData as Utility;
            // MPȸ���� ��� ������
            if (util.condition == INFLUENCE.MP) return;

            if (util.cost > remainMp)
                DisableCard();
            else
                AbleCard();
        }
        else return;    // ī�尡 ��ƿ��Ƽ�� �ƴϰų� ��ƿ��Ƽ�� ��ӹ��� �ʴ´ٸ� ����
    }
}