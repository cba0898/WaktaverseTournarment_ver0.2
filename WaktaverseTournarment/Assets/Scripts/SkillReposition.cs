using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReposition : MonoBehaviour
{
    [SerializeField] Transform skillTransform;      // 스킬의 위치 정보
    [SerializeField] Transform[] targetTransform;      // 스킬의 위치 정보

    private void OnEnable()
    {
        var targets = targetTransform;
        if (skillTransform.localScale.x.Equals(-1))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
