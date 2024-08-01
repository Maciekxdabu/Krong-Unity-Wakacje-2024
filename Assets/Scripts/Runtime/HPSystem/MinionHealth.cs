using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinionHealth : Health
{
    [ContextMenu("Kill minion")]
    private void KillMinion()
    {
        TakeDamage(_maxHealthPoints);
    }
    [ContextMenu("Respawn minion")]
    private void RespawnMinion()
    {
        TakeHealing(_maxHealthPoints);
        OnRespawn();
    }
    protected override void OnDeath()
    {
        if (!isAlive)
        {
            Destroy(gameObject);
        }
    }
    private void OnRespawn()
    {
        
    }
}
