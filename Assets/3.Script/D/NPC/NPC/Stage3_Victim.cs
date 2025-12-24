using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_Victim : NPC_Controller {
	[Header("������ ���� ����")]
	[SerializeField] private int cultive_count;
	[SerializeField] private int max_cultive_count;

	[Header("������ ��� �ֱ�")]
	[SerializeField] private float min_sec;
	[SerializeField] private float max_sec;
	private WaitForSeconds seconds;

	private void OnEnable() {
		cultive_count = 0;
	}

	private void OnDisable() {
		StopAllCoroutines();
	}

	public override void start() {
		base.start();
		StartCoroutine(cultive_co());
	}

	protected override IEnumerator routine_co() {
		yield return StartCoroutine(Move_co(end_pos));
		gameObject.SetActive(false);
		transform.position = start_pos;
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
