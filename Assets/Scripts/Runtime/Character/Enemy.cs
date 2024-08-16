using Assets.Scripts.Runtime.Character;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private float _damageCooldown = 1.5f;

    private Hero _hero;
    private NavMeshAgent _agent;
    private float _current_damage_cooldown;


    void Start()
    {
        _hero = FindObjectOfType<Hero>();
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = 1.5f;
        _current_damage_cooldown = _damageCooldown;
    }

    void Update()
    {
        _agent.destination = _hero.transform.position;
        if (_agent.remainingDistance < 2.0f)
        {
            tryDamaging();
        }
        else
        {
            _current_damage_cooldown = _damageCooldown;
        }
    }

    private void tryDamaging()
    {
        _current_damage_cooldown -= Time.deltaTime;
        if (_current_damage_cooldown < 0)
        {
            Instantiate(_damagePrefab, _damageLocation.transform.position, _damageLocation.transform.rotation, null);
            _current_damage_cooldown = _damageCooldown;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + new Vector3(0, 1, 0), $"{_current_damage_cooldown}");

    }
}
