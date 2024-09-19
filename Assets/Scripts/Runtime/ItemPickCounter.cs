

namespace Assets.Scripts.Runtime
{
    [System.Serializable]
    public class ItemPickCounter
    {
        [UnityEngine.SerializeField] private BonusItemType _id;
        private int _amount;

        public string GetStringID { get { return _id.ToString(); } }
        public string GetCurrentAmountAsString { get { return _amount.ToString(); } }

        public void Initialize()
        {
            _amount = 0;
        }

        public void Add(int addAmount = 1)
        {
             _amount += addAmount;
        }
    }
}