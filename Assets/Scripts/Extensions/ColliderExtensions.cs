using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static bool TryExtractHealth(this Collider other, out Health health)
        {
            var target = other.gameObject;
            if (other.attachedRigidbody != null)
            {
                target = other.attachedRigidbody.gameObject;
            }
            return target.TryGetComponent<Health>(out health);
        }
    }
}
