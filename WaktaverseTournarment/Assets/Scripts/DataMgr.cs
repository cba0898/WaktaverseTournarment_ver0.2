using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Character
{
    start = 0,
    Wakgood, 
    Roentgenium, 
    Viichan, 
    Ine, 
    Lilpa, 
    Gosegu,
    Jingburger, 
    Jururu,
    end
}

public class DataMgr : MonoBehaviour
{
    #region instance
    private static DataMgr instance = null;
    public static DataMgr Instance { get { return instance; } }

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

    //--------------텍스트 파일 읽기---------
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    /*--------------캐릭터 정보---------*/
    [SerializeField] private Character currentPlayer;   // 현재 플레이어 캐릭터
    public Character CurrentPlayer { get { return currentPlayer; } set { currentPlayer = value; } }

    [SerializeField] private Character currentEnemy;    // 현재 적 캐릭터
    public Character CurrentEnemy { get { return currentEnemy; } set { currentEnemy = value; } }

    [SerializeField] private List<Character> enemyList = new List<Character>(); // 적 캐릭터 리스트

    private int enemyIndex = 0; // 적 캐릭터 리스트의 인덱스
    [SerializeField] public float playerPosRate;
    [SerializeField] public float enemyPosRate;
    public Vector2 SetUnitPos(float xInterval, float rate)
    {
        Vector2 pos = new Vector2(xInterval * rate, 0f);
        return pos;
    }

    /*--------------캐릭터 정보---------*/

    /*--------------스킬 정보------------*/
    [SerializeField] public Normal[] arrPublicSkill;    // 공용 스크립터블 오브젝트를 받아오기 위한 배열
    [SerializeField] public Normal[] arrPlayerSkill;    // 캐릭터별 스크립터블 오브젝트를 받아오기 위한 배열
    [SerializeField] public Normal[] arrEnemySkill;     // 캐릭터별 스크립터블 오브젝트를 받아오기 위한 배열
    [SerializeField] public Normal[] arrUniqueSkill;    // 캐릭터별 스크립터블 오브젝트를 받아오기 위한 배열
    /*--------------스킬 정보-----------*/


    /*--------------특수 카드 정보------------*/
    [SerializeField] public List<Normal> playerOwnUniqueList;    // 플레이어가 얻은 유니크 스킬 리스트
    [SerializeField] public List<Normal> playerUniqueList;    // 유니크 스킬 데이터를 사용하기 위한 리스트
    [SerializeField] public List<Normal> enemyOwnUniqueList;    // 적이 얻은 유니크 스킬 리스트
    [SerializeField] public List<Normal> enemyUniqueList;    // 유니크 스킬 데이터를 사용하기 위한 리스트
    /*--------------특수 카드 정보------------*/


    /*--------------전투 정보------------*/
    [SerializeField] private List<Card> SelectCardList = new List<Card>();  // 플레이어가 선택한 카드 리스트
    [SerializeField] private List<Card> EnemyCardList = new List<Card>();   // 적이 선택한 카드 리스트

    [SerializeField] private int maxTurn; // 뭐임 이건

    public int turnCount { get; private set; } // 턴(카드 한 장) 진행상황
    public Vector2 tileInterval;    // 타일의 간격

    public int Round { get; private set; } = 1; // 라운드
    // 다음 라운드 세팅
    public void SetNextRound()
    {
        Round++;
        Round = Mathf.Clamp(Round, 0, 99);
    }
    // 라운드 초기화
    public void InitRound()
    {
        Round = 1;
    }
    /*--------------전투 정보------------*/


    /*-------------------스킬 이펙트----------------------*/
    [SerializeField] public GameObject[] skillEffects;  // 스킬 이펙트를 로드해오는 배열
    List<GameObject> skillEffectPool = new List<GameObject>();  // 실제 사용할 스킬 이펙트 리스트
    /*-------------------스킬 이펙트----------------------*/

    /*--------------캐릭터 정보---------*/
    // 적 리스트 얻어옴
    public List<Character> GetEnemyList()
    {
        return enemyList;
    }

