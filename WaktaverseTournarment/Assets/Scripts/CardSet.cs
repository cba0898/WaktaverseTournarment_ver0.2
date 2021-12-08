using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSet : MonoBehaviour
{
    [SerializeField] private GameObject SelectUnique;

    public void OnSelectUnique()
    {
        SelectUnique.SetActive(true);
    }

    public void OffSelectUnique()
    {
        SelectUnique.SetActive(false);
    }
}
