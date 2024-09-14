
using Assets.Scripts.Runtime.Enums;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Wawe
    {
        [SerializeField] private ScriptableObjects.StageData _stage;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _finalPoint;

        public Enemy[] GetContent { get { return _stage.GetContent; } }
        public EventTypeCaller GetEventTypeCaller { get { return _stage.GetEventTypeCaller; } }
        public float GetStartingTime { get { return _stage.GetStartingTime; } }
        public Vector3 GetSpawnPoint { get { return _spawnPoint.localPosition; } }
        public Vector3 GetFinalPoint { get { return _finalPoint.localPosition; } }
    }
}