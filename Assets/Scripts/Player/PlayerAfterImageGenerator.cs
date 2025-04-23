using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageGenerator : MonoBehaviour
{
    // References
    [Header("References")]
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private PlayerAfterImage afterImagePrefab;

    // Generator variables
    [Header("Generator Variables")]
    [SerializeField] private float spawnFrequency = 0.05f;
    private float nextSpawnTime;

    // Afterimage variables
    [Header("Afterimage Variables")]
    [SerializeField] private Color startColour;
    [SerializeField] private float startAlpha = 0.8f;
    [SerializeField] private float afterImageDuration = 0.2f;

    private void Update()
    {
        // Spawn afterimages at the spawn frequency
        if (Time.time > nextSpawnTime)
        {
            SpawnAfterImage();
            nextSpawnTime = Time.time + spawnFrequency;
        }
    }

    private void SpawnAfterImage()
    {
        // Instantiate the afterimage
        PlayerAfterImage afterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
        // Initialise it
        afterImage.Init(playerSR.flipX, startColour, startAlpha, afterImageDuration);
    }
}
