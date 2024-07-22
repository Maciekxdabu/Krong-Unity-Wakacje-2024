using UnityEngine;

namespace Assets.Scripts.Runtime.Order
{
    [CreateAssetMenu(fileName = "OrderData", menuName = "Orders/OrderData", order = 1)]
    public class OrderData : ScriptableObject
    {
        [SerializeField] protected float maksimumDistance;
        public float GetMaksimumDistance { get { return maksimumDistance; } }
    }
}