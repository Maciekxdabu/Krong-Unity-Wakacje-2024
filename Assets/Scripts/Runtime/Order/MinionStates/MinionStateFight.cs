using Assets.Scripts.Runtime.Character;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Runtime.Order.MinionStates
{
    class MinionStateFight : IMinionState
    {
        private bool _stateActive;

        private readonly Hero _player;
        private readonly Minion _minion;

        private Enemy _currentEnemy;

        private const float STOPPING_DISTANCE = 1.0f;


        public MinionStateFight(
                Minion minion,
                Hero player)
        {
            _minion = minion;
            _player = player;
        }

        public string GetDebugStateString()
        {
            return "Fighting";
        }

        public void StateEnter()
        {
            _stateActive = true;
            if (_currentEnemy != null)
            {
                _minion.destination = _currentEnemy.transform.position;
            }
        }

        public void Update()
        {
            Assert.IsTrue(_stateActive, "inactive state updated");
            if (_currentEnemy != null)
            {
                _minion.destination = _currentEnemy.transform.position;
                var enemyInRange = _minion.remainingDistance < STOPPING_DISTANCE;
                _minion.isStopped = enemyInRange;
                if (enemyInRange) {
                    _minion.TryAttacking();
                }

            }
            else
            {
                _minion.InterruptCurrentOrder();
            }
        }

        public void StateEnd()
        {
            _stateActive = false;
            _currentEnemy = null;
        }

        public void MinionDied()
        {
        }

        public bool ShouldFightEnemyInRange(Enemy e)
        {
            if (_currentEnemy == null)
            {
                _currentEnemy = e;
                return true;
            }
            return false;
        }
    }
}
