using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageScript : MonoBehaviour
{
    [SerializeField] private float _damage = 20;
    private ParticleSystem _pfx;
    private float _timer;
    private bool _fired;
    private List<Health> _objectsWithHealth = new List<Health>();
    const float ANTICIPATION_ANIM_TIME = 0.3f;

    void Start()
    {
        _pfx = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, _timer / ANTICIPATION_ANIM_TIME);

        if (_timer > ANTICIPATION_ANIM_TIME)
        {
            updateAfterAnticipationAnim();
        }
    }

    private void updateAfterAnticipationAnim()
    {
        if ( !_fired)
        {
            fireDamage();
            return;
        }
        // waitng for pfx to finish if hit something
        if (_pfx == null || !_pfx.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    private void fireDamage()
    {
        _fired = true;
        bool hitSomething = false;
        foreach (var o in _objectsWithHealth)
        {
            o.TakeDamage(_damage);
            hitSomething = true;
        }
        if (hitSomething && _pfx != null)
        {
            // hide mesh, play hit pfx
            GetComponent<MeshRenderer>().enabled = false;
            _pfx.Play();
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
