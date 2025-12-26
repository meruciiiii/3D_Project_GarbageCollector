using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_NPC : NPC_Base {
	[Header("쓰레기 드랍 횟수")]
	[SerializeField] private float max_drop_cnt = 3;
	public float drop_cnt = 0;

	[Header("쓰레기 드랍 주기")]
	[SerializeField] private float min_sec = 3;
	[SerializeField] private float max_sec = 6;

	private Transform area_object;

	protected override void Awake() {
		base.Awake();
		area_object = GameObject.FindGameObjectWithTag("Area03").transform;
		npc_create_trash.area = area_object;
	}

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
}
