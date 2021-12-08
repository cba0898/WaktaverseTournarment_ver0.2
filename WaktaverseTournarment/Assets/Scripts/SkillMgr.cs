using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    //  Singleton Instance 선언
    private static SkillMgr instance = null;

    // Singleton Instance에 접근하기 위한 프로퍼티
    public static SkillMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance를 유일 오브젝트로 만든다
        instance = this;

        // Scene 이동 시 삭제 되지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private Normal[] arrScriptableObject;   // 스크립터블 오브젝트 데이터 저장 배열

    // Start is called before the first frame update
    void Start()
    {
        arrScriptableObject = Resources.LoadAll<Normal>("ScriptableObject");
        //Debug.Log(arrScriptableObject[0].range + " / " + (0 != (arrScriptableObject[0].range & LOCATION.CENTER)));
    }

    public Vector2[] GetLOCATION(LOCATION type, Normal data)
    {
        Vector2[] arrSkillRange = new Vector2[9];
        // 선택된 스킬이 있을 경우에만 실행
        if (data)
        {
            // 3x3의 범위 전체를 검사
            for (int i = 0; 9 > i; i++)
            {
                // 방향 값을 저장하는 변수
                var ioc = (LOCATION)(1 << (i + 1));
                // 방향 벡터를 저장하는 변수. 
                Vector2 v2 = new Vector2(2, 2);

                // 해당 방향과 매치가 되는지 확인
                if (0 != (type & ioc))
                {
                    //방향에 맞는 범위 지정
                    switch (ioc)
                    {
                        case LOCATION.LEFT_TOP:
                            v2.x = -1;
                            v2.y = 1;
                            break;
                        case LOCATION.CENTER_TOP:
                            v2.x = 0;
                            v2.y = 1;
                            break;
                        case LOCATION.RIGHT_TOP:
                            v2.x = 1;
                            v2.y = 1;
                            break;
                        case LOCATION.LEFT:
                            v2.x = -1;
                            v2.y = 0;
                            break;
                        case LOCATION.CENTER:
                            v2.x = 0;
                            v2.y = 0;
                            break;
                        case LOCATION.RIGHT:
                            v2.x = 1;
                            v2.y = 0;
                            break;
                        case LOCATION.LEFT_BOTTOM:
                            v2.x = -1;
                            v2.y = -1;
                            break;
                        case LOCATION.CENTER_BOTTOM:
                            v2.x = 0;
                            v2.y = -1;
                            break;
                        case LOCATION.RIGHT_BOTTOM:
                            v2.x = 1;
                            v2.y = -1;
                            break;
                        default:
                            Debug.LogError("ioc switch error");
                            break;
                    }
                }
                arrSkillRange[i] = v2;
            }
        }
        return arrSkillRange;
    }

}
