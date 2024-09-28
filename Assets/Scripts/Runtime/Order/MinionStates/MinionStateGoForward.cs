using Assets.Scripts.Runtime.Character;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateGoForward : IMinionState
    {
        private bool _stateActive;

        private readonly Minion _minion;
        private readonly Hero _player;

        private Vector3 _requestedDestination;

        private const float STOPPING_DISTANCE = 1.5f;

        public MinionStateGoForward(
                Minion minion,
                Hero player)
        {
            _minion = minion;
            _player = player;
        }

        public string GetDebugStateString()
        {
            return "GoForward";
        }

        public void StateEnter(object enterParams)
        {
            if (! (enterParams is Vector3))
            {
                Debug.LogError($"State {nameof(MinionStateGoForward)} - invalid StateEnter enterParams");
                _stateActive = true;
                return;
            }
            _requestedDestination = (Vector3) enterParams;

            _stateActive = true;
            _minion.destination = _requestedDestination;
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            if (_minion.remainingDistance >= STOPPING_DISTANCE)
            {
                _minion.isStopped = false;
            }
            else
            {
                _minion.DestinationReached();
            }
        }

        public void StateEnd()
        {
            _stateActive = false;
        }

        public void MinionDied()
        {

        }
    }
}
