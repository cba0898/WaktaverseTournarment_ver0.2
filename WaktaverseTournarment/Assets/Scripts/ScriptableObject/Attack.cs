using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Skill/Attack", order = 1)]
public class Attack : Utility
{
    public int applyCount;
    public bool isSpread;

    public Attack() { priority = 2; }
}
