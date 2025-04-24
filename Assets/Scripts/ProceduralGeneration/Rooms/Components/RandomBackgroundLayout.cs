using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackgroundLayout : MonoBehaviour
{
    [SerializeField] private GameObject[] layouts;

    private void Start()
    {
        layouts[Random.Range(0, layouts.Length)].SetActive(true);
    }
}
