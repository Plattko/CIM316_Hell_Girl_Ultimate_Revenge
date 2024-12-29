using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireExplosionSpell", menuName = "Spells/FireExplosion")]
public class FireExplosionSpell : Spell
{
    [Header("Explosion Variables")]
    [SerializeField] private GameObject fireExplosionPrefab;
    [SerializeField] private float explosionDuration = 0.25f;
    
    public override void Cast(GameObject parent)
    {
        // Spawn the fire explosion
        GameObject fireExplosion = Instantiate(fireExplosionPrefab, parent.transform.position, Quaternion.identity);
        // Set the explosion's values
        fireExplosion.GetComponent<Explosion>().Initialise(damage, explosionDuration);
    }
}
