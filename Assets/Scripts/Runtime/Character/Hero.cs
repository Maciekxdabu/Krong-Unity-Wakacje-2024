using Assets.Scripts.Runtime.Order;
using Assets.Scripts.Runtime.UI;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        private const float MAX_INTERACTION_DISTANCE_SQUARED = 3 * 3;
        private const int MAX_MINIONS = 10;

        /// <summary> Scriptable Object with order params </summary>
        public OrderData SendOrderData;

        [SerializeField] private ThirdPersonController _controller;
        [SerializeField] private Transform _frontTransform;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private Collider _weaponCollider;

        [SerializeField] private Minion.MinionType controlledType = Minion.MinionType.none;
        [SerializeField] private List<Minion> _minions;
        [SerializeField] private Vector3 _respawnPosition;

        private PlayerHealth _health;
        [SerializeField] private List<ItemPickCounter> _itemPickCounter;
        private List<Minion> _minionsThatAreExecutingAnOrder = new List<Minion>();
        private List<Minion> _minionsThatAreNotExecutingAnOrder = new List<Minion>();

        //getters
        public Minion.MinionType ControlledType { get { return controlledType; } }
        public int MinionCount { get { return _minions.Count; } }

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
            _health = GetComponent<PlayerHealth>();
            initializeItemPickCounter();
        }

        private void initializeItemPickCounter()
        {
            for (int i = 0; i < _itemPickCounter.Count; i++)
            {
                _itemPickCounter[i].Initialize();
            }
        }

        private void Start()
        {
            HUD.Instance.RefreshHUD(this);
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                float damage = UnityEngine.Random.Range(_damageMin, _damageMax);
                enemy.TakeDamage(damage);
            }
            else if (other.TryGetComponent(out BonusItem bonusItem))
            {
                string bonusItemID = bonusItem.GetId.ToString();
                for (int i = 0; i < _itemPickCounter.Count; i++)
                {
                    if (bonusItemID == _itemPickCounter[i].GetStringID)
                    {
                        _itemPickCounter[i].Add();
                        HUD.Instance.RefreshCustomHUD(_itemPickCounter[i]);
                        break;
                    }
                }

                bonusItem.Delete();
            }
        }

        public void OnSendOrder()
        {
            if (!hasFreeMinion()) { return; }

            var minion = getRandomFreeMinion();
            if (minion == null) { return; }

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

        public void OnSlashAttack()
        {
            if (_health.GetIsAlive())
            {
                _localAnimator.SetBool("SlashAttack", true);
                disableThirdPersonController();
                starterAssetsInputs.StopCharacterMove();
            }
        }

        private void OnChooseMinion(InputValue val)
        {
            controlledType = (Minion.MinionType)val.Get<float>();

            HUD.Instance.RefreshHUD(this);
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
                HUD.Instance.RefreshHUD(this);
            }
            _minionsThatAreNotExecutingAnOrder.Add(m);
            m.destination = transform.position;
            m.destination = transform.position;
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
            List<Minion> availableMinions = _minionsThatAreNotExecutingAnOrder.FindAll(x => x.Type == controlledType || controlledType == Minion.MinionType.none);
            if (availableMinions.Count == 0)//check if there is any minion of the given controlled type
                return null;

            //get random available minion
            var _randomIndex = UnityEngine.Random.Range(0, availableMinions.Count);
            return availableMinions[_randomIndex];
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
            HUD.Instance.RefreshHUD(this);
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

        internal void EnableThirdPersonController()
        {
            starterAssetsInputs.EnableInputs();
        }

        internal void EnableColliderOfWeapon()
        {
            _weaponCollider.enabled = true;
        }

        internal void DisableColliderOfWeapon()
        {
            _weaponCollider.enabled = false;
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

        private void disableThirdPersonController()
        {
            starterAssetsInputs.DisableInputs();
        }

        protected override void OnDeath()
        {
            HUD.Instance.RefreshHUD(this);

            if (!isAlive)
            {
                Died();
            }
        }

        protected override void Respawning()
        {
            transform.position = _respawnPosition;
            Physics.SyncTransforms();
            Respawn(_respawnPosition);
        }

        public void OnRespawn(InputValue inputValue)
        {
            TakeHealing(_maxHp);
            Respawning();
        }

        [ContextMenu("Kill player")]
        private void KillPlayer()
        {
            TakeDamage(_maxHp);
        }

        [ContextMenu("Respawn player")]
        private void RespawnPlayer()
        {
            TakeHealing(_maxHp);
            Respawning();
        }

    }
}