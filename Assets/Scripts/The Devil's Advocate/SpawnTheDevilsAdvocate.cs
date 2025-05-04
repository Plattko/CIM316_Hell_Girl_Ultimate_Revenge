using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTheDevilsAdvocate : MonoBehaviour
{
    [SerializeField] private GameObject TDA;

    [Header("VFX")]
    [SerializeField] private ParticleSystem poofVFX;

    public void SpawnTDA()
    {
        TDA.SetActive(true);
        poofVFX.Stop();
        poofVFX.Play();
    }

    public void RemoveTDA()
    {
        TDA.SetActive(false);
        poofVFX.Stop();
        poofVFX.Play();
    }

    public void RemoveTDAPostDelay()
    {
        StartCoroutine(RemoveAfterDelay());
    }

    public IEnumerator RemoveAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        RemoveTDA();
    }
}
