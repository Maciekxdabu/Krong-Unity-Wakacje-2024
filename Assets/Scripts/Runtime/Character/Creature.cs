using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Runtime.Character
{
    public abstract class Creature : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected Animator _localAnimator;
        [SerializeField] protected float _damageMin;
        [SerializeField] protected float _damageMax;
        [SerializeField] protected float _maxHp;
        protected float _hp;

        //creature health variables and methods
        protected virtual Boolean isAlive => _hp > 0;
        public UnityEvent onHealthChange = new UnityEvent();

        public Boolean GetIsAlive() { return isAlive; }

        public float HealthPoints
        {
            get { return _hp; }
            set
            {
                if (value != _hp)
                {
                    _hp = value;
                    onHealthChange.Invoke();
                }
            }
        }
        public float MaxHealthPoints
        {
            get { return _maxHp; }
        }
        protected void InitHp()
        {
            HealthPoints = _maxHp;
            onHealthChange.AddListener(OnDeath);
        }
        protected virtual void OnDeath() { }
        protected virtual void Respawning() { }
        public virtual void TakeDamage(float value)
        {
            HealthPoints -= value;
            HealthPoints = Mathf.Clamp(HealthPoints, 0, _maxHp);
            Debug.Log(HealthPoints);
        }

        public virtual void TakeHealing(float value)
        {
            TakeDamage(-value);
        }
    }
}