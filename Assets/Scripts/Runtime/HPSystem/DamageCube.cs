using System.Collections.Generic;
using UnityEngine;

public class DamageCube : MonoBehaviour
{
    [SerializeField] private float dmgPerTick;
    [SerializeField] private float timeBetweenTicks;
    private List<GameObject> objectsInside = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(objectsInside.Count > 0)
        {
            DamageAllInside();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        objectsInside.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        objectsInside.Remove(other.gameObject);
        
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
            foreach (GameObject @object in objectsInside)
            {
                Health health = @object.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(dmgPerTick);
                }
            }
            time = 0;
        }
        
    }
}
