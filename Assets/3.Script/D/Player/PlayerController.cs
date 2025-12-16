using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour {
	//플레이어 컨트롤러로 이름을 바꿔야할듯.
	private Rigidbody playerRB;
	[Header("인풋 컴포넌트 추가!")]
	[SerializeField] private PlayerInput input;

	[Header("플레이어 이동속도")]
	public float movementSpeed;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float runSpeed;
	[SerializeField] private float minSpeed = 0.1f;

	[Header("플레이어 점프력")]
	[SerializeField] private float jumpPower;

	[Header("플레이어 감도")]
	//추후 설정에서 감도 조절을 할 수 있어야 하기에, public으로 구현
	public float sensitive;
	[Header("카메라 오브젝트")]
	[SerializeField] private Camera cameraObject;

	private float currentY;
	private float rotateBoundary = 80f;

	//컴포넌트 설정
	private void Awake() {
		TryGetComponent(out playerRB);
		Walk();
	}
	//이벤트 등록
	private void Start() {
		input.onJump += Jump;
		input.onRun += Run;
		input.onWalk += Walk;
	}
	//움직임
	private void FixedUpdate() {
		Move();
	}
	//시야
	private void LateUpdate() {
		Rotate();
	}

	private void Move() {
		//플레이어 앞뒤(WS/Z축) + 좌우(AD/X축)에 이동속도 곱하기.
		//y값은 플레이어 점프 현황 물리 속도에 맞춰서 저장 (안하면 엄청 느리게 떨어짐)
		Vector3 playerMoveVector = (transform.forward * input.direction.y + transform.right * input.direction.x);
		playerMoveVector = Calc_Speed(playerMoveVector);
		playerMoveVector.y = playerRB.linearVelocity.y;
		playerRB.linearVelocity = playerMoveVector;
	}
	private void Rotate() {
		//플레이어 위아래 시점에 감도값 곱.
		currentY += input.mouseDelta.y * sensitive;
		//시점 제한(위아래)
		float newY = Mathf.Clamp(currentY, -rotateBoundary, rotateBoundary);

		//카메라 시점 변경
		//플레이어 좌우회전
		cameraObject.transform.localEulerAngles = new Vector3(-newY, 0, 0);
		transform.eulerAngles += new Vector3(0, input.mouseDelta.x * sensitive, 0);
	}
	private void Jump() {
		playerRB.linearVelocity = Vector3.zero;
		playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
	}
	private void Run() {
		movementSpeed = runSpeed;
	}
	private void Walk() {
		movementSpeed = walkSpeed;
	}

	[Header("테스트")]
	[SerializeField] private float inventory;
	[SerializeField] private float max_inventory;

	private Vector3 Calc_Speed(Vector3 vector) {
		//총 무게 가져오기

		//무게 계산 = movementSpeed에 무게로 인한 무게 90% + 최소한의 이동속도 10%
		float Speed = movementSpeed * ((1 - inventory / max_inventory) * 0.9f + minSpeed);
		//만약 내가 큰 쓰레기를 들었을 경우,,,
		//Speed *= 0.5f;
		vector *= Speed * Time.deltaTime;
		return vector;
	}
}
