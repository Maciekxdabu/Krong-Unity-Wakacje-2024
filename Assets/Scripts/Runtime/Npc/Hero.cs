using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.Npc
{
    public class Character : Creature
    {
        [SerializeField] private List<Minion> minions;
        [SerializeField] private ThirdPersonController localThirdPersonController;

        private void Awake()
        {
            InitializeForMinions();
        }

        private void InitializeForMinions()
        {
            for (int i = minions.Count - 1; i >= 0; i--)
            {
                localThirdPersonController.OnJumpEnd += minions[i].FollowTheCharacterPlayer;
                localThirdPersonController.OnMove += minions[i].FollowTheCharacterPlayer;
            }
        }
    }
}