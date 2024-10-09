using Assets.Scripts.Extensions;
using Assets.Scripts.Runtime.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class WaveEvent
    {
        [SerializeField] private Wave[] _waves;
        [SerializeField] private GameObject[] _passages;
        [SerializeField] private Timer _timer;
        [SerializeField] private Collider _trigger;

        private List<Enemy> _currentlySpawned;
        private int _amountOfEnemiesAtStage;
        private int _numberOfWave;

        private Wave CurrentWave => _waves[_numberOfWave - 1];

        internal void Awake()
        {
            initializeAmountOfEnemiesAtStage();
            initializeTimer();
        }

        internal bool Intersects(Collider collider)
        {
            if (collider == null) { return false; }
            if (Physics.GetIgnoreCollision(_trigger, collider)) { return false; }
            return _trigger.bounds.Intersects(collider.bounds);
        }


        internal void FirstEventStart()
        {
            if (_trigger == null) Start();
        }

        internal void Start()
        {
            initializeCounter();
            runWave();
            if (_trigger != null)
            {
                _trigger.gameObject.SetActive(false);
            }
        }

        private void initializeTimer()
        {
            _timer = new Timer();
            _timer.OnStart += blockPassages;
            _timer.OnEnd += spawn;
        }

        private void initializeAmountOfEnemiesAtStage()
        {
            _amountOfEnemiesAtStage = 0;
        }

        private void initializeCounter()
        {
            _numberOfWave = 1;
        }

        private void initializeCurrentTimer()
        {
            _timer.InitializeCurrent(CurrentWave);
        }

        private void blockPassages()
        {
            for (int i = 0; i < _passages.Length; i++)
            {
                _passages[i].SetActive(true);
            }
        }

        private void freePassages()
        {
            for (int i = 0; i < _passages.Length; i++)
            {
                _passages[i].SetActive(false);
            }
        }

        private void runWave()
        {
            Debug.Log("runWave");

            initializeCurrentTimer();
            runTimer();
        }

        private void runTimer()
        {
            _timer.Run();
        }

        private void moveToNextWave()
        {
            _numberOfWave++;
        }

        private void spawn()
        {
            spawnContent();
        }

        private void spawnContent()
        {
            CurrentWave.OnWaveStart?.Invoke();

            Enemy spawnedEnemy;
            _currentlySpawned = new List<Enemy>();
            for (int i = 0; i < CurrentWave.Spawns.Length; i++)
            {
                for (int j = 0; j < CurrentWave.Spawns[i].EnemyAmount; j++)
                {
                    var correct = NavmeshExtensions.TrySnapToNavmesh(CurrentWave.Spawns[i].GetSpawnPoint, out var navmeshDestination);

                    spawnedEnemy = Object.Instantiate(
                        CurrentWave.Spawns[i].GetContent,
                        navmeshDestination,
                        Quaternion.identity);

                    _amountOfEnemiesAtStage++;
                    spawnedEnemy.OnDeathEvent += tryToFinishCurrentWave;
                    spawnedEnemy.OnSpawn(CurrentWave.Spawns[i].GetFinalPoint);

                    _currentlySpawned.Add(spawnedEnemy);
                }
            }
        }
        private void tryToFinishCurrentWave()
        {
            decreaseAmountOfEnemiesAtStage();

            if (areAllDead())
            {
                freePassages();
                CurrentWave.OnWaveEnd?.Invoke();
                if (isNextStageExists())
                {
                    runNextWave();
                }
                else
                {
                    HUD.Instance.ShowFinishedWaveOfEnemies();
                }
            }
        }

        private void decreaseAmountOfEnemiesAtStage()
        {
            _amountOfEnemiesAtStage--;
        }

        private void runNextWave()
        {
            moveToNextWave();
            runWave();
        }

        private bool areAllDead()
        {
            return _amountOfEnemiesAtStage == 0;
        }

        private bool isNextStageExists()
        {
            return _numberOfWave < _waves.Length;
        }
    }
}