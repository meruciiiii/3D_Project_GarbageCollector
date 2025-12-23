using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Victim : NPC_Controller {
	private Vector3 cultive_pos;
	private Vector3 cultive_dir;

	//스테이지1 시민은 확률적으로 중간에 멈춰서 쓰레기를 던짐!
	public void set_pos(Vector3 start_pos, Vector3 end_pos, Vector3 cultive_pos, Vector3 cultive_dir) {
		set_pos(start_pos, end_pos);

		this.cultive_pos = cultive_pos;
		this.cultive_dir = cultive_dir;
	}

	protected override IEnumerator routine_co() {
		if(Random.Range(0,101) < cultive_percent) {
			yield return StartCoroutine(Move_co(cultive_pos));
			Vector3 throw_dir = npc_create_trash.throw_vector;
			transform.LookAt(cultive_dir);
			//플레이어 쓰레기 던지는 모션
			yield return new WaitForSeconds(1f);
			npc_create_trash.Throw_Trash();
			yield return new WaitForSeconds(1f);
		}
		yield return StartCoroutine(Move_co(end_pos));
		gameObject.SetActive(false);
	}
}
