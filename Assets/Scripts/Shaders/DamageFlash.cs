using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    [ColorUsage(true, true)]
    [SerializeField] private Color flashColour = Color.red;
    [SerializeField] private float flashTime = 0.25f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        materials = new Material[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    public void DoDamageFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        foreach (Material material in materials)
        {
            material.SetColor("_FlashColour", flashColour);
        }

        float curFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            curFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashTime);
            foreach (Material material in materials)
            {
                material.SetFloat("_FlashAmount", curFlashAmount);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
