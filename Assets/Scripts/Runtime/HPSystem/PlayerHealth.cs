using Assets.Scripts.Runtime.Character;
using Assets.Scripts.Runtime.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : Health
{
    [SerializeField] private Vector3 _respawnPosition;
    [SerializeField] private Hero _hero;

    protected override void OnDeath()
    {
        HUD.Instance.RefreshHUD(_hero);

        if (!isAlive)
        {
            _hero.Died();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _respawnPosition = transform.position;
    }

    protected override void Respawning()
    {
        transform.position = _respawnPosition;
        Physics.SyncTransforms();
        _hero.Respawn(_respawnPosition);
    }
    
    public void OnRespawn(InputValue inputValue)
    {
        TakeHealing(_maxHealthPoints);
        Respawning();
    }

    [ContextMenu("Kill player")]
    private void KillPlayer()
    {
        TakeDamage(_maxHealthPoints);
    }

    [ContextMenu("Respawn player")]
    private void RespawnPlayer()
    {
        TakeHealing(_maxHealthPoints);
        Respawning();
    }
}
