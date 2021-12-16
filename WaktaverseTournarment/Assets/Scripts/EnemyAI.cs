using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    DEFUALT = 0,
    MOVE,
    ATTACK,
    DEF,
    HP,
    MP,
    BUFF
}

public class ThisAction
{
    public Action action;
    public float weight;
}

// 조건에 해당하면 해당 2진수 값(키워드)를 넣어서 다같이 보냄.
public class EnemyAI
{
    private ThisAction rateHpHeal = new ThisAction();
    private ThisAction rateMpHeal = new ThisAction();
    private ThisAction rateBuff = new ThisAction();
    private ThisAction rateAttack = new ThisAction();
    private ThisAction rateDefense = new ThisAction();
    private ThisAction rateMove = new ThisAction();
    private List<ThisAction> weightList = new List<ThisAction>();
    private float rateSum;
    List<Normal> skills = new List<Normal>();

    public EnemyAI()
    {
        InitRateAction();
    }
    private void InitRateAction()
    {
        rateHpHeal.action = Action.HP;
        rateMpHeal.action = Action.MP;
        rateBuff.action = Action.BUFF;
        rateAttack.action = Action.ATTACK;
        rateDefense.action = Action.DEF;
        rateMove.action = Action.MOVE;
        weightList.Add(rateHpHeal);
        weightList.Add(rateMpHeal);
        weightList.Add(rateBuff);
        weightList.Add(rateAttack);
        weightList.Add(rateDefense);
        weightList.Add(rateMove);
    }

    //적이 현재 사용가능한 카드를 체크
    private void CheckEnemyCost(Unit enemy)
    {
        // 현재 마나 값을 알아옴
        var enemyMp = enemy.mp;
        var enemyRemainMp = enemy.mpRemain;

        // 전체 카드 검사
        for (int i = 0; i < skills.Count; i++)
        {
            // 코스트가 있는 경우에만 동작
            if (skills[i] is Utility)
            {
                Utility util = skills[i] as Utility;
                // 현재 사용하려는 카드의 비용이 현재 마나량보다 적을 경우 비활성화
                if (enemyMp < util.cost)
                    skills.RemoveAt(i);
            }
        }
    }

    // 원하는 개수만큼의 Action이 들어있는 리스트 반환
    public List<Action> GetEnemyAction(Unit player, Unit enemy, int listCount)
    {
        skills.AddRange(DataMgr.Instance.arrPublicSkill);
        skills.AddRange(DataMgr.Instance.arrEnemySkill);
        skills.AddRange(DataMgr.Instance.enemyOwnUniqueList); 
        List<Action> actionList = new List<Action>();

        var hps = skills.FindAll(data => (data.thisAction.Equals(Action.HP)));
        var buffs = skills.FindAll(data => (data.thisAction.Equals(Action.BUFF)));

        List<float> pivotList = new List<float>();

        // 현재 마나 값을 알아옴
        //var enemyMp = enemy.mp;
        //var enemyRemainMp = enemy.mpRemain;
        //
        //// 전체 카드 검사
        //for (int i = 0; i < skills.Count; i++)
        //{
        //    // 코스트가 있는 경우에만 동작
        //    if (skills[i] is Utility)
        //    {
        //        Utility util = skills[i] as Utility;
        //        // 현재 사용하려는 카드의 비용이 현재 마나량보다 적을 경우 비활성화
        //        if (enemyMp < util.cost)
        //            skills.RemoveAt(i);
        //    }
        //}
        //GameMgr.Instance.Enemy.SetRemainCost(DataMgr.Instance.GetRemainMana(skills, 1));
        
        var list = DataMgr.Instance.arrPublicSkill;
        if (GameMgr.Instance.maxPos.x <= enemy.GetUnitPos().x)
        {
            for (int i = 0; i < list.Length; i++)
            {
                var skill = list[i];
                if (skill.name == "MoveRight")
                {
                    skills.Remove(skill);
                    break;
                }
            }
        }
        float maxDistance = Vector2.Distance(GameMgr.Instance.maxPos, GameMgr.Instance.minPos);
        float dist = Vector2.Distance(enemy.GetUnitPos(), player.GetUnitPos());

        // 체력에 반비례한 회복 비중 * 회복 기술의 개수(0개 고려)
        rateHpHeal.weight = (100 - enemy.hp) * hps.Count;
        // 마나에 반비례한 회복 비중
        rateMpHeal.weight = 100 - enemy.mp;
        // 버프 기술의 개수(0개 고려)
        rateBuff.weight = 20 * buffs.Count;
        // 유닛 사이의 거리에 반비례한 공격 비중*0.1f
        rateAttack.weight = (maxDistance - dist) * 0.1f;
        // 유닛 사이의 거리에 반비례한 방어 비중*0.05f
        rateDefense.weight = (maxDistance - dist) * 0.05f;
        // 기본 이동 비중 + 유닛 사이의 거리에 비례한 이동 비중*0.1f
        rateMove.weight = 10 + dist * 0.1f;

        // 총합
        rateSum = rateHpHeal.weight + rateMpHeal.weight + rateBuff.weight + rateAttack.weight + rateDefense.weight + rateMove.weight;
        // 행동의 개수만큼 피봇을 설정
        for (int i = 0; i < listCount; i++)
        {
            pivotList.Add(Random.Range(0, rateSum));
        }

        bool isCheck = false;
        // 피봇의 개수만큼 행동을 지정
        for (int i = 0; listCount > i; i++)
        {
            isCheck = false;
            // 가중치 리스트 종류만큼 차례대로 확인
            for (int j = 0; j < weightList.Count; j++)
            {
                var wieght = weightList[j];
                if (pivotList[i] < wieght.weight)
                {
                    switch(wieght.action)
                    {
                        case Action.BUFF:
                            isCheck = actionList.Contains(Action.BUFF);break;
                        case Action.HP:
                            isCheck = actionList.Contains(Action.HP); break;
                        case Action.MP:
                            isCheck = actionList.Contains(Action.MP); break;
                        case Action.DEF:
                            isCheck = actionList.Contains(Action.DEF); break;
                    }
                    if (isCheck)
                    {
                        int rand = Random.Range(0, 3);
                        switch(rand)
                        {
                            case 0:
                                actionList.Add(Action.ATTACK);
                                break;
                            default:
                                actionList.Add(Action.MOVE);
                                break;
                        }
                    }
                    else
                        actionList.Add(wieght.action);
                    break;
                }
                pivotList[i] -= wieght.weight;
            }
        }
        return actionList;
    }

    private List<Normal> AddList(List<Action> curActions, List<Normal> list)
    {
        List<Normal> returnList = new List<Normal>();
        for (int i = 0; i < curActions.Count; i++)
        {
            var tmp = list.FindAll(data => (data.thisAction.Equals(curActions[i]) && !returnList.Contains(data)));
            var re = tmp[Random.Range(0, tmp.Count)];
            returnList.Add(re);
        }
        return returnList;
    }

    public List<Normal> GetEnemyCardData(int listCount)
    {
        List<Action> curAction = GetEnemyAction(GameMgr.Instance.Player, GameMgr.Instance.Enemy, listCount);

        return AddList(curAction, skills);
    }
}
