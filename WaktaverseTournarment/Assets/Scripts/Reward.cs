using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField] private Image[] rewards;   // 승리 보상 배열
    [SerializeField] private Image effect;   // 승리 보상 효과
    [SerializeField] private Sprite[] rewardEffects;   // 승리 보상 효과 종류 배열

    [SerializeField] private Image FinalReward; // 마지막 보상
    [SerializeField] private Sprite FinalEffects; // 마지막 효과

    
    // 승리 보상 효과
    public void SuccessReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(true);
    }

    // 패배, 무승부 보상 효과
    public void FailReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(false);
    }

    // 빛나는 효과 실행
    private void OnEffect(int index)
    {
        // 이펙트 비활성화
        effect.gameObject.SetActive(false);
        // 이펙트를 해당 차례에 맞게 스프라이트 교체
        effect.sprite = rewardEffects[index];
        // 이펙트 활성화
        effect.gameObject.SetActive(true);
    }

    // 보상 초기화
    public void InitReward()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i].gameObject.SetActive(false);
        }
        effect.gameObject.SetActive(false);
        FinalReward.gameObject.SetActive(false);
    }

    // 엔딩 연출
    public void SetFinalReward()
    {
        UIMgr.Instance.SetResultSubText("완성된 디스크에는 [2021 연말공모전 출품작]이라고 쓰여 있었다. 왁물원에 올려두면 다시는 잃어버리지 않겠지?");
        effect.gameObject.SetActive(false);
        FinalReward.gameObject.SetActive(true);
        effect.sprite = FinalEffects;
        effect.gameObject.SetActive(true);

        UIMgr.Instance.OffEndButton();
        UIMgr.Instance.OnToMainButton();
    }
}
