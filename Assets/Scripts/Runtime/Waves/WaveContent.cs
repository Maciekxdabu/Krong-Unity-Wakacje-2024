using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public struct WaveContent
    {
        [SerializeField] private Enemy _enemy;
        [SerializeField] private int _amount;

        public Enemy Enemy { get { return _enemy; } }
        public int EnemyAmount { get { return _amount; } }
    }
}