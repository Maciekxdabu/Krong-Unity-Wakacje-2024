using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;
    private List<Health> objectsInsideHps = new List<Health>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(objectsInsideHps.Count > 0)
        {
            DamageAllInside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Health otherHealth = other.gameObject.GetComponent<Health>();
        if (!objectsInsideHps.Contains(otherHealth))
        {
            objectsInsideHps.Add(otherHealth);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Health otherHealth = other.gameObject.GetComponent<Health>();
        if (objectsInsideHps.Contains(otherHealth) )
        {
            objectsInsideHps.Remove(otherHealth);
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
            foreach (Health health in objectsInsideHps)
            {
                if (health != null)
                {
                    health.TakeDamage(dmgPerTick);
                }
            }
            time = 0;
        }
        
    }
}
