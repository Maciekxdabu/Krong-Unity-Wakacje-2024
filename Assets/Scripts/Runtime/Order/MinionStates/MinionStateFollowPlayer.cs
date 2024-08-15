using Assets.Scripts.Runtime.Character;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateFollowPlayer : IMinionState
    {
        private bool _stateActive;

        private readonly Hero _player;
        private readonly Minion _minion;
        
        private Vector3 _lastHeroLocation;

        private const float STOPPING_DISTANCE = 1.5f;


        public MinionStateFollowPlayer(
                Minion minion,
                Hero player)
        {
            _minion = minion;
            _player = player;

            _player.OnJumpEnd += UpdateHeroLocation;
            _player.OnMove += UpdateHeroLocation;
        }

        public string GetDebugStateString()
        {
            return "PlayerFollow";
        }

        public void StateEnter()
        {
            _stateActive = true;
            _minion.destination = _lastHeroLocation;
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            _minion.isStopped = _minion.remainingDistance < STOPPING_DISTANCE;
        }

        public void StateEnd()
        {
            _stateActive = false;
        }

        private void UpdateHeroLocation(Vector3 heroLocation)
        {
            _lastHeroLocation = heroLocation;
            if (_stateActive) {
                _minion.destination = _lastHeroLocation;
            }
        }

        public void MinionDied()
        {
            _player.OnJumpEnd -= UpdateHeroLocation;
            _player.OnMove -= UpdateHeroLocation;
        }
    }
}
