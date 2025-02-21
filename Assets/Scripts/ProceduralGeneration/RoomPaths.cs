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
        // Subscribe to each of the path trigger's path triggered event
        foreach (Collider pathTrigger in pathTriggers)
        {
            pathTrigger.GetComponent<PathTrigger>().onPathTriggered += CallPathTriggered;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from each of the path trigger's path triggered event
        foreach (Collider pathTrigger in pathTriggers)
        {
            pathTrigger.GetComponent<PathTrigger>().onPathTriggered -= CallPathTriggered;
        }
    }

    public void SetPaths(Room roomData)
    {
        // Toggle the open and closed paths of the room depending on which doors the room has
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
        // Evoke the path triggered event with an int representing the direction representing the path the player triggered
        if (col == pathTriggers[0]) // Player went UP
        {
            onPathTriggered?.Invoke(0);
        }
        else if (col == pathTriggers[1]) // Player went DOWN
        {
            onPathTriggered?.Invoke(1);
        }
        else if (col == pathTriggers[2]) // Player went LEFT
        {
            onPathTriggered?.Invoke(2);
        }
        else if (col == pathTriggers[3]) // Player went RIGHT
        {
            onPathTriggered?.Invoke(3);
        }
    }
}
