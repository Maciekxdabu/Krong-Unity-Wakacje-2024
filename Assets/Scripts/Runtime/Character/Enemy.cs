
using Assets.Scripts.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;
using System;
using UnityEngine.Rendering;

public class Enemy : Creature
{
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private GameObject _damagePfxPrefab;
    [SerializeField] private LayerMask _attackLayerMask;



    private NavMeshAgent _agent;
    private Vector3 _spawnPosition;

    private GameObject _aggroTarget;

    public const float AGGRO_RANGE_SQUARED = 4 * 4;
    public const float AGGRO_LOSE_RANGE_SQUARED = 8 * 8;
    public const float ATTACK_RANGE = 2.0f;
    public const float NAVMESH_AGENT_STOP_DISTANCE = 1.5f;
    public const float DAMAGE_RADIUS = 1.5f;


    public override void Awake()
    {
        base.Awake();
        _spawnPosition = transform.position;

        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.speed = speed;

        _agent.stoppingDistance = NAVMESH_AGENT_STOP_DISTANCE;
    }

    public void Start()
    { 
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
        _localAnimator?.SetFloat("Speed", _agent.velocity.magnitude);

        if (_aggroTarget != null && !this.IsInRangeSquared(_aggroTarget, AGGRO_LOSE_RANGE_SQUARED))
        {
            // aggro lost
            _aggroTarget = null;
        }

        if (_aggroTarget == null)
        {
            // no aggro, returning to spawn point
            _agent.destination = _spawnPosition;
            return;
        }

        //  has aggro
        _agent.destination = _aggroTarget.transform.position;

        if (_agent.remainingDistance < ATTACK_RANGE)
        {
            _localAnimator?.SetTrigger("Attack");
        }
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
    }

    public void TrySettingAggroOn(GameObject heroOrMinion)
    {
        if (_aggroTarget == null || _aggroTarget.GetDistanceSquared(this) > heroOrMinion.GetDistanceSquared(this)){
            _aggroTarget = heroOrMinion;
        }
    }

    internal void UpdateTarget(Vector3 newPosition)
    {
        _spawnPosition = newPosition;
        _agent.destination = newPosition;
    }

    internal void AttackFrame()
    {
        if (_damagePfxPrefab  != null){
            Instantiate(_damagePfxPrefab, _damageLocation.transform.position, _damageLocation.transform.rotation, null);
        }
        var hitTargets = Physics.OverlapSphere(_damageLocation.transform.position, DAMAGE_RADIUS, _attackLayerMask);
        foreach (var hit in hitTargets)
        {
            if (!hit.TryExtractHealth(out var e))
            {
                continue;
            }
            e.TakeDamage(GetDamageValue());
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_damageLocation.transform.position, DAMAGE_RADIUS);
    }

    internal void SetIsDuringAttack(bool isDoingAnAttack)
    {
        _agent.speed = isDoingAnAttack ? 0.0f : speed;
        _localAnimator.ResetTrigger("Attack");
    }
}