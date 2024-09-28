using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Extensions
{
    public static class NavmeshExtensions
    {
        public static bool TrySnapToNavmesh(Vector3 position, out Vector3 navmeshPosition)
        {
            const float MAX_NAVMESH_DISTANCE = 50.0f;
            var result = NavMesh.SamplePosition(
                position,
                out var navMeshHit,
                MAX_NAVMESH_DISTANCE,
                NavMesh.AllAreas
            );
            navmeshPosition = result ? navMeshHit.position : position;
            return result;
        }
    }
}
