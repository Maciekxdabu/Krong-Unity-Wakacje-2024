using Assets.Scripts.Runtime.Character;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateFollowPlayer : IMinionState
    {
        private bool _stateActive;

        private readonly Minion _minion;
        private readonly NavMeshAgent _minionNavmeshAgent;
        
        private readonly Hero _player;
        private readonly ThirdPersonController localThirdPersonController;
        private Vector3 _lastHeroLocation;

        private const float STOPPING_DISTANCE = 1.5f;


        public MinionStateFollowPlayer(
                Minion minion,
                Hero player,
                ThirdPersonController localThirdPersonController,
                NavMeshAgent minionNavmeshAgent)
        {
            _minion = minion;
            _player = player;
            this.localThirdPersonController = localThirdPersonController;
            _minionNavmeshAgent = minionNavmeshAgent;


            localThirdPersonController.OnJumpEnd += UpdateHeroLocation;
            localThirdPersonController.OnMove += UpdateHeroLocation;
        }

        public string GetDebugStateString()
        {
            return "PlayerFollow";
        }

        public void StateEnter()
        {
            _stateActive = true;
            _minionNavmeshAgent.destination = _lastHeroLocation;
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            _minionNavmeshAgent.isStopped = _minionNavmeshAgent.remainingDistance < STOPPING_DISTANCE;
        }

        public void StateEnd()
        {
            _stateActive = false;
        }

        private void UpdateHeroLocation(Vector3 heroLocation)
        {
            _lastHeroLocation = heroLocation;
            if (_stateActive) {
               _minionNavmeshAgent.destination = _lastHeroLocation;
            }
        }

        public void MinionDied()
        {
            localThirdPersonController.OnJumpEnd -= UpdateHeroLocation;
            localThirdPersonController.OnMove -= UpdateHeroLocation;
        }
    }
}
