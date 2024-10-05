using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Spawn
    {
        [SerializeField] private WaveContent _content;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _finalPoint;

        public Enemy GetContent { get { return _content.Enemy; } }
        public int EnemyAmount { get { return _content.EnemyAmount; } }
        public Vector3 GetSpawnPoint { get { return _spawnPoint.position; } }
        public Vector3 GetFinalPoint { get { return _finalPoint.localPosition; } }
    }
}