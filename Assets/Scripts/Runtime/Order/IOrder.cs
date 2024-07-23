using Assets.Scripts.Runtime.Character;

namespace Assets.Scripts.Runtime.Order
{
    public interface IOrder
    {
        void Initialize(Minion minion, Hero hero);
        void Execute();
    }
}