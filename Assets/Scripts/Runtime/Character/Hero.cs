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

        private PlayerHealth _health;
        [SerializeField] private List<ItemPickCounter> _itemPickCounter;
        private List<Minion> _minionsThatAreExecutingAnOrder = new List<Minion>();
        private List<Minion> _minionsThatAreNotExecutingAnOrder = new List<Minion>();

        //getters
        public MinionType ControlledType { get { return controlledType; } }
        public int MinionCount { get { return _minions.Count; } }

        private MinionSpawner _currentSpawner;

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
                        _itemPickCounter[i].Add(bonusItem.Amount);
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
            foreach (var minion in busyMinions)
            {
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
            controlledType = (MinionType)val.Get<float>();

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

        private void disableThirdPersonController()
        {
            starterAssetsInputs.DisableInputs();
        }

        internal List<Minion> GetMinions()
        {
            return new List<Minion>(_minions);
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