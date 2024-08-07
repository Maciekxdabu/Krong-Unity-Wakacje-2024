using Assets.Scripts.Runtime.Character;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateInteract : IMinionState
    {
        private bool _stateActive;

        private readonly Minion _minion;
        private readonly NavMeshAgent _minionNavmeshAgent;

        private Interactable _interactable;

        private const float STOPPING_DISTANCE = 1.5f;

        public MinionStateInteract(
                Minion minion,
                Hero player,
                ThirdPersonController localThirdPersonController,
                NavMeshAgent minionNavmeshAgent)
        {
            _minion = minion;
            _minionNavmeshAgent = minionNavmeshAgent;
        }

        public string GetDebugStateString()
        {
            return "Interacting";
        }


        public bool SetupInteraction(Interactable interactable)
        {
            if (_stateActive && _interactable != null) {
                return false;
            }
            _interactable = interactable;
            return true;
        }

        public void StateEnter()
        {
            _stateActive = true;
            _minionNavmeshAgent.destination = _interactable.transform.position;
            _interactable.StartInteractionWithMinion(_minion);
            _interactable.TaskDoneCallback.AddListener(InteractableTaskFinished);
        }

        private void InteractableTaskFinished(Interactable interactable)
        {
            Debug.Log($"InteractableTaskFinished {_minion} {interactable}");
            Assert.AreEqual(_interactable, interactable);
            _minion.InteractionTaskFinished();
        }

        public void InteractableLost(Interactable interactable)
        {
            if (interactable != _interactable) { return; }

            _minion.InterruptCurrentOrder();
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            Assert.IsNotNull(_interactable, "interactable not set");

            _minionNavmeshAgent.isStopped = _minionNavmeshAgent.remainingDistance < STOPPING_DISTANCE;
        }

        public void StateEnd()
        {
            _interactable.TaskDoneCallback.RemoveListener(InteractableTaskFinished);
            _interactable.EndInteractionWithMinion(_minion);
            _interactable = null;
            _stateActive = false;
        }

    }
}
