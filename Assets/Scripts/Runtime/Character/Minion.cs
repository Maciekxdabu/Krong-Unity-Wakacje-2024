using Assets.Scripts.Runtime.Order;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class Minion : Creature
    {
        [SerializeField] private UnityEngine.AI.NavMeshAgent localNavMeshAgent;
        [SerializeField] private float minDistanceToStartFollowTheCharacterPlayer;

        public System.Action<Minion> OnFishedOrder;

        private Vector3 m_newPosition;
        private bool _isGoingAlready;
        private const float STOPPING_DISTANCE = 0.5f;

        private void Awake()
        {
            m_newPosition = new Vector3();
            localNavMeshAgent.speed = speed;
        }

        public void FollowHero(Vector3 newPosition)
        {
            updateTarget(newPosition);

            if (isPlayerCharacterOutOfRange())
            {
                localNavMeshAgent.SetDestination(m_newPosition);
                if (!_isGoingAlready)
                {
                    StartCoroutine(go());
                }
            }
        }

        public void GiveOrder(AbstractOrder newOrder)
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
            localNavMeshAgent.SetDestination(m_newPosition);
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
            return Vector3.Distance(transform.localPosition, m_newPosition) > minDistanceToStartFollowTheCharacterPlayer;
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
            m_newPosition = newPosition;
        }
    }
}