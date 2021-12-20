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

    public void OnHitEnter()
    {
        if (anim) anim.SetBool("isHit", true);
    }
    public void OnHitExit()
    {
        if (anim) anim.SetBool("isHit", false);
    }
    public void OnGuardEnter()
    {
        if (anim) anim.SetBool("isGuard", true);
    }
    public void OnGuardExit()
    {
        if (anim) anim.SetBool("isGuard", false);
    }
    
    public void OnBackEnter()
    {
        if (anim) anim.SetBool("isBack", true);
    }
    public void OnBackExit()
    {
        if (anim) anim.SetBool("isBack", false);
    }
    public void OnActionEnter()
    {
        if (anim) anim.SetBool("isAction", true);
    }
    public void OnActionExit()
    {
        if (anim) anim.SetBool("isAction", false);
    }
    public void OnNonActionEnter()
    {
        if (anim) anim.SetBool("isNonAction", true);
    }
    public void OnNonActionExit()
    {
        if (anim) anim.SetBool("isNonAction", false);
    }
    public void OnDieEnter()
    {
        if (anim) anim.SetBool("isDie", true);
    }
    public void OnDieExit()
    {
        if (anim) anim.SetBool("isDie", false);
    }
    // 죽는 시늉하는 시간을 측정
    public void OnDyingEnter()
    {
        if (anim) anim.SetBool("isDying", true);
    }
    public void OnDyingExit()
    {
        if (anim) anim.SetBool("isDying", false);
    }
    public bool IsDiying()
    {
        return (anim.GetBool("isDying"));
    }
}
