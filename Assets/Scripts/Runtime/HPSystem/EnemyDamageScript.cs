using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageScript : MonoBehaviour
{
    [SerializeField] private float _damage = 20;
    private ParticleSystem _pfx;
    private float _timer;
    private bool _fired;
    private List<Health> _objectsWithHealth = new List<Health>();
    const float ANIM_TIME = 0.3f;

    void Start()
    {
        _pfx = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, _timer / ANIM_TIME);

        if (_timer > ANIM_TIME) {
            if (_fired)
            {
                if (!_pfx.IsAlive()) { 
                    Destroy(gameObject);
                }
                return;
            }
            _fired = true;
            bool hitSth = false;
            foreach (var o in _objectsWithHealth){
                o.TakeDamage(_damage);
                hitSth = true;
            }
            if (hitSth) {
                GetComponent<MeshRenderer>().enabled = false;
                _pfx.Play();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            _objectsWithHealth.Add(health);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out var health))
        {
            _objectsWithHealth.Remove(health);
        }
    }

}
