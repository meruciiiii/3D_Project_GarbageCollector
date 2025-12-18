using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Victim : NPC_Controller {
	private Vector3 cultive_pos;
	[Header("쓰레기 던질 확률")]
	[SerializeField] [Range(0, 100)] private int cultive_percent;

	//스테이지1 시민은 확률적으로 중간에 멈춰서 쓰레기를 던짐!
	public void set_pos(Vector3 start_pos, Vector3 end_pos, Vector3 cultive_pos) {
		set_pos(start_pos, end_pos);

		cultive_pos.y = npc_height;
		this.cultive_pos = cultive_pos;
	}

	protected override IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(cultive_pos));
		if(Random.Range(0,101) < cultive_percent) {
			//쓰레기를 던지는 메소드
			yield return new WaitForSeconds(2f);
		}
		yield return StartCoroutine(Move_co(end_pos));
		Destroy(gameObject);
	}
}
