using Assets.Scripts.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;

public class Enemy : Creature
{
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private GameObject _damagePfxPrefab;
    [SerializeField] private LayerMask _attackLayerMask;
    [SerializeField] private BonusItem _goldDropPrefab;
    [SerializeField] private int _goldDropValue;

    private bool _isAttacking;
    private bool _hasBeenSpawned;

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

        _agent = gameObject.GetComponent<NavMeshAgent>();
        _agent.speed = speed;
        _agent.stoppingDistance = NAVMESH_AGENT_STOP_DISTANCE;
        _spawnPosition = transform.position;
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
            if (_goldDropPrefab != null)
            {
                var gold = Instantiate(_goldDropPrefab, transform.position, transform.rotation, null);
                gold.SetAmount(_goldDropValue);
            }
        }
    }

    public void Update()
    {
        _localAnimator.SetFloat("Speed", _agent.velocity.magnitude);

        if (_hasBeenSpawned)
        {
            return;
        }

        if (_aggroTarget != null && !this.IsInRangeSquared(_aggroTarget, AGGRO_LOSE_RANGE_SQUARED))
        {
            // aggro lost
            _aggroTarget = null;
        }

        if (_aggroTarget == null)
        {
            // no aggro, returning to spawn point
            _agent.destination = _spawnPosition;
            _localAnimator.ResetTrigger("Attack");

            return;
        }

        //  has aggro
        _agent.destination = _aggroTarget.transform.position;

        if (isTargetInAttackRange())
        {
            if (!_isAttacking)
            {
                rotateToTarget();
            }

            _localAnimator.SetTrigger("Attack");
        }
    }

    private bool isTargetInAttackRange()
    {
        return Vector3.Distance(
            transform.position,
            _aggroTarget.transform.position) < ATTACK_RANGE;
    }

    private void rotateToTarget()
    {
        Quaternion rotateToTarget = Quaternion.LookRotation(
            _aggroTarget.transform.position - transform.position,
            Vector3.up * Time.deltaTime);

        rotateToTarget.x = 0f;
        rotateToTarget.z = 0f;
        transform.rotation = rotateToTarget;
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
    }

    public void TrySettingAggroOn(GameObject heroOrMinion)
    {
        if (_aggroTarget == null || _aggroTarget.GetDistanceSquared(this) > heroOrMinion.GetDistanceSquared(this))
        {
            _aggroTarget = heroOrMinion;
            _hasBeenSpawned = false;
        }
    }

    internal void OnSpawn(Vector3 newPosition)
    {
        updateTarget(newPosition);
        _hasBeenSpawned = true;
    }

    private void updateTarget(Vector3 newPosition)
    {
        _agent.destination = newPosition;
    }

    internal void OnStartAttack()
    {
        _isAttacking = true;
    }

    internal void AttackFrame()
    {
        if (_damagePfxPrefab != null)
        {
            Instantiate(_damagePfxPrefab, _damageLocation.transform.position, _damageLocation.transform.rotation, null);
        }

        AudioManager.Instance.PlayEnemyAttack(this);
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

    internal void OnEndAttack()
    {
        _isAttacking = false;
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