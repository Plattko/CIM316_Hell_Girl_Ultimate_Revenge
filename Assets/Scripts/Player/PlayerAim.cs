using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mouseLayer;

    [SerializeField] private Transform weaponAimPivot;
    [SerializeField] private Transform spellAimPivot;

    private void Update()
    {
        if (GameManager.Instance.IsGamePaused) return;

        // Update the player's aim
        Aim();
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
        // Face the spell aim pivot towards the aim direction
        spellAimPivot.forward = direction;
        // Aim the weapon pivot
        AimWeapon(spellAimPivot.eulerAngles.y);
    }

    private void AimWeapon(float rot)
    {
        float pivotRot = rot;

        if (rot <= 180)
        {
            pivotRot = Mathf.Clamp(pivotRot, 60f, 120f);
        }
        else
        {
            pivotRot = Mathf.Clamp(pivotRot, 240f, 300f);
        }
        
        weaponAimPivot.rotation = Quaternion.Euler(0f, pivotRot, 0f);
    }

    public (bool success, Vector3 position) GetMouseWorldPosition()
    {
        // Create a ray from the mouse's position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        // If the raycast hit something, return the position
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, mouseLayer))
        {
            return (success: true, position: hitInfo.point);
        }
        // Otherwise, return that it was unsuccessful
        return (success: false, position: Vector3.zero);
    }
}
