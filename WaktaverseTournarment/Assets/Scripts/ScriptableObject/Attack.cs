using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Attack", menuName = "Skill/Attack", order = 1)]
public class Attack : Utility
{
    public int applyCount;
    public bool isSelf;
    public bool isIdle; // 캐릭터가 Idle을 유지할지 말지 확인
    public bool isArts; // 캐릭터가 직접 움직이는 효과인지 확인

    public Attack() { priority = 2; }
}
