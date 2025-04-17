using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// How the Weapon and Weapon Manager communicate
public class Weapon : MonoBehaviour
{
    // Events
    public event Action onEnter;
    public event Action onExit;

    // References
    public WeaponDataSO Data { get; private set; }
    public PlayerController PlayerController { get; private set; }

    private GameObject animGameObject;
    public Animator Anim { get; private set; }
    public WeaponAnimationEventHandler AnimEventHandler { get; private set; }

    // Attack counter variables
    public int CurAttackCounter
    {
        get => curAttackCounter;
        private set => curAttackCounter = value >= Data.NumberOfAttacks ? 0 : value;
    }
    private int curAttackCounter;
    private Timer attackCounterResetTimer;
    [SerializeField] private float attackCounterResetTime;

    private void Awake()
    {
        animGameObject = transform.Find("Animations").gameObject;
        Anim = animGameObject.GetComponent<Animator>();
        AnimEventHandler = animGameObject.GetComponent<WeaponAnimationEventHandler>();

        attackCounterResetTimer = new Timer(attackCounterResetTime);
    }

    private void Update()
    {
        attackCounterResetTimer.Tick();
    }

    private void OnEnable()
    {
        AnimEventHandler.OnFinished += Exit;
        attackCounterResetTimer.onTimerDone += ResetAttackCounter;
    }

    private void OnDisable()
    {
        AnimEventHandler.OnFinished -= Exit;
        attackCounterResetTimer.onTimerDone -= ResetAttackCounter;
    }

    public void SetPlayerController(PlayerController playerController)
    {
        PlayerController = playerController;
    }

    public void SetData(WeaponDataSO data)
    {
        Data = data;
    }

    public void Enter()
    {
        Debug.Log("Entered " + transform.name);

        // Pause the timer so it doesn't end during an attack
        attackCounterResetTimer.StopTimer();
        Anim.SetBool("isActive", true);
        // Play the swing SFX
        SFXManager.Instance.PlayAudioClip(Data.swingSFX, transform, 1.1f, 1.25f, true);
        Anim.SetInteger("attackCounter", CurAttackCounter);
        onEnter?.Invoke();
    }

    private void Exit()
    {
        Anim.SetBool("isActive", false);
        CurAttackCounter++;
        attackCounterResetTimer.StartTimer();
        onExit?.Invoke();
    }

    private void ResetAttackCounter() => CurAttackCounter = 0;
}
