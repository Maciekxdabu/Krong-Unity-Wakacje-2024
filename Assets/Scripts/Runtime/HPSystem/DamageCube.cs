using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;
    private List<Health> objectsHealth = new List<Health>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(objectsHealth.Count > 0)
        {
            DamageAllInside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsHealth.Add(other.gameObject.GetComponent<Health>());
    }
    private void OnTriggerExit(Collider other)
    {
        objectsHealth.Remove(other.gameObject.GetComponent<Health>());
        
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
