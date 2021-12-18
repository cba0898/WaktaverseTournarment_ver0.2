using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Round : MonoBehaviour
{
    [SerializeField] private Image[] leftright;    // ����, ������ �̹���
    [SerializeField] private Sprite[] Numbers;    // ���� �ѹ� �̹���

    private void Start()
    {
        // ���ڸ� 01�� �ʱ�ȭ
        leftright[0].sprite = Numbers[0];
        leftright[1].sprite = Numbers[1];
    }

    public void NextRound()
    {
        DataMgr.Instance.SetNextRound();
        var round = DataMgr.Instance.Round;

        int remainder = round % 10;
        int front = (int)(round * 0.1f);

        leftright[0].sprite = Numbers[front];
        leftright[1].sprite = Numbers[remainder];
    }

    public void InitReoundImg()
    {
        leftright[0].sprite = Numbers[0];
        leftright[1].sprite = Numbers[1];
    }
}
