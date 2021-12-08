using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    //  Singleton Instance ����
    private static SkillMgr instance = null;

    // Singleton Instance�� �����ϱ� ���� ������Ƽ
    public static SkillMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance�� ���� ������Ʈ�� �����
        instance = this;

        // Scene �̵� �� ���� ���� �ʵ��� ó��
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private Normal[] arrScriptableObject;   // ��ũ���ͺ� ������Ʈ ������ ���� �迭

    // Start is called before the first frame update
    void Start()
    {
        arrScriptableObject = Resources.LoadAll<Normal>("ScriptableObject");
        //Debug.Log(arrScriptableObject[0].range + " / " + (0 != (arrScriptableObject[0].range & LOCATION.CENTER)));
    }

    public Vector2[] GetLOCATION(LOCATION type, Normal data)
    {
        Vector2[] arrSkillRange = new Vector2[9];
        // ���õ� ��ų�� ���� ��쿡�� ����
        if (data)
        {
            // 3x3�� ���� ��ü�� �˻�
            for (int i = 0; 9 > i; i++)
            {
                // ���� ���� �����ϴ� ����
                var ioc = (LOCATION)(1 << (i + 1));
                // ���� ���͸� �����ϴ� ����. 
                Vector2 v2 = new Vector2(2, 2);

                // �ش� ����� ��ġ�� �Ǵ��� Ȯ��
                if (0 != (type & ioc))
                {
                    //���⿡ �´� ���� ����
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
