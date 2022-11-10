using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : MonoBehaviour
{
    [SerializeField]
    protected float _maxEnergy = 1f;
    [SerializeField]
    protected float _currentEnergy = 1f;
    [SerializeField]
    protected float _activationCost = 1f;
    public float ActivationCost { get { return _activationCost; } }
    [SerializeField]
    protected float _regenRate = 1f;
    public bool doubleRegenRate = false;

    public bool CanActivate { get { return _currentEnergy - _activationCost >= 0f; } }

    public virtual void Activate()
    {
        _currentEnergy -= _activationCost;
    }

    protected abstract void OnUpdate();

    // Not sure I need Start in the parent class.
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (_currentEnergy < _maxEnergy)
        {
            _currentEnergy += doubleRegenRate ? _regenRate * 2 * Time.deltaTime : _regenRate * Time.deltaTime;
        }
        if (_currentEnergy > _maxEnergy)
        {
            _currentEnergy = _maxEnergy;
        }

        OnUpdate();
    }

    public virtual void Recharge(float amount)
    {
        _currentEnergy += amount;
    }
}
