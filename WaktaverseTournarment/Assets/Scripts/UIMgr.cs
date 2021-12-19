using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SLOT
{
    add = 1,
    sub = -1
}

public enum SCENE
{
    Main,
    CharSelect,
    CharMatch,
    Play,
    Battle,
    CardSet,
    GameOver
}

public enum BUTTON
{
    Main_Start,
    CharSelect_Select,
    CharMatch_Start,
    CharMatch_Change,
    CardSet_Start,
    CardSet_Clear,
    NextRound
}

public enum RESULTSCENE
{
    Win,
    Lose,
    Draw
}

public class UIMgr : MonoBehaviour
{
    #region instance
    private static UIMgr instance = null;
    public static UIMgr Instance { get { return instance; } }

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


    //--------------ȭ�� ����Ʈ-------------
    [Header("ȭ�� ����Ʈ")]
    [SerializeField] private List<GameObject> sceneList;  // �� ����Ʈ

    //--------------ī�� ���� ȭ��-----------
    [Header("ī�� ���� ȭ��")]
    [SerializeField] private List<Card> cardList;  //ī�� ����Ʈ

    [SerializeField] private Vector2 cardPos;    // ī���� ��ġ
    private Vector2 cardSize;   // ī���� ũ��

    [SerializeField] private List<Toggle> slotList;   //slot ��ġ�� �޾ƿ��� ���� ����Ʈ
    private int currentSlotIndex;    // ���� ������ ���� ��ġ �ε���
    private bool[] isSlotSet;   // ���Կ� ī�尡 ���õǾ� �ִ��� Ȯ���ϴ� ����

    public Vector3 selectSlotPos { get { return slotList[currentSlotIndex].transform.position; } } // ������ ������ ��ġ

    [SerializeField] private Card[] uniqueCards;    // Ư��ī�� �迭

    [SerializeField] public CardSet cardSet;    // ī�� ���� ȭ�� ��ũ��Ʈ
    [SerializeField] private MiniMap miniMap;    // �̴ϸ�
    public MiniMap GetMiniMap()
    {
        return miniMap;
    }

    [SerializeField] private Button battleStartButton; // ��Ʋ ���� ��ư
    //--------------ī�� ���� ȭ��-----------


    //--------------��� ü�� �� UI----------
    [Header("��� ü�� ��")]
    [SerializeField] private Image playerIcon;    // �÷��̾� �̹���
    [SerializeField] private Image enemyIcon;     // �� �̹���
    [SerializeField] private Text PlayerName;     // �÷��̾� �̸�
    [SerializeField] private Text EnemyName;     // �� �̸�
    [SerializeField] public Round roundImg;
    //--------------��� ü�� �� UI----------


    //--------------ĳ���� ��Ī ȭ��----------
    [Header("ĳ���� ��Ī ȭ��")]
    [SerializeField] private CharMatch charMatch;
    //[SerializeField] private Image PlayerMatchImg;  // �÷��̾� �̹���
    //[SerializeField] private Image[] enemyMatchImg;  // �� �̹���
    //--------------ĳ���� ��Ī ȭ��----------


    //--------------���� ȭ��------------    
    [Header("���� ȭ��")]
    [SerializeField] private Button nextRound;  // ���� ���� ��ư

    [SerializeField] private GameObject EffectField;    // ����Ʈ�� �����ϴ� �ʵ�
    [SerializeField] private GameObject frontEffectField;    // ����Ʈ�� �����ϴ� �ʵ�
    public GameObject GetFrontEffectField()
    {
        return EffectField;
    }
    public GameObject GetEffectField()
    {
        return EffectField;
    }

    [SerializeField] private DamageText DamageTextPrefab;   // ������ �ؽ�Ʈ ������
    [SerializeField] private Transform textField;
    private Queue<DamageText> damageTextQueue = new Queue<DamageText>();

    public DamageText GetDamageText()
    {
        if (DamageTextPrefab && textField)
        {
            if (0 >= damageTextQueue.Count)
            {
                damageTextQueue.Enqueue(CreateTextObj());
            }

            return damageTextQueue.Dequeue();
        }

        return null;
    }

