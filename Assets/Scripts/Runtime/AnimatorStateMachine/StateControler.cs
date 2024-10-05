
using Assets.Scripts.Runtime.Character;
using UnityEngine;

namespace Assets.Scripts.Runtime.AnimatorStateMachine
{
    public class StateControler : StateMachineBehaviour
    {
        [SerializeField] private AnimationState _state;
        [SerializeField] private string _boolConditionToReset;
        [SerializeField] private CharacterType characterType;

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

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            switch (_state)
            {
                case AnimationState.StateExit:
                    animator.SetBool(_boolConditionToReset, false);
                    switch (characterType)
                    {
                        case CharacterType.Player:
                            animator.TryGetComponent(out Hero hero);
                            hero.AttackAnimExit();
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