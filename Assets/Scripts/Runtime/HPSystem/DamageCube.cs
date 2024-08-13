using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;

    float _time = 0;
    private List<Health> _objectsWithHealth = new List<Health>();

    void Update()
    {
        if(_objectsWithHealth.Count > 0)
        {
            DamageAllInside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            _objectsWithHealth.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            _objectsWithHealth.Remove(health);
        }
    }

    private void DamageAllInside()
    {
        DamageOverTime(dmgPerTick, timeBetweenTicks);
    }

    private void DamageOverTime(float dmgPerTick, float timeBetweenTicks)
    {
        
        if(_time < timeBetweenTicks)
        {
            _time += Time.deltaTime;
        }
        else
        {
            foreach (Health health in _objectsWithHealth)
            {
                if (health != null && health.GetIsAlive())
                {
                    health.TakeDamage(dmgPerTick);
                }
            }
            _time = 0;
        }
        
    }
}
