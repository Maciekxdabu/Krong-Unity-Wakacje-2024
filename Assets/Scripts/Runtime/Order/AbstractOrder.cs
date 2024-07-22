using Assets.Scripts.Runtime.Npc;

namespace Assets.Scripts.Runtime.Order
{
    public abstract class AbstractOrder
    {
        protected Minion _minion;

        public virtual void Execute()
        {

        }
    }
}