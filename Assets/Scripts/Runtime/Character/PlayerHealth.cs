using Assets.Scripts.Runtime.Character;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : Health
{
    [SerializeField] private ThirdPersonController _playerInput;
    private void Awake()
    {
        onHealthChange.AddListener(OnDeath);
    }
    private void OnDeath()
    {
        if(!isAlive)
        {
            _playerInput.enabled = false;
        }
    }

    [ContextMenu("Kill player")]
    private void KillPlayer()
    {
        TakeDamage(_maxHealthPoints);
    }
}
