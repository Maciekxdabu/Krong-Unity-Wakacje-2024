using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Timer
    {
        [SerializeField] private GameObject _parentTimer;
        [SerializeField] private TMPro.TMP_Text _timer;
        private float _currentTimer;

        internal System.Action OnEnd;

        internal void InitializeCurrentTimer(Stages stage)
        {
            _currentTimer = stage.GetStartingTime;
        }

        internal void Run()
        {
            showTimer();
            _timer.StartCoroutine(runTimerCorutine());
        }

        private void showTimer()
        {
            _parentTimer.SetActive(true);
        }

        private void hideTimer()
        {
            _parentTimer.SetActive(false);
        }

        private IEnumerator runTimerCorutine()
        {
            while (_currentTimer >= 0)
            {
                _currentTimer -= Time.deltaTime;
                _timer.text = _currentTimer.ToString("f2");

                yield return null;
            }

            hideTimer();
            OnEnd?.Invoke();
        }
    }
}