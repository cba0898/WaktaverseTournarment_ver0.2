using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharMatch : MonoBehaviour
{
    [SerializeField] public Image vsImg;            // VS �̹���
    [SerializeField] public Image playerMatchImg;   // �÷��̾� �̹���
    [SerializeField] public Image player3DImg;   // �÷��̾� 3D �̹���
    [SerializeField] public Image enemy3DImg;   // �� 3D �̹���
    [SerializeField] public Image[] enemyMatchImg;  // �� �̹���
    [SerializeField] private int matchImgInterval;  // ��ġ �̹��� ����
    [SerializeField] private Vector3 initPos;       // ��ġ �̹��� ����
    [SerializeField] private Button charChangeButton;       // ��ġ �̹��� ����
    // ��ġ �̹��� ����
    public void SetMatchImg(int matchIndex)
    {
        UIMgr.Instance.SetCharImg(player3DImg, DataMgr.Instance.CurrentPlayer, "Sprites/Characters/3D/3D_total", "3D");
        UIMgr.Instance.SetCharImg(enemy3DImg, DataMgr.Instance.CurrentEnemy, "Sprites/Characters/3D/3D_total", "3D");
        if (0 != matchIndex)
            charChangeButton.gameObject.SetActive(false);
        else
            charChangeButton.gameObject.SetActive(true);
        float posY = enemyMatchImg[0].rectTransform.sizeDelta.y;
        playerMatchImg.transform.localPosition = initPos + new Vector3(matchIndex * matchImgInterval, posY, 0);
        for (int i = 0; i < enemyMatchImg.Length; i++)
        {
            if (i != matchIndex)
            {
                enemyMatchImg[i].color = new Color(0.39f, 0.39f, 0.39f);
            }else enemyMatchImg[i].color = new Color(1, 1, 1);
            enemyMatchImg[i].transform.localPosition = initPos + new Vector3(i * matchImgInterval, 0, 0);
        }
        vsImg.transform.localPosition = initPos + new Vector3(matchIndex * matchImgInterval, posY * 0.5f, 0);
    }
}
