using Assets.Scripts.Runtime.Enums;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public struct Stage
    {
        internal Vector3 GetSpawnPoint { get; private set; }
        internal Vector3 GetFinalPoint { get; private set; }
        internal Enemy[] GetContent { get; private set; }
        internal EventTypeCaller GetEventTypeCaller { get; private set; }
        internal float GetStartingTime{ get; private set; }

        internal void Initialize(Wawe wave)
        {
            GetContent = wave.GetContent;
            GetEventTypeCaller = wave.GetEventTypeCaller;
            GetStartingTime = wave.GetStartingTime;
            GetSpawnPoint = wave.GetSpawnPoint;
            GetFinalPoint = wave.GetFinalPoint;
        }
    }
}