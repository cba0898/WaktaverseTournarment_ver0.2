using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Attack", menuName = "Skill/Attack", order = 1)]
public class Attack : Utility
{
    public int applyCount;
    public bool isSelf;
    public bool isIdle; // ĳ���Ͱ� Idle�� �������� ���� Ȯ��
    public bool isArts; // ĳ���Ͱ� ���� �����̴� ȿ������ Ȯ��

    public Attack() { priority = 2; }
}
