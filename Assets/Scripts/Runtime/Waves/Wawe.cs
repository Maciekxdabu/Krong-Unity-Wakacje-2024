using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Wave
    {
        [SerializeField] private Spawn[] _spawns;
        [SerializeField] private float _startingTime;

        internal System.Action OnStart;
        internal System.Action OnEnd;

        public Spawn[] Spawns { get { return _spawns; } }
        public float GetStartingTime { get { return _startingTime; } }
    }
}