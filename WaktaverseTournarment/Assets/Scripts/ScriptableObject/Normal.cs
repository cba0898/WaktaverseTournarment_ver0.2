using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum TARGET
{
    //NONE = 1 << 0,
    PLAYER = 1 << 1,
    ENEMY = 1 << 2,
    //ALL = PLAYER + ENEMY
};

[Flags]
public enum LOCATION
{
    LEFT_TOP = 1 << 1,
    CENTER_TOP = 1 << 2,
    RIGHT_TOP = 1 << 3,
    LEFT = 1 << 4,
    CENTER = 1 << 5,
    RIGHT = 1 << 6,
    LEFT_BOTTOM = 1 << 7,
    CENTER_BOTTOM = 1 << 8,
    RIGHT_BOTTOM = 1 << 9
};

[CreateAssetMenu(fileName = "Normal", menuName = "Skill/Normal", order = 1)]
public class Normal : ScriptableObject
{
    public int priority;    // ��ų ���� �켱����
    public string skillName;
    public string discription;
    public TARGET target;
    public LOCATION range;    // ���� ����
    public LOCATION movePos;  // �̵� ��ġ
    public GameObject effect;
    public Action thisAction;   // �ش� �ൿ�� ����
    public bool isUsed;   // �ش� ī�尡 �̹� ��ġ�Ǿ����� Ȯ���ϴ� ����
    public int MoveCount;    // �ൿ Ƚ��
    public AudioClip voiceSFX;    // ��� ȿ����

    public Normal() { priority = 1; }
}
