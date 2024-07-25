using Assets.Scripts.Runtime.Character;
using UnityEngine;

namespace Assets.Scripts.Runtime.Order
{
    public abstract class AbstractOrder : IOrder
    {
        protected Minion _minion;
        protected Transform _transformOfMinion;

        public virtual void Initialize(Minion minion, Hero hero)
        {

        }

        public virtual void Execute()
        {

        }
    }
}