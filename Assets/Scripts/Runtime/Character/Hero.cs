using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System.Collections.Generic;
using System.Linq;
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

        private const float MAX_INTERACTION_DISTANCE_SQUARED = 3 * 3;
        private const int MAX_MINIONS = 10;

        private IOrder _sendOrder;
        private List<Minion> _minionsThatAreNotExecutingAnOrder;
        private Spawner _currentSpawner;

        public Transform GetFrontTransform { get { return frontTransform; } }

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

        public void OnInteract()
        {
            if (_currentSpawner != null)
            {
                _currentSpawner.Interact(this);
            }
        }

        public void FixedUpdate()
        {
            var closestSpawner = getClosestSpawner();
            if (closestSpawner == _currentSpawner)
            {
                return;
            }
            else
            {
                if (_currentSpawner != null) { _currentSpawner.SetSelected(false); }
                if (closestSpawner != null) { closestSpawner.SetSelected(true); }

                _currentSpawner = closestSpawner;
            }
        }

        private Spawner getClosestSpawner() {
            // FIXME: inefficient
            var spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
            var closestSpawner = spawners
                .OrderBy(distanceToSpawnerSq)
                .FirstOrDefault();
            if (closestSpawner == null)
            {
                return null;
            }
            if (distanceToSpawnerSq(closestSpawner) > MAX_INTERACTION_DISTANCE_SQUARED)
            {
                return null;
            }
            return closestSpawner;
        }

        private float distanceToSpawnerSq(Spawner s)
        {
            return (s.transform.position - transform.position).sqrMagnitude;
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

        internal void addMinion(Minion m)
        {
            minions.Add(m);
            _minionsThatAreNotExecutingAnOrder.Add(m);
            
            localThirdPersonController.OnJumpEnd += m.FollowHero;
            localThirdPersonController.OnMove += m.FollowHero;
            m.OnFishedOrder += reactivatePassiveFollowHero;
        }

        public bool canGetAnotherMinion(){
            Debug.Log($"Minions count {minions.Count}");
            return minions.Count < MAX_MINIONS;
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