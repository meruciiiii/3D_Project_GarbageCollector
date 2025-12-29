using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_NPC : NPC_Base {
	[HideInInspector] public IState becomeTrashState;

	[Header("쓰레기 드랍 갯수")]
	[SerializeField] private float max_drop_cnt = 3;
	public float drop_cnt = 0;

	[Header("쓰레기 드랍 주기")]
	[SerializeField] private float min_sec = 3;
	[SerializeField] private float max_sec = 6;

	[Header("충돌 관련")]
	[SerializeField] private float flyPower = 10f;
	public bool isClone = false;

	//area 판정 확인
	public bool inArea = false;

	private Transform area_object;

	protected override void Awake() {
		base.Awake();
		becomeTrashState = new BecomeTrashState(this);
		area_object = GameObject.FindGameObjectWithTag("Area03").transform;
		npc_create_trash.area = area_object;
	}


	//코루틴 판정
	public void run_coroutine() {StartCoroutine(drop_trash());}
	public void stop_coroutine() {StopCoroutine(drop_trash());}
	private IEnumerator drop_trash() {
		while (drop_cnt < max_drop_cnt) {
			float maxTime = Random.Range(min_sec, max_sec);
			float timer = 0f;

			if (Random.Range(0, 101) <= percent && isActive) {
				npc_create_trash.trash_Spawn();
				drop_cnt++;
			}
			while (timer < maxTime) {
				timer += Time.deltaTime;
				yield return null;
			}
		}
	}
	protected override void Event_ChangeArea(int area) {
		if (area.Equals(3)) { isActive = true; } 
		else { isActive = false; }
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Car") && !isClone && inArea) {
			CloneTrashRuntime(other.transform.position);
		}
	}

	private void CloneTrashRuntime(Vector3 car_pos) {
		GameObject cloneGO = Instantiate(gameObject);
		if(cloneGO.TryGetComponent(out Trash cloneTrash)) {	CloneSetting(cloneTrash); }
		if(cloneGO.TryGetComponent(out Stage3_NPC npc_script)) { npc_script.isClone = true; }
		if(cloneGO.TryGetComponent(out Rigidbody cloneRB)) {
			Vector3 fly_dir = (transform.position - car_pos) * flyPower;
			fly_dir.y *= 0.25f;
			cloneRB.AddForce(fly_dir, ForceMode.Impulse);
		}
		ChangeState(becomeTrashState);
	}

	private void CloneSetting(Trash cloneTrash) {
		if (cloneTrash.TryGetComponent(out UnityEngine.AI.NavMeshAgent Nav)) { Nav.enabled = false; }
		if (cloneTrash.TryGetComponent(out Stage3_NPC stageNpc)) { stageNpc.enabled = false;}
		if (cloneTrash.TryGetComponent(out NPC_Create_Trash create_Trash)) { create_Trash.enabled = false; }
		if (cloneTrash.TryGetComponent(out NPC_Random_Mesh random_Mesh)) { random_Mesh.enabled = false; }
		if (cloneTrash.TryGetComponent(out Animator animator)) { animator.enabled = false; }
	}
}
