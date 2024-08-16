using Assets.Scripts.Runtime;
using Assets.Scripts.Runtime.Character;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private float _damageCooldown = 1.5f;

    private NavMeshAgent _agent;
    private float _current_damage_cooldown;
    private Vector3 _spawn_position;

    private GameObject _aggro_target;

    public const float AGGRO_RANGE_SQUARED = 4 * 4;
    public const float AGGRO_LOSE_RANGE_SQUARED = 8 * 8;

    public void Start()
    {
        _spawn_position = transform.position;
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = 1.5f;
        _current_damage_cooldown = _damageCooldown;

        GameManager.Instance.RegisterEnemy(this);
    }

    public void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            GameManager.Instance.UnregisterEnemy(this);
        }
    }

    public void Update()
    {
        if (_aggro_target != null
            && (_aggro_target.transform.position - transform.position).sqrMagnitude > AGGRO_LOSE_RANGE_SQUARED)
        {
            _aggro_target = null;
        }
        _agent.destination = _aggro_target == null ? _spawn_position : _aggro_target.transform.position;
        if (_aggro_target != null && _agent.remainingDistance < 2.0f)
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

    //private void OnDrawGizmos()
    //{
    //    Handles.Label(transform.position + new Vector3(0, 1, 0), $"{_current_damage_cooldown}");
    //}

    public void TryAggroOn(GameObject hero)
    {
        _aggro_target = hero;
    }
}
