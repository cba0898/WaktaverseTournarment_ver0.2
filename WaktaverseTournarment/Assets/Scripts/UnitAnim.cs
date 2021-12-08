using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    [SerializeField] public Animator anim;   // 애니메이션

    public bool isMove = false;

    public void OnMoveEnter()
    {
        isMove = true;
    }
    public void OnMoveExit()
    {
        isMove = false;
    }
}
