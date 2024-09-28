using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Runtime.Waves
{
    public class WaveControler : MonoBehaviour
    {
        [SerializeField] private WaveEvent[] _events;

        private void Awake()
        {
            foreach (var item in _events)
            {
                item.Awake();
            }
        }

        private void Start()
        {
            _events.First()?.FirstEventStart();
        }

        private void OnTriggerEnter(Collider other)
        {
            findEvent(other);
        }

        private void findEvent(Collider other)
        {
            //Debug.Log($"{gameObject} vs {other.gameObject}");
            foreach (WaveEvent item in _events)
            {
                if (item.Intersects(other))
                {
                    Debug.Log($"{gameObject} vs {other.gameObject} - WAVE START");
                    item.Start();
                    break;
                }
            }
        }
    }
}