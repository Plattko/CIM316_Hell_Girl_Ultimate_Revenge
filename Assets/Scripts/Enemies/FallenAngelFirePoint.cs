using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenAngelFirePoint : MonoBehaviour
{
    public Transform centerObject;   // The object to stay around
    public float radius = 3f;        // How far from the center this object stays

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player is tagged 'Player'.");
        }
    }

    void Update()
    {
        if (centerObject == null || playerTransform == null)
            return;

        // Direction from center to player (flattened on the Y axis)
        Vector3 directionToPlayer = playerTransform.position - centerObject.position;
        directionToPlayer.y = 0f;
        directionToPlayer.Normalize();

        // Position the fire point at the edge of the radius in the direction of the player
        transform.position = centerObject.position + directionToPlayer * radius;

        // Look at the player
        Vector3 lookAtTarget = playerTransform.position;
        lookAtTarget.y = transform.position.y; // Optional: keep it level
        transform.LookAt(lookAtTarget);
    }
}
