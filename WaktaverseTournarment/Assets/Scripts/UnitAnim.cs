using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    [SerializeField] public Animator anim;   // �ִϸ��̼�

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
