using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscensionLeech : MonoBehaviour, IInteractable
{
    public void Interact(Interactor interactor)
    {
        GameManager.Instance.GenerateNewMap();
    }
}
