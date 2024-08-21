
using Assets.Scripts.Runtime.Character;
using System;
using UnityEngine;

namespace Assets.Scripts.Runtime.AnimatorStateMachine
{
    public class StateControler : StateMachineBehaviour
    {
        [Range(0, 1), SerializeField] private float _aimationPercent;
        [SerializeField] private AnimationState _state;
        [SerializeField] private string _boolConditionToReset;
        [SerializeField] private CharacterType characterType;

        private bool _onAnimationStateExecute;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            switch (_state)
            {
                case AnimationState.StateEnter:
                    animator.SetBool(_boolConditionToReset, false);
                    break;
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_onAnimationStateExecute &&
                stateInfo.normalizedTime > _aimationPercent)
            {
                _onAnimationStateExecute = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _onAnimationStateExecute = false;

            switch (_state)
            {
                case AnimationState.StateExit:
                    animator.SetBool(_boolConditionToReset, false);
                    switch (characterType)
                    {
                        case CharacterType.Player:
                            animator.TryGetComponent(out Hero hero);
                            hero.EnableThirdPersonController();
                            break;
                        case CharacterType.Minion:
                            break;
                        case CharacterType.EnemyAI:
                            break;
                    }
                    break;
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }

    public enum AnimationState
    {
        StateEnter = 1,
        StateExit
    }

    public enum CharacterType
    {
        Player = 1,
        Minion,
        EnemyAI
    }
}