using Assets.Scripts.Runtime.Character;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [SerializeField] private ThirdPersonController _playerInput;
    [SerializeField] private static Vector3 _respawnPosition;
    public Vector3 RespawnPosition
    {
        get { return _respawnPosition; }
    }
    protected override void OnDeath()
    {
        if(!isAlive)
        {
            _playerInput.enabled = false;
        }
    }
    private void OnRespawn()
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
