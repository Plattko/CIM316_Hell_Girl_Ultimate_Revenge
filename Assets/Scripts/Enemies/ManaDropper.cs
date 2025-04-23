using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDropper : MonoBehaviour
{
    [SerializeField] private GameObject manaPickupPrefab;
    [SerializeField] private int minManaDrop = 0;
    [SerializeField] private int maxManaDrop = 2;
    [SerializeField] private float minDropForceX = 2;
    [SerializeField] private float maxDropForceX = 3;
    [SerializeField] private float dropForceY = 2;

    public void DropMana(Transform manaParentTransform)
    {
        // Don't drop mana if the player has no spell
        if (!GameManager.Instance.PlayerHasSpell) return;
        
        // Roll the amount of mana to drop
        int manaDropAmount = Random.Range(minManaDrop, maxManaDrop + 1);
        // Spawn a number of mana pickups equal to the mana drop amount and give each one a random drop force
        for (int i = 0; i < manaDropAmount; i++)
        {
            // Instantiate the mana pickup
            GameObject manaPickup = Instantiate(manaPickupPrefab, transform.position, Quaternion.identity);
            // Set its parent to the parent of the object (e.g. the room the object is in)
            manaPickup.transform.parent = manaParentTransform;
            // Calculate the horizontal drop force
            Vector2 dropForceX = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(minDropForceX, maxDropForceX);
            // Calculate the total drop force
            Vector3 dropForce = new Vector3(dropForceX.x, dropForceY, dropForceX.y);
            // Apply the drop force to the mana pickup so it's thrown slightly in a random direction
            manaPickup.GetComponent<Rigidbody>().AddForce(dropForce, ForceMode.Impulse);
        }
    }
}
