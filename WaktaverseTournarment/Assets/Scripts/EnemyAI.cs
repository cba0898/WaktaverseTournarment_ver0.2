using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    DEFUALT = 0,
    MOVE = 1 << 1,
    ATTACK = 1 << 2,
    DEF = 1 << 3,
    HP = 1 << 4,
    MP = 1 << 5,
    BUFF = 1 << 6 // 이건 필요할지 잘 모르겠음
}

// 조건에 해당하면 해당 2진수 값(키워드)를 넣어서 다같이 보냄.
public class EnemyAI
{
    //[SerializeField] public Unit unit;

    private float rateHpHeal;
    private float rateMpHeal;
    private float rateAttack;
    private float rateDefense;
    private float rateMove;
    private float rateSum;
    private float nextAction;
    DataMgr dataIns = DataMgr.Instance;

    public Action GetEnemyAction(Unit player, Unit enemy)
    {
        if (GameMgr.Instance.maxPos.x < enemy.GetUnitPos().x)
        {
            for (int i = 0; i < dataIns.arrPublicSkill.Length; i++)
            {
                if (dataIns.arrPublicSkill[i].skillName == "MoveRight")
                    dataIns.arrPublicSkill[i].isUsed = true;
            }
        }
        rateHpHeal = 100 - enemy.hp;
        rateMpHeal = 100 - enemy.mp;
        rateAttack = 20;
        rateDefense = (100 - enemy.hp) * 0.5f;
        rateMove = 20;
        rateSum = rateHpHeal + rateMpHeal + rateAttack + rateDefense + rateMove;
        nextAction = Random.Range(0, rateSum);
        if (nextAction < rateHpHeal)
        {
            return Action.HP;
        }
        else if (nextAction < rateHpHeal + rateMpHeal)
        {
            return Action.MP;
        }
        else if (nextAction < rateHpHeal + rateMpHeal + rateAttack)
        {
            return Action.ATTACK;
        }
        else if (nextAction < rateHpHeal + rateMpHeal + rateAttack + rateDefense)
        {
            return Action.DEF;
        }
        else
            return Action.MOVE;
    }

    public Normal GetEnemyCardData()
    {
        List<Normal> normalList = new List<Normal>();
        EnemyAI eAi = new EnemyAI();
        Action curAction;

        curAction = eAi.GetEnemyAction(GameMgr.Instance.Player, GameMgr.Instance.Enemy);

        // 공용스킬 세팅
        for (int i = 0; i < dataIns.arrPublicSkill.Length; i++)
        {
            if (!dataIns.arrPublicSkill[i].isUsed)
            {
                normalList.Add(dataIns.arrPublicSkill[i]);
            }
        }

        // 캐릭터 전용스킬 세팅
        for (int i = 0; i < dataIns.arrEnemySkill.Length; i++)
        {
            if (curAction == dataIns.arrEnemySkill[i].thisAction && !dataIns.arrEnemySkill[i].isUsed)
            {
                normalList.Add(dataIns.arrEnemySkill[i]);
            }
        }
        return normalList[Random.Range(0, normalList.Count)];
    }
}
