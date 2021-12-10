using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum TILEPOS
{
    LEFTCENTER = -78,
    LEFT = -234,
    RIGHTCENTER = 78,
    RIGHT = 234,
    TOP = 90,
    CENTER = 0,
    BOTTOM = -90,
}

public class Buff
{
    public Unit buffTarget;
    public int addHp;
    public int addMp;
    public int addAtk;
    public int addDef;
    public int turn;

    public Buff()
    {
        buffTarget = null;
        addHp = 0;
        addMp = 0;
        addAtk = 0;
        addDef = 0;
        turn = 0;
    }

    public void DecsTurn()
    {
        this.turn--;
    }
};

public class GameMgr : MonoBehaviour
{
    #region instance
    private static GameMgr instance = null;
    public static GameMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance�� ���� ������Ʈ�� �����
        instance = this;

        // Scene �̵� �� ���� ���� �ʵ��� ó��
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public Unit Player;
    public Unit Enemy;

    public Vector2 minPos;      // ��ġ�� ������
    public Vector2 maxPos;      // ��ġ�� �ִ밪

    [SerializeField] public Image playerImg;    // �÷��̾� ���� �̹���
    [SerializeField] public Image enemyImg;     // �� ���� �̹���
    [SerializeField] private Image[] targetTile;
    [SerializeField] private List<Card> playerCardList;     // �÷��̾� ī�� ����Ʈ
    public int GetCardListCount()
    {
        return playerCardList.Count;
    }
    [SerializeField] private List<Card> enemyCardList;      // �� ī�� ����Ʈ
    private List<Buff> buffList = new List<Buff>();         // ���� ����Ʈ

    private Coroutine StartRoundCoroutine = null;

