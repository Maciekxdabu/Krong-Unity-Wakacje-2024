using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        [SerializeField] private List<Minion> minions;
        [SerializeField] private ThirdPersonController localThirdPersonController;
        [SerializeField] private OrderData sendOrderData;
        [SerializeField] private UnityEngine.AI.NavMeshObstacle navMeshObstacle;
        [SerializeField] private Transform frontTransform;

        private IOrder _sendOrder;
        private List<Minion> _minionsThatAreNotExecutingAnOrder;

        public List<Minion> GetMinions { get { return minions; } }
        public List<Minion> GetNotActiveMinions { get { return _minionsThatAreNotExecutingAnOrder; } }

        public Transform GetFrontTransform { get { return frontTransform; } }

        public ThirdPersonController GetThirdPersonController { get {  return localThirdPersonController; } }

        private void Awake()
        {
            _sendOrder = new SendOrder(sendOrderData);
            initializeForMinions();
            _minionsThatAreNotExecutingAnOrder = new List<Minion>(minions);
            localThirdPersonController.OnStop += enableNavMeshObstacle;
            localThirdPersonController.OnStartMove += disableNavMeshObstacle;
        }

        internal void ReleaseMinionInFavourOfAnOrder(Minion minion)
        {
            stopFollowTheCharacter(minion);
            removeMinionFromTheListThatAreNotExecutingAnOrder(minion);
        }

        public void OnSendOrder()
        {
            giveSendOrderToRandomlyMinion();
        }

        private void giveSendOrderToRandomlyMinion()
        {
            bool _areThereAnyMinionWithNoOrder = _minionsThatAreNotExecutingAnOrder.Count > 0;
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

        private void removeMinionFromTheListThatAreNotExecutingAnOrder(Minion minion)
        {
            _minionsThatAreNotExecutingAnOrder.Remove(minion);
        }

        private void stopFollowTheCharacter(Minion minion)
        {
            localThirdPersonController.OnMove -= minion.FollowHero;
        }

        private void giveOrder(Minion minion)
        {
            initializeOrder(minion);
        }

        private void initializeOrder(Minion minion)
        {
            _sendOrder.Initialize(minion, this);
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
            var _randomIndex = Random.Range(0, _minionsThatAreNotExecutingAnOrder.Count);

            return _minionsThatAreNotExecutingAnOrder[_randomIndex];
        }

        private void reactivatePassiveFollowHero(Minion minion)
        {
            localThirdPersonController.OnMove += minion.FollowHero;
            sendPostionToMinion();

            if (!_minionsThatAreNotExecutingAnOrder.Contains(minion))
            {
                returnMinionThatAreNotExecutingAnOrder(minion);
            }
        }

        private void returnMinionThatAreNotExecutingAnOrder(Minion minion)
        {
            _minionsThatAreNotExecutingAnOrder.Add(minion);
        }

        private void sendPostionToMinion()
        {
            localThirdPersonController.InvokeOnMove();
        }
    }
}