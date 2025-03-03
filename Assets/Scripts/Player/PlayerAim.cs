using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundMask;

    private void Update()
    {
        // Update the player's aim
        Aim();
    }

    private (bool success, Vector3 position) GetMouseWorldPosition()
    {
        // Create a ray from the mouse's position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // If the raycast hit something, return the position
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        // Otherwise, return that it was unsuccessful
        return (success: false, position: Vector3.zero);
    }

    private void Aim()
    {
        // Get the mouse world position
        var (success, position) = GetMouseWorldPosition();
        // Do nothing if getting the mouse's world position was unsuccessful
        if (!success) { return; }
        // Calculate the aim direction
        Vector3 direction = position - transform.position;
        // Ignore the y axis
        direction.y = 0;
        // Face the aim direction
        transform.forward = direction;
    }
}
