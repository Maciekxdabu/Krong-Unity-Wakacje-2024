using Assets.Scripts.Runtime.Character;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : Health
{
    [SerializeField] private ThirdPersonController playerThirdPersonController;
    [SerializeField] private Vector3 _respawnPosition;
    [SerializeField] private Hero _hero;

    protected override void OnDeath()
    {
        if(!isAlive)
        {
            playerThirdPersonController.enabled = false;
        }
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
