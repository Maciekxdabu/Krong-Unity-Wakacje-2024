using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    [System.Serializable]
    public class Timer
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private TMPro.TMP_Text _value;
        private float _current;

        internal System.Action OnEnd;

        internal void InitializeCurrent(Stage stage)
        {
            _current = stage.GetStartingTime;
        }

        internal void Run()
        {
            show();
            _value.StartCoroutine(runCorutine());
        }

        private void show()
        {
            _parent.SetActive(true);
        }

        private void turnOff()
        {
            _parent.SetActive(false);
        }

        private IEnumerator runCorutine()
        {
            while (_current >= 0)
            {
                _current -= Time.deltaTime;
                _value.text = _current.ToString("f2");

                yield return null;
            }

            turnOff();
            OnEnd?.Invoke();
        }
    }
}