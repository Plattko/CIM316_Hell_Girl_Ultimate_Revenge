using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFireProjectileSpell", menuName = "Spells/Fire Projectile")]
public class FireProjectileSpell : Spell
{
    [Header("Projectile Variables")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float projectileLifetime;

    public override void CastWithDirection(GameObject parent, Vector3 dir)
    {
        // Spawn the projectile
        GameObject fireExplosion = Instantiate(projectilePrefab, parent.transform.position, Quaternion.identity);
        // Set the projectile's values
        fireExplosion.GetComponent<Projectile>().Initialise(dir, moveSpeed, damage, projectileLifetime);
    }
}
