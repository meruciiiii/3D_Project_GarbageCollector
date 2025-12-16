using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Culprit : NPC_Controller {
	private Vector3 cultive_pos;

	//½ºÅ×ÀÌÁö1 ³ª»Û ½Ã¹ÎÀº Áß°£¿¡ ¸ØÃç¼­ ¾²·¹±â¸¦ ´øÁü!
	public void set_pos(Vector3 start_pos, Vector3 end_pos, Vector3 cultive_pos) {
		this.start_pos = start_pos;
		this.end_pos = end_pos;
		this.cultive_pos = cultive_pos;
	}

	protected override IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(cultive_pos));
		yield return new WaitForSeconds(2f);
		yield return StartCoroutine(Move_co(end_pos));
		Destroy(gameObject);
	}
}
