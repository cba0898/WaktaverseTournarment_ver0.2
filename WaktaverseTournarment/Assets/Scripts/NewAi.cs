using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAi
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

    private Vector2 prePos;

    public NewAi()
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
        var pos = GameMgr.Instance.Enemy.GetUnitPos();
        prePos = pos;
    }

    private void RemoveSkillToName(ref List<Normal> skillList, EffectivePos effectivePos)
    {
        var removes = skillList.FindAll(skillData => skillData.effectivePos.Equals(effectivePos));

        if (0 < removes.Count)
        {
            for (int i = 0; i < removes.Count; i++)
                skillList.Remove(removes[i]);
        }
    }

    public Action GetEnemyAction(Unit enemy, ref List<Normal> skillList)
    {
        float maxDistance = Vector2.Distance(GameMgr.Instance.maxPos, GameMgr.Instance.minPos);
        float dist = Vector2.Distance(prePos, GameMgr.Instance.Player.GetUnitPos());
        float distAbsX = Mathf.Abs(prePos.x - GameMgr.Instance.Player.GetUnitPos().x);
        Vector2 tileDist = DataMgr.Instance.tileInterval;

        // ���� ��ġ�� ���� ��ų �Ұ�
        if (prePos.x >= GameMgr.Instance.maxPos.x) 
        {
            RemoveSkillToName(ref skillList, EffectivePos.Right);
        }
        if (prePos.x <= GameMgr.Instance.minPos.x)
        {
            RemoveSkillToName(ref skillList, EffectivePos.Left);
        }
        // ���� ���� ���� ���� ���
        if(prePos.y == tileDist.y)
        {
            // ���ʿ��� ������ �ִ� ��ų�� ����
            RemoveSkillToName(ref skillList, EffectivePos.Up);
        }
        // ���� ���� �Ʒ��� ���� ���
        if (prePos.y == -tileDist.y)
        {
            // ���ʿ��� ������ �ִ� ��ų�� ����
            RemoveSkillToName(ref skillList, EffectivePos.Down);
        }
        // ���� ���� x ��ġ�� �������� �ʴ� ���
        if (distAbsX > tileDist.x)
        {
            // x ���� �߾ӿ��� ������ �ִ� ��ų�� ����
            RemoveSkillToName(ref skillList, EffectivePos.Center);
        }

        var hps = skillList.FindAll(data => (data.thisAction.Equals(Action.HP)));
        var buffs = skillList.FindAll(data => (data.thisAction.Equals(Action.BUFF)));
        var mps = skillList.FindAll(data => (data.thisAction.Equals(Action.MP)));
        var atks = skillList.FindAll(data => (data.thisAction.Equals(Action.ATTACK)));
        var defs = skillList.FindAll(data => (data.thisAction.Equals(Action.DEF)));

        // ü�¿� �ݺ���� ȸ�� ���� * ȸ�� ����� ����(0�� ���)
        rateHpHeal.weight = (100 - enemy.hp) * hps.Count;
        // ������ �ݺ���� ȸ�� ����
        rateMpHeal.weight = (100 - enemy.mpRemain) * mps.Count;
        // ���� ����� ����(0�� ���)
        rateBuff.weight = Mathf.Max((maxDistance - dist - 40) * 0.1f * buffs.Count, 0);
        // ���� ������ �Ÿ��� �ݺ���� ���� ����*0.2f
        rateAttack.weight = Mathf.Max((maxDistance - dist - 40) * 0.2f * atks.Count, 0);
        // ���� ������ �Ÿ��� �ݺ���� ��� ����*0.05f
        rateDefense.weight = Mathf.Max((maxDistance - dist - 40) * 0.05f * defs.Count, 0);
        // �⺻ �̵� ���� + ���� ������ �Ÿ��� ����� �̵� ����*0.1f
        rateMove.weight = 30 + dist * 0.1f;

        // ����
        rateSum = rateHpHeal.weight + rateMpHeal.weight + rateBuff.weight + rateAttack.weight + rateDefense.weight + rateMove.weight;

        float pivot = Random.Range(0, rateSum);
        // ����ġ ����Ʈ ������ŭ ���ʴ�� Ȯ��
        for (int j = 0; j < weightList.Count; j++)
        {
            var weight = weightList[j];
            if (pivot < weight.weight)
            {
                return weight.action;
            }
            pivot -= weight.weight;
        }
        return Action.MOVE;
    }

    public List<Normal> GetEnemyCardData(Unit enemy)
    {
        List<Normal> SelectCardList = new List<Normal>();

        skills.AddRange(DataMgr.Instance.arrPublicSkill);
        skills.AddRange(DataMgr.Instance.arrEnemySkill);
        skills.AddRange(DataMgr.Instance.enemyOwnUniqueList);

        // �� �� ���� ��ġ�� �޾ƿ�
        var enemyPos = enemy.GetUnitPos();
        prePos = enemyPos;

        for (int i = 0; i < 3; i++)
        {
            var skillList = skills.FindAll(skillData => skillData.cost <= enemy.mpRemain);
            var curAction = GetEnemyAction(enemy, ref skillList);

            var actionCard = skillList.FindAll(data => (data.thisAction.Equals(curAction)));
            var selectCard = actionCard[Random.Range(0, actionCard.Count)];
            SelectCardList.Add(selectCard);
            skills.Remove(selectCard);
            switch(selectCard.movePos)
            {
                case LOCATION.LEFT:
                    prePos.x -= DataMgr.Instance.tileInterval.x * selectCard.MoveCount;
                    break;
                case LOCATION.RIGHT:
                    prePos.x += DataMgr.Instance.tileInterval.x * selectCard.MoveCount;
                    break;
                case LOCATION.CENTER_TOP:
                    prePos.y += DataMgr.Instance.tileInterval.y * selectCard.MoveCount;
                    break;
                case LOCATION.CENTER_BOTTOM:
                    prePos.y -= DataMgr.Instance.tileInterval.y * selectCard.MoveCount;
                    break;
            }

            // ���� ����
            enemy.SetRemainCost(DataMgr.Instance.GetRemainMana(selectCard, enemy, -1));
        }


        return SelectCardList;
    }

}
