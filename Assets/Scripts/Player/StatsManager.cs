using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public TimeSpan RunTime { get; private set; }
    public int FloorReached { get; private set; }  = 0;
    public int RoomsCleared { get; private set; } = 0;
    public int EnemiesKilled { get; private set; } = 0;
    public int ItemsPickedUp { get; private set; } = 0;

    private float elapsedTime;
    private bool isRunTimerEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (isRunTimerEnabled)
        {
            elapsedTime += Time.deltaTime;
            RunTime = TimeSpan.FromSeconds(elapsedTime);
        }
    }

    public void ToggleRunTimer(bool enabled)
    {
        isRunTimerEnabled = enabled;
    }

    public void IncreaseFloorReached()
    {
        FloorReached++;
        Debug.Log("Floor reached: " + FloorReached);
    }

    public void IncreaseRoomsCleared()
    {
        RoomsCleared++;
        Debug.Log("Rooms cleared: " + RoomsCleared);
    }

    public void IncreaseEnemiesKilled()
    {
        EnemiesKilled++;
        Debug.Log("Enemies killed: " + EnemiesKilled);
    }

    public void IncreaseItemsPickedUp()
    {
        ItemsPickedUp++;
        Debug.Log("Items picked up: " + ItemsPickedUp);
    }
}
