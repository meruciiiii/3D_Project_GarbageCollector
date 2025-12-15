using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	//플레이어 컨트롤러로 이름을 바꿔야할듯.
	private Rigidbody playerRB;

	[Header("플레이어 이동속도")]
	public float movementSpeed = 0f;
	private float walkSpeed = 150f;
	private float runSpeed = 400;

	private Vector2 direction = Vector2.zero;

	[Header("플레이어 점프력")]
	public float jumpPower = 0f;

	[Header("플레이어 감도")]
	public float sensitive = 0f;
	[Header("카메라 오브젝트")]
	[SerializeField] private Camera cameraObject;

	private Vector2 mouseDelta;
	private float currentY;
	private float rotateBoundary = 80f;

	private void Awake() {
		TryGetComponent(out playerRB);
		movementSpeed = walkSpeed;
	}

	private void FixedUpdate() {
		Move();
	}

	private void LateUpdate() {
		Rotate();
	}

	private void Move() {
		//플레이어 앞뒤(WS/Z축) + 좌우(AD/X축)에 이동속도 곱하기.
		//y값은 플레이어 점프 현황 물리 속도에 맞춰서 저장 (안하면 엄청 느리게 떨어짐)
		Vector3 playerMoveVector = (transform.forward * direction.y + transform.right * direction.x) * movementSpeed * Time.deltaTime;
		playerMoveVector.y = playerRB.linearVelocity.y;
		playerRB.linearVelocity = playerMoveVector;
	}
	private void Rotate() {
		//플레이어 위아래 시점에 감도값 곱.
		currentY += mouseDelta.y * sensitive;
		//시점 제한(위아래)
		float newY = Mathf.Clamp(currentY, -rotateBoundary, rotateBoundary);

		//카메라 시점 변경
		//플레이어 좌우회전
		cameraObject.transform.localEulerAngles = new Vector3(-newY, 0, 0);
		transform.eulerAngles += new Vector3(0, mouseDelta.x * sensitive, 0);
	}

	//플레이어 무브
	public void OnMove(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Performed)) {
			direction = context.ReadValue<Vector2>();
		} else if (context.phase.Equals(InputActionPhase.Canceled)) {
			direction = Vector2.zero;
		}
	}
	//플레이어 점프
	public void OnJump(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Started)) {
			playerRB.linearVelocity = Vector3.zero;
			playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		}
	}
	//플레이어 시야 회전
	public void OnRotate(InputAction.CallbackContext context) {
		mouseDelta = context.ReadValue<Vector2>();
	}
	public void OnRun(InputAction.CallbackContext context) {
		if (context.phase.Equals(InputActionPhase.Performed)) {
			movementSpeed = runSpeed;
		} else if (context.phase.Equals(InputActionPhase.Canceled)) {
			movementSpeed = walkSpeed;
		}
	}
}
