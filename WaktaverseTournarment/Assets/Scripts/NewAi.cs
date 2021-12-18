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

        // 적의 위치에 따른 스킬 소거
        if (prePos.x >= GameMgr.Instance.maxPos.x) 
        {
            RemoveSkillToName(ref skillList, EffectivePos.Right);
        }
        if (prePos.x <= GameMgr.Instance.minPos.x)
        {
            RemoveSkillToName(ref skillList, EffectivePos.Left);
        }
        // 적이 가장 위에 있을 경우
        if(prePos.y == tileDist.y)
        {
            // 위쪽에만 영향을 주는 스킬은 제거
            RemoveSkillToName(ref skillList, EffectivePos.Up);
        }
        // 적이 가장 아래에 있을 경우
        if (prePos.y == -tileDist.y)
        {
            // 위쪽에만 영향을 주는 스킬은 제거
            RemoveSkillToName(ref skillList, EffectivePos.Down);
        }
        // 적이 나와 x 위치가 같아지지 않는 경우
        if (distAbsX > tileDist.x)
        {
            // x 기준 중앙에만 영향을 주는 스킬은 제거
            RemoveSkillToName(ref skillList, EffectivePos.Center);
        }

        var hps = skillList.FindAll(data => (data.thisAction.Equals(Action.HP)));
        var buffs = skillList.FindAll(data => (data.thisAction.Equals(Action.BUFF)));
        var mps = skillList.FindAll(data => (data.thisAction.Equals(Action.MP)));
        var atks = skillList.FindAll(data => (data.thisAction.Equals(Action.ATTACK)));
        var defs = skillList.FindAll(data => (data.thisAction.Equals(Action.DEF)));

        // 체력에 반비례한 회복 비중 * 회복 기술의 개수(0개 고려)
        rateHpHeal.weight = (100 - enemy.hp) * hps.Count;
        // 마나에 반비례한 회복 비중
        rateMpHeal.weight = (100 - enemy.mpRemain) * mps.Count;
        // 버프 기술의 개수(0개 고려)
        rateBuff.weight = Mathf.Max((maxDistance - dist - 40) * 0.1f * buffs.Count, 0);
        // 유닛 사이의 거리에 반비례한 공격 비중*0.2f
        rateAttack.weight = Mathf.Max((maxDistance - dist - 40) * 0.2f * atks.Count, 0);
        // 유닛 사이의 거리에 반비례한 방어 비중*0.05f
        rateDefense.weight = Mathf.Max((maxDistance - dist - 40) * 0.05f * defs.Count, 0);
        // 기본 이동 비중 + 유닛 사이의 거리에 비례한 이동 비중*0.1f
        rateMove.weight = 30 + dist * 0.1f;

        // 총합
        rateSum = rateHpHeal.weight + rateMpHeal.weight + rateBuff.weight + rateAttack.weight + rateDefense.weight + rateMove.weight;

        float pivot = Random.Range(0, rateSum);
        // 가중치 리스트 종류만큼 차례대로 확인
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

        // 매 턴 실제 위치를 받아옴
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

            // 적의 남게
            enemy.SetRemainCost(DataMgr.Instance.GetRemainMana(selectCard, enemy, -1));
        }


        return SelectCardList;
    }

}
