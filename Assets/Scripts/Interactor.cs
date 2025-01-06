using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private float interactRadius = 2f;

    public void Interact()
    {
        // Get a list of the nearby interactable objects
        List<Transform> nearbyInteractables = GetNearbyInteractables();
        // Do nothing if there are no nearby interactable objects
        if (nearbyInteractables.Count <= 0) { return; }
        // Get the nearest interactable object
        Transform nearestInteractable = GetNearestInteractable(nearbyInteractables);
        // Interact with the nearest interactable object
        nearestInteractable.GetComponent<IInteractable>().Interact(this);
    }

    private List<Transform> GetNearbyInteractables()
    {
        // Create a list for nearby interactables
        List<Transform> nearbyInteractables = new List<Transform>();
        // Get all of the colliders within the interact radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
        // Loop through each collider and add the interactable ones to the nearby interactables list
        foreach (Collider collider in hitColliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                nearbyInteractables.Add(collider.gameObject.transform);
            }
        }
        Debug.Log("Nearby interactable object count: " + nearbyInteractables.Count);
        return nearbyInteractables;
    }

    private Transform GetNearestInteractable(List<Transform> nearbyInteractables)
    {
        Transform nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        // Loop through all of the nearby interactable objects and find the nearest one
        for (int i = 0; i < nearbyInteractables.Count; i++)
        {
            float distance = (nearbyInteractables[i].position - transform.position).magnitude;

            if (distance < nearestDistance)
            {
                nearestInteractable = nearbyInteractables[i];
                nearestDistance = distance;
            }
        }
        Debug.Log("Nearest interactable object: " + nearestInteractable.name);
        return nearestInteractable;
    }

    //private List<Transform> nearbyInteractables = new List<Transform>();
    //[HideInInspector] public Transform nearestInteractable;

    //private void FixedUpdate()
    //{
    //    GetNearestInteractable();
    //    Debug.Log("Nearby interactable object count: " + nearbyInteractables.Count);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    // Ignore the object if it isn't interactable
    //    if (!other.CompareTag("Interactable")) { return; }

    //    // Add the object to the nearby interactable objects
    //    nearbyInteractables.Add(other.transform);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // Ignore the object if it isn't interactable
    //    if (!other.CompareTag("Interactable")) { return; }

    //    // Remove the object from the nearby interactable objects
    //    nearbyInteractables.Remove(other.transform);
    //}

    //private void GetNearestInteractable()
    //{
    //    // Do nothing if there are no nearby interactable objects
    //    if (nearbyInteractables.Count <= 0) { return; }

    //    float nearestDistance = float.MaxValue;

    //    // Loop through all of the nearby interactable objects and find the nearest one
    //    for (int i = 0; i < nearbyInteractables.Count; i++)
    //    {
    //        float distance = (nearbyInteractables[i].position - transform.position).magnitude;

    //        if (distance < nearestDistance)
    //        {
    //            nearestInteractable = nearbyInteractables[i];
    //            nearestDistance = distance;
    //        }
    //    }
    //}
}
