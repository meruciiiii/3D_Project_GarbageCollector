using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Spawner : NPC_Spawner {
	//Victim의 반댓말은 
	//culprit(중범죄까진 아니고, '경범죄를 일으킨' 이라는 뜻이라고 합니다. 쓰레기는 그래도... 범죄자까진 아니닌까...)
	[Header("범죄 위치 좌표")]
	[SerializeField] private GameObject cultive_pos;

	private void OnEnable() {
		StartCoroutine(NPC_Spawn_co());
	}

	private IEnumerator NPC_Spawn_co() {
		while(true) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			GameObject victim_npc = Instantiate(NPC);
			victim_npc.TryGetComponent(out Stage1_Victim victim_pattern);
			victim_pattern.set_pos(start_pos, end_pos, cultive_pos.transform.position);
			victim_pattern.start();
			yield return seconds;
		}
	}
}