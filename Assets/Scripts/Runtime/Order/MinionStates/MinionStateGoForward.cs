using Assets.Scripts.Runtime.Character;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateGoForward : IMinionState
    {
        private bool _stateActive;

        private readonly Minion _minion;
        private readonly Hero _player;

        private Vector3 _requestedDestination;

        private const float STOPPING_DISTANCE = 0.5f;
        private const float WAIT_AFTER_REACHING_SECONDS = 0.5f;

        private float _waitTimer = 0.0f;

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
            _minion.stoppingDistance = STOPPING_DISTANCE;

            _waitTimer = WAIT_AFTER_REACHING_SECONDS;
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            if (_minion.remainingDistance <= STOPPING_DISTANCE)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer < 0)
                {
                    _minion.DestinationReached();
                }
            }
            else
            {
                _waitTimer = WAIT_AFTER_REACHING_SECONDS;
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
