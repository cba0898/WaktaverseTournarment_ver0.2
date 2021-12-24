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
    [SerializeField] private Image Frame; // 보상 틀
    [SerializeField] private Sprite FinalEffects; // 마지막 효과
    private string resultSFXString;   // 보상 효과음 이름
    private string resultBGMString;   // 보상 효과음 이름
    
    // 승리 보상 효과
    public void SuccessReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(true);
        resultSFXString = "12.Getting disc";
        resultBGMString = SoundMgr.Instance.keyWin;
    }

    // 패배, 무승부 보상 효과
    public void FailReward(int enemyIndex)
    {
        OnEffect(enemyIndex);
        rewards[enemyIndex].gameObject.SetActive(false);
        resultSFXString = "13.getting disc fail";
        resultBGMString = SoundMgr.Instance.keyLose;
    }

    public void OnResultSFX()
    {
        SoundMgr.Instance.OnPlaySFX(resultSFXString);
        SoundMgr.Instance.CrossFadeAudio(resultBGMString);
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
        Frame.gameObject.SetActive(true);
    }

    // 엔딩 연출
    public void SetFinalReward()
    {
        // 이전의 보상 비활성화 
        for(int i=0;i< rewards.Length;i++)
        {
            rewards[i].gameObject.SetActive(false);
        }
        Frame.gameObject.SetActive(false);
        UIMgr.Instance.SetResultSubText("드디어 디스크를 완성했다! 이제 도파민 박사의 발명품에 넣어서 소원을 이루자!");
        effect.gameObject.SetActive(false);
        effect.sprite = FinalEffects;
        effect.gameObject.SetActive(true);

        SoundMgr.Instance.OnPlaySFX("14.100_ disc");
        FinalReward.gameObject.SetActive(true);

        UIMgr.Instance.OffEndButton();
        UIMgr.Instance.OnToMainButton();
    }


}
