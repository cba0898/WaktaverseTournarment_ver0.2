using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField] private Image[] rewards;   // �¸� ���� �迭
    [SerializeField] private Image effect;   // �¸� ���� ȿ��
    [SerializeField] private Sprite[] rewardEffects;   // �¸� ���� ȿ�� ���� �迭

    [SerializeField] private Image FinalReward; // ������ ����
    [SerializeField] private Image Frame; // ���� Ʋ
    [SerializeField] private Sprite FinalEffects; // ������ ȿ��
    private string resultSFXString;   // ���� ȿ���� �̸�
    
    // �¸� ���� ȿ��
    public void SuccessReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(true);
        resultSFXString = "12.Getting disc";
    }

    // �й�, ���º� ���� ȿ��
    public void FailReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(false);
        resultSFXString = "13.getting disc fail";
    }

    public void OnResultSFX()
    {
        SoundMgr.Instance.OnPlaySFX(resultSFXString);
    }


    // ������ ȿ�� ����
    private void OnEffect(int index)
    {
        // ����Ʈ ��Ȱ��ȭ
        effect.gameObject.SetActive(false);
        // ����Ʈ�� �ش� ���ʿ� �°� ��������Ʈ ��ü
        effect.sprite = rewardEffects[index];
        // ����Ʈ Ȱ��ȭ
        effect.gameObject.SetActive(true);
    }

    // ���� �ʱ�ȭ
    public void InitReward()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].gameObject.SetActive(false);
        }
        effect.gameObject.SetActive(false);
        FinalReward.gameObject.SetActive(false);
        Frame.gameObject.SetActive(true);
    }

    // ���� ����
    public void SetFinalReward()
    {
        // ������ ���� ��Ȱ��ȭ 
        for(int i=0;i< rewards.Length;i++)
        {
            rewards[i].gameObject.SetActive(false);
        }
        Frame.gameObject.SetActive(false);
        UIMgr.Instance.SetResultSubText("���� ��ũ�� �ϼ��ߴ�! ���� ���Ĺ� �ڻ��� �߸�ǰ�� �־ �ҿ��� �̷���!");
        effect.gameObject.SetActive(false);
        effect.sprite = FinalEffects;
        effect.gameObject.SetActive(true);

        FinalReward.gameObject.SetActive(true);
        SoundMgr.Instance.OnPlaySFX("14.100_ disc");

        UIMgr.Instance.OffEndButton();
        UIMgr.Instance.OnToMainButton();
    }


}
