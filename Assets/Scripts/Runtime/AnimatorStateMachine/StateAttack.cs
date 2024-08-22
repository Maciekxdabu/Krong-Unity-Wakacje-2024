using Assets.Scripts.Runtime.AnimatorStateMachine;
using Assets.Scripts.Runtime.Character;
using UnityEngine;

public class StateAttack : StateMachineBehaviour
{
    [SerializeField] private CharacterType characterType;

    [Range(0, 1), SerializeField] private float _activatingColliderOfWeaponAnimationPercent;
    [Range(0, 1), SerializeField] private float _deactivatingColliderOfWeaponAnimationPercent;

    private bool _activatingColliderOfWeaponAnimationStateExecute;
    private bool _deactivatingColliderOfWeaponAnimationStateExecute;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_activatingColliderOfWeaponAnimationStateExecute &&
            stateInfo.normalizedTime > _activatingColliderOfWeaponAnimationPercent)
        {
            _activatingColliderOfWeaponAnimationStateExecute = true;
            switch (characterType)
            {
                case CharacterType.Player:
                    animator.TryGetComponent(out Hero hero);
                    hero.EnableColliderOfWeapon();
                    break;
                case CharacterType.Minion:
                    break;
                case CharacterType.EnemyAI:
                    break;
            }
        }

        if (!_deactivatingColliderOfWeaponAnimationStateExecute &&
            stateInfo.normalizedTime > _deactivatingColliderOfWeaponAnimationPercent)
        {
            _deactivatingColliderOfWeaponAnimationStateExecute = true;
            switch (characterType)
            {
                case CharacterType.Player:
                    animator.TryGetComponent(out Hero hero);
                    hero.DisableColliderOfWeapon();
                    break;
                case CharacterType.Minion:
                    break;
                case CharacterType.EnemyAI:
                    break;
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _activatingColliderOfWeaponAnimationStateExecute = false;
        _deactivatingColliderOfWeaponAnimationStateExecute = false;
    }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

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
