using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class WaveController
    {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private ScriptableObjects.Stages[] _stages;
        [SerializeField] private Transform _finalPoint;
        [SerializeField] private Timer _timer;

        private Enemy[] _currentlySpawned;
        private Stages[] m_stages;
        private int _numberOfStage;
        private int _amountOfEnemiesAtStage;

        internal void Initialize()
        {
            initializeStages();
            initializeCounter();
            initializeAmountOfEnemiesAtStage();
            initializeTimer();
        }

        internal void RunStage()
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
            m_stages = new Stages[_stages.Length];

            for (int i = 0; i < m_stages.Length; i++)
            {
                m_stages[i].Content = _stages[i].GetContent;
                m_stages[i].eventTypeCaller = _stages[i].GetEventTypeCaller;
                m_stages[i].GetStartingTime = _stages[i].GetStartingTime;
            }
        }

        private void initializeCounter()
        {
            _numberOfStage = 0;
        }

        private void initializeCurrentTimer()
        {
            _timer.InitializeCurrentTimer(m_stages[_numberOfStage]);
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
            _currentlySpawned = new Enemy[m_stages[_numberOfStage].Content.Length];
            for (int i = 0; i < _stages[_numberOfStage].GetContent.Length; i++)
            {
                _currentlySpawned[i] = Object.Instantiate(
                    _stages[_numberOfStage].GetContent[i],
                    Character.NavMeshUtility.SampledPosition(_spawnPoints[0].transform.localPosition),
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
                _currentlySpawned[i].UpdateTarget(_finalPoint.localPosition);
            }
        }
    }
}