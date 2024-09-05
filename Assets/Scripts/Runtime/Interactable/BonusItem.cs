using Assets.Scripts.Runtime;
using UnityEngine;

public class BonusItem : MonoBehaviour
{
    [SerializeField] private BonusItemType _id;

    internal BonusItemType GetId
    {
        get { return _id; }
    }

    internal void Delete()
    {
        Destroy(gameObject);
    }
}