using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image skillImg;            // 스킬 이미지
    [SerializeField] private GameObject[] attackArea;   // 스킬 유효 범위 이미지
    [SerializeField] private GameObject movePos;        // 이동 방향 이미지
    [SerializeField] private GameObject dashPos;        // 대시 방향 이미지
    [SerializeField] private GameObject hpBuff;         // 체력 관련 버프 이미지
    [SerializeField] private GameObject mpBuff;         // 마나 관련 버프 이미지
    [SerializeField] private GameObject atkBuff;        // 공격력 관련 버프 이미지
    [SerializeField] private Text skillName;            // 카드 설명
    [SerializeField] private Text discription;          // 카드 설명
    [SerializeField] private Text value;                // 카드 데미지, 회복량 등 value
    [SerializeField] private Text cost;                 // 카드 소모량
    [SerializeField] private GameObject back;           // 카드 뒷면
    [SerializeField] private GameObject disable;        // 카드 사용 불과 효과
    [SerializeField] private Animation anim;        // 카드 애니메이션

    public bool isPlayAnimation { get { return (anim) ? anim.isPlaying : false; } }

    public Normal skillData { get; private set; }
    public Vector3 OriginPos { get; private set; }  // 카드의 원래 위치
    public bool IsSelect { get; private set; }  //카드 선택 여부
    public bool isDisable { get; private set; } // 비활성화 여부

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
            // 타겟이 플레이어일 경우
            case TARGET.PLAYER:
                break;
            // 타겟이 상대방일 경우
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
            // 영향을 주는 대상 확인
            switch (util.condition)
            {
                // HP에 영향을 주는 경우
                case INFLUENCE.HP:
                    // 회복
                    value.text = string.Format("HP  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    hpBuff.SetActive(true);
                    break;
                // MP에 영향을 주는 경우
                case INFLUENCE.MP:
                    // 총명
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP   {0:D2}", util.cost);

                    mpBuff.SetActive(true);
                    break;
                // 공격력에 영향을 주는 경우
                case INFLUENCE.ATK:
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    atkBuff.SetActive(true);
                    break;
                // 방어력에 영향을 주는 경우
                case INFLUENCE.DEF:
                    // 가드
                    value.text = string.Format("DM  {0:D2}", util.value);
                    cost.text = string.Format("MP  {0:D2}", util.cost);
                    hpBuff.SetActive(true);
                    break;
                // 상태이상의 경우
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
     
    // 카드 UI를 리셋
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

    // 이동 카드의 지시방향에 따라 화살표의 방향을 지정
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


    // 카드 뒷면 활성화
    public void BackCard()
    {
        back.SetActive(true);
    }
    public void FrontCard()
    {
        back.SetActive(false);
    }

    // 카드 사용불가 효과 활성화
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

    // 코스트가 넘치는지 확인하는 함수
    public void CheckOverCost(int remainMp)
    {    
        // 해당 카드의 코스트를 얻어옴
        if (skillData is Utility)
        {
            Utility util = skillData as Utility;
            // MP회복의 경우 미적용
            if (util.condition == INFLUENCE.MP) return;

            if (util.cost > remainMp)
                DisableCard();
            else
                AbleCard();
        }
        else return;    // 카드가 유틸리티가 아니거나 유틸리티를 상속받지 않는다면 종료
    }
}
