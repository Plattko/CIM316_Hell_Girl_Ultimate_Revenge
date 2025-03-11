using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// How the weapon and Weapon Manager communicate
public class Weapon : MonoBehaviour
{
    [SerializeField] private int numberOfAttacks;
    [SerializeField] private float attackCounterResetTime;

    public int CurAttackCounter
    {
        get => curAttackCounter;
        private set => curAttackCounter = value >= numberOfAttacks ? 0 : value;
    }
    
    // Events
    public event Action onExit;
    
    // References
    [SerializeField] private WeaponSO weaponData;
    private Animator anim;
    private GameObject animGameObject;
    private WeaponAnimationEventHandler animEventHandler;

    private int curAttackCounter;

    private Timer attackCounterResetTimer;

    // TEMPORARY
    private BoxCollider attackHitbox;

    private void Awake()
    {
        animGameObject = transform.Find("Animations").gameObject;
        anim = animGameObject.GetComponent<Animator>();
        animEventHandler = animGameObject.GetComponent<WeaponAnimationEventHandler>();

        attackCounterResetTimer = new Timer(attackCounterResetTime);

        attackHitbox = transform.Find("TempHitbox").GetComponent<BoxCollider>();
    }

    private void Update()
    {
        attackCounterResetTimer.Tick();
    }

    private void OnEnable()
    {
        animEventHandler.onFinished += Exit;
        animEventHandler.onAttackHitboxUpdated += UpdateAttackHitbox;
        attackCounterResetTimer.onTimerDone += ResetAttackCounter;
    }

    private void OnDisable()
    {
        animEventHandler.onFinished -= Exit;
        animEventHandler.onAttackHitboxUpdated += UpdateAttackHitbox;
        attackCounterResetTimer.onTimerDone -= ResetAttackCounter;
    }

    public void Enter()
    {
        Debug.Log("Entered " + transform.name);

        // Pause the timer so it doesn't end during an attack
        attackCounterResetTimer.StopTimer();
        anim.SetBool("isActive", true);
        anim.SetInteger("attackCounter", CurAttackCounter);
    }

    private void Exit()
    {
        anim.SetBool("isActive", false);
        CurAttackCounter++;
        attackCounterResetTimer.StartTimer();
        onExit?.Invoke();
    }

    private void ResetAttackCounter() => CurAttackCounter = 0;

    private void UpdateAttackHitbox(bool isEnabled)
    {
        attackHitbox.enabled = isEnabled;
    }
}
