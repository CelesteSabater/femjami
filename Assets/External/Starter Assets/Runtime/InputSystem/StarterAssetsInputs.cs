using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	using femjami.Utils.Singleton;
	
	public class StarterAssetsInputs : Singleton<StarterAssetsInputs>
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool smell;
		public bool listen;
		public bool sneak;
		public bool pause;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}
		public void OnSmell(InputValue value)
		{
			SmellInput(value.isPressed);
		}

		public void OnListen(InputValue value)
		{
			ListenInput(value.isPressed);
		}

		public void OnSneak(InputValue value)
		{
			if (value.isPressed)
				SneakInput(!sneak);
		}
		
		public void OnPause(InputValue value)
		{
			if (value.isPressed)
				PauseInput(!pause);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void SmellInput(bool newSmelltState)
		{
			smell = newSmelltState;
		}

		public void ListenInput(bool newListenState)
		{
			listen = newListenState;
		}

		public void SneakInput(bool newSneakState)
		{
			sneak = newSneakState;
		}

		public void PauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}