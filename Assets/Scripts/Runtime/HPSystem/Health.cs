using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private protected float _maxHealthPoints;

    private float _healthPoints;
    protected Boolean isAlive => _healthPoints > 0;
    public UnityEvent onHealthChange = new UnityEvent();

    public Boolean GetIsAlive() { return isAlive; }

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
    
    protected virtual void Awake()
    {
        HealthPoints = _maxHealthPoints;
        onHealthChange.AddListener(OnDeath);
    }
    protected virtual void OnDeath() { }
    protected virtual void Respawning() { }
    public void TakeDamage(float value)
    {
        HealthPoints -= value;
        HealthPoints = Mathf.Clamp(HealthPoints, 0, _maxHealthPoints);
        
        Debug.Log(HealthPoints);
    }

    public void TakeHealing(float value)
    {
        TakeDamage(-value);
    }
}
