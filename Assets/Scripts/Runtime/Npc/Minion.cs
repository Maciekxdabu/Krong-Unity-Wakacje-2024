using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Npc
{
    public class Minion : Creature
    {
        [SerializeField] private UnityEngine.AI.NavMeshAgent localNavMeshAgent;
        [SerializeField] private float minimumDistanceToStartFollowTheCharacterPlayer;

        private Vector3 m_newPosition;
        private bool _isGoingAlready;

        private void Awake()
        {
            m_newPosition = new Vector3();
            localNavMeshAgent.speed = speed;
        }

        public void FollowTheCharacterPlayer(Vector3 newPosition)
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

        private bool isPlayerCharacterOutOfRange()
        {
            return Vector3.Distance(transform.localPosition, m_newPosition) > minimumDistanceToStartFollowTheCharacterPlayer;
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