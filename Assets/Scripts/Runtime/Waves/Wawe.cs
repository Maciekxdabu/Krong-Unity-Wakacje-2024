using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Wave
    {
        [SerializeField] private Spawn[] _spawns;
        [SerializeField] private float _startingTime;

        public UnityEvent OnWaveStart;
        public UnityEvent OnWaveEnd;

        public Spawn[] Spawns { get { return _spawns; } }
        public float GetStartingTime { get { return _startingTime; } }
    }
}