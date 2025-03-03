using UnityEngine;
using System;

public class PathTrigger : MonoBehaviour
{
    public event Action<Collider> onPathTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPathTriggered?.Invoke(GetComponent<Collider>());
        }
    }
}
