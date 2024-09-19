
using Assets.Scripts.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.Character;

public class Enemy : Creature
{
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _damageLocation;
    [SerializeField] private float _damageCooldown = 1.5f;

    internal System.Action OnDeath;

    private NavMeshAgent _agent;
    private ParticleSystem _hitParticle;
    private float _currentDamageCooldown;
    private Vector3 _spawnPosition;

    private GameObject _aggroTarget;

    public const float AGGRO_RANGE_SQUARED = 4 * 4;
    public const float AGGRO_LOSE_RANGE_SQUARED = 8 * 8;
    public const float ATTACK_RANGE = 2.0f;
    public const float NAVMESH_AGENT_STOP_DISTANCE = 1.5f;

    public void Awake()
    {
        _spawnPosition = transform.position;

        _agent = gameObject.GetComponent<NavMeshAgent>();
        _hitParticle = gameObject.GetComponent<ParticleSystem>();

        _agent.stoppingDistance = NAVMESH_AGENT_STOP_DISTANCE;
        _currentDamageCooldown = _damageCooldown;
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
            tryDamaging();
        }
        else
        {
            _currentDamageCooldown = _damageCooldown;
        }
    }

    private void tryDamaging()
    {
        _currentDamageCooldown -= Time.deltaTime;
        if (_currentDamageCooldown < 0)
        {
            Instantiate(_damagePrefab, _damageLocation.transform.position, _damageLocation.transform.rotation, null);
            _currentDamageCooldown = _damageCooldown;
        }
    }

    public void TakeDamage(float value)
    {
        _hp -= value;

        _hitParticle.Play();

        UnityEngine.Debug.Log(name+" _hp "+ _hp);

        if (_hp < 0)
        {
            Debug.Log(name + " enemy died");
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
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
}