using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class FallenAngel : MonoBehaviour, IDamageable
{
    // Events
    public event Action onDied;
    public event Action<GameObject> onMinionSummoned;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float initialFireDelay = 1f;
    public float projectileSpeed = 10f;
    public float fireRate = 1.5f;
    private int attackCount = 0;

    [Header("Beam Attack Settings")]
    public GameObject beamPrefab;
    public float beamDelay = 3f;
    public int beamsPerAttack = 5; // Number of beams per beam attack

    [Header("Minion Summon Settings")]
    [SerializeField] private GameObject enemySpawnIndicatorPrefab;
    public GameObject[] minions;
    public Transform[] summonPoints;
    //private bool summonedMinions = false;
    private int timesSummonedMinions = 0;

    private Transform player;
    private bool usingBeamAttack = false;
    private Coroutine attackRoutine;

    [SerializeField] private FallenAngelWingAnimation wingAnimationScript; // DRAG THIS IN FROM INSPECTOR

    [Header("Eye Ring Beam Animation")]
    [SerializeField] private Animator eyeRingAnimator;
    [SerializeField] private string eyeRingTrigger = "EyeAttack"; // Make sure this matches the Trigger name

    private DamageFlash damageFlash;

    [Header("Audio")]
    [SerializeField] private AudioClip[] damageSFX;
    [SerializeField] private AudioClip summonSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip unfurlingWingsSFX;
    [SerializeField] private AudioClip beamChargeSFX;
    [SerializeField] private AudioClip beamFireSFX;

    private void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
        
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        UIManager.Instance.SetBossHealthBar(maxHealth, currentHealth);

        // Start projectile attack loop
        attackRoutine = StartCoroutine(FireProjectile());
    }

    private void Update()
    {
        if (player == null) return;

        // Health -based behavior changes
        if (currentHealth <= maxHealth * 0.75f && timesSummonedMinions == 0)
        {
            StartCoroutine(SummonMinions());

            if (wingAnimationScript != null && !wingAnimationScript.enabled)
            {
                wingAnimationScript.enabled = true;
                Debug.Log("FallenAngelWingAnimation script has been ENABLED.");
            }
        }

        if (currentHealth <= maxHealth * 0.5f && !usingBeamAttack)
        {
            usingBeamAttack = true; // Enable beam attacks at 50% health

        }

        if (currentHealth <= maxHealth * 0.25f && timesSummonedMinions == 1)
        {
            StartCoroutine(SummonMinions());
        }
    }

    //-------------------------------------------------------------
    // PROJECTILE ATTACK
    //-------------------------------------------------------------
    private IEnumerator FireProjectile()
    {
        // Wait for the initial fire delay
        yield return new WaitForSeconds(initialFireDelay);
        
        while (!isDead)
        {
            if (player != null)
            {
                // Fire projectile
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                ImpProjectile projScript = projectile.GetComponent<ImpProjectile>();
                if (projScript != null)
                {
                    projScript.Initialise(player.position - firePoint.position);
                }

                attackCount++;

                // If below 50% health, trigger beam attack every 3 ranged attacks
                if (usingBeamAttack && attackCount % 3 == 0)
                {
                    yield return StartCoroutine(BeamAttack());
                }
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    //-------------------------------------------------------------
    // SUMMONING MINIONS
    //-------------------------------------------------------------
    private IEnumerator SummonMinions()
    {
        timesSummonedMinions++;

        // Play the summon SFX x4 to increase volume past 1
        SFXManager.Instance.PlayAudioClip(summonSFX, transform, 1f);
        SFXManager.Instance.PlayAudioClip(summonSFX, transform, 1f);
        SFXManager.Instance.PlayAudioClip(summonSFX, transform, 1f);
        SFXManager.Instance.PlayAudioClip(summonSFX, transform, 1f);

        yield return new WaitForSeconds(summonSFX.length * 0.33f);
        
        // Display an enemy spawn indicator and then spawn an enemy at each of the spawn layout's spawn points
        foreach (Transform summonPoint in summonPoints)
        {
            // Instantiate an enemy spawn indicator at the position of the spawn point
            GameObject enemySpawnIndicator = Instantiate(enemySpawnIndicatorPrefab, summonPoint.position, Quaternion.identity);
            // Spawn an enemy at the spawn point after the enemy spawn indicator's animation ends
            StartCoroutine(SpawnMinion(enemySpawnIndicator.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length, summonPoint.position));
        }
    }

    private IEnumerator SpawnMinion(float delay, Vector3 summonPoint)
    {
        // Wait for the spawn delay
        yield return new WaitForSeconds(delay);
        // Instantiate a random minion at the spawn point
        GameObject minion = Instantiate(minions[UnityEngine.Random.Range(0, minions.Length)], summonPoint + Vector3.up, Quaternion.identity);
        // Set the minion's parent to the room it is in
        minion.transform.parent = transform;
        // Set the minion's material to the divine enemy material to change their appearance and make them immune to spikes/beams
        minion.GetComponent<SpriteRenderer>().material = GameManager.Instance.DivineEnemyMaterial;
        // Signal that a minion was summoned
        onMinionSummoned?.Invoke(minion);
    }

    //-------------------------------------------------------------
    // BEAM ATTACK
    //-------------------------------------------------------------
    private IEnumerator BeamAttack()
    {
        Debug.Log("BeamAttack initiated.");

        if (eyeRingAnimator != null && !string.IsNullOrEmpty(eyeRingTrigger))
        {
            Debug.Log("Playing EyeAttack animation.");
            eyeRingAnimator.ResetTrigger(eyeRingTrigger);  // optional, helps if stuck in loop
            eyeRingAnimator.SetTrigger(eyeRingTrigger);
        }
        else
        {
            Debug.LogWarning("EyeRing animator or trigger not set.");
        }

        //yield return new WaitForSeconds(0.5f);

        // Play beam charge sound effect
        SFXManager.Instance.PlayAudioClip(beamChargeSFX, transform, 0.5f);

        for (int i = 0; i < beamsPerAttack; i++)
        {
            SpawnBeamAtRandomGround();
        }

        // Wait for the 50 frames of the beam animation before the beam fires
        yield return new WaitForSeconds(50f / 60f);

        // Play the beam fire sound effect
        SFXManager.Instance.PlayAudioClip(beamFireSFX, transform, 0.33f);

        //yield return new WaitForSeconds(beamDelay);
    }

    private void SpawnBeamAtRandomGround()
    {
        // Get all active ground objects
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground")
            .Where(obj => obj.activeInHierarchy)
            .ToArray();

        if (groundObjects.Length == 0)
        {
            Debug.LogWarning("No active ground objects found!");
            return;
        }

        // Pick a random ground object
        GameObject randomGround = groundObjects[UnityEngine.Random.Range(0, groundObjects.Length)];
        Collider groundCollider = randomGround.GetComponent<Collider>();

        if (groundCollider == null)
        {
            Debug.LogWarning("Selected ground object has no Collider!");
            return;
        }

        // Get a random position within the bounds
        Vector3 spawnPosition = GetRandomPointInBounds(groundCollider.bounds);

        // Spawn the beam
        Instantiate(beamPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y; // Ensure the beam spawns on top

        return new Vector3(x, y, z);
    }

    //-------------------------------------------------------------
    // HEALTH
    //-------------------------------------------------------------
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        // Play the damage flash
        damageFlash.DoDamageFlash();

        UIManager.Instance.UpdateBossHealthBar(currentHealth);

        if (currentHealth > 0)
        {
            // Play the damage SFX
            SFXManager.Instance.PlayRandomAudioClip(damageSFX, transform, 1.2f, 1f, true);
        }
        else
        {
            isDead = true;
            onDied?.Invoke();

            // Play the death SFX
            SFXManager.Instance.PlayAudioClip(deathSFX, transform, 2f);

            // Increase the enemies killed stat
            StatsManager.Instance.IncreaseEnemiesKilled();

            Destroy(gameObject);
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
