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
        private readonly Hero _player;

        private Interactable _interactable;

        private const float STOPPING_DISTANCE = 1.5f;

        public MinionStateInteract(
                Minion minion,
                Hero player)
        {
            _minion = minion;
            _player = player;
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
            return true;
        }

        public void StateEnter(object enterParams)
        {
            _interactable = enterParams as Interactable;

            _stateActive = true;
            _minion.destination = _interactable.AssignPosition();
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

            _minion.isStopped = _minion.transform.position == _minion.destination;
        }

        public void StateEnd()
        {
            _interactable.TaskDoneCallback.RemoveListener(InteractableTaskFinished);
            _interactable.EndInteractionWithMinion(_minion);
            _interactable = null;
            _stateActive = false;
        }

        public void MinionDied()
        {
            if(_interactable != null && _stateActive)
            {
                StateEnd();
            }
        }
    }
}
