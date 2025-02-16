using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomPaths : MonoBehaviour
{
    [SerializeField] private GameObject[] paths = new GameObject[4];
    [SerializeField] private GameObject[] closedPaths = new GameObject[4];
    [SerializeField] private Collider[] pathTriggers = new Collider[4];
    public Transform[] entryPoints = new Transform[4];

    public event Action<int> onPathTriggered;

    private void OnEnable()
    {
        foreach (Collider pathTrigger in pathTriggers)
        {
            pathTrigger.GetComponent<PathTrigger>().onPathTriggered += CallPathTriggered;
        }
    }
    private void OnDisable()
    {
        foreach (Collider pathTrigger in pathTriggers)
        {
            pathTrigger.GetComponent<PathTrigger>().onPathTriggered -= CallPathTriggered;
        }
    }

    public void SetPaths(Room roomData)
    {
        if (roomData.doorUp)
        {
            closedPaths[0].SetActive(false);
        }
        else
        {
            paths[0].SetActive(false);
        }

        if (roomData.doorDown)
        {
            closedPaths[1].SetActive(false);
        }
        else
        {
            paths[1].SetActive(false);
        }

        if (roomData.doorLeft)
        {
            closedPaths[2].SetActive(false);
        }
        else
        {
            paths[2].SetActive(false);
        }

        if (roomData.doorRight)
        {
            closedPaths[3].SetActive(false);
        }
        else
        {
            paths[3].SetActive(false);
        }
    }

    private void CallPathTriggered(Collider col)
    {
        if (col == pathTriggers[0])
        {
            onPathTriggered?.Invoke(0);
        }
        if (col == pathTriggers[1])
        {
            onPathTriggered?.Invoke(1);
        }
        if (col == pathTriggers[2])
        {
            onPathTriggered?.Invoke(2);
        }
        if (col == pathTriggers[3])
        {
            onPathTriggered?.Invoke(3);
        }
    }
}
