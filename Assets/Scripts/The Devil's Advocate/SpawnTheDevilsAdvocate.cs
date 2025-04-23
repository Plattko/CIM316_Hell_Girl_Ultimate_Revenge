using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTheDevilsAdvocate : MonoBehaviour
{
    [SerializeField] private GameObject TDA;

    public void SpawnTDA()
    {
        TDA.SetActive(true);
    }

    public void RemoveTDA()
    {
        TDA.SetActive(false);
    }
}
