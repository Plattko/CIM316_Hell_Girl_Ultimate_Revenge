using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireExplosion", menuName = "Spells/FireExplosion")]
public class FireExplosionSpell : Spell
{
    [Header("Fire Explosion")]
    [SerializeField] private GameObject fireExplosionPrefab;
    [SerializeField] private float explosionDuration = 0.25f;
    
    public override void CastSpell()
    {
        base.CastSpell();

        // Spawn the fire explosion
        GameObject fireExplosion = Instantiate(fireExplosionPrefab, spellManager.transform.position, Quaternion.identity);
        // Set the explosion's damage
        fireExplosion.GetComponent<FireExplosion>().damage = damage;
        // Destroy it after the duration ends
        Destroy(fireExplosion, explosionDuration);
    }
}
