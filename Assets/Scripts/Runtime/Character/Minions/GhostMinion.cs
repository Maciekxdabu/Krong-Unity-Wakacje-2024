using Assets.Scripts.Runtime.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class GhostMinion : Minion
    {
        protected override void Awake()
        {
            //calll parent Awake
            base.Awake();

            type = MinionType.Ghost;
            name = "Ghost Minion_" + (s_spawned_count-1);
        }
    }
}