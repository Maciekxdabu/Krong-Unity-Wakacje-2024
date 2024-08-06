using Assets.Scripts.Runtime.Order;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class Minion : Creature
    {
        [SerializeField] private UnityEngine.AI.NavMeshAgent localNavMeshAgent;
        [SerializeField] private float minDistanceToStartFollowTheCharacterPlayer;

        private const float STOPPING_DISTANCE = 0.5f;

        private static int s_spawned_count = 1;

        public System.Action<Minion> OnFishedOrder;

        private Vector3 _newPosition;
        private bool _isGoingToNewPositionAlready;
        private Coroutine _currentRuns;
        private Interactable _currentInteractable;

        private void Awake()
        {
            _newPosition = new Vector3();
            localNavMeshAgent.speed = speed;
            name = "Minion_" + s_spawned_count;
            ++s_spawned_count;
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

            if (_currentInteractable != null) {
                return; // ignore others when interacting already
                        // TODO: interactable priorities
            }
            if (_currentRuns == null)
            {
                return; // only interact during move order
            }
            interactable.StartInteractionWithMinion(this);
            _currentInteractable = interactable;
            updateTarget(_currentInteractable.transform.position);
            _currentInteractable.TaskDoneCallback.AddListener(InteractableTaskFinished);
        }
        
        private void InteractableLeftArea(Interactable interactable)
        {
            Debug.Log($"{name} - {nameof(InteractableLeftArea)} - {interactable}");

            interactable.EndInteractionWithMinion(this);
            _currentInteractable = null;
            _currentInteractable.TaskDoneCallback.RemoveListener(InteractableTaskFinished);
        }

        private void InteractableTaskFinished(Interactable interactable) {
            Debug.Log($"{name} - {nameof(InteractableTaskFinished)} - {interactable}");

            interactable.EndInteractionWithMinion(this);
            _currentInteractable = null;
        }


        public void InterruptCurrentOrder()
        {
            StopCoroutine(_currentRuns);
            _currentRuns = null;
            Debug.Log($"{name} - {nameof(InterruptCurrentOrder)} - OnFishedOrder");
            OnFishedOrder?.Invoke(this);
        }

        public void FollowHero(Vector3 newPosition)
        {
            updateTarget(newPosition);

            if (isPlayerCharacterOutOfRange())
            {
                localNavMeshAgent.SetDestination(_newPosition);
                if (!_isGoingToNewPositionAlready)
                {
                    StartCoroutine(goToHero());
                }
            }
        }

        public void GiveOrder(IOrder newOrder)
        {
            newOrder.Execute();
        }

        public Coroutine GoToPostion(Vector3 newPosition)
        {
            Debug.Log($"{name} - go to position - {newPosition}");
            updateTarget(newPosition);
            _currentRuns = StartCoroutine(goToPosition());
            return _currentRuns;
        }

        private IEnumerator goToHero()
        {
            Debug.Log($"{name} - {nameof(goToHero)} coroutine start");
            _isGoingToNewPositionAlready = true;
            while (isPlayerCharacterOutOfRange())
            {
                yield return null;
            }

            stop();
            _isGoingToNewPositionAlready = false;
            Debug.Log($"{name} - {nameof(goToHero)} coroutine end");
        }

        private IEnumerator goToPosition()
        {
            Debug.Log($"{name} - {nameof(goToPosition)} coroutine start");

            localNavMeshAgent.SetDestination(_newPosition);
            _isGoingToNewPositionAlready = true;

            while (localNavMeshAgent.pathPending)
            {
                yield return null;
            }

            while (localNavMeshAgent.remainingDistance > STOPPING_DISTANCE)
            {
                yield return null;
            }

            _isGoingToNewPositionAlready = false;

            Debug.Log($"{name} - {nameof(goToPosition)} target reached. Interactable {_currentInteractable}");

            while (_currentInteractable != null) {
                // wait for interactable task to finish
                yield return null;
            }
            Debug.Log($"{name} - {nameof(goToPosition)} - OnFishedOrder");

            OnFishedOrder?.Invoke(this);
            _currentRuns = null;

            Debug.Log($"{name} - {nameof(goToPosition)} coroutine end");

        }

        private bool isPlayerCharacterOutOfRange()
        {
            return Vector3.Distance(transform.localPosition, _newPosition) > minDistanceToStartFollowTheCharacterPlayer;
        }

        private void stop()
        {
            setCurrentTargetToLocalPosition();
            localNavMeshAgent.SetDestination(transform.localPosition);
        }

        private void setCurrentTargetToLocalPosition()
        {
            updateTarget(transform.localPosition);
        }

        private void updateTarget(Vector3 newPosition)
        {
            _newPosition = newPosition;
            localNavMeshAgent.SetDestination(_newPosition);
        }
    }
}