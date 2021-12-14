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
    //[SerializeField] public Unit unit;

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
    // 원하는 개수만큼의 Action이 들어있는 리스트 반환
    public List<Action> GetEnemyAction(Unit player, Unit enemy, int listCount)
    {
        skills.AddRange(DataMgr.Instance.arrPublicSkill);
        skills.AddRange(DataMgr.Instance.arrEnemySkill);
        skills.AddRange(DataMgr.Instance.enemyOwnUniqueList); List<Action> actionList = new List<Action>();

        var hps = skills.FindAll(data => (data.thisAction.Equals(Action.HP)));
        var buffs = skills.FindAll(data => (data.thisAction.Equals(Action.BUFF)));

        List<float> pivotList = new List<float>();

        var list = DataMgr.Instance.arrPublicSkill;
        if (GameMgr.Instance.maxPos.x <= enemy.GetUnitPos().x)
        {
            for (int i = 0; i < list.Length; i++)
            {
                var skill = list[i];
                if (skill.skillName == "MoveRight")
                    skill.isUsed = true;
            }
        }
        rateHpHeal.weight = 0;// (100 - enemy.hp) * hps.Count;
        rateMpHeal.weight = 0;// 100 - enemy.mp;
        rateBuff.weight = 20 * buffs.Count;
        rateAttack.weight = 0;// 20;
        rateDefense.weight = 0;//(100 - enemy.hp) * 0.5f;
        rateMove.weight = 0;// 20;

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
                        actionList.Add(Action.ATTACK);
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
            //!returnList.Contains(data)
            var tmp = list.FindAll(data => (data.thisAction.Equals(curActions[i]) && !returnList.Contains(data)));
            var re = tmp[Random.Range(0, tmp.Count)];
            returnList.Add(re);
            //for (int j = 0; j < tmp.Count; j++)
            //{
            //    var skill = tmp[j];
            //    // 일치하는 행동인지 체크 && 중복 체크
            //    if (!skill.isUsed)
            //    {
            //        normalList.Add(skill);
            //    }
            //}
        }

        return returnList;
    }

    public List<Normal> GetEnemyCardData(int listCount)
    {
        List<Action> curAction = GetEnemyAction(GameMgr.Instance.Player, GameMgr.Instance.Enemy, listCount);

        return AddList(curAction, skills);

        //normal = normal.FindAll(data => data.thisAction.Equals(curAction[0]));




        //AddList(curAction, DataMgr.Instance.arrPublicSkill, ref normalList);
        //AddList(curAction, DataMgr.Instance.arrEnemySkill, ref normalList);
        //AddList(curAction, DataMgr.Instance.enemyOwnUniqueList.ToArray(), ref normalList);



        //var list = DataMgr.Instance.arrPublicSkill;
        //// 공용스킬 세팅
        //for (int i = 0; i < list.Length; i++)
        //{
        //    var skill = list[i];
        //    // 중복 체크
        //    if (!skill.isUsed)
        //    {
        //        normalList.Add(skill);
        //    }
        //}

        //list = DataMgr.Instance.arrEnemySkill;
        //// 캐릭터 전용스킬 세팅
        //for (int i = 0; i < list.Length; i++)
        //{
        //    var skill = list[i];
        //    // 현재 액션과 캐릭터의 액션이 동일한지 체크(?)
        //    if (curAction == skill.thisAction && !skill.isUsed)
        //    {
        //        normalList.Add(skill);
        //    }
        //}
        //
        //for (int i=0;i<DataMgr.Instance.enemyOwnUniqueList.Count;i++)
        //{
        //    if (!DataMgr.Instance.enemyOwnUniqueList[i].isUsed)
        //        normalList.Add(DataMgr.Instance.enemyOwnUniqueList[i]);
        //}
    }
}
