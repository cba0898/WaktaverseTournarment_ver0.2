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

    //--------------�ؽ�Ʈ ���� �б�---------
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    /*--------------ĳ���� ����---------*/
    [SerializeField] private Character currentPlayer;   // ���� �÷��̾� ĳ����
    public Character CurrentPlayer { get { return currentPlayer; } set { currentPlayer = value; } }

    [SerializeField] private Character currentEnemy;    // ���� �� ĳ����
    public Character CurrentEnemy { get { return currentEnemy; } set { currentEnemy = value; } }

    [SerializeField] private List<Character> enemyList = new List<Character>(); // �� ĳ���� ����Ʈ

    private int enemyIndex = 0; // �� ĳ���� ����Ʈ�� �ε���
    [SerializeField] public float playerPosRate;
    [SerializeField] public float enemyPosRate;
    public Vector2 SetUnitPos(float xInterval, float rate)
    {
        Vector2 pos = new Vector2(xInterval * rate, 0f);
        return pos;
    }

    /*--------------ĳ���� ����---------*/

    /*--------------��ų ����------------*/
    [SerializeField] public Normal[] arrPublicSkill;    // ���� ��ũ���ͺ� ������Ʈ�� �޾ƿ��� ���� �迭
    [SerializeField] public Normal[] arrPlayerSkill;    // ĳ���ͺ� ��ũ���ͺ� ������Ʈ�� �޾ƿ��� ���� �迭
    [SerializeField] public Normal[] arrEnemySkill;     // ĳ���ͺ� ��ũ���ͺ� ������Ʈ�� �޾ƿ��� ���� �迭
    [SerializeField] public Normal[] arrUniqueSkill;    // ĳ���ͺ� ��ũ���ͺ� ������Ʈ�� �޾ƿ��� ���� �迭
    /*--------------��ų ����-----------*/


    /*--------------Ư�� ī�� ����------------*/
    [SerializeField] public List<Normal> playerOwnUniqueList;    // �÷��̾ ���� ����ũ ��ų ����Ʈ
    [SerializeField] public List<Normal> playerUniqueList;    // ����ũ ��ų �����͸� ����ϱ� ���� ����Ʈ
    [SerializeField] public List<Normal> enemyOwnUniqueList;    // ���� ���� ����ũ ��ų ����Ʈ
    [SerializeField] public List<Normal> enemyUniqueList;    // ����ũ ��ų �����͸� ����ϱ� ���� ����Ʈ
    /*--------------Ư�� ī�� ����------------*/


    /*--------------���� ����------------*/
    [SerializeField] private List<Card> SelectCardList = new List<Card>();  // �÷��̾ ������ ī�� ����Ʈ
    [SerializeField] private List<Card> EnemyCardList = new List<Card>();   // ���� ������ ī�� ����Ʈ

    [SerializeField] private int maxTurn; // ���� �̰�

    public int turnCount { get; private set; } // ��(ī�� �� ��) �����Ȳ
    public Vector2 tileInterval;    // Ÿ���� ����

    public int Round { get; private set; } = 1; // ����
    // ���� ���� ����
    public void SetNextRound()
    {
        Round++;
        Round = Mathf.Clamp(Round, 0, 99);
    }
    // ���� �ʱ�ȭ
    public void InitRound()
    {
        Round = 1;
    }
    /*--------------���� ����------------*/


    /*-------------------��ų ����Ʈ----------------------*/
    [SerializeField] public GameObject[] skillEffects;  // ��ų ����Ʈ�� �ε��ؿ��� �迭
    List<GameObject> skillEffectPool = new List<GameObject>();  // ���� ����� ��ų ����Ʈ ����Ʈ
    /*-------------------��ų ����Ʈ----------------------*/

    /*--------------ĳ���� ����---------*/
    // �� ����Ʈ ����
    public List<Character> GetEnemyList()
    {
        return enemyList;
    }

    // ���� �������� ���Կ� �ִ� ī����� ���� �ѷ��� ������ ���� ��ȯ
    public int GetRemainMana(Normal normal,Unit unit ,int sign)
    {
        int remainMana = unit.mpRemain;
        if (normal is Utility)
        {
            Utility util = normal as Utility;
            // sign : ��ȣ(+1,-1)
            // mpȸ���� ��� ��ȣ�� ������ ġȯ�Ѵ�.
            if (util.thisAction == Action.MP)
                sign = -sign;
            remainMana += sign * util.cost;
        }
        return remainMana;
    }
    /*--------------ĳ���� ����---------*/


    /*-------------------�� ��Ī ����----------------------*/
    // �� ����.
    public void SetEnemy()
    {
        enemyList.Clear();
        switch (CurrentPlayer)
        {
            // Wakgood ������ ĳ���� ���� �� ������ Roentgenuim ����
            case Character.Wakgood:
            case Character.Viichan:
            case Character.Lilpa:
            case Character.Ine:
                CharShuffle();
                enemyList.Add(Character.Roentgenium);
                break;
            // Roentgenuim ������ ĳ���� ���� �� ������ Wakgood ����
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

    // �� ��Ī�� ������
    private void CharShuffle()
    {
        for (var i = Random.Range(1, (int)Character.end); enemyList.Count < 4; i++)
        {
            // i�� enum ���� �ʰ� �� �ʱ�ȭ
            if ((Character)i >= Character.end)
                i = 1;
            // ���� ���� && ���� ����
            if (CurrentPlayer != (Character)i &&
                ((Character)i != Character.Wakgood && (Character)i != Character.Roentgenium))
            {
                enemyList.Add((Character)i);
            }
        }
    }

    // ���� ��
    public void NextEnemy()
    {
        currentEnemy = enemyList[++enemyIndex];
    }
    // �� ��° ������ ����
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

    // ��� ���� �����߷ȴ��� Ȯ��
    public bool IsAllClear()
    {
        if (enemyList.Count <= enemyIndex + 1)
            return true;
        return false;
    }
    /*-------------------�� ��Ī ����----------------------*/


    /*-------------------��ų �ִϸ��̼�----------------------*/
    // �ش� ��ų ����Ʈ�� �ε����� ������
    public int GetEffectIndex(Normal normal)
    {
        for (int i = 0; i < skillEffects.Length; i++)
        {
            // �ش� ��ų�� ����Ʈ�� ã���� �ش� �ε����� ����
            if (skillEffects[i] == normal.effect)
                return i;
        }
        // �ش��ϴ� ��ų ����Ʈ�� ���� ��� -1 ����
        Debug.LogWarning("Can't found effect");
        return -1;
    }

    // �ִϸ��̼� Ȱ��ȭ
    public void PlayAnim(Unit unit, int index)
    {
        var skillObj = skillEffectPool[index];
        skillObj.transform.localPosition = unit.GetUnitPos();
        skillObj.transform.localScale = unit.transform.localScale;
        skillObj.SetActive(UIMgr.Instance.GetEffectField().activeInHierarchy);
    }

    // �ִϸ��̼� ��Ȳ��ȭ
    public void EndAnim(int index)
    {
        skillEffectPool[index].SetActive(false);
    }

    // ��ų ����Ʈ ����Ʈ���� �ִϸ����͸� �޾ƿ�
    public Animator GetSkillAnimator(int index)
    {
        return skillEffectPool[index].GetComponentInChildren<Animator>();
    }

    // �ִϸ����� ������ �ִϸ��̼��� ����Ǿ����� Ȯ��
    public bool IsEndAnim(Animator anim)
    {
        if (1 <= anim.GetCurrentAnimatorStateInfo(0).normalizedTime)
            return false;
        return true;
    }
    /*-------------------��ų �ִϸ��̼�----------------------*/


    /*-------------------ī�� ����----------------------*/
    // �̸��� �´� ��������Ʈ ���丮 ���� ����
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

    // ĳ���� �ѱ��̸� ���ڿ� ����� ��ųʸ� ����
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
    // �ش� ĳ������ ��ũ���ͺ� ������Ʈ�� �ε�
    public void SetCharSkill(Character character, out Normal[] units)
    {
        string address = string.Format("ScriptableObject/Skills_character/{0}", character);
        units = Resources.LoadAll<Normal>(address);
    }

    // ī��, Ư��ī�� ��ų �����͸� ĳ���Ϳ� �°� ����
    public void SetCardData()
    {
        arrPublicSkill = Resources.LoadAll<Normal>("ScriptableObject/Skills_public");
        arrUniqueSkill = Resources.LoadAll<Normal>("ScriptableObject/Skills_Unique");
        // ����ũ ī�帮��Ʈ�� ó������ �ʱ�ȭ
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
            // �ʱⰪ ����
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
            // �ߺ����� �ʴ� ī�常 �ִ´�.
            if (!IsInArray(randoms, randomNum))
            {
                randoms[i] = randomNum;
                enemyOwnUniqueList.Add(enemyUniqueList[randoms[i]]);
                i++;
            }
        }

        // �÷��̾�� ī�带 ��ġ
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

    // Ư��ī�� ����Ʈ �ʱ�ȭ
    private void InitUniqueList()
    {
        playerUniqueList = new List<Normal>(arrUniqueSkill);
        enemyUniqueList = new List<Normal>(arrUniqueSkill);
    }

    // Ư��ī�� 3�� �߰�
    public void SetUniqueCards(Card[] cards)
    {
        if (null == playerUniqueList||0 >= playerUniqueList.Count) return;

        int[] exist = new int[3] { -1, -1, -1 };   // �ش� ī�尡 �����ϴ��� Ȯ���ϴ� �迭
        int existCount = 0;         // �ߺ�üũ�� �迭�� �ε���
        bool isOverlap = false;     //�ߺ�üũ 

        int cardIndex = 0;
        // ������ �� ���� ī�忡 ������ ����
        while(cardIndex < 3)
        {           
            // ������ ī�� �ε��� ����
            int randomIndex = Random.Range(0, playerUniqueList.Count);
            isOverlap = false;
            for (int i = 0; i <= existCount; i++)
            {
                // �ߺ��Ǵ� �ε����� ������ ��� �ߺ�üũ
                if (exist[i] == randomIndex)
                {
                    isOverlap = true;
                }
            }
            // �ߺ����� �ʴ� ��ų�� ���
            if (!isOverlap)
            {
                exist[existCount++] = randomIndex;
                cards[cardIndex].SetData(playerUniqueList[randomIndex]);
                cardIndex++;
            }

        }
    }

    // ī�帮��Ʈ�� �߰�
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
    // ī�帮��Ʈ�� �ش� ī�� ����
    public void RemoveCardList(Card card)
    {
        SelectCardList.Remove(card);
    }
    // ī�� ����Ʈ �ʱ�ȭ
    public void ClearSelectCardList()
    {
        foreach(var card in SelectCardList)
        {
            card.SetOriginPos();
        }
        SelectCardList.Clear();
    }
    // ���� ī�� ����Ʈ �ʱ�ȭ
    public void ClearEnemyCardList()
    {
        EnemyCardList.Clear();
    }

    // ī�帮��Ʈ�� �ִ��� Ȯ��
    public bool IsOnCardList(Card card)
    {
        return (SelectCardList.Contains(card));
    }

    // ��Ʋ �������� ī�� ����Ʈ�� ����
    public int GetCardListCount()
    {
        return (SelectCardList.Count);
    }

    // �÷��̾� ī�� ����Ʈ ��������
    public List<Card> GetPlayerCardList()
    {
        return SelectCardList;
    }

    // �� ī�� ����Ʈ ��������
    public List<Card> GetEnemyCardList()
    {
        return EnemyCardList;
    }
    
    // �÷��̾� ī�� ����(1��) ��������
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

        // ���뽺ų ����
        for (int i = 0; i < arrPublicSkill.Length; i++)
        {
            if (!arrEnemySkill[i].isUsed)
            {
                arrEnemySkill[i].isUsed = true;
                normalList.Add(arrPublicSkill[i]);
            }
        }

        // ĳ���� ���뽺ų ����
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

    /*-------------------ī�� ����----------------------*/

    /*--------------���� ����------------*/
    // ��(ī�� ����) �ѱ��
    public void NextTurn()
    {
        turnCount++;
    }

    // �� ī��Ʈ �ʱ�ȭ
    public void InitTurnCount()
    {
        turnCount = 0;
    }

    // ���� ���� �ؽ�Ʈ ����
    public void SetBuffData(string text, Normal data)
    {
        text = data.discription;
    }

    // Ư��ī�� ���� �ʱ�ȭ
    public void ResetUniqueList()
    {
        playerOwnUniqueList.Clear();    // �÷��̾ ���� ����ũ ��ų ����Ʈ
        playerUniqueList.Clear();    // ����ũ ��ų �����͸� ����ϱ� ���� ����Ʈ
        enemyOwnUniqueList.Clear();    // ���� ���� ����ũ ��ų ����Ʈ
        enemyUniqueList.Clear();    // ����ũ ��ų �����͸� ����ϱ� ���� ����Ʈ
    }
    /*--------------���� ����------------*/

    // DataMgr�� �����ϴ� ���� ���� ����
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
