using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private Wawe[] _waves;
        [SerializeField] private Timer _timer;

        private Enemy[] _currentlySpawned;
        private Stage[] m_stages;
        private int _numberOfStage;
        private int _amountOfEnemiesAtStage;

        private void Awake()
        {
            initializeStages();
            initializeCounter();
            initializeAmountOfEnemiesAtStage();
            initializeTimer();
        }

        private void Start()
        {
            RunStage();
        }
        private void RunStage()
        {
            initializeCurrentTimer();
            runTimer();
        }

        private void initializeTimer()
        {
            _timer.OnEnd += spawnStage;
            _timer.OnEnd += countNextStage;
        }

        private void initializeAmountOfEnemiesAtStage()
        {
            _amountOfEnemiesAtStage = 0;
        }

        private void initializeStages()
        {
            m_stages = new Stage[_waves.Length];
            for (int i = 0; i < m_stages.Length; i++)
            {
                m_stages[i].Initialize(_waves[i]);
            }
        }

        private void initializeCounter()
        {
            _numberOfStage = 0;
        }

        private void initializeCurrentTimer()
        {
            _timer.InitializeCurrent(m_stages[_numberOfStage]);
        }

        private void countNextStage()
        {
            _numberOfStage++;
        }

        private void spawnStage()
        {
            spawnContent();
            attackToPoint();
        }

        private void spawnContent()
        {
            _currentlySpawned = new Enemy[m_stages[_numberOfStage].GetContent.Length];
            for (int i = 0; i < _currentlySpawned.Length; i++)
            {
                _currentlySpawned[i] = Object.Instantiate(
                    m_stages[_numberOfStage].GetContent[i],
                    Character.NavMeshUtility.SampledPosition(m_stages[i].GetSpawnPoint),
                    Quaternion.identity);

                _amountOfEnemiesAtStage++;
                _currentlySpawned[i].OnDeath += decreaseAmountOfEnemiesAtStage;
            }
        }

        private void decreaseAmountOfEnemiesAtStage()
        {
            _amountOfEnemiesAtStage--;

            if (areAllDead() && isNextStageExists())
            {
                RunStage();
            }
        }

        private bool areAllDead()
        {
            return _amountOfEnemiesAtStage == 0;
        }

        private bool isNextStageExists()
        {
            return _numberOfStage < m_stages.Length;
        }

        private void runTimer()
        {
            _timer.Run();
        }

        private void attackToPoint()
        {
            for (int i = 0; i < _currentlySpawned.Length; i++)
            {
                _currentlySpawned[i].UpdateTarget(m_stages[_numberOfStage].GetFinalPoint);
            }
        }
    }
}