using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireExplosionSpell", menuName = "Spells/Fire Explosion")]
public class FireExplosionSpell : Spell
{
    [Header("Explosion Variables")]
    [SerializeField] private GameObject fireExplosionPrefab;
    [SerializeField] private float explosionDuration = 0.25f;
    
    public override void Cast(GameObject parent)
    {
        // Set the spawn position
        Vector3 spawnPos = new Vector3(parent.transform.position.x, 0f, parent.transform.position.z);
        // Spawn the fire explosion
        GameObject fireExplosion = Instantiate(fireExplosionPrefab, spawnPos, Quaternion.identity);
        // Set the explosion's values
        fireExplosion.GetComponent<Explosion>().Initialise(damage, explosionDuration);
    }
}
