using Assets.Scripts.Runtime.Character;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.ScriptableObjects
{
    public enum MinionType
    {
        None = 0,//mainly used by player for "All", but also for Minions that are (NYI = Not Yet Implemented)
        Any = 0,
        Skeleton = 1,
        Mummy = 2,//mummy???
        Ghost = 3,
        Vampire = 4
    }

    [CreateAssetMenu(fileName = "MinionConfig", menuName = "Configs", order = 1)]
    public class MinonConfigurationData : ScriptableObject
    {
        public float MoveOrderMaxDistance;

        public List<MinionSpawnerConfigEntry> SpawnerConfig;

    }

    [Serializable]
    public class MinionSpawnerConfigEntry
    {
        public MinionType Type;
        public Minion Prefab;
        public Material notLitMaterial;
        public Material litMaterial;
    }
}
