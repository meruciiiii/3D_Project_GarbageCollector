using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Victim : NPC_Controller {
	//스테이지1 평범 시민은 그냥 스폰 지점으로부터 삭제 지점까지 그냥 걸어감.
	protected override IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(end_pos));
		Destroy(gameObject);
	}
}
