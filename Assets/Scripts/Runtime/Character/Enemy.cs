
using Assets.Scripts.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;
using System;

public class Enemy : Creature
{
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private float _damageCooldown = 1.5f;

    private NavMeshAgent _agent;
    private ParticleSystem _hitParticle;
    private float _currentDamageCooldown;
    private Vector3 _spawnPosition;

    private GameObject _aggroTarget;

    public const float AGGRO_RANGE_SQUARED = 4 * 4;
    public const float AGGRO_LOSE_RANGE_SQUARED = 8 * 8;
    public const float ATTACK_RANGE = 2.0f;
    public const float NAVMESH_AGENT_STOP_DISTANCE = 1.5f;

    private float _baseSpeed;


    public override void Awake()
    {
        base.Awake();
        _spawnPosition = transform.position;

        _agent = gameObject.GetComponent<NavMeshAgent>();
        _hitParticle = gameObject.GetComponent<ParticleSystem>();

        _agent.stoppingDistance = NAVMESH_AGENT_STOP_DISTANCE;
        _currentDamageCooldown = _damageCooldown;

        _baseSpeed = _agent.speed;
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

        _hitParticle.Play();
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
        Instantiate(_damagePrefab, _damageLocation.transform.position, _damageLocation.transform.rotation, null);
    }

    internal void SetIsDuringAttack(bool isDoingAnAttack)
    {
        _agent.speed = isDoingAnAttack ? 0.0f : _baseSpeed;
        _localAnimator.ResetTrigger("Attack");
    }
}