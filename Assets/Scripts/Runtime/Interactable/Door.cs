using Assets.Scripts.Runtime.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    [SerializeField] private int keyAmountRequired = 1;
    [SerializeField] private Animator animator;

    // ---------- Unity methods

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Hero>(out Hero hero))
        {
            if (hero.TryUseKey(keyAmountRequired))
            {
                animator.SetTrigger("Open");
            }
        }
    }
}