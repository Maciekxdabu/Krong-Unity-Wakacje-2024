using Assets.Scripts.Runtime.Character;
using System;
using UnityEngine;

public class MinionHealth : Health
{
    private GameObject _player;
    [SerializeField] private Minion _minion;
    private Hero _playerHero;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        HealthPoints = _maxHealthPoints;
        onHealthChange.AddListener(OnDeath);
        _playerHero = _player.GetComponent<Hero>();
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
        _playerHero.MinionDied(_minion);
        _minion.Died();

        Destroy(gameObject);
    }
}
