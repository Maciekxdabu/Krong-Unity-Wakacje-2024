using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class WaveControler : MonoBehaviour
    {
        [SerializeField] private WaveEvent[] _events;

        private int _currentEvent;

        private void Awake()
        {
            foreach (var item in _events)
            {
                item.Awake();
            }

            _currentEvent = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            foreach (var item in _events)
            {
                if (item.Intersects(other))
                {
                    item.Start();
                    break;
                }
            }
        }
    }
}