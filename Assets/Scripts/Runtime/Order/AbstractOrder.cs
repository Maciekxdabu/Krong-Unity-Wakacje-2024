using Assets.Scripts.Runtime.Character;
using UnityEngine;

namespace Assets.Scripts.Runtime.Order
{
    public abstract class AbstractOrder
    {
        protected Minion _minion;
        protected Transform _transformOfMinion;

        public virtual void Execute()
        {

        }
    }
}