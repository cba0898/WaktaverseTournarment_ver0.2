using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Round : MonoBehaviour
{
    [SerializeField] private Image[] leftright;    // 왼쪽, 오른쪽 이미지
    [SerializeField] private Sprite[] Numbers;    // 숫자 넘버 이미지

    private void Start()
    {
        // 숫자를 01로 초기화
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
