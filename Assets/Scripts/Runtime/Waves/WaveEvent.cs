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

        internal void Awake()
        {
            initializeAmountOfEnemiesAtStage();
            initializeTimer();
        }

        internal bool Intersects(Collider trigger)
        {
            return _trigger.bounds.Intersects(trigger.bounds);
        }

        internal void Start()
        {
            initializeCounter();
            runWave();
            _trigger.gameObject.SetActive(false);
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
            _timer.InitializeCurrent(_waves[_numberOfWave - 1]);
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

        private void countNextWave()
        {
            _numberOfWave++;
        }

        private void spawn()
        {
            spawnContent();
        }

        private void spawnContent()
        {
            _waves[_numberOfWave - 1].OnStart?.Invoke();

            Enemy spawnedEnemy;
            _currentlySpawned = new List<Enemy>();
            for (int i = 0; i < _waves[_numberOfWave - 1].Spawns.Length; i++)
            {
                for (int j = 0; j < _waves[_numberOfWave - 1].Spawns[i].EnemyAmount; j++)
                {
                    spawnedEnemy = Object.Instantiate(
                    _waves[_numberOfWave - 1].Spawns[i].GetContent,
                    Character.NavMeshUtility.SampledPosition(_waves[_numberOfWave - 1].Spawns[i].GetSpawnPoint),
                    Quaternion.identity);

                    _amountOfEnemiesAtStage++;
                    spawnedEnemy.OnDeathEvent += tryToFinishCurrentWave;
                    spawnedEnemy.UpdateTarget(_waves[_numberOfWave - 1].Spawns[i].GetFinalPoint);

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
            _waves[_numberOfWave - 1].OnEnd?.Invoke();
            countNextWave();
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