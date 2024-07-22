using Assets.Scripts.Runtime.Npc;
using UnityEngine;

namespace Assets.Scripts.Runtime.Order
{
    public class Send : AbstractOrder
    {
        private float _maksimumDistance;
        private Vector3 _destinationPosition;

        public Send(OrderData orderData)
        {
            _maksimumDistance = orderData.GetMaksimumDistance;
        }

        public void Initialize(Minion minion, Hero hero)
        {
            _minion = minion;
            calculateTheDestinationPointForwardFromCharacter(hero);
        }

        private void calculateTheDestinationPointForwardFromCharacter(Hero hero)
        {
            _destinationPosition = hero.transform.localPosition + (hero.transform.forward * _maksimumDistance);
        }

        public override void Execute()
        {
            _minion.GoToPostion(_destinationPosition);
        }
    }
}