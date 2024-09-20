using UnityEngine;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.Runtime.Character
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
    }

    public abstract class Creature : MonoBehaviour, IDamageable
    {
        [SerializeField] protected float speed;
        [SerializeField] protected Animator _localAnimator;
        [SerializeField] protected float _damageMin;
        [SerializeField] protected float _damageMax;
        [SerializeField] protected float _maxHp;

        [SerializeField] protected ParticleSystem _hitParticle;

        protected float _hp;

        //creature health variables and methods
        protected virtual bool isAlive => _hp > 0;
        public UnityEvent onHealthChange = new UnityEvent();

        internal Action OnDeathEvent;


        public bool GetIsAlive() { return isAlive; }

        public float HealthPoints => _hp;

        public float MaxHealthPoints
        {
            get { return _maxHp; }
        }

        public float GetDamageValue()
        {
            return UnityEngine.Random.Range(_damageMin, _damageMax);
        }

        public virtual void Awake()
        {
            InitHp();
        }

        protected void InitHp()
        {
            _hp = _maxHp;
        }

        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public virtual void TakeDamage(float value)
        {
            var prevHp = HealthPoints;
            _hp -= value;
            _hp = Mathf.Clamp(_hp, 0, _maxHp);
            Debug.Log($"{name}: DAMAGE {value}; hp change {prevHp} -> {_hp}", this);

            if (prevHp != _hp)
            {
                _hitParticle?.Play();
                onHealthChange.Invoke();
                if (_hp <= 0)
                {
                    OnDeathEvent?.Invoke();
                    OnDeath();
                }
            }
        }

        public virtual void TakeHealing(float value)
        {
            TakeDamage(-value);
        }
    }
}
