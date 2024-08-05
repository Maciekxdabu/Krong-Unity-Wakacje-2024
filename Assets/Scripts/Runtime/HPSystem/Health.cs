using System;
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
                onHealthChange.Invoke();
            } 
        }
    }
    public float MaxHealthPoints
    {
        get { return _maxHealthPoints; }
    }
    
    private void Awake()
    {
        HealthPoints = _maxHealthPoints;
        onHealthChange.AddListener(OnDeath);
    }
    protected virtual void OnDeath() { }
    protected virtual void OnRespawn() { }
    public void TakeDamage(float value)
    {
        HealthPoints -= value;
        if(HealthPoints < 0)
        {
            HealthPoints = 0;
        }
        
        Debug.Log(HealthPoints);
    }
    public void TakeHealing(float value)
    {
        HealthPoints += value;
        if (HealthPoints > _maxHealthPoints)
        {
            HealthPoints = _maxHealthPoints;
        }
        
        Debug.Log(HealthPoints);
    }
}
