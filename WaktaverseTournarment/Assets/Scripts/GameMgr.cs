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
        // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance를 유일 오브젝트로 만든다
        instance = this;

        // Scene 이동 시 삭제 되지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public Unit Player;
    public Unit Enemy;

    public Vector2 minPos;      // 위치의 최저값
    public Vector2 maxPos;      // 위치의 최대값

    [SerializeField] public Image playerImg;    // 플레이어 유닛 이미지
    [SerializeField] public Image enemyImg;     // 적 유닛 이미지
    [SerializeField] private Image[] targetTile;
    [SerializeField] private List<Card> playerCardList;     // 플레이어 카드 리스트
    public int GetCardListCount()
    {
        return playerCardList.Count;
    }
    [SerializeField] private List<Card> enemyCardList;      // 적 카드 리스트
    private List<Buff> buffList = new List<Buff>();         // 버프 리스트

    private Coroutine StartRoundCoroutine = null;

    // 게임 시작
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

        // 턴 초기화
        DataMgr.Instance.InitTurnCount();

        // 얘는 따로 어디 가야댐
        if (1 == DataMgr.Instance.Round)
        {
            UIMgr.Instance.miniMap.InitMiniMapPos();
            InitCharPos();
        }

        SetSelectedCards();

        StartRoundCoroutine = StartCoroutine(StartRound());
    }

    // 이렇게 쓰면 안됨
    public void InitCharPos()
    {        
        Player.transform.localPosition = DataMgr.Instance.SetUnitPos(DataMgr.Instance.tileInterval.x, DataMgr.Instance.playerPosRate);
        Enemy.transform.localPosition = DataMgr.Instance.SetUnitPos(DataMgr.Instance.tileInterval.x, DataMgr.Instance.enemyPosRate);

    }

    // 배틀 화면 초기화
    public void InitBattleScene()
    {
        // 캐릭터 위치 초기화
        InitCharPos();
        FaceUnit(Player.gameObject, Enemy.gameObject);
        UIMgr.Instance.miniMap.InitMiniMapPos();
        // 배틀을 처음 시작한 경우에만 초기 위치를 저장
        if (DataMgr.Instance.IsFirstEnemy())
        {
            for (int i = 0; i < 3; i++)
            {
                playerCardList[i].InitOriginPos();
                enemyCardList[i].InitOriginPos();
            }
        }
    }

    /*----------------------전투--------------------*/
    // 라운드 진행
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

    // 턴 진행
    private IEnumerator StartTurn()
    {
        // 첫 번째 턴(카드를 처음 뒤집었을 때) 카드 세팅 / 얘도 딴데 가야할듯
        //if (0 == DataMgr.Instance.turnCount) SetSelectedCards();

        var playerCard = playerCardList[DataMgr.Instance.turnCount];
        var enemyCard = enemyCardList[DataMgr.Instance.turnCount];

        // 첫 번째 카드가 뒤집히는 애니메이션 실행
        playerCard.CardOpen();
        enemyCard.CardOpen();

        while (playerCard.isPlayAnimation && enemyCard.isPlayAnimation) yield return null;

        // 스킬의 우선순위가 높은(priority값이 낮은) 순서대로 실행
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

        // 스킬 실행이 끝나면 비활성화
        playerCard.gameObject.SetActive(false);
        enemyCard.gameObject.SetActive(false);
    }

    // 전투 화면에 선택한 카드들을 세팅
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

    // 배틀 화면 카드 세팅
    private void SetBattleCard(Card card, Normal data)
    {
        card.InitBattleCard(data);
    }
    /*----------------------전투--------------------*/


    /*----------------------스킬 적용--------------------*/
    // 플레이어, 적에 맞게 목표를 지정
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
        //매치된 이펙트가 있을 경우에만 실행
        if (-1 != index)
        {
            // 이펙트 실행
            DataMgr.Instance.PlayAnim(unit, index);
            // 이펙트 실행이 끝날때까지 대기
            var anim = DataMgr.Instance.GetSkillAnimator(index);
            while (DataMgr.Instance.IsEndAnim(anim)) yield return null;
            DataMgr.Instance.EndAnim(index);
        }
    }

    // 카드 실행
    private IEnumerator CardAction(Unit unit, Card card)
    {
        Unit target;
        // 해당 이펙트의 인덱스를 불러옴
        var effectIndex = DataMgr.Instance.GetEffectIndex(card.skillData);

        if (card.skillData.target == TARGET.PLAYER)  target = Player;   
        else target = Enemy;
        target = SetTarget(unit, target);

        // 공격 액션
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
        // 유틸 액션
        else if (card.skillData is Utility)
        {
            SetBuffTile(unit, card.skillData);
            yield return PlayAnim(unit, effectIndex);
            OnBuff(unit, target, card.skillData);
        }
        // 이동 액션
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
        // Die 애니메이션 종료
        if (Player.unitanim.anim) Player.unitanim.anim.SetBool("isDie", false);
        if (Enemy.unitanim.anim) Enemy.unitanim.anim.SetBool("isDie", false);
        // 캐릭터 오브젝트 초기화
        Player.gameObject.SetActive(false);
        Player.gameObject.SetActive(true);
        Enemy.gameObject.SetActive(false);
        Enemy.gameObject.SetActive(true);
    }

    // 이동 처리 코루틴
    private IEnumerator MoveCorutine(Unit unit, Vector2 destination, Vector2 direction)
    {
        bool isBack = false;

        // 유닛이 어느쪽을 바라보는지 체크 후 뒤로 이동하는 경우 뒤로가는 애니메이션 재생
        if (unit.transform.localScale == new Vector3(1, 1, 1) && direction == new Vector2(-1, 0)
            || unit.transform.localScale == new Vector3(-1, 1, 1) && direction == new Vector2(1, 0))
        {
            isBack = true;
        }

        // 액션 시작
        if (isBack)
            unit.unitanim.OnBackEnter();
        else
            unit.unitanim.OnActionEnter();

        // 액션 중 실제 이동을 시작하는 시점까지 대기
        while (!unit.unitanim.isMove) yield return null;

        float t = 0;
        Vector3 unitPos = unit.transform.localPosition;
        // 이동 액션이 끝날 때 까지만 이동
        while (unit.unitanim.isMove)
        {
            // 이동거리가 1(0~1)을 넘었을 경우 1로 조정
            if (t > 1) t = 1;
            unit.transform.localPosition = Vector3.Lerp(unitPos, destination, t);
            yield return null;
            t += 0.1f;
        }

        // 액션 종료. 다시 Idle로 전환
        if (isBack)
            unit.unitanim.OnBackExit();
        else
            unit.unitanim.OnActionExit();
    }

    // 무브 타일 효과 세팅
    private Vector2 SetMoveTile(Unit unit, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrMoveRange = SkillMgr.Instance.GetLOCATION(data.movePos, data);

        // 유닛의 위치를 벡터2로 구해옴
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
                // 유닛이 이동할 위치를 계산
                movedPos += DataMgr.Instance.tileInterval * arrMoveRange[i] * data.MoveCount;

                // 유닛이 이동할 위치가 맵 크기보다 클 경우 제자리 이동
                if (movedPos.x < minPos.x || movedPos.x > maxPos.x || movedPos.y < minPos.y || movedPos.y > maxPos.y)
                    movedPos = unitPos;
                else
                {            
                    // 문제가 없을 경우 미니맵 캐릭터의 위치를 계산
                    miniMapPos += mini.miniMapInterval * arrMoveRange[i];
                }
                // 미니맵 위치 적용
                mini.SetMiniMapPos(unit, miniMapPos);

                // 무브타일의 위치를 이동할 위치로 이동
                targetTile[i].transform.localPosition = movedPos;
                // 타일의 색을 녹색으로 변경
                targetTile[i].color = new Color(0.17647f, 0.529412f, 0.027450f);
                // 해당 무브타일 활성화
                targetTile[i].gameObject.SetActive(true);

                // 유닛은 해당 이동범위로 이동 시작
                StartCoroutine(MoveCorutine(unit, movedPos, arrMoveRange[i]));
            }
        }
        return movedPos;
    }

    // 유닛을 마주보도록 처리
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

    // 타일 리셋
    private void ResetTile()
    {
        for (int i = 0; i < targetTile.Length; i++)
        {
            targetTile[i].gameObject.SetActive(false);
        }
    }

    // 공격 타일 효과 세팅
    private void SetAttackTile(Unit unit, Unit target, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrAttackRange = SkillMgr.Instance.GetLOCATION(data.range, data);

        // 유닛의 위치를 벡터2로 구해옴
        Vector2 unitPos = unit.transform.localPosition;
        for (int i = 0; i < 9; i++)
        {
            if (arrAttackRange[i] != nullvec)
            {
                // 어택타일의 위치를 이동할 위치로 이동
                var atkPos = unitPos + DataMgr.Instance.tileInterval * arrAttackRange[i];
                targetTile[i].transform.localPosition = atkPos;

                // 유닛이 이동할 위치가 맵 크기보다 크지 않을 경우만 활성화
                if (atkPos.x >= minPos.x && atkPos.x <= maxPos.x && atkPos.y >= minPos.y && atkPos.y <= maxPos.y)
                {
                    // 타일의 색을 붉은색으로 변경
                    targetTile[i].color = new Color(0.690196f, 0, 0);
                    // 해당 어택타일 활성화
                    targetTile[i].gameObject.SetActive(true);
                }

                // 타겟이 범위안에 있을 경우 범위안에 있다는 신호를 참으로 세팅
                if (target.transform.localPosition == targetTile[i].transform.localPosition ||
                    IsHit(target.transform.localPosition, targetTile[i].transform.localPosition))
                {
                    target.isInArea = true;
                }
            }
        }
    }

    // 타겟이 공격 범위 안에 있는지 확인
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
    /*-------------버프---------------*/
    // 버프 타일 효과 세팅
    private void SetBuffTile(Unit unit, Normal data)
    {
        var nullvec = new Vector2(2, 2);
        Vector2[] arrRange = SkillMgr.Instance.GetLOCATION(data.range, data);

        for (int i = 0; i < 9; i++)
        {
            if (arrRange[i] != nullvec)
            {
                // 무브타일의 위치를 유닛 위치로 이동
                targetTile[i].transform.localPosition = unit.transform.localPosition;
                switch (data.thisAction)
                {
                    case Action.HP:
                    case Action.DEF:
                        // 타일의 색을 녹색으로 변경
                        targetTile[i].color = new Color(0.17647f, 0.529412f, 0.027450f);
                        break;
                    case Action.MP:
                        // 타일의 색을 파란색으로 변경
                        targetTile[i].color = new Color(0.223529f, 0.298039f, 0.776471f);
                        break;
                    case Action.BUFF:
                        // 타일의 색을 분홍색으로 변경
                        targetTile[i].color = new Color(1.0f, 0.0f, 0.458824f);
                        break;
                }
                // 해당 무브타일 활성화
                targetTile[i].gameObject.SetActive(true);
            }
        }
    }

    // 버프 적용을 위해 버프 리스트에 값 추가
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
                    buff.addMp = util.cost; //MP는 cost 값을 가져옴
                    break;
                case INFLUENCE.ATK:
                    buff.addAtk = util.value;
                    break;
                case INFLUENCE.DEF:
                    buff.addDef = util.value;
                    break;
            }
            // 한 턴에 2씩 깎이기 때문에 *2
            buff.turn = util.turns * 2;
            //MP회복을 제외하고 계산 적용
            if (util.condition != INFLUENCE.MP) unit.AddMP(-util.cost);
            buffList.Add(buff);
        }
    }

    // 버프 발동
    private void ActiveBuff()
    {
        // 버프가 하나도 없을때는 작동하지 않는다.
        if (0 >= buffList.Count) return;

        Buff playerBuff = new Buff();
        Buff enemyBuff = new Buff();
        for (int i = 0; i < buffList.Count;)
        {
            // 버프 지속시간이 만료된 경우 해당 버프 제거
            if (0 >= buffList[i].turn)
            {
                buffList.RemoveAt(i);
                Debug.Log("buff end");
                continue;
            }

            // 타겟이 플레이어일 경우
            if (buffList[i].buffTarget == Player)
            {
                playerBuff = AddToBuff(playerBuff, buffList[i]);
            }
            else
            {
                enemyBuff = AddToBuff(enemyBuff, buffList[i]);
            }
            // 지속 턴 감소
            buffList[i].DecsTurn();
            Debug.Log("buff active");
            Debug.Log(buffList[i].turn);
            i++;
        }
        BuffProcess(Player, playerBuff);
        BuffProcess(Enemy, enemyBuff);
    }

    // 버프 합치기
    private Buff AddToBuff(Buff mainBuff, Buff subbuff)
    {
        mainBuff.addAtk += subbuff.addAtk;
        mainBuff.addDef += subbuff.addDef;
        mainBuff.addHp += subbuff.addHp;
        mainBuff.addMp += subbuff.addMp;
        return mainBuff;
    }

    // 버프 처리
    private void BuffProcess(Unit target, Buff buff)
    {
        target.AddAtk(buff.addAtk);
        target.AddDefense(buff.addDef);
        target.AddHP(buff.addHp);
        target.AddMP(buff.addMp);
    }
    /*-------------버프---------------*/

    // 데미지 처리
    private void DamageProcess(Unit target,int dmg, int addAtk, int applyCount, int targetDef)
    {
        target.AddHP(Mathf.Min(-1 * (dmg + addAtk + targetDef) * applyCount, 0));
    }
    /*----------------------스킬 적용--------------------*/

    private bool WinCheck(Unit player, Unit enemy)
    {
        // 어느 누구도 죽지 않았을 경우
        if (player.hp != 0 && enemy.hp != 0)
        {
            return false;
        }
        // 결판이 났을 경우 데이터 초기화 + 시합 종료 UI 활성화
        else
        {
            // 나중에 인스턴스별로 묶어서 함수 만들자
            DataMgr.Instance.InitRound();
            DataMgr.Instance.ClearSelectCardList();
            // 시합 종료 UI 활성화
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
            // 전원을 다 처치하지 않았다면 다음 적으로 넘어감
            if (!DataMgr.Instance.IsAllClear())
            {
                DataMgr.Instance.NextEnemy();
                UIMgr.Instance.cardSet.OnSelectUnique();
            }
        }
        return true;
    }

    // GameMgr가 가지고 있는 정보 초기화
    public void ResetGameData()
    {
        InitBattleScene();
        ResetTile();
        Player.InitUnit();
        Enemy.InitUnit();
        buffList.Clear();
        // 플레이어와 적 카드 리스트 개수는 같다.
        for(int i=0;i< playerCardList.Count;i++)
        {
            playerCardList[i].ResetCardUI();
            enemyCardList[i].ResetCardUI();
        }
    }

    // 게임의 모든 변동정보 초기화
    public void ResetGame()
    {
        ResetGameData();
        UIMgr.Instance.ResetUIData();
        DataMgr.Instance.ResetData();
    }
}

