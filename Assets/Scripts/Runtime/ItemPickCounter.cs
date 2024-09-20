

using Assets.Scripts.Runtime.UI;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Runtime
{
    [System.Serializable]
    public class ItemPickCounter
    {
        [UnityEngine.SerializeField] private BonusItemType _id;
        private int _amount;

        public BonusItemType ItemType => _id;
        public int Amount => _amount;


        public string GetStringID { get { return _id.ToString(); } }
        public string GetCurrentAmountAsString { get { return _amount.ToString(); } }

        public void Initialize()
        {
            _amount = 0;
        }

        public void Add(int addAmount = 1)
        {
             _amount += addAmount;
            HUD.Instance.RefreshCustomHUD(this);
        }

        public bool TryPaying(int cost )
        {
            if (Amount < cost) {
                return false;
            }
            _amount -= cost;
            HUD.Instance.RefreshCustomHUD(this);
            return true;
        }
    }
}