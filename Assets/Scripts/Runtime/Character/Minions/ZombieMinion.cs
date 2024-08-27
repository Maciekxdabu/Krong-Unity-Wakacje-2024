using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class MummyMinion : Minion
    {
        protected override void Awake()
        {
            //calll parent Awake
            base.Awake();

            type = MinionType.MUMMY;
            name = "Mummy Minion_" + (s_spawned_count-1);
        }
    }
}