    private DamageText CreateTextObj()
    {
        DamageText newObj = Instantiate(DamageTextPrefab, textField);
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    private void Start()
    {
        if (DamageTextPrefab && textField)
        {
            for (int i = 0; 10 > i; i++)
            {
                damageTextQueue.Enqueue(CreateTextObj());
            }
        }
        battleStartButton.interactable = false;
    }

    public List<KeyCode> skipButton; // ��ȭ�� ������ �ѱ� �� �ִ� Ű
    void Update()
    {
        foreach (var element in skipButton) // ��ư �˻�
        {
            if (Input.GetKeyDown(element))
            {
                isButtonClicked = true;
            }
        }
    }
    //--------------���� ȭ��------------    


    //--------------���� ���� ȭ��-------------
    [Header("���� ���� ȭ��")]
    [SerializeField] private Text mainText;
    [SerializeField] private Text subText;
    [SerializeField] private Reward reward;
    [SerializeField] private GameObject end;
    [SerializeField] private GameObject ToMainButton;
    //--------------���� ���� ȭ��-------------

    //-------------�ɼ�--------------
    [Header("�ɼ�")]
    [SerializeField] private Option option;

    public void ToggleOptionWindow()
    {
        // �ɼ�â�� �������� ��� �ٽ� ����
        if (option.OptionObject.activeSelf)
            option.ResetOption();
        else
            option.OptionObject.SetActive(true);
    }
    // �ɼ� â �ݱ�
    public void CloseOption()
    {
        option.ResetOption();
    }
    //-------------�ɼ�--------------


    // ���� ���� ��ư Ȱ��ȭ
    public void ActiveNextRound(bool value)
    {
        nextRound.interactable = value;
    }

    /*------------------ī�� ���� ȭ��---------------*/
    // ī�� ���� ȭ�� �ʱ�ȭ
    private void InitCardSetScene()
    {
        currentSlotIndex = 0;    // ���� ���� ��ġ�� ó��0 ���� ����

        isSlotSet = new bool[slotList.Count];

        cardSize = cardList[0].GetComponent<RectTransform>().sizeDelta;

        Character CurPlayer = DataMgr.Instance.CurrentPlayer;
        Character CurEnemy = DataMgr.Instance.CurrentEnemy;

        Dictionary<string, string> nameTable = DataMgr.Instance.SetNameTable();

        PlayerName.text = nameTable[CurPlayer.ToString()];
        EnemyName.text = nameTable[CurEnemy.ToString()];

        SetCharImg(playerIcon, CurPlayer, "Sprites/Characters/Icon/Icon_total", "Icon");
        SetCharImg(GameMgr.Instance.playerImg, CurPlayer, "Sprites/Characters/Characters_total", "Characters");
        SetCharImg(miniMap.playerMiniMapIcon, CurPlayer, "Sprites/Characters/Characters_total", "Characters");


        SetCharImg(enemyIcon, CurEnemy, "Sprites/Characters/Icon/Icon_total", "Icon");
        SetCharImg(GameMgr.Instance.enemyImg, CurEnemy, "Sprites/Characters/Characters_total", "Characters");
        SetCharImg(miniMap.enemyMiniMapIcon, CurEnemy, "Sprites/Characters/Characters_total", "Characters");
    }

    [SerializeField] private Image background;
    [SerializeField] private Sprite[] backgroundSprites;
    // ���� ȭ�� ��׶��� ����
    private void InitBattleBackground()
    {
        background.sprite = backgroundSprites[Random.Range(0, backgroundSprites.Length)];
    }

    // ī�忡 �����͸� �ְ� ī���� ��ġ�� ����
    private void setCard(int listIndex, Normal data)
    {
        Vector2 pos = cardPos;
        if (cardList.Count > listIndex)
        {
            cardList[listIndex].ResetCardUI();
            if (7 > listIndex) pos.x = cardPos.x + ((int)cardSize.x + 10) * listIndex;
            else
            {
                pos.x = cardPos.x + ((int)cardSize.x + 10) * (listIndex - 7);
                pos.y -= (int)cardSize.y + 10;
            }
            cardList[listIndex].SetData(data, pos);
        }
    }

    public bool IsSlotFull()
    {
        return (slotList.Count <= DataMgr.Instance.GetCardListCount());
    }

    // ī�� ���� ��ȣ�ۿ�
    public void OnSlot(Card card)
    {
        switch (card.IsSelect)
        {
            // ī�带 ���Կ� �ִ�(ī�带 ������) ���
            case true:
                // ������ �� á�� ��� Ȥ�� ī�尡 ��Ȱ��ȭ�� ��� �������� �ʴ´�.
                if (IsSlotFull() || card.isDisable)
                    return;

                // �ش� ī�带 �������� �̵�
                card.SetPos(selectSlotPos);

                // �ش� ī�� ������ ī�� ����Ʈ�� �߰�
                DataMgr.Instance.AddCardList(currentSlotIndex, card);

                // ���� ���� ����
                setSlot(SLOT.add);

                SoundMgr.Instance.OnPlaySFX("7.card click");

                // ���� �ʿ�
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, GameMgr.Instance.Player, -1));

                if(IsSlotFull()) battleStartButton.interactable = true;

                break;

            // ���Կ� �ִ� ī�带 �ٽ� ���� ���
            case false:
                // ī�尡 ��Ȱ��ȭ�� ��� Ȥ�� ī�尡 ���Կ� �ִ� ī�尡 �ƴ� ��� �������� �ʴ´�.
                if (card.isDisable || !DataMgr.Instance.IsOnCardList(card)) return;
                // currentSlotIndex�� ���� ������ �������� ����
                for (int i = 0; i < slotList.Count; i++)
                {
                    if (card.transform.position == slotList[i].transform.position)
                        currentSlotIndex = i;
                }
                card.SetOriginPos();
                setSlot(SLOT.sub);

                // �ش� ī�� ������ ī�� ����Ʈ���� ����
                DataMgr.Instance.RemoveCardList(card);

                SoundMgr.Instance.OnPlaySFX("8.card cancle");

                // ���� �ʿ�
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, GameMgr.Instance.Player, 1));

                battleStartButton.interactable = false;
                break;
        }
        CheckDisable(GameMgr.Instance.Player.mpRemain);
    }
    public Text ChatText; // ���� ä���� ������ �ؽ�Ʈ
    public string writerText = "";
    bool isButtonClicked = false;
    IEnumerator NormalChat(string narration)
    {
        writerText = "";

        //�ؽ�Ʈ Ÿ���� ȿ��
        for (int i = 0; i < narration.Length; i++)
        {
            writerText += narration[i];
            ChatText.text = writerText;
            yield return null;
        }

        //Ű�� �ٽ� ���� �� ���� ������ ���
        while (true)
        {
            if (isButtonClicked)
            {
                isButtonClicked = false;
                break;
            }
            yield return null;
        }
    }
    IEnumerator TextPractice()
    {
        yield return StartCoroutine(NormalChat("�̰��� Ÿ���� ȿ���� ���� ���â�� �����ϴ� ����"));
        yield return StartCoroutine(NormalChat("�ȳ��ϼ���, �ݰ����ϴ�."));
    }

    // ���� �������� ����� ���� ī�带 ��Ȱ��ȭ
    private void CheckDisable(int remainCost)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            // ī�� ������Ʈ�� ��Ȱ��ȭ �����̰ų� ���Կ� ��ϵǾ������� �Ѿ��.
            if (!cardList[i].gameObject.activeSelf || DataMgr.Instance.IsOnCardList(cardList[i]))
            {
                continue;
            }
            // ����� �ʰ��Ǹ� ��Ȱ��ȭ
            cardList[i].CheckOverCost(remainCost);
        }
    }

    // ���� ������ ��ȯ
    public void setCurrentSlot(int value)
    {
        // �������ߴµ� ������ �𸣰ڤ���
        SoundMgr.Instance.OnPlaySFX("6.slot click");
        currentSlotIndex = value;
    }

    // ���� ���õ� ������ ��ȯ
    private int getCurrentSlot()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            // ���Կ� ī�尡 �Ҵ�Ǿ������� �Ѿ
            if (isSlotSet[i])
            {
                continue;
            }
            // ī�尡 �Ҵ�Ǿ����� ���� ���� ������ ������ ���� �������� ����
            slotList[i].isOn = true;
            if (slotList[i])
            {
                //Debug.Log("found currentSlot");
                return i;
            }
        }

        //Debug.Log("Can't found currentSlot");
        return 0;   //���� ������ ã�� �� ���°�� ���� ������ 1�� �������� ����
    }

    // ���� ���Կ� ī�尡 ���õǾ��ִ��� ���θ� ����
    private void setSlot(SLOT value)
    {
        switch (value)
        {
            // ī�� �߰��� ��� true
            case SLOT.add:
                isSlotSet[currentSlotIndex] = true;
                break;
            // ī�带 ������ ��� false
            case SLOT.sub:
                isSlotSet[currentSlotIndex] = false;
                break;
        }
        // currentSlotIndex�� ����
        currentSlotIndex = getCurrentSlot();
    }

    // ī�带 �ʵ忡 ��ġ
    public void ArrangeCard(Normal[] publics, Normal[] units, List<Normal> uniques)
    {
        int arrP = 0;
        int arrC = 0;
        int arrU = 0;
        var publicCount = publics.Length;
        var unitCount = publicCount + units.Length;
        // ī�帮��Ʈ ������ŭ ����
        for (int i = 0; i < cardList.Count; i++)
        {
            // ��ũ���ͺ� ������Ʈ�� ������ ��� ������ ī��� ��Ȱ��ȭ
            if (i >= unitCount + uniques.Count)
            {
                cardList[i].gameObject.SetActive(false);
                continue;
            }
            else cardList[i].gameObject.SetActive(true);

            // ���뽺ų ���� ���
            if (i < publicCount)
            {
                setCard(i, publics[arrP++]);
            }
            // ĳ���� ��ų ���
            else if (i < unitCount) 
            {
                setCard(i, units[arrC++]);
            }
            // ����ũ ��ų ���
            else
            {
                setCard(i, uniques[arrU++]);
            }
        }
    }

    // ���� �ʱ�ȭ
    private void ClearSlot()
    {
        for (int i = 0; isSlotSet.Length > i; i++)
        {
            isSlotSet[i] = false;
        }
        slotList[currentSlotIndex = 0].isOn = true;
        DataMgr.Instance.ClearSelectCardList();

        // ���ԵǴ� ���� �ʱ�ȭ, disable �׸� �ʱ�ȭ
        GameMgr.Instance.Player.SetRemainCost(GameMgr.Instance.Player.mp);
        CheckDisable(GameMgr.Instance.Player.mpRemain);

        // ��ŸƮ ��ư ��Ȱ��ȭ
        battleStartButton.interactable = false;
    }

    // Ư��ī�� ���� ����(ī�� ����)
    private void InitUniqueCards()
    {
        DataMgr.Instance.SetUniqueCards(uniqueCards);
    }

    [SerializeField] private GameObject disable;
    private IEnumerator SelectUniqueCardAction(int index)
    {
        if (uniqueCards.Length > index)
        {
            uniqueCards[index].CardOpen();
            disable.SetActive(true);
            while (uniqueCards[index].IsCardOpened()) yield return null;

            // ����ũ ���� â ��Ȱ��ȭ
            cardSet.OffSelectUnique();
            disable.SetActive(false);
            // ȹ���� ī��� ��ȹ�� ����ũ ����Ʈ���� ����
            DataMgr.Instance.playerUniqueList.Remove(uniqueCards[index].skillData);
            // �ٽ� �����·� �ǵ�����.
            uniqueCards[index].ResetCardUI();
            uniqueCards[index].BackCard();
            ResetUniqueCardUI();

            // �÷��̾� ī�� ����Ʈ�� �߰�
            DataMgr.Instance.playerOwnUniqueList.Add(uniqueCards[index].skillData);
            ArrangeCard(DataMgr.Instance.arrPublicSkill, DataMgr.Instance.arrPlayerSkill, DataMgr.Instance.playerOwnUniqueList);
        }
    }
    // Ư��ī�� ����(��ư) �̺�Ʈ
    public void SelectUniqueCard(int index)
    {
        StartCoroutine(SelectUniqueCardAction(index));
    }
    /*--------------------ī�� ���� ȭ��-------------------*/


    /*-------------------���� ��ɿ� �ڵ�------------------*/


    // ĳ���� �̹��� ����
    public void SetCharImg(Image img, Character character, string address, string key)
    {
        Dictionary<string, Sprite> dictionary = DataMgr.Instance.SetDictionary<Sprite>(address);

        string nameIndex = string.Format("{0}_{1}", key, character.ToString());
        img.sprite = dictionary[nameIndex];
    }

    /*-------------------���� ��ɿ� �ڵ�------------------*/



    // ��ü ��ư �̺�Ʈ ����
    public void ButtonEvent(int index)
    {
        BUTTON button = (BUTTON)index;
        switch(button)
        {
            case BUTTON.Main_Start:
                MoveScene(SCENE.Main, SCENE.CharSelect);
                //main start
                SoundMgr.Instance.OnPlaySFX("main start");
                break;
            case BUTTON.CharSelect_Select:
                DataMgr.Instance.SetEnemy();
                InitMatchScene();
                MoveScene(SCENE.CharSelect, SCENE.CharMatch);
                SoundMgr.Instance.OnPlaySFX("character accept");                
                break;
            case BUTTON.CharMatch_Change:
                if (DataMgr.Instance.IsFirstEnemy())
                    MoveScene(SCENE.CharMatch, SCENE.CharSelect);
                SoundMgr.Instance.OnPlaySFX("character change");
                break;
            case BUTTON.CharMatch_Start:
                CharMatchStart();
                SoundMgr.Instance.OnPlaySFX("match start");                
                // ù ���忡�� ���
                if (DataMgr.Instance.Round == 1)
                    SoundMgr.Instance.OnPlayBGM(SoundMgr.Instance.keyCardSet);
                break;
            case BUTTON.CardSet_Start:
                CardSetStart();
                SoundMgr.Instance.OnPlaySFX("match start");
                SoundMgr.Instance.OnPlayBGM(SoundMgr.Instance.keyBattle);
                break;
            case BUTTON.CardSet_Clear:
                SoundMgr.Instance.OnPlaySFX("9.card clear");
                ClearSlot();
                break;
            case BUTTON.NextRound:
                // ���� ������ ���� 15 ȸ��
                GameMgr.Instance.Player.AddMP(15);
                GameMgr.Instance.Enemy.AddMP(15);

                // ���� ������ ���� ������ ��Ȱ��ȭ
                GameMgr.Instance.Player.InitBuffIcon();
                GameMgr.Instance.Enemy.InitBuffIcon();

                ClearSlot();

                MoveScene(SCENE.Battle, SCENE.CardSet);
                SoundMgr.Instance.OnPlayBGM(SoundMgr.Instance.keyCardSet);
                break;
        }
    }

    /*-------------------ȭ�� ����------------------*/

    // ĳ���� ���� �� ��Ī ȭ������ ��ȯ
    private void InitMatchScene()
    {
        charMatch.SetMatchImg(DataMgr.Instance.GetEnemyIndex());
        SetCharImg(charMatch.playerMatchImg, DataMgr.Instance.CurrentPlayer, "Sprites/Characters/Icon/Icon_total", "Icon");

        List<Character> enemys = DataMgr.Instance.GetEnemyList();
        for (int i = 0; i < enemys.Count; i++)
        {
            SetCharImg(charMatch.enemyMatchImg[i], enemys[i], "Sprites/Characters/Icon/Icon_total", "Icon");
        }
    }
    // ĳ���� ���� â���� ī�� ���� â����
    private void CharMatchStart()
    {
        GameMgr.Instance.Player.InitUnit();
        GameMgr.Instance.Enemy.InitUnit();

        MoveScene(SCENE.CharMatch, SCENE.Play);

        InitCardSetScene();
        InitBattleBackground();
        GameMgr.Instance.InitBattleScene();
        DataMgr.Instance.SetCardData();

        InitUniqueCards();
    }
    // ī�� ���� â���� ���� â����
    private void CardSetStart()
    {
        // ī�� �� ���� ��ġ�� ������ ������ ����
        if (3 != DataMgr.Instance.GetCardListCount()) return;
        //Debug.Log("CardSetStart");
        battleStartButton.interactable = false;
        MoveScene(SCENE.CardSet, SCENE.Battle);

        GameMgr.Instance.OnPlay();
    }
    [SerializeField] GameObject gameSet;

    // ���� ���� UI Ȱ��ȭ
    public void OnGameSetUI()
    {
        if (!gameSet.activeSelf) gameSet.SetActive(true);
    }
    // ���� ���� ��Ȱ��ȭ �� ���� ���â���� �̵�
    public void OffGameSetUI()
    {
        // ���� ���� UI ��Ȱ��ȭ
        if (gameSet.activeSelf) gameSet.SetActive(false);
        GameMgr.Instance.OnResetDie();
        // ��Ʋ ȭ�� ��Ȱ��ȭ, ī�� ����ȭ�� Ȱ��ȭ(�ʱ�ȭ)
        MoveScene(SCENE.Battle, SCENE.CardSet);
        // �÷��� ȭ�� ��Ȱ��ȭ, ���� ���� â Ȱ��ȭ
        MoveScene(SCENE.Play, SCENE.GameOver);
    }
    // ���� â���� ��� â����
    public void SetGameOverUI(RESULTSCENE result)
    {
        switch (result)
        {
            case RESULTSCENE.Win:
                reward.SuccessReward(DataMgr.Instance.GetEnemyIndex());
                mainText.text = "WIN!";
                SetResultSubText("�¸�! ��ũ ������ ȹ���ߴ�!");
                break;
            case RESULTSCENE.Lose:
                reward.FailReward(DataMgr.Instance.GetEnemyIndex());
                mainText.text = "LOSE!";
                SetResultSubText("�й�! ������ ��µ� �����ߴ�..! �ٽ�..!");
                break;
            case RESULTSCENE.Draw:
                reward.FailReward(DataMgr.Instance.GetEnemyIndex());
                mainText.text = "DRAW!";
                SetResultSubText("��.. �Ʊ���! ������ ��µ� �����ߴ�! �ٽ�!!");
                break;
        }
    }
    public void SetResultSubText(string text)
    {
        subText.text = text;
    }
    public void OnEndButton()
    {
        end.SetActive(true);
    }
    public void OffEndButton()
    {
        end.SetActive(false);
    }
    [SerializeField] private GameObject endingMovieObj; // ���� ���� ������Ʈ
    [SerializeField] private GameObject endingScene; // ���� ȭ�� ������Ʈ
    [SerializeField] private Animator endingMovieAnim;  // ���� ���� �ִϸ��̼�
    public void OnEndingMovie()
    {
        StartCoroutine(PlayEndingMovie());
    }
    private IEnumerator PlayEndingMovie()
    {
        endingMovieObj.SetActive(true);
        while (DataMgr.Instance.IsEndAnim(endingMovieAnim))yield return null;
        endingMovieObj.SetActive(false);
        endingScene.SetActive(true);
    }
    public void OffEnding()
    {
        endingMovieObj.SetActive(false);
        endingScene.SetActive(false);
    }
    public void OnToMainButton()
    {
        ToMainButton.SetActive(true);
    }
    public void OffToMainButton()
    {
        ToMainButton.SetActive(false);
    }
    public void ResetUniqueCardUI()
    {
        for(int i =0;i < uniqueCards.Length;i++)
        {
            if(uniqueCards[i]) uniqueCards[i].ResetCardUI();
        }       
    }

    public bool IsMain()
    {
        return (sceneList[(int)SCENE.Main].gameObject.activeSelf);
    }
    // �����. ���� ����, Ư��ī�� ������ ��������.
    public void Restart()
    {
        MoveScene(SCENE.GameOver, SCENE.CharMatch);
        charMatch.SetMatchImg(DataMgr.Instance.GetEnemyIndex());
        GameMgr.Instance.ResetGameData();
        CheckDisable(GameMgr.Instance.Player.mpRemain);
        DataMgr.Instance.InitTurnCount();
        roundImg.InitReoundImg();
        ResetUniqueCardUI();
        battleStartButton.interactable = false;
    }

    // �ʱ� �� Ȱ��ȭ ���� ����
    public void ResetUIData()
    {
        sceneList[(int)SCENE.Main].gameObject.SetActive(true);
        sceneList[(int)SCENE.CharSelect].gameObject.SetActive(false);
        sceneList[(int)SCENE.CharMatch].gameObject.SetActive(false);
        sceneList[(int)SCENE.Play].gameObject.SetActive(false);
        sceneList[(int)SCENE.Battle].gameObject.SetActive(false);
        sceneList[(int)SCENE.CardSet].gameObject.SetActive(true);
        gameSet.SetActive(false);
        ResetUniqueCardUI();
        sceneList[(int)SCENE.GameOver].gameObject.SetActive(false);
        reward.InitReward();
        OffEndButton();
        OffToMainButton();
        battleStartButton.interactable = false;
        endingMovieObj.SetActive(false);
        endingScene.SetActive(false);
    }

    // �� ��ȯ
    private void MoveScene(SCENE curScene, SCENE targetScene)
    {
        sceneList[(int)curScene].gameObject.SetActive(false);
        sceneList[(int)targetScene].gameObject.SetActive(true);
    }
    /*-------------------���� ��ɿ� �ڵ�------------------*/
}
