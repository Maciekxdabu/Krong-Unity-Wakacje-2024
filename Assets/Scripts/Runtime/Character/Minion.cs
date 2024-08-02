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

        private static int s_count = 1;

        public System.Action<Minion> OnFishedOrder;

        private Vector3 _newPosition;
        private bool _isGoingToNewPositionAlready;
        private Coroutine _currentRuns;

        private void Awake()
        {
            _newPosition = new Vector3();
            localNavMeshAgent.speed = speed;
            name = "Minion_" + s_count;
            ++s_count;
        }


        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject && collider.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                interactable.StartInteractionWithMinion(this);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject && collider.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                interactable.EndInteractionWithMinion(this);
            }
        }

        public void InterruptCurrentOrder()
        {
            StopCoroutine(_currentRuns);
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

        public void GoToPostion(Vector3 newPosition)
        {
            updateTarget(newPosition);
            _currentRuns = StartCoroutine(goToPosition());
        }

        private IEnumerator goToHero()
        {
            _isGoingToNewPositionAlready = true;
            while (isPlayerCharacterOutOfRange())
            {
                yield return null;
            }

            stop();
            _isGoingToNewPositionAlready = false;
        }

        private IEnumerator goToPosition()
        {
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
            OnFishedOrder?.Invoke(this);
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
        }
    }
}