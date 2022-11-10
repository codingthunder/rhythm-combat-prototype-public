using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody),typeof(Health))]
public class EnemyController : MonoBehaviour
{

    public event Action<EnemyState> ChangeStateAction;

    [SerializeField]
    private int baseDamage = 1;
    [SerializeField]
    private float damageMultiplier = 1f;
    [SerializeField]
    private bool _multiplyDamage = false;
    [SerializeField]
    private float maxPosture = 3f;
    [SerializeField]
    private float currentPosture = 3f;
    [SerializeField]
    private float postureRecoveryRate = 0.25f;
    [SerializeField]
    private float critMultiplier = 2f;

    [SerializeField]
    private PlayerController player;

    private Rigidbody rb;
    private Health health;

    [SerializeField, Range(0f,1f)]
    private float _idleLikelihood = 1;

    [SerializeField, Range(0f, 1f)]
    private float _blockableLikelihood = 1;

    [SerializeField, Range(0f, 1f)]
    private float _critLikelihood = 1;

    [SerializeField]
    private float _idleLength = 1f;
    [SerializeField]
    private float _blockableWindowLength = 1f;
    [SerializeField]
    private float _parryWindowLength = 0.5f;
    [SerializeField]
    private float _recoilLength = 0.5f;
    [SerializeField]
    private float _critWindowLength = 0.5f;
    [SerializeField]
    private float _dazedWindowLength = 1f;
    [SerializeField]
    private float _dyingLength = 1f;

    private EnemyState currentState;
    private bool isBlocked = false;
    //private float timeInState = 0f;
    //private float stateLength = 0f;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        ChangeStateAction += ChangeState;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = EnemyState.UNENGAGED;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.DEAD)
        {
            //Probably set this to disable later.
            Destroy(gameObject);
        }

        //timeInState += Time.deltaTime;
        //if (timeInState > stateLength)
        //{
        //    ChooseNewState();
        //}
    }

    //Separated as a function just so it could be invoked by the action and not be separately assigned.
    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
    }

    public void Engage(PlayerController player)
    {
        this.player = player;
        ChooseNewState();
    }

    void ChooseNewState()
    {
        float idleDecision = _idleLikelihood * UnityEngine.Random.Range(0f, 1f);
        float blockableDecision = _blockableLikelihood * UnityEngine.Random.Range(0f, 1f);
        float critDecision = _critLikelihood * UnityEngine.Random.Range(0f, 1f);

        if (idleDecision > blockableDecision && idleDecision > critDecision)
        {
            StartCoroutine(RunIdle());
            return;
        }
        if (blockableDecision > critDecision)
        {
            StartCoroutine(RunBlockable());
            return;
        }

        StartCoroutine(RunCrittable());
    }

    private IEnumerator RunIdle()
    {
        ChangeStateAction.Invoke(EnemyState.IDLE);
        yield return new WaitForSeconds(_idleLength);
        ChooseNewState();
    }

    private IEnumerator RunBlockable()
    {
        isBlocked = false;
        ChangeStateAction.Invoke(EnemyState.BLOCKABLE);
        yield return new WaitForSeconds(_blockableWindowLength);
        ChangeStateAction.Invoke(EnemyState.PARRYWINDOW);
        yield return new WaitForSeconds(_parryWindowLength);
        if (!isBlocked)
        {
            int damage = _multiplyDamage ? (int)(baseDamage * damageMultiplier) : baseDamage;
            player.GetHit(damage);
        }
        ChooseNewState();
    }

    private IEnumerator RunCrittable()
    {
        ChangeStateAction.Invoke(EnemyState.CRITTABLE);
        yield return new WaitForSeconds(_critWindowLength);
        ChooseNewState();
    }

    private void GetRecoiled()
    {
        StopAllCoroutines();
        StartCoroutine(RunRecoiled());
    }

    private IEnumerator RunRecoiled()
    {
        ChangeStateAction.Invoke(EnemyState.RECOILED);
        yield return new WaitForSeconds(_recoilLength);
        if (currentPosture <= 0)
        {
            GetDazed();
        }
        else
        {
            ChooseNewState();
        }
    }

    private void GetDazed()
    {
        StopAllCoroutines();
        StartCoroutine(RunDazed());
    }

    private IEnumerator RunDazed()
    {
        ChangeStateAction.Invoke(EnemyState.DAZED);
        yield return new WaitForSeconds(_dazedWindowLength);
        currentPosture = maxPosture;
        ChooseNewState();
    }

    private void StartDying()
    {
        StopAllCoroutines();
        StartCoroutine(RunDying());
        //I handle the logic for this state in update.
        //ChangeStateAction.Invoke(EnemyState.DYING);
    }

    private IEnumerator RunDying()
    {
        ChangeStateAction.Invoke(EnemyState.DYING);
        yield return new WaitForSeconds(_dyingLength);
        ChangeStateAction.Invoke(EnemyState.DEAD);
    }

    public void GetHit(int baseDamage)
    {
        if (currentState == EnemyState.CRITTABLE)
        {
            health.takeDamage((int)(baseDamage * critMultiplier));
            currentPosture -= 1;
        }
        else if (currentState == EnemyState.DAZED)
        {
            health.takeDamage((int)(baseDamage * critMultiplier));
        }
        else
        {
            health.takeDamage(baseDamage);
        }

        if (health.isDead)
        {
            StartDying();
            return;
        }
        if (currentPosture <= 0)
        {
            GetDazed();
        }

    }

    public void GetBlocked()
    {
        if (currentState == EnemyState.PARRYWINDOW)
        {
            currentPosture -= 1;
            GetRecoiled();
            return;
        }
        if (currentState == EnemyState.BLOCKABLE)
        {
            isBlocked = true;
        }

    }




}

public enum EnemyState
{
    UNENGAGED,
    IDLE,
    BLOCKABLE,
    PARRYWINDOW,
    RECOILED,
    CRITTABLE,
    DAZED,
    DYING,
    DEAD
}
