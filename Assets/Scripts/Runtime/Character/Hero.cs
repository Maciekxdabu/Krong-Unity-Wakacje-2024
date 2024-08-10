using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        private const float MAX_INTERACTION_DISTANCE_SQUARED = 3 * 3;
        private const int MAX_MINIONS = 10;

        /// <summary> Scriptable Object with order params </summary>
        public OrderData SendOrderData;

        [SerializeField] private ThirdPersonController _controller;
        [SerializeField] private NavMeshObstacle _navMeshObstacle;
        [SerializeField] private Transform _frontTransform;

        [SerializeField] private List<Minion> _minions;
        private List<Minion> _minionsThatAreExecutingAnOrder = new List<Minion>();
        private List<Minion> _minionsThatAreNotExecutingAnOrder = new List<Minion>();
        
        private Spawner _currentSpawner;

        public event Action<Vector3> OnJumpEnd
        {
            add { _controller.OnJumpEnd += value; }
            remove { _controller.OnJumpEnd -= value; }
        }

        public event Action<Vector3> OnMove
        {
            add { _controller.OnMove += value; }
            remove { _controller.OnMove -= value; }
        }

        private void Awake()
        {
            initializeMinionsOnAwake();

            //enableNavMeshObstacle();
            //localThirdPersonController.OnStop += enableNavMeshObstacle;
            //localThirdPersonController.OnStartMove += disableNavMeshObstacle;
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
            // duplicate as minions will be removed during foreach
            var busyMinions = _minionsThatAreExecutingAnOrder.ToList();
            foreach (var minion in busyMinions){
                minion.InterruptCurrentOrder();
            }
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

        #region Minions
        // TODO: remove region add class for managing minions set


        private void initializeMinionsOnAwake()
        {
            foreach (var mininon in _minions)
            {
                addMinion(mininon, alreadyInMinions:true);
            }
        }

        public void addMinion(Minion m, bool alreadyInMinions = false)
        {
            m.Init(this);
            m.OnOrderFinished += minionOrderFinished;

            if (!alreadyInMinions){
                _minions.Add(m);
            }
            _minionsThatAreNotExecutingAnOrder.Add(m);
        }

        public bool canGetAnotherMinion(){
            Debug.Log($"Minions count {_minions.Count}");
            return _minions.Count < MAX_MINIONS;
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
            var _randomIndex = UnityEngine.Random.Range(0, _minionsThatAreNotExecutingAnOrder.Count);
            return _minionsThatAreNotExecutingAnOrder[_randomIndex];
        }

        private void minionOrderFinished(Minion minion)
        {
            Assert.IsFalse(_minionsThatAreNotExecutingAnOrder.Contains(minion));
            Assert.IsTrue(_minionsThatAreExecutingAnOrder.Contains(minion));

            _minionsThatAreNotExecutingAnOrder.Add(minion);
            _minionsThatAreExecutingAnOrder.Remove(minion);
        }

        internal void MinionDied(Minion minion)
        {
            _minions.Remove(minion);
            _minionsThatAreExecutingAnOrder.Remove(minion);
            _minionsThatAreNotExecutingAnOrder.Remove(minion);
        }

        #endregion Minions

        internal void Died()
        {
            _controller.enabled = false;
        }

        internal void Respawn(Vector3 position)
        {
            if (!_controller.enabled)
            {
                _controller.enabled = true;
            }
            foreach (Minion minion in _minions)
            {
                minion.PlayerRespawnedAt(position);
            }
            
        }

        public Vector3 CalculateGoOrderDestination()
        {
            var MAX_DISTANCE = SendOrderData.MaxDistance;
            const float MAX_NAVMESH_DISTANCE = 100f;

            var ray = new Ray(_frontTransform.position, _frontTransform.forward);
            var layerMask = Physics.DefaultRaycastLayers;
            bool wallDetected = Physics.Raycast(
                    ray,
                    out var wallHit,
                    MAX_DISTANCE,
                    layerMask,
                    QueryTriggerInteraction.Ignore
                );

            if (wallDetected)
            {
                NavMesh.SamplePosition(
                    wallHit.point,
                    out var navMeshHit,
                    MAX_NAVMESH_DISTANCE,
                    NavMesh.AllAreas
                    );

                return navMeshHit.position;
            }

            return _frontTransform.position + (_frontTransform.forward * MAX_DISTANCE);
        }


        //private void enableNavMeshObstacle()
        //{
        //    navMeshObstacle.enabled = true;
        //}

        //private void disableNavMeshObstacle()
        //{
        //    navMeshObstacle.enabled = false;
        //}

    }
}