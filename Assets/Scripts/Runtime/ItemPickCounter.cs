
namespace Assets.Scripts.Runtime
{
    [System.Serializable]
    public class ItemPickCounter
    {
        public BonusItemType _id;
        public int _amount;

        public string GetID { get { return _id.ToString(); } }
        public string GetCurrentAmountAsString { get { return _amount.ToString(); } }

        public void Add()
        {
            _amount++;
        }
    }
}