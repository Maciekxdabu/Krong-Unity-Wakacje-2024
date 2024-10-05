using Assets.Scripts.Runtime.UI;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class Timer
    {
        private float _current;

        internal System.Action OnStart;
        internal System.Action OnEnd;

        internal void InitializeCurrent(Wave wave)
        {
            _current = wave.GetStartingTime;
        }

        internal void Run()
        {
            OnStart?.Invoke();
            show();
            HUD.Instance.StartCoroutine(runCorutine());
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
            HUD _hud = HUD.Instance;
            while (_current >= 0)
            {
                _current -= Time.deltaTime;
                _hud.UpdateStartingTimeToWave(_current);

                yield return null;
            }

            OnEnd?.Invoke();
            turnOff();
        }
    }
}