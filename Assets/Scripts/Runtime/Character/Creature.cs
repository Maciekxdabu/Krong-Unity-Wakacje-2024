using UnityEngine;

namespace Assets.Scripts.Runtime.Character
{
    public abstract class Creature : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected Animator _localAnimator;
        [SerializeField] protected float _damageMin;
        [SerializeField] protected float _damageMax;
        [SerializeField] protected float _hp;
    }
}