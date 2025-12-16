using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Spawner : NPC_Spawner {
	[Header("악질 NPC 등장 확률 (0~100%)")]
	[SerializeField] private float culprit_spawn_percent;
	private float random_value;
	//Victim의 반댓말은 
	//culprit(중범죄까진 아니고, '경범죄를 일으킨' 이라는 뜻이라고 합니다. 쓰레기는 그래도... 범죄자까진 아니닌까...)
	[Header("범죄 위치 좌표")]
	[SerializeField] private GameObject cultive_pos;

	[Header("NPC 생성 시간 간격 (단위 : 초)")]
	[SerializeField] private WaitForSeconds seconds;
	[SerializeField] private float min_sec;
	[SerializeField] private float max_sec;

	private void OnEnable() {
		StartCoroutine(NPC_Spawn_co());
	}

	private IEnumerator NPC_Spawn_co() {
		while(true) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			random_value = Random.Range(0, 100);
			if(random_value < culprit_spawn_percent) {
				//Cultive NPC 생성
				GameObject cultive_npc = Instantiate(NPC_list[1]);
				cultive_npc.TryGetComponent(out Stage1_Culprit cultive_pattern);
				cultive_pattern.set_pos(start_pos, end_pos, cultive_pos.transform.position);
				cultive_pattern.start();
			} else {
				//Victim NPC 생성
				GameObject victim_npc = Instantiate(NPC_list[0]);
				victim_npc.TryGetComponent(out Stage1_Victim victim_pattern);
				victim_pattern.set_pos(start_pos, end_pos);
				victim_pattern.start();
			}
			yield return seconds;
		}
	}
}