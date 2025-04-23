using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscensionLeech : MonoBehaviour, IInteractable
{
    [SerializeField] private bool startsRetracted;
    [SerializeField] private float retractedHeight = 20;
    [SerializeField] private float descendTime = 2f;
    private Vector3 targetPos;

    private void Start()
    {
        if (startsRetracted)
        {
            targetPos = transform.position;
            transform.position = new Vector3(transform.position.x, transform.position.y + retractedHeight, transform.position.z);
        }
    }

    public void Interact(Interactor interactor)
    {
        GameManager.Instance.GenerateNewMap();
    }

    public IEnumerator Descend()
    {
        Vector3 initPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < descendTime)
        {
            transform.position = Vector3.Lerp(initPos, targetPos, elapsedTime / descendTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
