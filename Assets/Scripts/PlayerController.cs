using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Health))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _movementSpeed = 1;
    [SerializeField]
    private float damageMultiplier = 2;

    private PointSystem score;
    private LightShield lightShield;
    private LightSword lightSword;
    private Rigidbody rb;
    private Health health;

    private bool _inCombat = false;
    private EnemyController enemy;
    private EnemyState? currentEnemyState = null;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        lightShield = GetComponent<LightShield>();
        lightSword = GetComponent<LightSword>();

        if (lightShield == null || lightSword == null)
        {
            Debug.LogError("Critical Failure. Please attach Lightsword and LightShield to player controller.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        score = PointSystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_inCombat)
        {
            rb.MovePosition(transform.position + Vector3.forward * Time.deltaTime * _movementSpeed);
        }
    }

    //void OnBlock(InputValue value)
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent<EnemyController>(out EnemyController newEnemy))
        {
           Engage(newEnemy);
        }
    }

    private void Engage(EnemyController newEnemy)
    {
        _inCombat = true;
        enemy = newEnemy;
        enemy.ChangeStateAction += EnemyStateChange;
        enemy.Engage(this);
    }

    private void Disengage()
    {
        enemy.ChangeStateAction -= EnemyStateChange;
        enemy = null;
        _inCombat = false;
        currentEnemyState = null;
    }

    private void EnemyStateChange(EnemyState newState)
    {

        if (currentEnemyState == EnemyState.DAZED && newState != EnemyState.DAZED)
        {
            lightSword.doubleRegenRate = false;
        }

        if (newState == EnemyState.DYING)
        {
            Disengage();
            return;
        }
        if (newState == EnemyState.DAZED)
        {
            lightSword.doubleRegenRate = true;
        }


        currentEnemyState = newState;
    }

    void OnBlock()
    {
        if (lightShield.CanActivate && _inCombat)
        {
            TryBlock();
        }
    }

    void OnAttack()
    {
        if (lightSword.CanActivate && _inCombat)
        {
            TryAttack();
        }

    }

    void TryBlock()
    {
        lightShield.Activate();
        if (currentEnemyState == EnemyState.PARRYWINDOW)
        {
            lightShield.Recharge(lightShield.ActivationCost);
        }

        enemy.GetBlocked();
    }

    void TryAttack()
    {
        lightSword.Activate();
        if (currentEnemyState == EnemyState.CRITTABLE || currentEnemyState == EnemyState.DAZED)
        {
            score.IncreaseCombo();
        }
        int damage = (int)(lightSword.baseDamage * damageMultiplier);

        enemy.GetHit(damage);
        score.AddPoints(damage);
    }

    public void GetHit(int damage)
    {
        health.takeDamage(damage);
    }

}
