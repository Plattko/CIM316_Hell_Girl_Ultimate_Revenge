using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private PlayerUI playerUI;

    private void OnEnable()
    {
        roomManager.onPlayerSpawned += InitialisePlayerUI;
    }

    private void OnDisable()
    {
        roomManager.onPlayerSpawned -= InitialisePlayerUI;
    }

    private void InitialisePlayerUI(GameObject player)
    {
        playerUI.Initialise(player);
    }
}
