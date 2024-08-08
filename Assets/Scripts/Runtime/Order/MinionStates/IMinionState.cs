namespace Assets.Scripts.Runtime.Order.MinionStates
{
    interface IMinionState
    {
        string GetDebugStateString();

        void StateEnter();
        void Update();
        void StateEnd();
        void MinionDied();
    }

    enum StateSlot {
        STATE_FOLLOW_HERO,
        STATE_MOVE_TO_POINT,
        STATE_INTERACT,
    };
}
