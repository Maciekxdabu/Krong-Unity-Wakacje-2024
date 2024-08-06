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
    
    protected override void OnRespawn()
    {
        transform.position = _respawnPosition;
        if (!playerThirdPersonController.enabled)
        {
            playerThirdPersonController.enabled = true;
        }
        if (_hero.GetMinions.Count > 0)
        {
            foreach (Minion minion in _hero.GetMinions)
            {
                minion.gameObject.transform.position = _respawnPosition;
                minion.GoToPostion(_respawnPosition);
            }
        }
    }

    public void Respawning(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            RespawnPlayer();
        }

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
