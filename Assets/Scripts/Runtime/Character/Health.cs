using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private protected float _maxHealthPoints;

    private float _healthPoints;
    protected Boolean isAlive => _healthPoints > 0;
    public UnityEvent onHealthChange = new UnityEvent();
    public float HealthPoints
    {
        get { return _healthPoints; }
        set {
            if (value != _healthPoints)
            {
                _healthPoints = value;
                
            } 
        }
    }
    public float MaxHealthPoints
    {
        get { return _maxHealthPoints; }
    }
    private void Awake()
    {
        _healthPoints = _maxHealthPoints;
    }

    public void TakeDamage(float value)
    {
        _healthPoints -= value;
        if(_healthPoints < 0)
        {
            _healthPoints = 0;
        }
        onHealthChange.Invoke();
        Debug.Log(_healthPoints);
    }
    public void TakeHealing(float value)
    {
        _healthPoints -= value;
        if (_healthPoints < 0)
        {
            _healthPoints = 0;
        }
        onHealthChange.Invoke();
        Debug.Log(_healthPoints);
    }
}
