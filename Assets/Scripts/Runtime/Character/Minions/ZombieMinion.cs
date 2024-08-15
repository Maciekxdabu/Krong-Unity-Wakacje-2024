using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class ZombieMinion : Minion
    {
        protected override void Awake()
        {
            //calll parent Awake
            base.Awake();

            type = MinionType.zombie;
            name = "Zombie Minion_" + (s_spawned_count-1);
        }
    }
}