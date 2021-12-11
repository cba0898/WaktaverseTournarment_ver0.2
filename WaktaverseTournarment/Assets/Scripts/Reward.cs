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
    [SerializeField] private Sprite FinalEffects; // ������ ȿ��

    
    // �¸� ���� ȿ��
    public void SuccessReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(true);
    }

    // �й�, ���º� ���� ȿ��
    public void FailReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(false);
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
    }

    // ���� ����
    public void SetFinalReward()
    {
        UIMgr.Instance.SetResultSubText("�ϼ��� ��ũ���� [2021 ���������� ��ǰ��]�̶�� ���� �־���. �ι����� �÷��θ� �ٽô� �Ҿ������ �ʰ���?");
        effect.gameObject.SetActive(false);
        FinalReward.gameObject.SetActive(true);
        effect.sprite = FinalEffects;
        effect.gameObject.SetActive(true);

        UIMgr.Instance.OffEndButton();
        UIMgr.Instance.OnToMainButton();
    }
}
