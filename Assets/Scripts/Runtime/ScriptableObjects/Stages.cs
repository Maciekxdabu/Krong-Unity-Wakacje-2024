using UnityEngine;

namespace Assets.Scripts.Runtime.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Stage", menuName = "Stages/new stage", order = 1)]
    public class Stages : ScriptableObject
    {
        [SerializeField] private Enemy[] enemies;

        public Enemy[] GetContent
        {
            get { return enemies; }
        }
    }
}