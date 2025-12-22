using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Car_Controller : MonoBehaviour {
	private NavMeshAgent car_agent;

	[Header("NPC 이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3 end_pos;

	private void Awake() {
		TryGetComponent(out car_agent);
	}

	public void set_pos(Vector3 start_pos, Vector3 end_pos) {
		this.start_pos = start_pos;
		this.end_pos = end_pos;
	}

	public virtual void start() {
		StartCoroutine(routine_co());
	}
	protected virtual IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(end_pos));
		Destroy(gameObject);
	}
	protected IEnumerator Move_co(Vector3 target_point) {
		transform.LookAt(target_point);
		while (Vector3.SqrMagnitude(transform.position - target_point) >= 0.01f) {
			car_agent.SetDestination(target_point);
			yield return null;
		}
		transform.position = target_point;
	}
}
