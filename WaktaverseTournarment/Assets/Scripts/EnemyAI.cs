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

    //���� ���� ��밡���� ī�带 üũ
    private void CheckEnemyCost(Unit enemy)
    {
        // ���� ���� ���� �˾ƿ�
        var enemyMp = enemy.mp;
        var enemyRemainMp = enemy.mpRemain;

        // ��ü ī�� �˻�
        for (int i = 0; i < skills.Count; i++)
        {
            // �ڽ�Ʈ�� �ִ� ��쿡�� ����
            if (skills[i] is Utility)
            {
                Utility util = skills[i] as Utility;
                // ���� ����Ϸ��� ī���� ����� ���� ���������� ���� ��� ��Ȱ��ȭ
                if (enemyMp < util.cost)
                    skills.RemoveAt(i);
            }
        }
    }

    // ���ϴ� ������ŭ�� Action�� ����ִ� ����Ʈ ��ȯ
    public List<Action> GetEnemyAction(Unit player, Unit enemy, int listCount)
    {
        skills.AddRange(DataMgr.Instance.arrPublicSkill);
        skills.AddRange(DataMgr.Instance.arrEnemySkill);
        skills.AddRange(DataMgr.Instance.enemyOwnUniqueList); 
        List<Action> actionList = new List<Action>();

        var hps = skills.FindAll(data => (data.thisAction.Equals(Action.HP)));
        var buffs = skills.FindAll(data => (data.thisAction.Equals(Action.BUFF)));

        List<float> pivotList = new List<float>();

        // ���� ���� ���� �˾ƿ�
        //var enemyMp = enemy.mp;
        //var enemyRemainMp = enemy.mpRemain;
        //
        //// ��ü ī�� �˻�
        //for (int i = 0; i < skills.Count; i++)
        //{
        //    // �ڽ�Ʈ�� �ִ� ��쿡�� ����
        //    if (skills[i] is Utility)
        //    {
        //        Utility util = skills[i] as Utility;
        //        // ���� ����Ϸ��� ī���� ����� ���� ���������� ���� ��� ��Ȱ��ȭ
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

        // ü�¿� �ݺ���� ȸ�� ���� * ȸ�� ����� ����(0�� ���)
        rateHpHeal.weight = (100 - enemy.hp) * hps.Count;
        // ������ �ݺ���� ȸ�� ����
        rateMpHeal.weight = 100 - enemy.mp;
        // ���� ����� ����(0�� ���)
        rateBuff.weight = 20 * buffs.Count;
        // ���� ������ �Ÿ��� �ݺ���� ���� ����*0.1f
        rateAttack.weight = (maxDistance - dist) * 0.1f;
        // ���� ������ �Ÿ��� �ݺ���� ��� ����*0.05f
        rateDefense.weight = (maxDistance - dist) * 0.05f;
        // �⺻ �̵� ���� + ���� ������ �Ÿ��� ����� �̵� ����*0.1f
        rateMove.weight = 10 + dist * 0.1f;

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
