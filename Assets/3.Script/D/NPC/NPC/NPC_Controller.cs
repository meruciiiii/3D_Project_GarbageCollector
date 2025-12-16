using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour {
	[Header("NPC 이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3[] middle_pos_array;
	[SerializeField] protected Vector3 end_pos;

	[Header("NPC 속도")]
	[SerializeField] private float moveSpeed = 150f;

	//중간 포인트가 없을 경우
	public void set_pos(Vector3 start_pos, Vector3 end_pos) {
		this.start_pos = start_pos;
		middle_pos_array = null;
		this.end_pos = end_pos;
	}

	//중간 포인트가 있을 경우
	public void set_pos(Vector3 start_pos, Vector3[] middle_pos_array, Vector3 end_pos) {
		this.start_pos = start_pos;
		this.middle_pos_array = middle_pos_array;
		this.end_pos = end_pos;
	}


	public void start() {
		revese();
		transform.position = start_pos;
		StartCoroutine(routine_co());
	}

	private void revese() {
		//50% 확률로 역순으로 가는 경우도 하기.
		if(UnityEngine.Random.Range(0,2).Equals(0)) {
			Vector3 temp_pos = start_pos;
			start_pos = end_pos;
			end_pos = temp_pos;
			if(middle_pos_array != null) {
				Array.Reverse(middle_pos_array);
			}
		}
	}
	protected virtual IEnumerator routine_co() {
		yield return null;
	}

	protected IEnumerator Move_co(Vector3 target_point) {
		while(Vector3.SqrMagnitude(transform.position - target_point) >= 0.1f) {
			transform.position = Vector3.MoveTowards(transform.position, target_point, moveSpeed * Time.deltaTime);
			yield return null;
		}
		transform.position = target_point;
	}
}
