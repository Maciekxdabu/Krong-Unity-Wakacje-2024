using Assets.Scripts.Runtime.Order;
using Assets.Scripts.Runtime.Order.MinionStates;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Runtime.Character
{
    public class Minion : Creature
    {
        [SerializeField] private NavMeshAgent localNavMeshAgent;
        [SerializeField] private float minDistanceToStartFollowTheCharacterPlayer;

        private static int s_spawned_count = 1;

        private IMinionState _currentState;
        private StateSlot _currentStateEnum;
        private Dictionary<StateSlot, IMinionState> _allStates = new Dictionary<StateSlot, IMinionState>();

        public System.Action<Minion> OnFishedOrder;

        private void Awake()
        {
            localNavMeshAgent.speed = speed;
            name = "Minion_" + s_spawned_count;
            ++s_spawned_count;
        }

        internal void Init(Hero hero, ThirdPersonController localThirdPersonController)
        {
            _allStates[StateSlot.STATE_FOLLOW_HERO] = new MinionStateFollowPlayer(this, hero, localThirdPersonController, localNavMeshAgent);

            _currentStateEnum = StateSlot.STATE_FOLLOW_HERO;
            _currentState = _allStates[_currentStateEnum];

            _currentState.StateEnter();
        }

        private void Update()
        {
            _currentState?.Update();
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
            Gizmos.DrawSphere(localNavMeshAgent.destination, 0.3f);
        }

        private void InteractableEncountered(Interactable interactable)
        {
            Debug.Log($"{name} - {nameof(InteractableEncountered)} - {interactable}");

            //if (_currentInteractable != null) {
            //    return; // ignore others when interacting already
            //            // TODO: interactable priorities
            //}
            //if (_currentRuns == null)
            //{
            //    return; // only interact during move order
            //}
            //interactable.StartInteractionWithMinion(this);
            //_currentInteractable = interactable;
            //updateTarget(_currentInteractable.transform.position);
            //_currentInteractable.TaskDoneCallback.AddListener(InteractableTaskFinished);
        }
        
        private void InteractableLeftArea(Interactable interactable)
        {
            Debug.Log($"{name} - {nameof(InteractableLeftArea)} - {interactable}");

            //interactable.EndInteractionWithMinion(this);
            //_currentInteractable = null;
            //_currentInteractable.TaskDoneCallback.RemoveListener(InteractableTaskFinished);
        }

        private void InteractableTaskFinished(Interactable interactable) {
            Debug.Log($"{name} - {nameof(InteractableTaskFinished)} - {interactable}");

            //interactable.EndInteractionWithMinion(this);
            //_currentInteractable = null;
        }


        public void InterruptCurrentOrder()
        {
        //    StopCoroutine(_currentRuns);
        //    _currentRuns = null;
        //    Debug.Log($"{name} - {nameof(InterruptCurrentOrder)} - OnFishedOrder");
        //    OnFishedOrder?.Invoke(this);
        }

        public void FollowHero(Vector3 newPosition)
        {
            //updateTarget(newPosition);

            //if (isPlayerCharacterOutOfRange())
            //{
            //    localNavMeshAgent.SetDestination(_newPosition);
            //    if (!_isGoingToNewPositionAlready)
            //    {
            //        StartCoroutine(goToHero());
            //    }
            //}
        }

        public void GiveOrder(IOrder newOrder)
        {
            //newOrder.Execute();
        }

        public void GoToPostion(Vector3 newPosition)
        {
            //Debug.Log($"{name} - go to position - {newPosition}");
            //updateTarget(newPosition);
            //_currentRuns = StartCoroutine(goToPosition());
            //return _currentRuns;
        }

        private IEnumerator goToHero()
        {
            yield return null;
            //Debug.Log($"{name} - {nameof(goToHero)} coroutine start");
            //_isGoingToNewPositionAlready = true;
            //while (isPlayerCharacterOutOfRange())
            //{
            //    yield return null;
            //}

            //stop();
            //_isGoingToNewPositionAlready = false;
            //Debug.Log($"{name} - {nameof(goToHero)} coroutine end");
        }

        private IEnumerator goToPosition()
        {
            yield return null;
            //Debug.Log($"{name} - {nameof(goToPosition)} coroutine start");

            //localNavMeshAgent.SetDestination(_newPosition);
            //_isGoingToNewPositionAlready = true;

            //while (localNavMeshAgent.pathPending)
            //{
            //    yield return null;
            //}

            //while (localNavMeshAgent.remainingDistance > STOPPING_DISTANCE)
            //{
            //    yield return null;
            //}

            //_isGoingToNewPositionAlready = false;

            //Debug.Log($"{name} - {nameof(goToPosition)} target reached. Interactable {_currentInteractable}");

            //while (_currentInteractable != null) {
            //    // wait for interactable task to finish
            //    yield return null;
            //}
            //Debug.Log($"{name} - {nameof(goToPosition)} - OnFishedOrder");

            //OnFishedOrder?.Invoke(this);
            //_currentRuns = null;

            //Debug.Log($"{name} - {nameof(goToPosition)} coroutine end");

        }

        private bool isPlayerCharacterOutOfRange()
        {
            return false;
            //return Vector3.Distance(transform.localPosition, _newPosition) > minDistanceToStartFollowTheCharacterPlayer;
        }

        private void stop()
        {
            //setCurrentTargetToLocalPosition();
            //localNavMeshAgent.SetDestination(transform.localPosition);
        }

        private void setCurrentTargetToLocalPosition()
        {
            //updateTarget(transform.localPosition);
        }

        private void updateTarget(Vector3 newPosition)
        {
            //_newPosition = newPosition;
            //localNavMeshAgent.SetDestination(_newPosition);
        }

    }
}