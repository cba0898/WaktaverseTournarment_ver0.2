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
    [SerializeField] private List<GameObject> sceneList;  // �� ����Ʈ

    //--------------ī�� ���� ȭ��-----------
    [SerializeField] private List<Card> cardList;  //ī�� ����Ʈ

    [SerializeField] private Vector2 cardPos;    // ī���� ��ġ
    private Vector2 cardSize;   // ī���� ũ��

    [SerializeField] private List<Toggle> slotList;   //slot ��ġ�� �޾ƿ��� ���� ����Ʈ
    private int currentSlotIndex;    // ���� ������ ���� ��ġ �ε���
    private bool[] isSlotSet;   // ���Կ� ī�尡 ���õǾ� �ִ��� Ȯ���ϴ� ����

    public Vector3 selectSlotPos { get { return slotList[currentSlotIndex].transform.position; } } // ������ ������ ��ġ

    [SerializeField] private Card[] uniqueCards;    // Ư��ī�� �迭

    [SerializeField] public CardSet cardSet;    // ī�� ���� ȭ�� ��ũ��Ʈ
    [SerializeField] public MiniMap miniMap;    // �̴ϸ�
    //--------------ī�� ���� ȭ��-----------


    //--------------��� ü�� �� UI----------
    [SerializeField] private Image playerIcon;    // �÷��̾� �̹���
    [SerializeField] private Image enemyIcon;     // �� �̹���
    [SerializeField] private Text PlayerName;     // �÷��̾� �̸�
    [SerializeField] private Text EnemyName;     // �� �̸�
    //--------------��� ü�� �� UI----------


    //--------------ĳ���� ��Ī ȭ��----------
    [SerializeField] private CharMatch charMatch;
    //[SerializeField] private Image PlayerMatchImg;  // �÷��̾� �̹���
    //[SerializeField] private Image[] enemyMatchImg;  // �� �̹���
    //--------------ĳ���� ��Ī ȭ��----------


    //--------------���� ȭ��------------    
    [SerializeField] private Button nextRound;  // ���� ���� ��ư

    [SerializeField] public GameObject BattleObj;
    //--------------���� ȭ��------------    


    //--------------���� ���� ȭ��-------------
    [SerializeField] private Text mainText;
    [SerializeField] private Text subText;
    //--------------���� ���� ȭ��-------------

    //--------------����---------------
    // bgm key ���ڿ� ������
    private string keyMain = "Main";
    private string keyCardSet = "CardSet";
    private string keyBattle = "Battle";
    private string keyWin = "Win";
    private string keyLose = "Lose";
    private string keyEnding = "Ending";
    //--------------����---------------

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

    // ī�忡 �����͸� �ְ� ī���� ��ġ�� ����
    private void setCard(int listIndex, Normal data)
    {
        Vector2 pos = cardPos;
        cardList[listIndex].ResetCardUI();
        if (7 > listIndex) pos.x = cardPos.x + ((int)cardSize.x + 10) * listIndex;
        else
        {
            pos.x = cardPos.x + ((int)cardSize.x + 10) * (listIndex - 7);
            pos.y -= (int)cardSize.y + 10;
        }
        cardList[listIndex].SetData(data, pos);
    }

    // ī�� ���� ��ȣ�ۿ�
    public void OnSlot(Card card)
    {
        switch (card.IsSelect)
        {
            // ī�带 ���Կ� �ִ�(ī�带 ������) ���
            case true:
                // ������ �� á�� ��� Ȥ�� ī�尡 ��Ȱ��ȭ�� ��� �������� �ʴ´�.
                if (slotList.Count <= DataMgr.Instance.GetCardListCount() || card.isDisable)
                    return;

                card.SetPos(selectSlotPos);
                setSlot(SLOT.add);

                // �ش� ī�� ������ ī�� ����Ʈ�� �߰�
                DataMgr.Instance.AddCardList(card);

                // ���� �ʿ�
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, -1));
                CheckDisable(GameMgr.Instance.Player.mpRemain);

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

                // ���� �ʿ�
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, 1));
                CheckDisable(GameMgr.Instance.Player.mpRemain);

                break;
        }
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
                Debug.Log("found currentSlot");
                return i;
            }
        }

        Debug.Log("Can't found currentSlot");
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
        // ī�帮��Ʈ ������ŭ ����
        for (int i = 0; i < cardList.Count; i++)
        {
            // ��ũ���ͺ� ������Ʈ�� ������ ��� ������ ī��� ��Ȱ��ȭ
            if (i >= units.Length + publics.Length + uniques.Count)
            {
                cardList[i].gameObject.SetActive(false);
                continue;
            }
            else cardList[i].gameObject.SetActive(true);

            // ���뽺ų ���� ���
            if (i < publics.Length)
            {
                setCard(i, publics[arrP++]);
            }
            // ĳ���� ��ų ���
            else if (i < publics.Length + units.Length) 
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
        DataMgr.Instance.ClearCardList();

        // ���ԵǴ� ���� �ʱ�ȭ, disable �׸� �ʱ�ȭ
        GameMgr.Instance.Player.SetRemainCost(GameMgr.Instance.Player.mp);
        CheckDisable(GameMgr.Instance.Player.mpRemain);
    }

    // Ư��ī�� ���� ����(ī�� ����)
    private void InitUniqueCards()
    {
        DataMgr.Instance.SetUniqueCards(uniqueCards);
    }

    [SerializeField] private GameObject disable;
    private IEnumerator SelectUniqueCardAction(int index)
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

        // �÷��̾� ī�� ����Ʈ�� �߰�
        DataMgr.Instance.playerOwnUniqueList.Add(uniqueCards[index].skillData);
        ArrangeCard(DataMgr.Instance.arrPublicSkill, DataMgr.Instance.arrPlayerSkill, DataMgr.Instance.playerOwnUniqueList);
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
                //SoundMgr.Instance.SetBGMVolume(0.7f);
                MoveScene(SCENE.Main, SCENE.CharSelect);
                break;
            case BUTTON.CharSelect_Select:
                InitMatchScene();
                MoveScene(SCENE.CharSelect, SCENE.CharMatch);
                break;
            case BUTTON.CharMatch_Change:
                if (DataMgr.Instance.IsFirstEnemy())
                    MoveScene(SCENE.CharMatch, SCENE.CharSelect);
                break;
            case BUTTON.CharMatch_Start:
                CharMatchStart();
                SoundMgr.Instance.OnPlayBGM(keyCardSet);
                break;
            case BUTTON.CardSet_Start:
                CardSetStart();
                break;
            case BUTTON.CardSet_Clear:
                ClearSlot();
                break;
            case BUTTON.NextRound:
                // ���� ������ ���� 15 ȸ��
                GameMgr.Instance.Player.AddMP(15);
                GameMgr.Instance.Enemy.AddMP(15);

                ClearSlot();

                MoveScene(SCENE.Battle, SCENE.CardSet);
                SoundMgr.Instance.OnPlayBGM(keyCardSet);
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
        GameMgr.Instance.InitBattleScene();
        DataMgr.Instance.SetCardData();

        InitUniqueCards();
    }
    // ī�� ���� â���� ���� â����
    private void CardSetStart()
    {
        // ī�� �� ���� ��ġ�� ������ ������ ����
        if (3 != DataMgr.Instance.GetCardListCount()) return;
        SoundMgr.Instance.StopBGM();
        Debug.Log("CardSetStart");

        MoveScene(SCENE.CardSet, SCENE.Battle);

        GameMgr.Instance.OnPlay();
    }

    // ���� â���� ��� â����
    public void GameOverUI(RESULTSCENE result)
    {
        switch (result)
        {
            case RESULTSCENE.Win:
                mainText.text = "WIN!";
                subText.text = "�¸��߽��ϴ�!";
                break;
            case RESULTSCENE.Lose:
                mainText.text = "LOSE!";
                subText.text = "�й��߽��ϴ�!";
                break;
            case RESULTSCENE.Draw:
                mainText.text = "DRAW!";
                subText.text = "���º�!";
                break;
        }
        // ��Ʋ ȭ�� ��Ȱ��ȭ, ī�� ����ȭ�� Ȱ��ȭ(�ʱ�ȭ)
        MoveScene(SCENE.Battle, SCENE.CardSet);
        // �÷��� ȭ�� ��Ȱ��ȭ, ���� ���� â Ȱ��ȭ
        MoveScene(SCENE.Play, SCENE.GameOver);
    }

    // �����
    public void Restart()
    {
        MoveScene(SCENE.GameOver, SCENE.CharMatch);
        charMatch.SetMatchImg(DataMgr.Instance.GetEnemyIndex());
        GameMgr.Instance.Player.InitUnit();
        GameMgr.Instance.Enemy.InitUnit();
        CheckDisable(GameMgr.Instance.Player.mpRemain);
        DataMgr.Instance.InitTurnCount();
    }

    // ��Ʋ �� �ʱ�ȭ
    public void ResetBattleScene()
    {

    }

    // �ʱ� �� Ȱ��ȭ ���� ����
    public void InitScene()
    {
        sceneList[(int)SCENE.Main].gameObject.SetActive(true);
        sceneList[(int)SCENE.CharSelect].gameObject.SetActive(false);
        sceneList[(int)SCENE.CharMatch].gameObject.SetActive(false);
        sceneList[(int)SCENE.Play].gameObject.SetActive(false);
        sceneList[(int)SCENE.Battle].gameObject.SetActive(false);
        sceneList[(int)SCENE.CardSet].gameObject.SetActive(true);
        SoundMgr.Instance.LoadAudio();
        SoundMgr.Instance.OnPlayBGM(keyMain);
    }

    // �� ��ȯ
    private void MoveScene(SCENE curScene, SCENE targetScene)
    {
        sceneList[(int)curScene].gameObject.SetActive(false);
        sceneList[(int)targetScene].gameObject.SetActive(true);
    }
    /*-------------------���� ��ɿ� �ڵ�------------------*/
}
