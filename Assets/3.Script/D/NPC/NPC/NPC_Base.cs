using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Base : MonoBehaviour {
	//쓰레기 생성 컴포넌트
	[HideInInspector] public NPC_Create_Trash npc_create_trash;


	[Header("범죄 확률")]
	[Range(0,100)] public float percent;

	[Header("이동 속도")]
	public float NPC_speed = 3.5f;

	[Header("이동 좌표")]
	public List<Vector3> pos_list;
	public Vector3 start_pos;
	public Vector3 end_pos;

	[Header("AreaManager")]
	[SerializeField] protected AreaManager areaManager;

	//NavMesh Agent
	[HideInInspector] public NavMeshAgent agent;

	//State Pattern
	public IState currentState;

	public IState spawnState;
	public IState moveState;
	public IState setPosState;

	// ---
	//구역 이벤트 수신용
	[SerializeField] protected bool isActive;

	//-------------------------------------------------------------

	protected virtual void Awake() {
		TryGetComponent(out agent);
		TryGetComponent(out npc_create_trash);
		agent.speed = NPC_speed;

		spawnState = new SpawnState(this);
		moveState = new MoveState(this);
		setPosState = new SetPosState(this);

		AreaManager.instance.onAreaChanged += Event_ChangeArea;
	}

	private void Update() {
		if(currentState != null) {
			currentState.Update();
		}
	}

	//상태 변경 함수. 
	//1. 현재 패턴을 변경하려고 할때
	//2. 현재 패턴의 Exit(종료 코드)을 실행하고
	//3. 바꿀 패턴(매개변수 newState)를 현재 패턴에 적용 한 다음,
	//4. 바뀐 패턴(currentState)의 Enter(시작 코드)를 실행한다.
	public void ChangeState(IState newState) {
		if (currentState != null) {
			currentState.Exit();
		}

		currentState = newState;
		currentState.Enter();
	}

	protected virtual void Event_ChangeArea(int area) { }
}
