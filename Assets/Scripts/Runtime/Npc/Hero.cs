using Assets.Scripts.Runtime.Order;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Npc
{
    public class Hero : Creature
    {
        [SerializeField] private List<Minion> minions;
        [SerializeField] private ThirdPersonController localThirdPersonController;
        [SerializeField] private OrderData sendOrderData;

        private Send _sendOrder;
        [SerializeField] private List<Minion> m_minionsThatAreNotExecutingAnOrder;

        private void Awake()
        {
            _sendOrder = new Send(sendOrderData);
            InitializeForMinions();
            m_minionsThatAreNotExecutingAnOrder = new List<Minion>(minions);
        }

        public void OnSendOrder()
        {
            GiveSendOrderToRandomlyMinion();
        }

        public void GiveSendOrderToRandomlyMinion()
        {
            if (m_minionsThatAreNotExecutingAnOrder.Count > 0)
            {
                var _randomMinion = randomMinion();
                _sendOrder.Initialize(_randomMinion, this);//zainicjuj rozkaz
                _randomMinion.GiveOrder(_sendOrder);//przezekaz rozkaz
                localThirdPersonController.OnMove -= _randomMinion.FollowTheCharacterPlayer;//przestan chodzic za postacia gracza
                m_minionsThatAreNotExecutingAnOrder.Remove(_randomMinion);//zapamietaj moba ktory wykonuje rozkaz
            }
        }

        private Minion randomMinion()
        {
            var _randomIndex = Random.Range(0, m_minionsThatAreNotExecutingAnOrder.Count);
            
            return m_minionsThatAreNotExecutingAnOrder[_randomIndex];
        }

        private void InitializeForMinions()
        {
            for (int i = minions.Count - 1; i >= 0; i--)
            {
                localThirdPersonController.OnJumpEnd += minions[i].FollowTheCharacterPlayer;
                localThirdPersonController.OnMove += minions[i].FollowTheCharacterPlayer;
                minions[i].OnFishedOrder += reactivatePassiveFollowTheCharacter;
            }
        }

        private void reactivatePassiveFollowTheCharacter(Minion minion) 
        {
            localThirdPersonController.OnMove += minion.FollowTheCharacterPlayer;
            localThirdPersonController.InvokeOnMove();//zasymuluj ruch gracza aby dac minionkowi wspolrzedne gdzie znajduje sie gracz

            if (!m_minionsThatAreNotExecutingAnOrder.Contains(minion))
            {
                m_minionsThatAreNotExecutingAnOrder.Add(minion);//wywal miniona ktory wykonal rozkaz, ,wlasnie wraca na passywce do postaci gracza
            }
        }
    }
}