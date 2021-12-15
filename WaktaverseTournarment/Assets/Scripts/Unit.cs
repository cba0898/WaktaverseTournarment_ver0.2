using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    private Rigidbody2D unitRigid2D;
    [SerializeField] public int hp { get; private set; }        // ü��   
    [SerializeField] public int hpMax { get; private set; }     // ü��   
    [SerializeField] public int mp { get; private set; }        // ����
    [SerializeField] public int mpMax { get; private set; }     // �ִ� ����
    [SerializeField] public int mpRemain { get; private set; }          // ���ԵǴ� ����
    [SerializeField] public int addAtk { get; private set; }    // �߰� ������
    [SerializeField] public int defense { get; private set; }   // ����. �޴� ������ ��/��

    [SerializeField] public UnitAnim unitanim;  // �ִϸ��̼�
    [SerializeField] private Slider hpSlider;   // hp��
    [SerializeField] private Slider mpSlider;   // mp��
    [SerializeField] private Text hpText;   // Hp��
    [SerializeField] private Text mpText;   // mp��

    public bool isInArea;                   // ĳ���Ͱ� ��ų �����ȿ� �ִ��� Ȯ��
    public int buffCount { get; private set; }    // ĳ���Ͱ� �������� ���� ����

    [SerializeField] public List<Image> buffIcons; //���� ������ ����Ʈ
    [SerializeField] public List<Text> buffTurnTexts; //���� �� ��
    [SerializeField] public List<Text> buffDiscriptions; //���� ����

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
        // ������ ������ ��� ���� ��Ʈ �߰�
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
