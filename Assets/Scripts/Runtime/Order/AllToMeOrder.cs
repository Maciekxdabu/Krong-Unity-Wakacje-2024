
namespace Assets.Scripts.Runtime.Order
{
    public class AllToMeOrder : AbstractOrder
    {
        public override void Execute()
        {
            _minion.InterruptCurrentOrder();
        }
    }
}