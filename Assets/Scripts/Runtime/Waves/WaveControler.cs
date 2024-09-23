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

        private void OnTriggerEnter(Collider other)
        {
            findEvent(other);
        }

        private void findEvent(Collider other)
        {
            foreach (WaveEvent item in _events)
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