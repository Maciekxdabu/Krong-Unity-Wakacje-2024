using Assets.Scripts.Runtime.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class SkeletonMinion : Minion
    {
        public override void Awake()
        {
            //calll parent Awake
            base.Awake();

            type = MinionType.Skeleton;
            name = "Skeleton Minion_" + (s_spawned_count-1);
        }

    }
}