using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    interface IMinionState
    {
        string GetStateName();

        void StateEnter();
        void Update();
        void StateEnd();
    }

    enum StateSlot {
        STATE_FOLLOW_HERO,
        STATE_MOVE_TO_POINT,
        STATE_INTERACT,
    };
}
