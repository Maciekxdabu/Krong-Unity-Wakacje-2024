using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class WaveControler : MonoBehaviour
    {
        [SerializeField] private Wave[] _waves;
        [SerializeField] private Timer _timer;

        private List<Enemy> _currentlySpawned;
        [SerializeField] private int _numberOfWave;
        [SerializeField] private int _amountOfEnemiesAtStage;

        private void Awake()
        {
            initializeCounter();
            initializeAmountOfEnemiesAtStage();
            initializeTimer();
        }

        private void Start()
        {
            runWave();
        }

        private void runWave()
        {
            Debug.Log("runWave");

            initializeCurrentTimer();
            runTimer();
        }

        private void initializeTimer()
        {
            Debug.Log("initializeTimer");
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

        private void runTimer()
        {
            _timer.Run();
        }

        private void countNextWave()
        {
            _numberOfWave++;
            Debug.Log("countNextWave "+ _numberOfWave);
        }

        private void spawn()
        {
            spawnContent();
        }

        private void spawnContent()
        {
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
                    spawnedEnemy.OnDeathEvent += decreaseAmountOfEnemiesAtStage;
                    spawnedEnemy.UpdateTarget(_waves[_numberOfWave - 1].Spawns[i].GetFinalPoint);

                    _currentlySpawned.Add(spawnedEnemy);
                }
            }
        }
        private void decreaseAmountOfEnemiesAtStage()
        {
            _amountOfEnemiesAtStage--;

            if (areAllDead() && isNextStageExists())
            {
                countNextWave();
                runWave();
            }
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