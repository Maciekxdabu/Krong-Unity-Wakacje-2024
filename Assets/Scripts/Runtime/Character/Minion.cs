using Assets.Scripts.Runtime.Order;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class Minion : Creature
    {
        [SerializeField] private UnityEngine.AI.NavMeshAgent localNavMeshAgent;
        [SerializeField] private float minDistanceToStartFollowTheCharacterPlayer;

        private const float STOPPING_DISTANCE = 0.5f;

        public System.Action<Minion> OnFishedOrder;

        private Vector3 _newPosition;
        private bool _isGoingAlready;

        private void Awake()
        {
            _newPosition = new Vector3();
            localNavMeshAgent.speed = speed;
        }

        public void FollowHero(Vector3 newPosition)
        {
            updateTarget(newPosition);

            if (isPlayerCharacterOutOfRange())
            {
                localNavMeshAgent.SetDestination(_newPosition);
                if (!_isGoingAlready)
                {
                    StartCoroutine(go());
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
            StartCoroutine(executeSendOrder());
        }

        private IEnumerator go()
        {
            _isGoingAlready = true;
            while (isPlayerCharacterOutOfRange())
            {
                yield return null;
            }

            stop();
            _isGoingAlready = false;
        }

        private IEnumerator executeSendOrder()
        {
            localNavMeshAgent.SetDestination(_newPosition);
            _isGoingAlready = true;

            while (localNavMeshAgent.pathPending)
            {
                yield return null;
            }

            while (localNavMeshAgent.remainingDistance > STOPPING_DISTANCE)
            {
                yield return null;
            }

            _isGoingAlready = false;
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