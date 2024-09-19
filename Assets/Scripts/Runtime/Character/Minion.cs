using Assets.Scripts.Runtime.Order.MinionStates;
using Assets.Scripts.Runtime.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Character
{
    public abstract class Minion : Creature
    {
        protected MinionType type = MinionType.None;
        public MinionType Type { get { return type; } }

        [SerializeField] protected MinonConfigurationData _config;
        [SerializeField] protected NavMeshAgent _localNavMeshAgent;
        [SerializeField] protected LayerMask _attackLayerMask;


        protected static int s_spawned_count = 1;

        private IMinionState _currentState;
        private StateSlot _currentStateEnum;
        private Dictionary<StateSlot, IMinionState> _allStates = new Dictionary<StateSlot, IMinionState>();

        private MinionStateFollowPlayer _followPlayerState;
        private MinionStateGoForward _goForwardState;
        private MinionStateInteract _interactState;
        private MinionStateFight _fightState;

        private bool _isFollowingAnOrder;
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

        protected virtual void Awake()
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
            _fightState = new MinionStateFight(this, hero);

            _allStates[StateSlot.STATE_FOLLOW_HERO] = _followPlayerState;
            _allStates[StateSlot.STATE_MOVE_TO_POINT] = _goForwardState;
            _allStates[StateSlot.STATE_INTERACT] = _interactState;
            _allStates[StateSlot.STATE_FIGHT] = _fightState;

            _currentStateEnum = StateSlot.STATE_FOLLOW_HERO;
            _currentState = _allStates[_currentStateEnum];
            _currentState.StateEnter();
        }

        public void Update()
        {
            _currentState?.Update();
            if (_localAnimator != null)
            {
                _localAnimator.SetFloat("Speed", _localNavMeshAgent.velocity.magnitude);
            }
        }

        internal void TryAttacking()
        {
            _localAnimator.SetTrigger("Attack");
        }

        public void AttackFrame()
        {
            //Debug.Log("Attack Frame");
            var hitTargets = Physics.OverlapSphere(transform.position, 1.0f, _attackLayerMask);
            foreach (var hit in hitTargets)
            {
                if (!hit.TryGetComponent<Enemy>(out var e)) {
                    continue;
                }
                e.TakeDamage(10.0f);
            }
        }

        private void GoToState(StateSlot newState) {
            Assert.AreNotEqual(_currentStateEnum, newState);

            _currentState.StateEnd();

            _currentStateEnum = newState;
            _currentState = _allStates[_currentStateEnum];
            _localAnimator.ResetTrigger("Attack");

            _currentState.StateEnter();
        }


        public void SendForward()
        {
            Assert.AreEqual(_currentStateEnum, StateSlot.STATE_FOLLOW_HERO, "SendForward outside STATE_FOLLOW_HERO");
            _isFollowingAnOrder = true;
            GoToState(StateSlot.STATE_MOVE_TO_POINT);
        }

        public void DestinationReached()
        {
            Assert.AreEqual(_currentStateEnum, StateSlot.STATE_MOVE_TO_POINT, "DestinationReached outside STATE_MOVE_TO_POINT");

            GoToState(StateSlot.STATE_FOLLOW_HERO);

            if (_isFollowingAnOrder){
               OnOrderFinished.Invoke(this);
                _isFollowingAnOrder = false;
            }
        }

        public void InterruptCurrentOrder()
        {
            if (_currentStateEnum == StateSlot.STATE_FOLLOW_HERO) return; // nothing to interrupt

            GoToState(StateSlot.STATE_FOLLOW_HERO);

            if (_isFollowingAnOrder)
            {
                OnOrderFinished.Invoke(this);
                _isFollowingAnOrder = false;
            }
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

            if (_interactState.SetupInteraction(interactable) && interactable.DoesNeedMoreMinions()) {
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
            gameObject.GetComponent<NavMeshAgent>().Warp(position);
        }

        public void EnemyInRange(Enemy e)
        {
            if (_fightState.ShouldFightEnemyInRange(e))
            {
                if (_currentStateEnum == StateSlot.STATE_MOVE_TO_POINT || _currentStateEnum == StateSlot.STATE_FOLLOW_HERO)
                {
                    GoToState(StateSlot.STATE_FIGHT);
                }
            }
        }

        public Vector3 CalculateGoOrderDestination()
        {
            var MAX_DISTANCE = _config.MoveOrderMaxDistance;
            var heroFront = GameManager.Instance.Hero.GetFrontTransform();

            var ray = new Ray(heroFront.position, heroFront.forward);
            var layerMask = Physics.DefaultRaycastLayers;
            bool wallDetected = Physics.Raycast(
                    ray,
                    out var wallHit,
                    MAX_DISTANCE,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                );

            if (wallDetected)
            {
                return NavMeshUtility.SampledPosition(wallHit.point);
            }

            return heroFront.position + (heroFront.forward * MAX_DISTANCE);
        }
    }
}