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


    //--------------화면 리스트-------------
    [SerializeField] private List<GameObject> sceneList;  // 씬 리스트

    //--------------카드 선택 화면-----------
    [SerializeField] private List<Card> cardList;  //카드 리스트

    [SerializeField] private Vector2 cardPos;    // 카드의 위치
    private Vector2 cardSize;   // 카드의 크기

    [SerializeField] private List<Toggle> slotList;   //slot 위치를 받아오기 위한 리스트
    private int currentSlotIndex;    // 현재 지정된 슬롯 위치 인덱스
    private bool[] isSlotSet;   // 슬롯에 카드가 세팅되어 있는지 확인하는 변수

    public Vector3 selectSlotPos { get { return slotList[currentSlotIndex].transform.position; } } // 선택한 슬롯의 위치

    [SerializeField] private Card[] uniqueCards;    // 특수카드 배열

    [SerializeField] public CardSet cardSet;    // 카드 선택 화면 스크립트
    [SerializeField] public MiniMap miniMap;    // 미니맵
    //--------------카드 선택 화면-----------


    //--------------상단 체력 바 UI----------
    [SerializeField] private Image playerIcon;    // 플레이어 이미지
    [SerializeField] private Image enemyIcon;     // 적 이미지
    [SerializeField] private Text PlayerName;     // 플레이어 이름
    [SerializeField] private Text EnemyName;     // 적 이름
    //--------------상단 체력 바 UI----------


    //--------------캐릭터 매칭 화면----------
    [SerializeField] private CharMatch charMatch;
    //[SerializeField] private Image PlayerMatchImg;  // 플레이어 이미지
    //[SerializeField] private Image[] enemyMatchImg;  // 적 이미지
    //--------------캐릭터 매칭 화면----------


    //--------------전투 화면------------    
    [SerializeField] private Button nextRound;  // 다음 라운드 버튼

    [SerializeField] public GameObject BattleObj;
    //--------------전투 화면------------    


    //--------------게임 오버 화면-------------
    [SerializeField] private Text mainText;
    [SerializeField] private Text subText;
    //--------------게임 오버 화면-------------

    //--------------사운드---------------
    // bgm key 문자열 변수들
    private string keyMain = "Main";
    private string keyCardSet = "CardSet";
    private string keyBattle = "Battle";
    private string keyWin = "Win";
    private string keyLose = "Lose";
    private string keyEnding = "Ending";
    //--------------사운드---------------

    // 다음 라운드 버튼 활성화
    public void ActiveNextRound(bool value)
    {
        nextRound.interactable = value;
    }

    /*------------------카드 선택 화면---------------*/
    // 카드 선택 화면 초기화
    private void InitCardSetScene()
    {
        currentSlotIndex = 0;    // 현재 슬롯 위치를 처음0 으로 지정

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

    // 카드에 데이터를 넣고 카드의 위치를 지정
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

    // 카드 슬롯 상호작용
    public void OnSlot(Card card)
    {
        switch (card.IsSelect)
        {
            // 카드를 슬롯에 넣는(카드를 누르는) 경우
            case true:
                // 슬롯이 꽉 찼을 경우 혹은 카드가 비활성화일 경우 실행하지 않는다.
                if (slotList.Count <= DataMgr.Instance.GetCardListCount() || card.isDisable)
                    return;

                card.SetPos(selectSlotPos);
                setSlot(SLOT.add);

                // 해당 카드 정보를 카드 리스트에 추가
                DataMgr.Instance.AddCardList(card);

                // 수정 필요
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, -1));
                CheckDisable(GameMgr.Instance.Player.mpRemain);

                break;

            // 슬롯에 있는 카드를 다시 빼는 경우
            case false:
                // 카드가 비활성화일 경우 혹은 카드가 슬롯에 있는 카드가 아닐 경우 실행하지 않는다.
                if (card.isDisable || !DataMgr.Instance.IsOnCardList(card)) return;
                // currentSlotIndex를 현재 빼려는 슬롯으로 지정
                for (int i = 0; i < slotList.Count; i++)
                {
                    if (card.transform.position == slotList[i].transform.position)
                        currentSlotIndex = i;
                }
                card.SetOriginPos();
                setSlot(SLOT.sub);

                // 해당 카드 정보를 카드 리스트에서 제거
                DataMgr.Instance.RemoveCardList(card);

                // 수정 필요
                GameMgr.Instance.Player.SetRemainCost(DataMgr.Instance.GetRemainMana(card.skillData, 1));
                CheckDisable(GameMgr.Instance.Player.mpRemain);

                break;
        }
    }

    // 현재 마나보다 비용이 높은 카드를 비활성화
    private void CheckDisable(int remainCost)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            // 카드 오브젝트가 비활성화 상태이거나 슬롯에 등록되어있으면 넘어간다.
            if (!cardList[i].gameObject.activeSelf || DataMgr.Instance.IsOnCardList(cardList[i]))
            {
                continue;
            }
            // 비용이 초과되면 비활성화
            cardList[i].CheckOverCost(remainCost);
        }
    }

    // 현재 슬롯을 반환
    public void setCurrentSlot(int value)
    {
        currentSlotIndex = value;
    }

    // 현재 선택된 슬롯을 반환
    private int getCurrentSlot()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            // 슬롯에 카드가 할당되어있으면 넘어감
            if (isSlotSet[i])
            {
                continue;
            }
            // 카드가 할당되어있지 않은 가장 왼쪽의 슬롯을 현재 슬롯으로 지정
            slotList[i].isOn = true;
            if (slotList[i])
            {
                Debug.Log("found currentSlot");
                return i;
            }
        }

        Debug.Log("Can't found currentSlot");
        return 0;   //현재 슬롯을 찾을 수 없는경우 현재 슬롯을 1번 슬롯으로 지정
    }

    // 현재 슬롯에 카드가 세팅되어있는지 여부를 지정
    private void setSlot(SLOT value)
    {
        switch (value)
        {
            // 카드 추가일 경우 true
            case SLOT.add:
                isSlotSet[currentSlotIndex] = true;
                break;
            // 카드를 제거할 경우 false
            case SLOT.sub:
                isSlotSet[currentSlotIndex] = false;
                break;
        }
        // currentSlotIndex를 지정
        currentSlotIndex = getCurrentSlot();
    }

    // 카드를 필드에 배치
    public void ArrangeCard(Normal[] publics, Normal[] units, List<Normal> uniques)
    {
        int arrP = 0;
        int arrC = 0;
        int arrU = 0;
        // 카드리스트 개수만큼 시작
        for (int i = 0; i < cardList.Count; i++)
        {
            // 스크립터블 오브젝트가 부족할 경우 나머지 카드들 비활성화
            if (i >= units.Length + publics.Length + uniques.Count)
            {
                cardList[i].gameObject.SetActive(false);
                continue;
            }
            else cardList[i].gameObject.SetActive(true);

            // 공용스킬 먼저 등록
            if (i < publics.Length)
            {
                setCard(i, publics[arrP++]);
            }
            // 캐릭터 스킬 등록
            else if (i < publics.Length + units.Length) 
            {
                setCard(i, units[arrC++]);
            }
            // 유니크 스킬 등록
            else
            {
                setCard(i, uniques[arrU++]);
            }
        }
    }

    // 슬롯 초기화
    private void ClearSlot()
    {
        for (int i = 0; isSlotSet.Length > i; i++)
        {
            isSlotSet[i] = false;
        }
        slotList[currentSlotIndex = 0].isOn = true;
        DataMgr.Instance.ClearCardList();

        // 남게되는 마나 초기화, disable 항목 초기화
        GameMgr.Instance.Player.SetRemainCost(GameMgr.Instance.Player.mp);
        CheckDisable(GameMgr.Instance.Player.mpRemain);
    }

    // 특수카드 정보 세팅(카드 셔플)
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

        // 유니크 선택 창 비활성화
        cardSet.OffSelectUnique();
        disable.SetActive(false);
        // 획득한 카드는 미획득 유니크 리스트에서 제거
        DataMgr.Instance.playerUniqueList.Remove(uniqueCards[index].skillData);
        // 다시 원상태로 되돌린다.
        uniqueCards[index].ResetCardUI();
        uniqueCards[index].BackCard();

        // 플레이어 카드 리스트에 추가
        DataMgr.Instance.playerOwnUniqueList.Add(uniqueCards[index].skillData);
        ArrangeCard(DataMgr.Instance.arrPublicSkill, DataMgr.Instance.arrPlayerSkill, DataMgr.Instance.playerOwnUniqueList);
    }
    // 특수카드 선택(버튼) 이벤트
    public void SelectUniqueCard(int index)
    {
        StartCoroutine(SelectUniqueCardAction(index));
    }
    /*--------------------카드 선택 화면-------------------*/


    /*-------------------보조 기능용 코드------------------*/


    // 캐릭터 이미지 세팅
    public void SetCharImg(Image img, Character character, string address, string key)
    {
        Dictionary<string, Sprite> dictionary = DataMgr.Instance.SetDictionary<Sprite>(address);

        string nameIndex = string.Format("{0}_{1}", key, character.ToString());
        img.sprite = dictionary[nameIndex];
    }

    /*-------------------보조 기능용 코드------------------*/



    // 전체 버튼 이벤트 관리
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
                // 턴이 끝나면 마나 15 회복
                GameMgr.Instance.Player.AddMP(15);
                GameMgr.Instance.Enemy.AddMP(15);

                ClearSlot();

                MoveScene(SCENE.Battle, SCENE.CardSet);
                SoundMgr.Instance.OnPlayBGM(keyCardSet);
                break;
        }
    }

    /*-------------------화면 관리------------------*/

    // 캐릭터 선택 후 매칭 화면으로 전환
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
    // 캐릭터 선택 창에서 카드 선택 창으로
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
    // 카드 선택 창에서 전투 창으로
    private void CardSetStart()
    {
        // 카드 세 장이 배치가 끝나기 전에는 리턴
        if (3 != DataMgr.Instance.GetCardListCount()) return;
        SoundMgr.Instance.StopBGM();
        Debug.Log("CardSetStart");

        MoveScene(SCENE.CardSet, SCENE.Battle);

        GameMgr.Instance.OnPlay();
    }

    // 전투 창에서 결과 창으로
    public void GameOverUI(RESULTSCENE result)
    {
        switch (result)
        {
            case RESULTSCENE.Win:
                mainText.text = "WIN!";
                subText.text = "승리했습니다!";
                break;
            case RESULTSCENE.Lose:
                mainText.text = "LOSE!";
                subText.text = "패배했습니다!";
                break;
            case RESULTSCENE.Draw:
                mainText.text = "DRAW!";
                subText.text = "무승부!";
                break;
        }
        // 배틀 화면 비활성화, 카드 세팅화면 활성화(초기화)
        MoveScene(SCENE.Battle, SCENE.CardSet);
        // 플레이 화면 비활성화, 게임 오버 창 활성화
        MoveScene(SCENE.Play, SCENE.GameOver);
    }

    // 재시작
    public void Restart()
    {
        MoveScene(SCENE.GameOver, SCENE.CharMatch);
        charMatch.SetMatchImg(DataMgr.Instance.GetEnemyIndex());
        GameMgr.Instance.Player.InitUnit();
        GameMgr.Instance.Enemy.InitUnit();
        CheckDisable(GameMgr.Instance.Player.mpRemain);
        DataMgr.Instance.InitTurnCount();
    }

    // 배틀 씬 초기화
    public void ResetBattleScene()
    {

    }

    // 초기 씬 활성화 상태 구현
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

    // 씬 전환
    private void MoveScene(SCENE curScene, SCENE targetScene)
    {
        sceneList[(int)curScene].gameObject.SetActive(false);
        sceneList[(int)targetScene].gameObject.SetActive(true);
    }
    /*-------------------보조 기능용 코드------------------*/
}
