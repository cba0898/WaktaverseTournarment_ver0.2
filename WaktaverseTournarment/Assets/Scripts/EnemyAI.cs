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

// ���ǿ� �ش��ϸ� �ش� 2���� ��(Ű����)�� �־ �ٰ��� ����.
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
    // ���ϴ� ������ŭ�� Action�� ����ִ� ����Ʈ ��ȯ
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

        // ����
        rateSum = rateHpHeal.weight + rateMpHeal.weight + rateBuff.weight + rateAttack.weight + rateDefense.weight + rateMove.weight;
        // �ൿ�� ������ŭ �Ǻ��� ����
        for (int i = 0; i < listCount; i++)
        {
            pivotList.Add(Random.Range(0, rateSum));
        }


        bool isCheck = false;
        // �Ǻ��� ������ŭ �ൿ�� ����
        for (int i = 0; listCount > i; i++)
        {
            isCheck = false;
            // ����ġ ����Ʈ ������ŭ ���ʴ�� Ȯ��
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
            //    // ��ġ�ϴ� �ൿ���� üũ && �ߺ� üũ
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
        //// ���뽺ų ����
        //for (int i = 0; i < list.Length; i++)
        //{
        //    var skill = list[i];
        //    // �ߺ� üũ
        //    if (!skill.isUsed)
        //    {
        //        normalList.Add(skill);
        //    }
        //}

        //list = DataMgr.Instance.arrEnemySkill;
        //// ĳ���� ���뽺ų ����
        //for (int i = 0; i < list.Length; i++)
        //{
        //    var skill = list[i];
        //    // ���� �׼ǰ� ĳ������ �׼��� �������� üũ(?)
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
