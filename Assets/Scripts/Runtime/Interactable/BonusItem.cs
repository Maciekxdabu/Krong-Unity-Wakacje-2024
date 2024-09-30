using Assets.Scripts.Runtime;
using System;
using UnityEngine;

public class BonusItem : MonoBehaviour
{
    [SerializeField] private BonusItemType _id;
    [SerializeField] public int Amount;
    public AudioClip CollectSFX;

    internal BonusItemType GetId
    {
        get { return _id; }
    }

    internal void Delete()
    {
        Destroy(gameObject);
    }

    internal void SetAmount(int goldDropValue)
    {
        Amount = goldDropValue;
    }
}