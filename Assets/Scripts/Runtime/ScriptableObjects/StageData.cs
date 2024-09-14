using Assets.Scripts.Runtime.Enums;
using UnityEngine;

namespace Assets.Scripts.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Stage", menuName = "Stages/new stage", order = 1)]
    public class StageData : ScriptableObject
    {
        [SerializeField] private Enemy[] _enemies;
        [SerializeField] private EventTypeCaller _eventTypeCaller;
        [SerializeField] private float _startingTime;

        public Enemy[] GetContent
        {
            get { return _enemies; }
        }

        public EventTypeCaller GetEventTypeCaller
        {
            get { return _eventTypeCaller; }
        }

        public float GetStartingTime
        {
            get { return _startingTime; }
        }
    }
}