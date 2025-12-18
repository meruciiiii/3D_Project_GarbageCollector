using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2_Window : MonoBehaviour {
	private NPC_Create_Trash npc_create_trash;

	[Header("쓰레기 생성 확률")]
	[SerializeField] [Range(0, 100)] protected int cultive_percent;

	[Header("쓰레기 드랍 주기")]
	[SerializeField] private float min_sec;
	[SerializeField] private float max_sec;
	private WaitForSeconds seconds;

	private void Awake() {
		TryGetComponent(out npc_create_trash);
		StartCoroutine(routine_co());
	}

	private IEnumerator routine_co() {
		while(true) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			if (Random.Range(0, 101) < cultive_percent) {
				npc_create_trash.Throw_Trash();
			}
			yield return seconds;
		}
	}
}
