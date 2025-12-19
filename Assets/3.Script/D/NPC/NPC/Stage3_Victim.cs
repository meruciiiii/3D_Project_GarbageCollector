using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_Victim : NPC_Controller {
	[Header("쓰레기 버림 갯수")]
	[SerializeField] private int cultive_count;
	[SerializeField] private int max_cultive_count;

	[Header("쓰레기 드랍 주기")]
	[SerializeField] private float min_sec;
	[SerializeField] private float max_sec;
	private WaitForSeconds seconds;

	public override void start() {
		base.start();
		StartCoroutine(cultive_co());
	}

	protected override IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(end_pos));
		Destroy(gameObject);
	}

	private IEnumerator cultive_co() {
		while(cultive_count < max_cultive_count) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			if(Random.Range(0, 101) < cultive_percent) {
				npc_create_trash.Throw_Trash();
				cultive_count++;
			}
			yield return seconds;
		}
	}
}
