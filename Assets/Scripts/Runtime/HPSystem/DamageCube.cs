using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;
    private List<Health> objectsHealth = new List<Health>();

    void Update()
    {
        if(objectsHealth.Count > 0)
        {
            DamageAllInside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            objectsHealth.Add(health);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            objectsHealth.Remove(health);
        }
        
        
    }
    private void DamageAllInside()
    {
        DamageOverTime(dmgPerTick, timeBetweenTicks);
    }
    float time = 0;
    private void DamageOverTime(float dmgPerTick, float timeBetweenTicks)
    {
        
        if(time < timeBetweenTicks)
        {
            time += Time.deltaTime;
        }
        else
        {
            foreach (Health health in objectsHealth)
            {
                if (health != null && health.GetIsAlive())
                {
                    health.TakeDamage(dmgPerTick);
                }
            }
            time = 0;
        }
        
    }
}
