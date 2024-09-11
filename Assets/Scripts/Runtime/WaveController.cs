using UnityEngine;

namespace Assets.Scripts.Runtime
{
    [System.Serializable]
    public class WaveController
    {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private ScriptableObjects.Stages[] _stages;
        [SerializeField] private Transform _finalPoint;

        private Enemy[] _currentlySpawned;
        internal void SpawnNextStage()
        {
            _currentlySpawned = new Enemy[_stages[0].GetContent.Length];
            for (int i = 0; i < _stages[0].GetContent.Length; i++)
            {
                _currentlySpawned[i] = Object.Instantiate(
                    _stages[0].GetContent[i],
                    _spawnPoints[0].transform.localPosition,
                    Quaternion.identity);

                _currentlySpawned[i].transform.localPosition = Character.NavMeshUtility.SampledPosition(_currentlySpawned[i].transform.localPosition);
            }
        }

        internal void Send()
        {
            for (int i = 0; i < _currentlySpawned.Length; i++)
            {
                _currentlySpawned[i].UpdateTarget(_finalPoint.localPosition);
            }
        }
    }
}