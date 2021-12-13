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
    public int priority;    // 스킬 실행 우선순위
    public string skillName;
    public string discription;
    public TARGET target;
    public LOCATION range;    // 적용 범위
    public LOCATION movePos;  // 이동 위치
    public GameObject effect;
    public Action thisAction;   // 해당 행동의 유형
    public bool isUsed;   // 해당 카드가 이미 배치되었는지 확인하는 변수
    public int MoveCount;    // 행동 횟수
    public AudioClip voiceSFX;    // 대사 효과음

    public Normal() { priority = 1; }
}
