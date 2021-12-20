using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillReposition : MonoBehaviour
{
    [SerializeField] Transform skillTransform;      // ��ų�� ��ġ ����
    [SerializeField] Transform[] targetTransform;      // ��ų�� ��ġ ����

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
