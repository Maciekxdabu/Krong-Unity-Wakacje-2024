using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        private bool disabledInput;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			if (!disabledInput)
			{
				MoveInput(value.Get<Vector2>());
			}
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			if (!disabledInput)
			{
				JumpInput(value.isPressed);
			}
		}

		public void OnSprint(InputValue value)
		{
			if (!disabledInput)
			{
				SprintInput(value.isPressed);
			}
		}
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			if (!disabledInput)
			{
				move = newMoveDirection;
			}
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			if (!disabledInput)
			{
				jump = newJumpState;
			}
		}

		public void SprintInput(bool newSprintState)
		{
			if (!disabledInput)
			{
				sprint = newSprintState;
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

        internal void DisableInputs()
        {
			disabledInput = true;
        }

        internal void EnableInputs()
        {
            disabledInput = false;
        }

        internal void StopCharacterMove()
        {
			move = Vector2.zero;
        }
    }
}