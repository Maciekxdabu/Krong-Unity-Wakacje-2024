using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;

    float _time = 0;
    private List<IDamageable> _objectsWithHealth = new List<IDamageable>();

    void Update()
    {
        if(_objectsWithHealth.Count > 0)
        {
            DamageOverTime(dmgPerTick, timeBetweenTicks);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryExtractHealth(out var health))
        {
            _objectsWithHealth.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryExtractHealth(out var health))
        {
            _objectsWithHealth.Remove(health);
        }
    }

    private void RemoveDestroyed()
    {
        // needed as destroyed objects sometimes skip OnTriggerExit
        for (int i = _objectsWithHealth.Count-1; i >= 0; --i)
        {
            if (_objectsWithHealth[i] as MonoBehaviour == null) // magic check for destroyed objects
            {
                _objectsWithHealth.RemoveAt(i);
            }
        }
    }

    private void DamageOverTime(float dmgPerTick, float timeBetweenTicks)
    {
        if(_time < timeBetweenTicks)
        {
            _time += Time.deltaTime;
            return;
        }
        _time = 0;
        RemoveDestroyed();
        foreach (IDamageable health in _objectsWithHealth)
        {
            if (health != null)
            {
                health.TakeDamage(dmgPerTick);
            }
        }
        
    }
}
