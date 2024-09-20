using Assets.Scripts.Runtime.Character;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static bool TryExtractHealth(this Collider other, out IDamageable health)
        {
            var target = other.gameObject;
            if (other.attachedRigidbody != null)
            {
                target = other.attachedRigidbody.gameObject;
            }
            return target.TryGetComponent<IDamageable>(out health);
        }
    }
}
