using Assets.Scripts.Runtime.Order.MinionStates;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Character
{
    public class Minion : Creature
    {
        [SerializeField] private NavMeshAgent _localNavMeshAgent;

        private static int s_spawned_count = 1;

        private IMinionState _currentState;
        private StateSlot _currentStateEnum;
        private Dictionary<StateSlot, IMinionState> _allStates = new Dictionary<StateSlot, IMinionState>();

        private MinionStateFollowPlayer _followPlayerState;
        private MinionStateGoForward _goForwardState;
        private MinionStateInteract _interactState;

        public Action<Minion> OnOrderFinished;

        public Vector3 destination
        {
            get { return _localNavMeshAgent.destination; }
            set { _localNavMeshAgent.destination = value; }
        }
        public bool isStopped
        {
            get { return _localNavMeshAgent.isStopped; }
            set { _localNavMeshAgent.isStopped = value; }
        }
        public float remainingDistance => _localNavMeshAgent.remainingDistance;

        private void Awake()
        {
            _localNavMeshAgent.speed = speed;
            name = "Minion_" + s_spawned_count;
            ++s_spawned_count;
        }

        internal void Init(Hero hero)
        {
            _followPlayerState = new MinionStateFollowPlayer(this, hero);
            _goForwardState = new MinionStateGoForward(this, hero);
            _interactState = new MinionStateInteract(this, hero);

            _allStates[StateSlot.STATE_FOLLOW_HERO] = _followPlayerState;
            _allStates[StateSlot.STATE_MOVE_TO_POINT] = _goForwardState;
            _allStates[StateSlot.STATE_INTERACT] = _interactState;

            _currentStateEnum = StateSlot.STATE_FOLLOW_HERO;
            _currentState = _allStates[_currentStateEnum];
            _currentState.StateEnter();
        }

        private void Update()
        {
            _currentState?.Update();
        }

        private void GoToState(StateSlot newState) {
            Assert.AreNotEqual(_currentStateEnum, newState);

            _currentState.StateEnd();

            _currentStateEnum = newState;
            _currentState = _allStates[_currentStateEnum];
            _currentState.StateEnter();
        }


        public void SendForward()
        {
            Assert.AreEqual(_currentStateEnum, StateSlot.STATE_FOLLOW_HERO, "SendForward outside STATE_FOLLOW_HERO");

            GoToState(StateSlot.STATE_MOVE_TO_POINT);
        }

        public void DestinationReached()
        {
            Assert.AreEqual(_currentStateEnum, StateSlot.STATE_MOVE_TO_POINT, "DestinationReached outside STATE_MOVE_TO_POINT");

            GoToState(StateSlot.STATE_FOLLOW_HERO);

            OnOrderFinished.Invoke(this);
        }

        public void InterruptCurrentOrder()
        {
            if (_currentStateEnum == StateSlot.STATE_FOLLOW_HERO) return; // nothing to interrupt

            GoToState(StateSlot.STATE_FOLLOW_HERO);

            OnOrderFinished.Invoke(this);
        }


        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject && collider.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                // FIXME: Why is this called twice?
                InteractableEncountered(interactable);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject && collider.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                // FIXME: Why is this called twice?
                InteractableLeftArea(interactable);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(destination, 0.3f);
            Handles.Label(transform.position + new Vector3(0,1,0), _currentState?.GetDebugStateString()??"");
        }

        private void InteractableEncountered(Interactable interactable)
        {
            //Debug.Log($"{name} - {nameof(InteractableEncountered)} - {interactable}");

            if (_interactState.SetupInteraction(interactable)) {
                if (_currentStateEnum == StateSlot.STATE_MOVE_TO_POINT){
                    GoToState(StateSlot.STATE_INTERACT);
                }
            }
        }

        public void InteractionTaskFinished()
        {
            Assert.AreEqual(_currentStateEnum, StateSlot.STATE_INTERACT, "InteractionTaskFinished outside STATE_INTERACT");

            GoToState(StateSlot.STATE_FOLLOW_HERO);

            OnOrderFinished.Invoke(this);
        }

        private void InteractableLeftArea(Interactable interactable)
        {
            //Debug.Log($"{name} - {nameof(InteractableLeftArea)} - {interactable}");

            if (_currentStateEnum != StateSlot.STATE_INTERACT) return;

            _interactState.InteractableLost(interactable);
        }

        internal void Died()
        {
            foreach(var (_,state) in _allStates)
            {
                state.MinionDied();
            }
        }

        internal void PlayerRespawnedAt(Vector3 position)
        {
            gameObject.transform.position = position;
        }
    }
}