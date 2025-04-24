using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFallenAngelSlider : MonoBehaviour
{
    public GameObject uiElement;  // Assign the UI element to toggle
    private GameObject fallenAngel;

    private bool wasActive = false;

    void Start()
    {
        fallenAngel = GameObject.Find("Fallen Angel");

        if (fallenAngel == null)
        {
            Debug.LogWarning("Fallen Angel object not found in the scene.");
        }

        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    void Update()
    {
        // Try to re-find it if it's null (in case it was instantiated later)
        if (fallenAngel == null)
        {
            fallenAngel = GameObject.Find("FallenAngel");

            // If still not found and UI is active, disable it
            if (uiElement != null && wasActive)
            {
                uiElement.SetActive(false);
                wasActive = false;
            }

            return;
        }

        // Toggle UI based on Fallen Angel's active state
        if (fallenAngel.activeInHierarchy)
        {
            if (uiElement != null && !wasActive)
            {
                uiElement.SetActive(true);
                wasActive = true;
            }
        }
        else
        {
            if (uiElement != null && wasActive)
            {
                uiElement.SetActive(false);
                wasActive = false;
            }
        }
    }
}