    // ���� ����
    public void Start()
    {
        UIMgr.Instance.ResetUIData();
        SoundMgr.Instance.LoadAudio();
        SoundMgr.Instance.StopBGM();
        SoundMgr.Instance.OnPlayBGM(SoundMgr.Instance.keyMain);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIMgr.Instance.ToggleOptionWindow();
        }
    }

    public void OnPlay()
    {
        if (null != StartRoundCoroutine) StopCoroutine(StartRoundCoroutine);

        DataMgr.Instance.InitUsed();

        // �� �ʱ�ȭ
        DataMgr.Instance.InitTurnCount();

        // ��� ���� ��� ���ߴ�
        if (1 == DataMgr.Instance.Round)
        {
            UIMgr.Instance.miniMap.InitMiniMapPos();
            InitCharPos();
        }

        SetSelectedCards();

        StartRoundCoroutine = StartCoroutine(StartRound());
    }

    // �̷��� ���� �ȵ�
    public void InitCharPos()
    {        
        Player.transform.localPosition = DataMgr.Instance.SetUnitPos(DataMgr.Instance.tileInterval.x, DataMgr.Instance.playerPosRate);
        Enemy.transform.localPosition = DataMgr.Instance.SetUnitPos(DataMgr.Instance.tileInterval.x, DataMgr.Instance.enemyPosRate);

    }

    // ��Ʋ ȭ�� �ʱ�ȭ
    public void InitBattleScene()
    {
        // ĳ���� ��ġ �ʱ�ȭ
        InitCharPos();
        FaceUnit(Player.gameObject, Enemy.gameObject);
        UIMgr.Instance.miniMap.InitMiniMapPos();
        // ��Ʋ�� ó�� ������ ��쿡�� �ʱ� ��ġ�� ����
        if (DataMgr.Instance.IsFirstEnemy())
        {
            for (int i = 0; i < 3; i++)
            {
                playerCardList[i].InitOriginPos();
                enemyCardList[i].InitOriginPos();
            }
        }
    }

    /*----------------------����--------------------*/
    // ���� ����
    private IEnumerator StartRound()
    {
        UIMgr.Instance.ActiveNextRound(false);
        for (int i = 0; i < 3; i++)
        {
            yield return StartTurn();
            if (WinCheck(Player, Enemy))
                break;
            else
                DataMgr.Instance.NextTurn();
        }
        UIMgr.Instance.ActiveNextRound(true);
    }

    // �� ����
    private IEnumerator StartTurn()
    {
        // ù ��° ��(ī�带 ó�� �������� ��) ī�� ���� / �굵 ���� �����ҵ�
        //if (0 == DataMgr.Instance.turnCount) SetSelectedCards();

        var playerCard = playerCardList[DataMgr.Instance.turnCount];
        var enemyCard = enemyCardList[DataMgr.Instance.turnCount];

        // ù ��° ī�尡 �������� �ִϸ��̼� ����
        playerCard.CardOpen();
        enemyCard.CardOpen();

        while (playerCard.isPlayAnimation && enemyCard.isPlayAnimation) yield return null;

        // ��ų�� �켱������ ����(priority���� ����) ������� ����
        if (playerCard.skillData.priority <= enemyCard.skillData.priority)
        {
            yield return CardAction(Player, playerCard);
            yield return CardAction(Enemy, enemyCard);
        }
        else
        {
            yield return CardAction(Enemy, enemyCard);
            yield return CardAction(Player, playerCard);
        }

        // ��ų ������ ������ ��Ȱ��ȭ
        playerCard.gameObject.SetActive(false);
        enemyCard.gameObject.SetActive(false);
    }

    // ���� ȭ�鿡 ������ ī����� ����
    private void SetSelectedCards()
    {
        EnemyAI enemyAI = new EnemyAI();
        List<Card> playerList = DataMgr.Instance.GetPlayerCardList();
        Normal enemyNormal;
        for (int i = 0; i < 3; i++)
        {
            SetBattleCard(playerCardList[i], playerList[i].skillData);

            enemyNormal = enemyAI.GetEnemyCardData();
            enemyNormal.isUsed = true;
            SetBattleCard(enemyCardList[i], enemyNormal);
        }
    }

    // ��Ʋ ȭ�� ī�� ����
    private void SetBattleCard(Card card, Normal data)
    {
        card.InitBattleCard(data);
    }
    /*----------------------����--------------------*/


    /*----------------------��ų ����--------------------*/
    // �÷��̾�, ���� �°� ��ǥ�� ����
    private Unit SetTarget(Unit unit, Unit target)
    {
        if (unit == Player)
        {
            if (target == Player) target = Player;
            else target = Enemy;
        }
        else if (unit == Enemy)
        {
            if (target == Player) target = Enemy;
            else target = Player;
        }
        return target;
    }

    private IEnumerator PlayAnim(Unit unit, int index)
    {
        //��ġ�� ����Ʈ�� ���� ��쿡�� ����
        if (-1 != index)
        {
            // ����Ʈ ����
            DataMgr.Instance.PlayAnim(unit, index);
            // ����Ʈ ������ ���������� ���
            var anim = DataMgr.Instance.GetSkillAnimator(index);
            while (DataMgr.Instance.IsEndAnim(anim)) yield return null;
            DataMgr.Instance.EndAnim(index);
        }
    }

    // ī�� ����
    private IEnumerator CardAction(Unit unit, Card card)
    {
        Unit target;
        // �ش� ����Ʈ�� �ε����� �ҷ���
        var effectIndex = DataMgr.Instance.GetEffectIndex(card.skillData);

        if (card.skillData.target == TARGET.PLAYER)  target = Player;   
        else target = Enemy;
        target = SetTarget(unit, target);

        // ���� �׼�
        if (card.skillData is Attack)
        {
            Attack atk = card.skillData as Attack;
            unit.AddMP(-1 * atk.cost);
            SetAttackTile(unit, target, card.skillData);
            yield return new WaitForSeconds(0.5f);
            unit.unitanim.OnActionEnter();
            yield return PlayAnim(unit, effectIndex);
            unit.unitanim.OnActionExit();
            if (target.isInArea)
            {
                DamageProcess(target, atk.value, unit.addAtk, atk.applyCount, target.defense);
                target.isInArea = false;
            }
        }
        // ��ƿ �׼�
        else if (card.skillData is Utility)
        {
            SetBuffTile(unit, card.skillData);
            yield return PlayAnim(unit, effectIndex);
            OnBuff(unit, target, card.skillData);
        }
        // �̵� �׼�
        else if (card.skillData is Normal)
        {
            var pos = SetMoveTile(target, card.skillData);
            yield return new WaitForSeconds(0.5f);
            FaceUnit(Player.gameObject, Enemy.gameObject);
        }
        yield return new WaitForSeconds(1.0f);
        ActiveBuff();
        ResetTile();
    }


    public void OnResetDie()
    {
        // Die �ִϸ��̼� ����
        if (Player.unitanim.anim) Player.unitanim.anim.SetBool("isDie", false);
        if (Enemy.unitanim.anim) Enemy.unitanim.anim.SetBool("isDie", false);
        // ĳ���� ������Ʈ �ʱ�ȭ
        Player.gameObject.SetActive(false);
        Player.gameObject.SetActive(true);
        Enemy.gameObject.SetActive(false);
        Enemy.gameObject.SetActive(true);
    }

    // �̵� ó�� �ڷ�ƾ
    private IEnumerator MoveCorutine(Unit unit, Vector2 destination, Vector2 direction)
    {
        bool isBack = false;

        // ������ ������� �ٶ󺸴��� üũ �� �ڷ� �̵��ϴ� ��� �ڷΰ��� �ִϸ��̼� ���
        if (unit.transform.localScale == new Vector3(1, 1, 1) && direction == new Vector2(-1, 0)
            || unit.transform.localScale == new Vector3(-1, 1, 1) && direction == new Vector2(1, 0))
        {
            isBack = true;
        }

        // �׼� ����
        if (isBack)
            unit.unitanim.OnBackEnter();
        else
            unit.unitanim.OnActionEnter();

        // �׼� �� ���� �̵��� �����ϴ� �������� ���
        while (!unit.unitanim.isMove) yield return null;

        float t = 0;
        Vector3 unitPos = unit.transform.localPosition;
        // �̵� �׼��� ���� �� ������ �̵�
        while (unit.unitanim.isMove)
        {
            // �̵��Ÿ��� 1(0~1)�� �Ѿ��� ��� 1�� ����
            if (t > 1) t = 1;
            unit.transform.localPosition = Vector3.Lerp(unitPos, destination, t);
            yield return null;
            t += 0.1f;
        }

        // �׼� ����. �ٽ� Idle�� ��ȯ
        if (isBack)
            unit.unitanim.OnBackExit();
        else
            unit.unitanim.OnActionExit();
    }

    // ���� Ÿ�� ȿ�� ����
    private Vector2 SetMoveTile(Unit unit, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrMoveRange = SkillMgr.Instance.GetLOCATION(data.movePos, data);

        // ������ ��ġ�� ����2�� ���ؿ�
        Vector2 unitPos = unit.transform.localPosition;
        Vector2 movedPos = unitPos;
        MiniMap mini = UIMgr.Instance.miniMap;
        Vector2 miniMapPos = new Vector2();
        if (unit == Player)
            miniMapPos = mini.GetMiniMapPlayerPos();
        else miniMapPos = mini.GetMiniMapEnemyPos();

        for (int i = 0; i < 9; i++)
        {
            if (arrMoveRange[i] != nullvec)
            {
                // ������ �̵��� ��ġ�� ���
                movedPos += DataMgr.Instance.tileInterval * arrMoveRange[i] * data.MoveCount;

                // ������ �̵��� ��ġ�� �� ũ�⺸�� Ŭ ��� ���ڸ� �̵�
                if (movedPos.x < minPos.x || movedPos.x > maxPos.x || movedPos.y < minPos.y || movedPos.y > maxPos.y)
                    movedPos = unitPos;
                else
                {            
                    // ������ ���� ��� �̴ϸ� ĳ������ ��ġ�� ���
                    miniMapPos += mini.miniMapInterval * arrMoveRange[i];
                }
                // �̴ϸ� ��ġ ����
                mini.SetMiniMapPos(unit, miniMapPos);

                // ����Ÿ���� ��ġ�� �̵��� ��ġ�� �̵�
                targetTile[i].transform.localPosition = movedPos;
                // Ÿ���� ���� ������� ����
                targetTile[i].color = new Color(0.17647f, 0.529412f, 0.027450f);
                // �ش� ����Ÿ�� Ȱ��ȭ
                targetTile[i].gameObject.SetActive(true);

                // ������ �ش� �̵������� �̵� ����
                StartCoroutine(MoveCorutine(unit, movedPos, arrMoveRange[i]));
            }
        }
        return movedPos;
    }

    // ������ ���ֺ����� ó��
    public void FaceUnit(GameObject player, GameObject enemy)
    {
        if (player.transform.localPosition.x > enemy.transform.localPosition.x)
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
            enemy.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            player.transform.localScale = new Vector3(1, 1, 1);
            enemy.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // Ÿ�� ����
    private void ResetTile()
    {
        for (int i = 0; i < targetTile.Length; i++)
        {
            targetTile[i].gameObject.SetActive(false);
        }
    }

    // ���� Ÿ�� ȿ�� ����
    private void SetAttackTile(Unit unit, Unit target, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrAttackRange = SkillMgr.Instance.GetLOCATION(data.range, data);

        // ������ ��ġ�� ����2�� ���ؿ�
        Vector2 unitPos = unit.transform.localPosition;
        for (int i = 0; i < 9; i++)
        {
            if (arrAttackRange[i] != nullvec)
            {
                // ����Ÿ���� ��ġ�� �̵��� ��ġ�� �̵�
                var atkPos = unitPos + DataMgr.Instance.tileInterval * arrAttackRange[i];
                targetTile[i].transform.localPosition = atkPos;

                // ������ �̵��� ��ġ�� �� ũ�⺸�� ũ�� ���� ��츸 Ȱ��ȭ
                if (atkPos.x >= minPos.x && atkPos.x <= maxPos.x && atkPos.y >= minPos.y && atkPos.y <= maxPos.y)
                {
                    // Ÿ���� ���� ���������� ����
                    targetTile[i].color = new Color(0.690196f, 0, 0);
                    // �ش� ����Ÿ�� Ȱ��ȭ
                    targetTile[i].gameObject.SetActive(true);
                }

                // Ÿ���� �����ȿ� ���� ��� �����ȿ� �ִٴ� ��ȣ�� ������ ����
                if (target.transform.localPosition == targetTile[i].transform.localPosition ||
                    IsHit(target.transform.localPosition, targetTile[i].transform.localPosition))
                {
                    target.isInArea = true;
                }
            }
        }
    }

    // Ÿ���� ���� ���� �ȿ� �ִ��� Ȯ��
    private bool IsHit(Vector2 target, Vector2 Area)
    {
        Vector2 errorMargin = DataMgr.Instance.tileInterval * 0.5f;

        if (Area.x - errorMargin.x < target.x &&
            Area.x + errorMargin.x > target.x &&
            Area.y - errorMargin.y < target.y &&
            Area.y + errorMargin.y > target.y)
            return true;
        else return false;
    }    
    /*-------------����---------------*/
    // ���� Ÿ�� ȿ�� ����
    private void SetBuffTile(Unit unit, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrRange = SkillMgr.Instance.GetLOCATION(data.range, data);

        for (int i = 0; i < 9; i++)
        {
            if (arrRange[i] != nullvec)
            {
                // ����Ÿ���� ��ġ�� ���� ��ġ�� �̵�
                targetTile[i].transform.localPosition = unit.transform.localPosition;
                switch (data.thisAction)
                {
                    case Action.HP:
                    case Action.DEF:
                        // Ÿ���� ���� ������� ����
                        targetTile[i].color = new Color(0.17647f, 0.529412f, 0.027450f);
                        break;
                    case Action.MP:
                        // Ÿ���� ���� �Ķ������� ����
                        targetTile[i].color = new Color(0.223529f, 0.298039f, 0.776471f);
                        break;
                    case Action.BUFF:
                        // Ÿ���� ���� ��ȫ������ ����
                        targetTile[i].color = new Color(1.0f, 0.0f, 0.458824f);
                        break;
                }
                // �ش� ����Ÿ�� Ȱ��ȭ
                targetTile[i].gameObject.SetActive(true);
            }
        }
    }

    // ���� ������ ���� ���� ����Ʈ�� �� �߰�
    private void OnBuff(Unit unit, Unit target, Normal data)
    {
        if (data is Utility)
        {
            Utility util = data as Utility;
            Buff buff = new Buff();
            buff.buffTarget = target;

            switch (util.condition)
            {
                case INFLUENCE.HP:
                    buff.addHp = util.value;
                    break;
                case INFLUENCE.MP:
                    buff.addMp = util.cost; //MP�� cost ���� ������
                    break;
                case INFLUENCE.ATK:
                    buff.addAtk = util.value;
                    break;
                case INFLUENCE.DEF:
                    buff.addDef = util.value;
                    break;
            }
            // �� �Ͽ� 2�� ���̱� ������ *2
            buff.turn = util.turns * 2;
            //MPȸ���� �����ϰ� ��� ����
            if (util.condition != INFLUENCE.MP) unit.AddMP(-util.cost);
            buffList.Add(buff);
        }
    }

    // ���� �ߵ�
    private void ActiveBuff()
    {
        // ������ �ϳ��� �������� �۵����� �ʴ´�.
        if (0 >= buffList.Count) return;

        Buff playerBuff = new Buff();
        Buff enemyBuff = new Buff();
        for (int i = 0; i < buffList.Count;)
        {
            // ���� ���ӽð��� ����� ��� �ش� ���� ����
            if (0 >= buffList[i].turn)
            {
                buffList.RemoveAt(i);
                Debug.Log("buff end");
                continue;
            }

            // Ÿ���� �÷��̾��� ���
            if (buffList[i].buffTarget == Player)
            {
                playerBuff = AddToBuff(playerBuff, buffList[i]);
            }
            else
            {
                enemyBuff = AddToBuff(enemyBuff, buffList[i]);
            }
            // ���� �� ����
            buffList[i].DecsTurn();
            Debug.Log("buff active");
            Debug.Log(buffList[i].turn);
            i++;
        }
        BuffProcess(Player, playerBuff);
        BuffProcess(Enemy, enemyBuff);
    }

    // ���� ��ġ��
    private Buff AddToBuff(Buff mainBuff, Buff subbuff)
    {
        mainBuff.addAtk += subbuff.addAtk;
        mainBuff.addDef += subbuff.addDef;
        mainBuff.addHp += subbuff.addHp;
        mainBuff.addMp += subbuff.addMp;
        return mainBuff;
    }

    // ���� ó��
    private void BuffProcess(Unit target, Buff buff)
    {
        target.AddAtk(buff.addAtk);
        target.AddDefense(buff.addDef);
        target.AddHP(buff.addHp);
        target.AddMP(buff.addMp);
    }
    /*-------------����---------------*/

    // ������ ó��
    private void DamageProcess(Unit target,int dmg, int addAtk, int applyCount, int targetDef)
    {
        target.AddHP(Mathf.Min(-1 * (dmg + addAtk + targetDef) * applyCount, 0));
    }
    /*----------------------��ų ����--------------------*/

    private bool WinCheck(Unit player, Unit enemy)
    {
        // ��� ������ ���� �ʾ��� ���
        if (player.hp != 0 && enemy.hp != 0)
        {
            return false;
        }
        // ������ ���� ��� ������ �ʱ�ȭ + ���� ���� UI Ȱ��ȭ
        else
        {
            // ���߿� �ν��Ͻ����� ��� �Լ� ������
            DataMgr.Instance.InitRound();
            DataMgr.Instance.ClearSelectCardList();
            // ���� ���� UI Ȱ��ȭ
            UIMgr.Instance.OnGameSetUI();
        }

        if (player.hp == 0 && enemy.hp == 0)
        {
            player.unitanim.OnDieEnter();
            enemy.unitanim.OnDieEnter();
            UIMgr.Instance.GameOverUI(RESULTSCENE.Draw);
        }
        else if (player.hp == 0)
        {
            player.unitanim.OnDieEnter();
            UIMgr.Instance.GameOverUI(RESULTSCENE.Lose);
        }
        else if (enemy.hp == 0)
        {
            enemy.unitanim.OnDieEnter();
            UIMgr.Instance.GameOverUI(RESULTSCENE.Win);
            // ������ �� óġ���� �ʾҴٸ� ���� ������ �Ѿ
            if (!DataMgr.Instance.IsAllClear())
            {
                DataMgr.Instance.NextEnemy();
                UIMgr.Instance.cardSet.OnSelectUnique();
            }
        }
        return true;
    }

    // GameMgr�� ������ �ִ� ���� �ʱ�ȭ
    public void ResetGameData()
    {
        InitBattleScene();
        ResetTile();
        Player.InitUnit();
        Enemy.InitUnit();
        buffList.Clear();
        // �÷��̾�� �� ī�� ����Ʈ ������ ����.
        for(int i=0;i< playerCardList.Count;i++)
        {
            playerCardList[i].ResetCardUI();
            enemyCardList[i].ResetCardUI();
        }
    }

    // ������ ��� �������� �ʱ�ȭ
    public void ResetGame()
    {
        ResetGameData();
        UIMgr.Instance.ResetUIData();
        DataMgr.Instance.ResetData();
    }
}

