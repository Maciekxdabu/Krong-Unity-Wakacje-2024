using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        [SerializeField] private List<Minion> minions;
        [SerializeField] private ThirdPersonController localThirdPersonController;
        public OrderData sendOrderData;
        [SerializeField] private UnityEngine.AI.NavMeshObstacle navMeshObstacle;
        [SerializeField] private Transform frontTransform;

        private const float MAX_INTERACTION_DISTANCE_SQUARED = 3 * 3;
        private const int MAX_MINIONS = 10;

        [SerializeField]  private List<Minion> _minionsThatAreExecutingAnOrder;
        [SerializeField]  private List<Minion> _minionsThatAreNotExecutingAnOrder;
        private Spawner _currentSpawner;

        public Transform GetFrontTransform { get { return frontTransform; } }

        private void Awake()
        {
            initializeMinionsOnAwake();

            localThirdPersonController.OnStop += enableNavMeshObstacle;
            localThirdPersonController.OnStartMove += disableNavMeshObstacle;
        }


        public void OnSendOrder()
        {
            if (!hasFreeMinion()) { return; }

            var minion = getRandomFreeMinion();
            minion.SendForward();
            markAsWorking(minion);

        }


        public void OnToMeAllOrder()
        {
            
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

        private void initializeMinionsOnAwake()
        {
            foreach (var mininon in minions)
            {
                mininon.Init(this, localThirdPersonController);
                mininon.OnFishedOrder += minionOrderFinished;
            }

            _minionsThatAreExecutingAnOrder = new List<Minion>();
            _minionsThatAreNotExecutingAnOrder = new List<Minion>(minions);
        }

        internal void addMinion(Minion m)
        {
            m.Init(this, localThirdPersonController);
            m.OnFishedOrder += minionOrderFinished;

            minions.Add(m);
            _minionsThatAreNotExecutingAnOrder.Add(m);
        }

        public bool canGetAnotherMinion(){
            Debug.Log($"Minions count {minions.Count}");
            return minions.Count < MAX_MINIONS;
        }

        private void markAsWorking(Minion minion)
        {
            Assert.IsTrue(_minionsThatAreNotExecutingAnOrder.Contains(minion));
            Assert.IsFalse(_minionsThatAreExecutingAnOrder.Contains(minion));

            _minionsThatAreNotExecutingAnOrder.Remove(minion);
            _minionsThatAreExecutingAnOrder.Add(minion);
        }

        private bool hasFreeMinion()
        {
            return _minionsThatAreNotExecutingAnOrder.Any();
        }

        private Minion getRandomFreeMinion()
        {
            var _randomIndex = Random.Range(0, _minionsThatAreNotExecutingAnOrder.Count);
            return _minionsThatAreNotExecutingAnOrder[_randomIndex];
        }

        private void minionOrderFinished(Minion minion)
        {
            Assert.IsFalse(_minionsThatAreNotExecutingAnOrder.Contains(minion));
            Assert.IsTrue(_minionsThatAreExecutingAnOrder.Contains(minion));

            _minionsThatAreNotExecutingAnOrder.Add(minion);
            _minionsThatAreExecutingAnOrder.Remove(minion);
        }


        private void enableNavMeshObstacle()
        {
            navMeshObstacle.enabled = true;
        }

        private void disableNavMeshObstacle()
        {
            navMeshObstacle.enabled = false;
        }

    }
}