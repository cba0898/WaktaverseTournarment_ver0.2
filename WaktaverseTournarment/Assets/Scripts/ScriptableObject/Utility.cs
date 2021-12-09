using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INFLUENCE
{
    HP = 1 << 1,
    MP = 1 << 2,
    STURN = 1 << 3,
    ATK = 1 << 4,
    DEF = 1 << 5
};

[CreateAssetMenu(fileName = "Utility", menuName = "Skill/Utility", order = 1)]
public class Utility : Normal
{
    public INFLUENCE condition; // mp, hp, addDamage, sturn enum
    public int value;
    public int cost;
    public int turns;       //����ȿ�� �� ��, 3�� = 1����

    public Utility() { priority = 0; }
}