using Assets.Scripts.Runtime.Character;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateGoForward : IMinionState
    {
        private bool _stateActive;

        private readonly Minion _minion;
        private readonly NavMeshAgent _minionNavmeshAgent;
        
        private readonly Hero _player;

        private Vector3 _requestedDestination;

        private const float STOPPING_DISTANCE = 1.5f;

        public MinionStateGoForward(
                Minion minion,
                Hero player,
                ThirdPersonController localThirdPersonController,
                NavMeshAgent minionNavmeshAgent)
        {
            _minion = minion;
            _player = player;
            _minionNavmeshAgent = minionNavmeshAgent;
        }

        public string GetDebugStateString()
        {
            return "GoForward";
        }

        public void StateEnter()
        {
            _requestedDestination = FindDestination();

            _stateActive = true;
            _minionNavmeshAgent.destination = _requestedDestination;
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            if (_minionNavmeshAgent.remainingDistance >= STOPPING_DISTANCE)
            {
                _minionNavmeshAgent.isStopped = false;
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


        private Vector3 FindDestination()
        {
            var heroTransform = _player.GetFrontTransform;
            var maxDistance = _player.sendOrderData.MaxDistance;

            var result = heroTransform.position + (heroTransform.forward * maxDistance);

            var ray = new Ray(heroTransform.position, heroTransform.forward);
            var layerMask = Physics.DefaultRaycastLayers;
            bool wallDetected = Physics.Raycast(
                    ray,
                    out var wallHit,
                    maxDistance,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                );

            if (wallDetected)
            {
                const float MAX_NAVMESH_DISTANCE = 100f;
                NavMesh.SamplePosition(
                    wallHit.point,
                    out var navMeshHit,
                    MAX_NAVMESH_DISTANCE,
                    NavMesh.AllAreas
                    );

                return navMeshHit.position;
            }

            return result;
        }

        public void MinionDied()
        {

        }
    }
}
