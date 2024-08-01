using Assets.Scripts.Runtime.Character;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Runtime.Order
{
    public class SendOrder : AbstractOrder
    {
        private Vector3 _destinationPosition;
        private float _maxDistance;
        public bool IsThisMinFreeSpaceToExecuteTheOrder { get { return Vector3.Distance(_transformOfMinion.localPosition, _destinationPosition) > 1f; } }

        public SendOrder(OrderData orderData)
        {
            _maxDistance = orderData.GetMaxDistance;
        }

        public override void Initialize(Minion minion, Hero hero)
        {
            base.Initialize(minion, hero);

            calculateTheDestinationPointForwardFromCharacter(hero);

            if (IsThisMinFreeSpaceToExecuteTheOrder)
            {
                hero.ReleaseMinionInFavourOfAnOrder(_minion);
            }
        }

        public override void Execute()
        {
            _minion.GoToPostion(_destinationPosition);
        }

        private void calculateTheDestinationPointForwardFromCharacter(Hero hero)
        {
            Transform _heroFrontTransform = hero.GetFrontTransform;
            _destinationPosition = _heroFrontTransform.position + (hero.transform.forward * _maxDistance);

            if (hasObstacleBeenDetected(_heroFrontTransform, out NavMeshHit navMeshHit))
            {
                _destinationPosition = navMeshHit.position;
            }
        }

        private bool hasObstacleBeenDetected(Transform heroFrontTransform, out NavMeshHit navMeshHit)
        {
            navMeshHit = new NavMeshHit();
            Vector3 _forwardDirectionFromHero = heroFrontTransform.TransformDirection(Vector3.forward);

            bool _wallDetected = Physics.Raycast(
                heroFrontTransform.position,
                _forwardDirectionFromHero * _maxDistance,
                out RaycastHit objectHit,
                _maxDistance);

            if (_wallDetected)
            {
                NavMesh.SamplePosition(
                    objectHit.point,
                    out navMeshHit,
                    100f,
                    NavMesh.AllAreas
                    );

                return true;
            }

            return false;
        }
    }
}