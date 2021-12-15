using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    private Rigidbody2D unitRigid2D;
    [SerializeField] public int hp { get; private set; }        // 체력   
    [SerializeField] public int hpMax { get; private set; }     // 체력   
    [SerializeField] public int mp { get; private set; }        // 마나
    [SerializeField] public int mpMax { get; private set; }     // 최대 마나
    [SerializeField] public int mpRemain { get; private set; }          // 남게되는 마나
    [SerializeField] public int addAtk { get; private set; }    // 추가 데미지
    [SerializeField] public int defense { get; private set; }   // 방어력. 받는 데미지 가/감

    [SerializeField] public UnitAnim unitanim;  // 애니메이션
    [SerializeField] private Slider hpSlider;   // hp바
    [SerializeField] private Slider mpSlider;   // mp바
    [SerializeField] private Text hpText;   // Hp바
    [SerializeField] private Text mpText;   // mp바

    public bool isInArea;                   // 캐릭터가 스킬 범위안에 있는지 확인
    public int buffCount { get; private set; }    // 캐릭터가 적용중인 버프 개수

    [SerializeField] public List<Image> buffIcons; //버프 아이콘 리스트
    [SerializeField] public List<Text> buffTurnTexts; //버프 턴 수
    [SerializeField] public List<Text> buffDiscriptions; //버프 설명

    private int damageedValue = 0;
    private int damageedCount = 0;

    public void SetDamage(int value, int count)
    {
        damageedValue = value;
        damageedCount = count;
    }

    public void ApplyDamaged()
    {
        isInArea = false;
        if (0 < damageedValue * damageedCount)
        {
            AddHP(Mathf.Min(-1 * (damageedValue - defense) * damageedCount, 0));
            damageedValue = 0;
            damageedCount = 0;
        }
    }

    public void AddBuffCount()
    {
        buffCount++;
    }

    public void InitBuffIcon()
    {
        buffCount = 0;
        for (int i = 0; i < buffIcons.Count; i++)
        {
            buffIcons[i].gameObject.SetActive(false);
        }
    }
    public void Awake()
    {
        unitRigid2D = GetComponent<Rigidbody2D>();
    }

    public void AddHP(int value)
    {
        hp = Mathf.Clamp(hp + value, 0, 100);
        hpSlider.value = hp;
        hpText.text = string.Format("HP {0}", hp);
    }

    public void AddMP(int value)
    {
        mp = Mathf.Clamp(mp + value, 0, 100);
        mpSlider.value = mp;
        SetRemainCost(mp);
        mpText.text = string.Format("MP {0}", mp);
    }

    public void AddAtk(int value)
    {
        addAtk = value;
    }

    public void AddDefense(int value)
    {
        defense = value;
    }

    public void SetRemainCost(int value)
    {
        mpRemain = value;
    }

    public void InitUnit()
    {
        hp = 100;
        hpMax = 100;
        mp = 100;
        mpMax = 100;
        mpRemain = 100;
        addAtk = 0;
        defense = 0;
        hpSlider.value = hp;
        mpSlider.value = mp;
        hpText.text = string.Format("HP {0}", hp);
        mpText.text = string.Format("MP {0}", mp);
        InitBuffIcon();
    }

    public Vector2 GetUnitPos()
    {
        return transform.localPosition;
    }

    private string AttackTag = "Attack";
    private string BuffTag = "Buff";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == AttackTag)
        {
            unitanim.OnHitEnter();

            //if (0 < damageedValue * damageedCount)
            //damageedValue = 0;
            //damageedCount = 0;
        }
    }

    private IEnumerator OnDamagePop()
    {
        // 방어력이 존재할 경우 방어력 폰트 추가
        if (0 < defense)
        {

        }
        for(int i = 0; damageedCount > i; i++)
        {
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == AttackTag)
            unitanim.OnHitExit();
    }
}
