using Assets.Scripts.Extensions;
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
        public const int MAX_MINIONS = 10;
        private const float MAX_ORDER_TRACE_DISTANCE = 4.0f + 8.0f;
        [SerializeField] private ThirdPersonController _controller;
        [SerializeField] private Transform _frontTransform;
        [SerializeField] private StarterAssetsInputs starterAssetsInputs;
        [SerializeField] private Collider _weaponCollider;

        [SerializeField] private MinionType controlledType = MinionType.Any;
        [SerializeField] private List<Minion> _minions;
        [SerializeField] private Vector3 _respawnPosition;

        [SerializeField] private LayerMask _sendOrderRaycastMask;
        [SerializeField] private GameObject _sendOrderMarker;


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

        public bool TryUseKey(int amount)
        {
            return _itemPickCounter.Find(i => i.ItemType == BonusItemType.BonusKey)?.TryPaying(amount) ?? false;
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

        private void OnDrawGizmos()
        {
            drawOrderTargetGizmo();
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
            else if (other.TryGetComponent(out Checkpoint _))
            {
                _respawnPosition = gameObject.transform.position;
            }
        }

        private void drawOrderTargetGizmo()
        {
            Gizmos.color = Color.yellow;
            var orderTarget = OrderPointer.Calculate(Camera.current.transform, MAX_ORDER_TRACE_DISTANCE, _sendOrderRaycastMask);
            if (orderTarget.Debug_HitWall){
                Gizmos.DrawWireSphere(orderTarget.Debug_HitWallPosition, 0.5f);
            }
            Gizmos.DrawLine(orderTarget.Debug_Start, orderTarget.Debug_End);
            Gizmos.DrawWireSphere(orderTarget.NavmeshDestination, 0.5f);
        }

        public void OnSendOrder()
        {
            if (!hasFreeMinion()) { return; }

            var minion = getRandomFreeMinion();
            if (minion == null) { return; }

            var destination = OrderPointer.Calculate(Camera.main.transform, MAX_ORDER_TRACE_DISTANCE, _sendOrderRaycastMask);
            if (!destination.Correct) { return; }

            Instantiate(_sendOrderMarker, destination.NavmeshDestination, Quaternion.identity, null);
            minion.SendForward(destination.NavmeshDestination);
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

        private void OnClearChosenMinion(InputValue val)
        {
            controlledType = MinionType.Any;
            HUD.Instance.RefreshHUD(this);

        }

        private void OnMoveChosenMinionSelectionUp(InputValue val)
        {
            if (val.Get<float>() <= 0.1) return;
            var controlledAsInt = (int)controlledType;
            controlledAsInt += 1;
            controlledAsInt %= 4;
            controlledType = (MinionType) controlledAsInt;
            HUD.Instance.RefreshHUD(this);
        }

        private void OnMoveChosenMinionSelectionDown(InputValue val)
        {
            if (val.Get<float>() <= 0.1) return;
            var controlledAsInt = (int)controlledType;
            controlledAsInt -= 1;
            controlledAsInt %= 4;
            controlledType = (MinionType)controlledAsInt;
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
            HUD.Instance.RefreshAvailableMinions(this);
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
            HUD.Instance.RefreshAvailableMinions(this);
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
            HUD.Instance.RefreshAvailableMinions(this);
        }

        internal void MinionStartedFighting(Minion minion)
        {
            markAsWorking(minion);
        }

        internal void MinionDied(Minion minion)
        {
            _minions.Remove(minion);
            _minionsThatAreExecutingAnOrder.Remove(minion);
            _minionsThatAreNotExecutingAnOrder.Remove(minion);
            HUD.Instance.RefreshHUD(this);
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

        public bool IsMinionTypeActive(MinionType minionType)
        {
            if (controlledType == MinionType.Any) return true;
            return controlledType == minionType;
        }
        
        public int GetMinionsCount(MinionType minionType)
        {
            return _minions.Count(m => m.Type == minionType);
        }

        public int GetAvailableMinionsCount()
        {
            return _minionsThatAreNotExecutingAnOrder.Count(x => x.Type == controlledType || controlledType == MinionType.Any);
        }
    }

    public struct OrderPointer
    {
        public Vector3 NavmeshDestination;
        public bool Correct;

        public Vector3 Debug_Start;
        public Vector3 Debug_End;
        public Vector3 Debug_HitWallPosition;
        public bool Debug_HitWall;
        public bool Debug_FoundNavmesh;

        public static OrderPointer Calculate(Transform camera, float maxDistance, LayerMask wallMask) { 
            var result = new OrderPointer();
            result.calculate(camera, maxDistance, wallMask);
            return result;
        }
        
        private void calculate(Transform camera, float maxDistance, LayerMask wallMask)
        {
            Debug_Start = camera.position;
            var forward = camera.forward;
            Debug_End = Debug_Start + forward * maxDistance;

            var result = Debug_End;

            var ray = new Ray(Debug_Start, forward);
            Debug_HitWall = Physics.Raycast(
                ray,
                out var wallHit,
                maxDistance,
                wallMask,
                QueryTriggerInteraction.Ignore
            );
            if (Debug_HitWall)
            {
                Debug_HitWallPosition = wallHit.point;
                result = Debug_HitWallPosition;
            }

            Correct = NavmeshExtensions.TrySnapToNavmesh(result, out NavmeshDestination);
        }
    
    }
}