using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class RangeCheckExtensions
    {
        public static bool IsInRangeSquared(this MonoBehaviour me, MonoBehaviour other, float rangeSquared)
        {
            return me.GetDistanceSquared(other) <= rangeSquared;
        }

        public static bool IsInRangeSquared(this GameObject me, GameObject other, float rangeSquared)
        {
            return me.GetDistanceSquared(other) <= rangeSquared;
        }


        public static bool IsInRangeSquared(this GameObject me, MonoBehaviour other, float rangeSquared)
        {
            return me.GetDistanceSquared(other) <= rangeSquared;
        }

        public static bool IsInRangeSquared(this MonoBehaviour me, GameObject other, float rangeSquared)
        {
            return me.GetDistanceSquared(other) <= rangeSquared;
        }

        public static float GetDistanceSquared(this MonoBehaviour me, GameObject other)
        {
            return (me.transform.position - other.transform.position).sqrMagnitude;
        }

        public static float GetDistanceSquared(this MonoBehaviour me, MonoBehaviour other)
        {
            return (me.transform.position - other.transform.position).sqrMagnitude;
        }

        public static float GetDistanceSquared(this GameObject me, GameObject other)
        {
            return (me.transform.position - other.transform.position).sqrMagnitude;
        }

        public static float GetDistanceSquared(this GameObject me, MonoBehaviour other)
        {
            return (me.transform.position - other.transform.position).sqrMagnitude;
        }
    }
}
