using Assets.Scripts.Runtime.Character;
using StarterAssets;
using UnityEngine;

public class MinionHealth : Health
{
    private GameObject _player;
    [SerializeField] private Minion _minion;
    private Hero _playerHero;
    private ThirdPersonController _controller;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        HealthPoints = _maxHealthPoints;
        onHealthChange.AddListener(OnDeath);
        _playerHero = _player.GetComponent<Hero>();
        _controller = _playerHero.GetThirdPersonController;
    }

    [ContextMenu("Kill minion")]
    private void KillMinion()
    {
        TakeDamage(_maxHealthPoints);
    }
    [ContextMenu("Respawn minion")]
    private void RespawnMinion()
    {
        TakeHealing(_maxHealthPoints);
        Respawning();
    }
    protected override void OnDeath()
    {
        if (!isAlive)
        {
            MinionDeathBehaviour();
        }
    }
    
    private void MinionDeathBehaviour()
    {
        _controller.OnJumpEnd -= _minion.FollowHero;
        _controller.OnMove -= _minion.FollowHero;
        _playerHero.GetNotActiveMinions.Remove(_minion);
        _playerHero.GetMinions.Remove(_minion);
        Destroy(gameObject);
    }
}
