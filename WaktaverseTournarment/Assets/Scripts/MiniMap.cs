using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] public Vector2 miniMapInterval;
    [SerializeField] public GameObject miniMapPlayer;
    [SerializeField] public GameObject miniMapEnemy;
    [SerializeField] public Image playerMiniMapIcon;
    [SerializeField] public Image enemyMiniMapIcon;

    public void InitMiniMapPos()
    {
        miniMapPlayer.transform.localPosition = DataMgr.Instance.SetUnitPos(miniMapInterval.x, DataMgr.Instance.playerPosRate);
        miniMapEnemy.transform.localPosition = DataMgr.Instance.SetUnitPos(miniMapInterval.x, DataMgr.Instance.enemyPosRate);
        GameMgr.Instance.FaceUnit(miniMapPlayer, miniMapEnemy);
    }

    public void SetMiniMapPos(Unit unit, Vector2 pos)
    {
        if (unit == GameMgr.Instance.Player)
            miniMapPlayer.transform.localPosition = pos;
        else
            miniMapEnemy.transform.localPosition = pos;
        GameMgr.Instance.FaceUnit(miniMapPlayer, miniMapEnemy);
    }

    public Vector2 GetMiniMapPlayerPos()
    {
        return miniMapPlayer.transform.localPosition;
    }

    public Vector2 GetMiniMapEnemyPos()
    {
        return miniMapEnemy.transform.localPosition;
    }
}
