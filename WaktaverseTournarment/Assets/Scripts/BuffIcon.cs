using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject discriptionWindow;  // 버프를 가져다 대면 나오는 설명 창
    public void OnPointerEnter(PointerEventData eventData)
    {
        discriptionWindow.SetActive(true);
        Debug.Log("The cursor entered the selectable UI element.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        discriptionWindow.SetActive(false);
        Debug.Log("The cursor exited the selectable UI element.");
    }  

    public void InitDiscriptionWindow()
    {
        discriptionWindow.SetActive(false);
    }
}
