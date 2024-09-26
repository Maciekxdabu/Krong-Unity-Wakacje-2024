using Assets.Scripts.Runtime.Order;
using Assets.Scripts.Runtime.ScriptableObjects;
using Assets.Scripts.Runtime.UI;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Runtime.Character
{
    public class Hero : Creature
    {
        private const float MAX_INTERACTION_DISTANCE_SQUARED = 3 * 3;
        private const int MAX_MINIONS = 10;

        [SerializeField] private ThirdPersonController _controller;
        [SerializeField] private Transform _frontTransform;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private Collider _weaponCollider;

        [SerializeField] private MinionType controlledType = MinionType.Any;
        [SerializeField] private List<Minion> _minions;
        [SerializeField] private Vector3 _respawnPosition;


        [SerializeField] private List<ItemPickCounter> _itemPickCounter;
        private List<Minion> _minionsThatAreExecutingAnOrder = new List<Minion>();
        private List<Minion> _minionsThatAreNotExecutingAnOrder = new List<Minion>();

        //getters
        public MinionType ControlledType { get { return controlledType; } }
        public int MinionCount { get { return _minions.Count; } }

        private MinionSpawner _currentSpawner;

        public int GetGoldAmount()
        {
            return _itemPickCounter.Find(i => i.ItemType == BonusItemType.BonusGold)?.Amount ?? 0;
        }

        public bool TryPayGoldAmount(int cost)
        {
            var result = _itemPickCounter.Find(i => i.ItemType == BonusItemType.BonusGold)?.TryPaying(cost) ?? false;

            return result;
        }

        public bool TryUseKey()
        {
            return _itemPickCounter.Find(i => i.ItemType == BonusItemType.BonusKey)?.TryPaying(1) ?? false;
        }

        public Transform GetFrontTransform()
        {
            return _frontTransform;
        }

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

        public override void Awake()
        {
            base.Awake();
            initializeMinionsOnAwake();
            initializeItemPickCounter();
            _respawnPosition = transform.position;
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
            GameManager.Instance.Hero = this; // make sure to init GameManager
            HUD.Instance.RefreshHUD(this);
            onHealthChange.AddListener(() => { HUD.Instance.RefreshHUD(this); });
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
                enemy.TakeDamage(GetDamageValue());
            }
            else if (other.TryGetComponent(out BonusItem bonusItem))
            {
                var itemCounter = _itemPickCounter.Find(i => i.ItemType == bonusItem.GetId);
                if (itemCounter != null) {
                    itemCounter.Add(bonusItem.Amount);
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
            foreach (var minion in busyMinions)
            {
                minion.InterruptCurrentOrder(true);
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
            if (GetIsAlive())
            {
                _localAnimator.SetBool("SlashAttack", true);
                disableThirdPersonController();
                starterAssetsInputs.StopCharacterMove();
            }
        }

        private void OnChooseMinion(InputValue val)
        {
            controlledType = (MinionType)val.Get<float>();
            if (controlledType == MinionType.Vampire) { controlledType = MinionType.Any; }

            HUD.Instance.RefreshHUD(this);
        }

        private MinionSpawner getClosestSpawner()
        {
            // FIXME: inefficient
            var spawners = FindObjectsByType<MinionSpawner>(FindObjectsSortMode.None);
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

        private float distanceToSpawnerSq(MinionSpawner s)
        {
            return (s.transform.position - transform.position).sqrMagnitude;
        }

        #region Minions
        // TODO: remove region add class for managing minions set


        private void initializeMinionsOnAwake()
        {
            foreach (var mininon in _minions)
            {
                addMinion(mininon, alreadyInMinions: true);
            }
        }

        public void addMinion(Minion m, bool alreadyInMinions = false)
        {
            m.Init(this);
            m.OnOrderFinished += minionOrderFinished;

            if (!alreadyInMinions)
            {
                _minions.Add(m);
                HUD.Instance.RefreshHUD(this);
            }
            _minionsThatAreNotExecutingAnOrder.Add(m);
            m.destination = transform.position;
            m.destination = transform.position;
        }

        public bool canGetAnotherMinion()
        {
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
            List<Minion> availableMinions = _minionsThatAreNotExecutingAnOrder.FindAll(x => x.Type == controlledType || controlledType == MinionType.Any);
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

        internal void MinionStartedFighting(Minion minion)
        {
            markAsWorking(minion);
        }

        internal void MinionDied(Minion minion)
        {
            _minions.Remove(minion);
            HUD.Instance.RefreshHUD(this);
            _minionsThatAreExecutingAnOrder.Remove(minion);
            _minionsThatAreNotExecutingAnOrder.Remove(minion);
        }

        #endregion Minions


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

        private void disableThirdPersonController()
        {
            starterAssetsInputs.DisableInputs();
        }

        internal List<Minion> GetMinions()
        {
            return new List<Minion>(_minions);
        }

        protected override void OnDeath()
        {
            _controller.enabled = false;
        }

        protected void Respawning()
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


        public void OnGoBackToMenu(InputValue _)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public struct NavMeshUtility
    {
        private const int MAX_NAVMESH_DISTANCE = 50;
        private static NavMeshHit navMeshHit;

        internal static Vector3 SampledPosition(Vector3 point)
        {
            NavMesh.SamplePosition(
                point,
                out navMeshHit,
                MAX_NAVMESH_DISTANCE,
                NavMesh.AllAreas
                );

            return navMeshHit.position;
        }
    }
}