    // 현재 마나에서 슬롯에 있는 카드들의 마나 총량을 감소한 값을 반환
    public int GetRemainMana(Normal normal,Unit unit ,int sign)
    {
        int remainMana = unit.mpRemain;
        if (normal is Utility)
        {
            Utility util = normal as Utility;
            // sign : 부호(+1,-1)
            // mp회복의 경우 부호를 역으로 치환한다.
            if (util.thisAction == Action.MP)
                sign = -sign;
            remainMana += sign * util.cost;
        }
        return remainMana;
    }
    /*--------------캐릭터 정보---------*/


    /*-------------------적 매칭 정보----------------------*/
    // 적 순서.
    public void SetEnemy()
    {
        enemyList.Clear();
        switch (CurrentPlayer)
        {
            // Wakgood 진영의 캐릭터 선택 시 보스는 Roentgenuim 고정
            case Character.Wakgood:
            case Character.Viichan:
            case Character.Lilpa:
            case Character.Ine:
                CharShuffle();
                enemyList.Add(Character.Roentgenium);
                break;
            // Roentgenuim 진영의 캐릭터 선택 시 보스는 Wakgood 고정
            case Character.Roentgenium:
            case Character.Jururu:
            case Character.Jingburger:
            case Character.Gosegu:
                CharShuffle();
                enemyList.Add(Character.Wakgood);
                break;
        }
        currentEnemy = enemyList[enemyIndex];
    }

    // 적 매칭을 섞어줌
    private void CharShuffle()
    {
        for (var i = Random.Range(1, (int)Character.end); enemyList.Count < 4; i++)
        {
            // i가 enum 범위 초과 시 초기화
            if ((Character)i >= Character.end)
                i = 1;
            // 본인 제외 && 보스 제외
            if (CurrentPlayer != (Character)i &&
                ((Character)i != Character.Wakgood && (Character)i != Character.Roentgenium))
            {
                enemyList.Add((Character)i);
            }
        }
    }

    // 다음 적
    public void NextEnemy()
    {
        currentEnemy = enemyList[++enemyIndex];
    }
    // 몇 번째 적인지 리턴
    public int GetEnemyIndex()
    {
        return enemyIndex;
    }
    public void ResetEnemyIndex()
    {
        enemyIndex = 0;
    }
    public int GetMaxEnemy()
    {
        return enemyList.Count;
    }

    public bool IsFirstEnemy()
    {
        return (0 == enemyIndex) ? true : false;
    }

    // 모든 적을 쓰러뜨렸는지 확인
    public bool IsAllClear()
    {
        if (enemyList.Count <= enemyIndex + 1)
            return true;
        return false;
    }
    /*-------------------적 매칭 정보----------------------*/


    /*-------------------스킬 애니메이션----------------------*/
    // 해당 스킬 이펙트의 인덱스를 가져옴
    public int GetEffectIndex(Normal normal)
    {
        for (int i = 0; i < skillEffects.Length; i++)
        {
            // 해당 스킬의 이펙트를 찾으면 해당 인덱스를 리턴
            if (skillEffects[i] == normal.effect)
                return i;
        }
        // 해당하는 스킬 이펙트가 없을 경우 -1 리턴
        Debug.LogWarning("Can't found effect");
        return -1;
    }

    // 애니메이션 활성화
    public void PlayAnim(Unit unit, int index)
    {
        var skillObj = skillEffectPool[index];
        skillObj.transform.localPosition = unit.GetUnitPos();
        skillObj.transform.localScale = unit.transform.localScale;
        skillObj.SetActive(UIMgr.Instance.GetEffectField().activeInHierarchy);
    }

    // 애니메이션 비황성화
    public void EndAnim(int index)
    {
        skillEffectPool[index].SetActive(false);
    }

    // 스킬 이펙트 리스트에서 애니메이터를 받아옴
    public Animator GetSkillAnimator(int index)
    {
        return skillEffectPool[index].GetComponentInChildren<Animator>();
    }

