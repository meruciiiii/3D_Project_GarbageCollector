using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
	//public Action onMove;
	//public Action onLook;
	//public Action onJump;
	public Action onRun;
	public Action onWalk;
	public Action onPickUp;
	public Action offPickUp;
	public Action onInteract;

	public Vector2 direction = Vector2.zero;
	public Vector2 mouseDelta = Vector2.zero;

	//플레이어 움직임
	public void Event_Move(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Performed)) {
			direction = context.ReadValue<Vector2>();
		} else if (context.phase.Equals(InputActionPhase.Canceled)) {
			direction = Vector2.zero;
		}
	}
	//플레이어 시야 회전
	public void Event_Look(InputAction.CallbackContext context) {
		mouseDelta = context.ReadValue<Vector2>();
	}
	//플레이어 점프
	//public void Event_Jump(InputAction.CallbackContext context) {
	//	if (context.phase.Equals(InputActionPhase.Started)) {
	//		onJump();
	//	}
	//}
	//플레이어 달리기
	public void Event_Run(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Started)) {
			onRun();
		} else if (context.phase.Equals(InputActionPhase.Canceled)) {
			onWalk();
		}
	}
	//플레이어 좌클릭(줍기)
	public void Event_PickUp(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Started)) {
			onPickUp();
		}
		else if (context.phase.Equals(InputActionPhase.Canceled)) {
			offPickUp();
		}
	}
	//플레이어 상호작용
	public void Event_Interact(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Started)) {
			onInteract();
		}
	}
}
