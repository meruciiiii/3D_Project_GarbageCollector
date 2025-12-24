using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Controller : MonoBehaviour {
	protected NPC_Create_Trash npc_create_trash;
	private NavMeshAgent npc_agent;

	[Header("NPC 이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3 end_pos;

	//[Header("NPC 높이")]
	//[SerializeField] protected float npc_height = 0f;

	//[Header("NPC 속도")]
	//[SerializeField] private float moveSpeed = 150f;

	[Header("쓰레기 생성 확률")]
	[SerializeField] [Range(0, 100)] protected int cultive_percent;

	private void Awake() {
		TryGetComponent(out npc_create_trash);
		TryGetComponent(out npc_agent);
	}

	public void set_pos(Vector3 start_pos, Vector3 end_pos) {
		this.start_pos = start_pos;
		this.end_pos = end_pos;
		Debug.Log(start_pos);
		npc_agent.Warp(start_pos);
	}

	public virtual void start() {
		StartCoroutine(routine_co());
	}
	protected virtual IEnumerator routine_co() {
		yield return null;
	}
	protected IEnumerator Move_co(Vector3 target_point) {
		transform.LookAt(target_point);
		while (Vector3.SqrMagnitude(transform.position - target_point) >= 0.01f) {
			npc_agent.SetDestination(target_point);
			yield return null;
		}
		transform.position = target_point;
	}
}
