using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour {
	private Rigidbody playerRB;
	[Header("인풋 컴포넌트 추가!")]
	[SerializeField] private PlayerInput input;

	//--------------------------------
	//이동 관련
	private bool isCanRun = true;
	private bool isCanJump = true;

	[Header("플레이어 이동속도")]
	public float moveSpeed;
	[SerializeField] private float walkSpeed;
	[SerializeField] private float runSpeed;

	[Header("플레이어 점프력")]
	[SerializeField] private float jumpPower;

	//--------------------------------
	//시점 관련

	[Header("플레이어 감도")]
	//추후 설정에서 감도 조절을 할 수 있어야 하기에, public으로 구현
	[Range(0, 1)] public float sensitive;
	[Header("카메라 오브젝트")]
	[SerializeField] private Camera cameraObject;

	private float currentY;
	private float rotateBoundary = 80f;

	//---------------------------------------------------------------

	//사전 설정
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
		//Vector3 playerMoveVector = (transform.forward * input.direction.y + transform.right * input.direction.x);
		//playerMoveVector *= moveSpeed * Time.deltaTime;
		//playerMoveVector.y = playerRB.linearVelocity.y;
		//playerRB.linearVelocity = playerMoveVector;

		// 1. 목표로 하는 속도 계산
		// 2. 현재 속도에서 목표 속도로 서서히 변화 (Lerp 사용)
		// 3. Y축(중력) 유지 및 적용
		Vector3 targetVelocity = (transform.forward * input.direction.y + transform.right * input.direction.x) * moveSpeed;
		Vector3 currentVelocity = playerRB.linearVelocity;
		Vector3 nextVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * moveSpeed);

		nextVelocity.y = playerRB.linearVelocity.y;
		playerRB.linearVelocity = nextVelocity;
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
		if(isCanJump) {
			playerRB.linearVelocity = Vector3.zero;
			playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
		}
	}
	private void Run() {
		if(isCanRun) {
			moveSpeed = runSpeed;
			Calc_Speed();
		}
	}
	private void Walk() {
		moveSpeed = walkSpeed;
		Calc_Speed();
	}

	[Header("테스트")]
	[SerializeField] private float cur_weight;
	[SerializeField] private float max_weight;

	[Header("플레이어 속도 조절 비율")]
	[SerializeField] [Range(0, 1)] private float slow_start_bag_weight; //플레이어가 얼마나 가방이 찼을때, 느려지는지 비율
	[SerializeField] [Range(0, 1)] private float slow_percent; //플레이어의 이동속도 감소 비율

	public void Calc_Speed() {
		float Speed = moveSpeed;
		//총 무게 가져오기
		//float cur_weight = GameManager.instance.P_Weight 직접참조로 불러오기.
		//float max_weight = GameManager.instance.P_Maxbag 직접참조로 불러오기.

		//만약 일정 비율보다 무게가 늘어난다면
		if (cur_weight > (max_weight * slow_start_bag_weight)) {
			//이동속도 = 플레이어속도 x (무게 비례 속도 저하 비율 + 최소속도)
			//		   = 플레이어속도 x ((1 - 무게 비율) * 최저속도 비율) + 최소속도)
			Speed = moveSpeed * ((1 - cur_weight / max_weight) * slow_percent + (1 - slow_percent));
			isCanJump = false;
			isCanRun = false;
		} else {
			isCanJump = true;
			isCanRun = true;
		}
		/*
		//만약 내가 큰 쓰레기를 들었을 경우,,,
		if(큰 쓰레기 든 상태) {
			float half_reduction = 0.5f;
			int half_count = P_Str - 현재 든 물건;
			for(int i = 0; i<half_count; i++) {
				//힘 스탯이 현재 든 물건보다 half_count만큼 높을때, 속도 반감을 반감
				half_reduction *= 0.5f
			}
			Speed *= half.reduction
		}*/
		moveSpeed = Speed;
	}
}
