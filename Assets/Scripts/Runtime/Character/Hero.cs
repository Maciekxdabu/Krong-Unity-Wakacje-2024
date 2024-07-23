using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        [SerializeField] private List<Minion> minions;
        [SerializeField] private ThirdPersonController localThirdPersonController;
        [SerializeField] private OrderData sendOrderData;
        [SerializeField] private UnityEngine.AI.NavMeshObstacle navMeshObstacle;
        [SerializeField] private Transform frontTransform;

        private SendOrder _sendOrder;
        private List<Minion> m_minionsThatAreNotExecutingAnOrder;

        public Transform GetFrontTransform { get { return frontTransform; } }

        private void Awake()
        {
            _sendOrder = new SendOrder(sendOrderData);
            initializeForMinions();
            m_minionsThatAreNotExecutingAnOrder = new List<Minion>(minions);
            localThirdPersonController.OnStop += enableNavMeshObstacle;
            localThirdPersonController.OnStartMove += disableNavMeshObstacle;
        }

        public void OnSendOrder()
        {
            giveSendOrderToRandomlyMinion();
        }

        private void giveSendOrderToRandomlyMinion()
        {
            bool _areThereAnyMinionWithNoOrder = m_minionsThatAreNotExecutingAnOrder.Count > 0;
            if (_areThereAnyMinionWithNoOrder)
            {
                var _randomMinion = randomMinion();
                giveOrder(_randomMinion);
            }
        }

        private void initializeForMinions()
        {
            for (int i = minions.Count - 1; i >= 0; i--)
            {
                localThirdPersonController.OnJumpEnd += minions[i].FollowHero;
                localThirdPersonController.OnMove += minions[i].FollowHero;
                minions[i].OnFishedOrder += reactivatePassiveFollowHero;
            }
        }

        private void removeMinion(Minion randomMinion)
        {
            m_minionsThatAreNotExecutingAnOrder.Remove(randomMinion);
        }

        private void stopFollowTheCharacter(Minion randomMinion)
        {
            localThirdPersonController.OnMove -= randomMinion.FollowHero;
        }

        private void giveOrder(Minion randomMinion)
        {
            initializeOrder(randomMinion);
            if (_sendOrder.isThisMinFreeSpaceToExecuteTheOrder)
            {
                randomMinion.GiveOrder(_sendOrder);
                stopFollowTheCharacter(randomMinion);
                removeMinion(randomMinion);
            }
        }

        private void initializeOrder(Minion randomMinion)
        {
            _sendOrder.Initialize(randomMinion, this);
        }

        private void enableNavMeshObstacle()
        {
            navMeshObstacle.enabled = true;
        }

        private void disableNavMeshObstacle()
        {
            navMeshObstacle.enabled = false;
        }

        private Minion randomMinion()
        {
            var _randomIndex = Random.Range(0, m_minionsThatAreNotExecutingAnOrder.Count);

            return m_minionsThatAreNotExecutingAnOrder[_randomIndex];
        }

        private void reactivatePassiveFollowHero(Minion minion)
        {
            localThirdPersonController.OnMove += minion.FollowHero;
            sendPostionToMinion();

            if (!m_minionsThatAreNotExecutingAnOrder.Contains(minion))
            {
                returnMinionThatAreNotExecutingAnOrder(minion);
            }
        }

        private void returnMinionThatAreNotExecutingAnOrder(Minion minion)
        {
            m_minionsThatAreNotExecutingAnOrder.Add(minion);
        }

        private void sendPostionToMinion()
        {
            localThirdPersonController.InvokeOnMove();
        }
    }
}