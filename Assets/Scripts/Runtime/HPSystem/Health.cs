using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private protected float _maxHealthPoints;

    private float _hp;
    protected Boolean isAlive => _hp > 0;
    public UnityEvent onHealthChange = new UnityEvent();

    public Boolean GetIsAlive() { return isAlive; }

    public float HealthPoints
    {
        get { return _hp; }
        set {
            if (value != _hp)
            {
                _hp = value;
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
