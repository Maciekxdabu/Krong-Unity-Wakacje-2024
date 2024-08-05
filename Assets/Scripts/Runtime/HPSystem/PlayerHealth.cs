using StarterAssets;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private ThirdPersonController _playerInput;
    [SerializeField] private static Vector3 _respawnPosition;
    
    protected override void OnDeath()
    {
        if(!isAlive)
        {
            _playerInput.enabled = false;
        }
    }
    protected override void OnRespawn()
    {
        if(isAlive && !_playerInput.enabled)
        {
            _playerInput.enabled = true;
            
        }
        gameObject.transform.position = _respawnPosition;
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
        OnRespawn();
    }
}
