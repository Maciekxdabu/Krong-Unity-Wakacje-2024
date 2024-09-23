using Assets.Scripts.Runtime.UI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class Timer
    {
        private float _current;
        private MonoBehaviour _hud;

        internal System.Action OnStart;
        internal System.Action OnEnd;

        internal void InitializeCurrent(Wave wave)
        {
            _hud = HUD.Instance;
            _current = wave.GetStartingTime;
        }

        internal void Run()
        {
            OnStart?.Invoke();
            show();
            _hud.StartCoroutine(runCorutine());
        }

        private void show()
        {
            HUD.Instance.ShowStartingTimeToWave();
        }

        private void turnOff()
        {
            HUD.Instance.TurnOffStartingTimeToWave();
        }

        private IEnumerator runCorutine()
        {
            while (_current >= 0)
            {
                _current -= Time.deltaTime;
                HUD.Instance.UpdateStartingTimeToWave(_current);

                yield return null;
            }

            OnEnd?.Invoke();
            turnOff();
        }
    }
}