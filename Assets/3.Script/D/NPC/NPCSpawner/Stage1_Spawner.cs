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
			if(npc_pooling[pool_current].activeSelf) {
				NPC_Pooling_ReSize();
				pool_current = pool_count - 1;
			}
			GameObject victim = npc_pooling[pool_current];
			victim.TryGetComponent(out Stage1_Victim victim_pattern);
			Rnd_Set_Pos();
			victim_pattern.set_pos(start_pos.position, end_pos.position, cultive_pos.transform.position);
			victim.SetActive(true);
			victim_pattern.start();
			pool_current++;
			if(pool_current.Equals(pool_count)) { pool_current = 0; }
			yield return seconds;
		}
	}
}