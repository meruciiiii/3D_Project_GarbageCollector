using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRB;
    [Header("인풋 컴포넌트 추가!")]
    [SerializeField] private PlayerInput input;

    //--------------------------------
    //이동 관련
    [SerializeField] private bool isCanRun = true;
    //private bool isCanJump = true;

    [Header("플레이어 이동속도")]
    [SerializeField] private float moveSpeed;
    private float current_mode_speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float accelSpeed;

    //[Header("플레이어 점프력")]
    //[SerializeField] private float jumpPower;

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
        //input.onJump += Jump;
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


    private Coroutine walkingsound_co;
    private void Move() {
		#region 과거의 흔적
		//플레이어 앞뒤(WS/Z축) + 좌우(AD/X축)에 이동속도 곱하기.
		//y값은 플레이어 점프 현황 물리 속도에 맞춰서 저장 (안하면 엄청 느리게 떨어짐)
		//Vector3 playerMoveVector = (transform.forward * input.direction.y + transform.right * input.direction.x);
		//playerMoveVector *= moveSpeed * Time.deltaTime;
		//playerMoveVector.y = playerRB.linearVelocity.y;
		//playerRB.linearVelocity = playerMoveVector;

		// 1. 목표로 하는 속도 계산
		// 2. 현재 속도에서 목표 속도로 서서히 변화 (Lerp 사용)
		// 3. Y축(중력) 유지 및 적용
		//Vector3 targetVelocity = (transform.forward * input.direction.y + transform.right * input.direction.x) * moveSpeed;
		//Vector3 currentVelocity = playerRB.linearVelocity;
		//Vector3 nextVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * moveSpeed);

		//nextVelocity.y = playerRB.linearVelocity.y;
		//playerRB.linearVelocity = nextVelocity;

		//Vector3 playerMoveVector = (transform.forward * input.direction.y + transform.right * input.direction.x);
		//playerMoveVector *= moveSpeed * Time.deltaTime;
		//playerMoveVector.y = playerRB.linearVelocity.y;
		//playerRB.AddForce(playerMoveVector, ForceMode.Impulse);
		#endregion
		Vector3 targetVelocity = (transform.forward * input.direction.y + transform.right * input.direction.x) * moveSpeed;
        targetVelocity.y = playerRB.linearVelocity.y;
        playerRB.linearVelocity = Vector3.MoveTowards(playerRB.linearVelocity, targetVelocity, accelSpeed * Time.deltaTime);

        if (input.direction.sqrMagnitude > 0.01f) 
        {
            if (walkingsound_co == null) 
            {
                walkingsound_co = StartCoroutine(FootstepLoop());
            }
        } 
        else 
        {
            if (walkingsound_co != null) 
            {
                StopCoroutine(walkingsound_co);
                walkingsound_co = null;
            }
        }
    }
    private IEnumerator FootstepLoop()
    {
        while (true)
        {
            // 1. 현재 상태에 맞는 사운드 이름 결정
            // Walk_Sound, Run_Sound 등 AudioManager 딕셔너리에 등록된 이름 사용
            string soundName = "SFX14";

            // 2. 사운드 재생
            AudioManager.instance.PlayWalkingStep(soundName);

            // 3. 발소리 간격 계산
            // moveSpeed가 낮아지면(무거워지면) interval이 길어져 천천히 걷는 소리가 납니다.
            float stepInterval = 1.8f / Mathf.Max(moveSpeed, 1.0f);

            // 0.4초 클립의 길이를 고려하여 최소/최대 간격 제한 (너무 빠르거나 느리지 않게)
            yield return new WaitForSeconds(Mathf.Clamp(stepInterval, 0.25f, 0.6f));
        }
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
    //private void Jump()
    //{
    //    //BoxCast로 플레이어의 하단의 넓적한 네모만큼 범위를 탐색하여, 뭔가라도 아래에 있는(밟은) 상태면 점프 가능.
    //    if (isCanJump && Physics.BoxCast(transform.position, new Vector3(0.25f, 0.01f, 0.25f), Vector3.down,
    //                               Quaternion.identity, 0.01f + transform.localScale.y))
    //    {
    //        //playerRB.linearVelocity = Vector3.zero;
    //        playerRB.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    //    }
    //}
    private void Run() {
        if (isCanRun) {
            moveSpeed = runSpeed;
            current_mode_speed = runSpeed;
            Calc_Speed();
        }
    }
    private void Walk() {
        moveSpeed = walkSpeed;
        current_mode_speed = walkSpeed;
        Calc_Speed();
    }

    //-------------------------------------------------------------------
    //속도

    [Header("무게 보기")]
    [SerializeField] private float cur_weight;
    [SerializeField] private float max_weight;

    [Header("플레이어 속도 조절 비율")]
    [SerializeField] [Range(0, 1)] private float slow_start_bag_weight; //플레이어가 얼마나 가방이 찼을때, 느려지는지 비율
    [SerializeField] [Range(0, 1)] private float slow_percent; //플레이어의 이동속도 감소 비율

    public void Calc_Speed() {
        float Speed = current_mode_speed;
        //총 무게 가져오기
        cur_weight = GameManager.instance.P_Weight; //직접참조로 불러오기.
        max_weight = GameManager.instance.P_Maxbag; //직접참조로 불러오기.

        //만약 일정 비율보다 무게가 늘어난다면
        //현재 무게가 최대 무게의 50%라면...
        if (cur_weight > (max_weight * slow_start_bag_weight)) {
            //이동속도 = 플레이어속도 x (무게 비례 속도 저하 비율 + 최소속도)
            //         = 플레이어속도 x ((1 - 무게 비율) * 최저속도 비율) + 최소속도)
            Speed = current_mode_speed * ((1 - cur_weight / max_weight) * slow_percent + (1 - slow_percent));
            Debug.Log("moveSpeed : " + moveSpeed);
            Debug.Log("cur_weight : " + cur_weight);
            Debug.Log("max_weight : " + max_weight);
            Debug.Log("slow_percent : " + slow_percent);
            //isCanJump = false;
            isCanRun = false;
        } else {
            //isCanJump = true;
            isCanRun = true;
        }

        //만약 내가 큰 쓰레기를 들었을 경우,,,
        if (GameManager.instance.isGrabBigGarbage) {
            //Debug.Log("설마 여기냐?");
            float half_reduction = 0.5f;
            int half_count = GameManager.instance.P_Str - GameManager.instance.BigGarbageWeight;
            for (int i = 0; i < half_count; i++) {
                //힘 스탯이 현재 든 물건보다 half_count만큼 높을때, 속도 반감을 반감
                half_reduction *= 0.5f;
            }
            Speed *= half_reduction;
        }
        moveSpeed = Speed;
    }
}