    // 애니메이터 내부의 애니메이션이 종료되었는지 확인
    public bool IsEndAnim(Animator anim)
    {
        if (1 <= anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
            return false;
        return true;
    }
    /*-------------------스킬 애니메이션----------------------*/


    /*-------------------카드 정보----------------------*/
    // 이름에 맞는 스프라이트 디렉토리 정보 저장
    public Dictionary<string, T> SetDictionary<T>(string address) where T : Object
    {
        T[] objects = Resources.LoadAll<T>(address);
        Dictionary<string, T> dictionary = new Dictionary<string, T>();

        for (int i = 0; i < objects.Length; i++)
        {
            dictionary[objects[i].name] = objects[i];
        }
        return dictionary;
    }

    // 캐릭터 한글이름 문자열 저장용 딕셔너리 생성
    public Dictionary<string, string> SetNameTable()
    {
        Dictionary<string, string> dataTable = new Dictionary<string, string>();
        var nameTable = Resources.Load<TextAsset>("NameTable");
        var lines = Regex.Split(nameTable.text, LINE_SPLIT_RE);

        for (int i = 0; i < lines.Length; i++)
        {
            var data = lines[i].Split(' ');
            dataTable.Add(data[0], data[1]);
        }
        return dataTable;
    }
    // 해당 캐릭터의 스크립터블 오브젝트를 로드
    public void SetCharSkill(Character character, out Normal[] units)
    {
        string address = string.Format("ScriptableObject/Skills_character/{0}", character);
        units = Resources.LoadAll<Normal>(address);
    }

    // 카드, 특수카드 스킬 데이터를 캐릭터에 맞게 세팅
    public void SetCardData()
    {
        arrPublicSkill = Resources.LoadAll<Normal>("ScriptableObject/Skills_public");
        arrUniqueSkill = Resources.LoadAll<Normal>("ScriptableObject/Skills_Unique");
        // 유니크 카드리스트는 처음에만 초기화
        if (enemyIndex == 0)
            InitUniqueList();

        SetCharSkill(currentPlayer, out arrPlayerSkill);
        SetCharSkill(currentEnemy, out arrEnemySkill);

        for (int i = 0; i < skillEffects.Length; i++)
        {
            var skill = Instantiate(skillEffects[i], new Vector3(0, 0, 0), Quaternion.identity, UIMgr.Instance.GetEffectField().transform);
            skill.SetActive(false);
            skillEffectPool.Add(skill);
        }
        enemyOwnUniqueList.Clear();
        int[] randoms = new int[enemyIndex];
        for (int i = 0; i < enemyIndex; i++)
        {
            // 초기값 설정
            randoms[i] = -1;
        }
        for (int i = 0, j = 0; i < enemyIndex; j++)
        {
            if (j > enemyUniqueList.Count)
            {
                Debug.Log("can't found enemyUniqueCard");
                break;
            }
            int randomNum = Random.Range(0, enemyUniqueList.Count);
            // 중복되지 않는 카드만 넣는다.
            if (!IsInArray(randoms, randomNum))
            {
                randoms[i] = randomNum;
                enemyOwnUniqueList.Add(enemyUniqueList[randoms[i]]);
                i++;
            }
        }

        // 플레이어는 카드를 배치
        UIMgr.Instance.ArrangeCard(arrPublicSkill, arrPlayerSkill, playerOwnUniqueList);
    }
    private bool IsInArray(int[] array, int value)
    {
        for (int j = 0; j < array.Length; j++)
        {
            if (array[j] == value)
                return true;
        }
        return false;
    }

    // 특수카드 리스트 초기화
    private void InitUniqueList()
    {
        playerUniqueList = new List<Normal>(arrUniqueSkill);
        enemyUniqueList = new List<Normal>(arrUniqueSkill);
    }

    // 특수카드 3장 추가
    public void SetUniqueCards(Card[] cards)
    {
        if (null == playerUniqueList||0 >= playerUniqueList.Count) return;

        int[] exist = new int[3] { -1, -1, -1 };   // 해당 카드가 존재하는지 확인하는 배열
        int existCount = 0;         // 중복체크용 배열의 인덱스
        bool isOverlap = false;     //중복체크 

        int cardIndex = 0;
        // 랜덤한 세 장의 카드에 데이터 저장
        while(cardIndex < 3)
        {           
            // 랜덤한 카드 인덱스 추출
            int randomIndex = Random.Range(0, playerUniqueList.Count);
            isOverlap = false;
            for (int i = 0; i <= existCount; i++)
            {
                // 중복되는 인덱스가 존재할 경우 중복체크
                if (exist[i] == randomIndex)
                {
                    isOverlap = true;
                }
            }
            // 중복되지 않는 스킬만 등록
            if (!isOverlap)
            {
                exist[existCount++] = randomIndex;
                cards[cardIndex].SetData(playerUniqueList[randomIndex]);
                cardIndex++;
            }

        }
    }

    // 카드리스트에 추가
    public void AddCardList(int index, Card card)
    {
        if (index < SelectCardList.Count)
            SelectCardList.Insert(index, card);
        else SelectCardList.Add(card);
    }
    public void AddEnemyCardList(Card card)
    {
        EnemyCardList.Add(card);
    }
    // 카드리스트에 해당 카드 제거
    public void RemoveCardList(Card card)
    {
        SelectCardList.Remove(card);
    }
    // 카드 리스트 초기화
    public void ClearSelectCardList()
    {
        foreach(var card in SelectCardList)
        {
            card.SetOriginPos();
        }
        SelectCardList.Clear();
    }
    // 적의 카드 리스트 초기화
    public void ClearEnemyCardList()
    {
        EnemyCardList.Clear();
    }

    // 카드리스트에 있는지 확인
    public bool IsOnCardList(Card card)
    {
        return (SelectCardList.Contains(card));
    }

    // 배틀 씬에서의 카드 리스트의 개수
    public int GetCardListCount()
    {
        return (SelectCardList.Count);
    }

    // 플레이어 카드 리스트 가져오기
    public List<Card> GetPlayerCardList()
    {
        return SelectCardList;
    }

    // 적 카드 리스트 가져오기
    public List<Card> GetEnemyCardList()
    {
        return EnemyCardList;
    }
    
    // 플레이어 카드 한장(1턴) 가져오기
    public Card GetPlayerCard()
    {
        return SelectCardList[turnCount];
    }

    /*
    public Normal GetEnemyCardData()
    {
        List<Normal> normalList = new List<Normal>();
        EnemyAI eAi = new EnemyAI();
        Action curAction;

        curAction = eAi.GetEnemyAction(GameMgr.Instance.Player, GameMgr.Instance.Enemy);

        // 공용스킬 세팅
        for (int i = 0; i < arrPublicSkill.Length; i++)
        {
            if (!arrEnemySkill[i].isUsed)
            {
                arrEnemySkill[i].isUsed = true;
                normalList.Add(arrPublicSkill[i]);
            }
        }

        // 캐릭터 전용스킬 세팅
        for (int i = 0; i < arrEnemySkill.Length; i++)
        {
            if (curAction == arrEnemySkill[i].thisAction && !arrEnemySkill[i].isUsed)
            {
                arrEnemySkill[i].isUsed = true;
                normalList.Add(arrEnemySkill[i]);
            }
        }
        return normalList[Random.Range(0, normalList.Count)];
    }*/

    /*-------------------카드 정보----------------------*/

    /*--------------전투 정보------------*/
    // 턴(카드 한장) 넘기기
    public void NextTurn()
    {
        turnCount++;
    }

    // 턴 카운트 초기화
    public void InitTurnCount()
    {
        turnCount = 0;
    }

    // 버프 설명 텍스트 삽입
    public void SetBuffData(string text, Normal data)
    {
        text = data.discription;
    }

    // 특수카드 정보 초기화
    public void ResetUniqueList()
    {
        playerOwnUniqueList.Clear();    // 플레이어가 얻은 유니크 스킬 리스트
        playerUniqueList.Clear();    // 유니크 스킬 데이터를 사용하기 위한 리스트
        enemyOwnUniqueList.Clear();    // 적이 얻은 유니크 스킬 리스트
        enemyUniqueList.Clear();    // 유니크 스킬 데이터를 사용하기 위한 리스트
    }
    /*--------------전투 정보------------*/

    // DataMgr가 관리하는 변동 정보 제거
    public void ResetData()
    {
        //CurrentPlayer = Character.start;
        CurrentEnemy = Character.start;
        InitRound();
        InitTurnCount();
        ClearSelectCardList();
        ClearEnemyCardList();
        ResetUniqueList();
        ResetEnemyIndex();
    }
}
