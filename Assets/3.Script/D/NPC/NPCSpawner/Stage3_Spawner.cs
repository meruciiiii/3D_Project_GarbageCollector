using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_Spawner : NPC_Spawner {
	private void OnEnable() {
		StartCoroutine(NPC_Spawn_co());
	}

	private IEnumerator NPC_Spawn_co() {
		while (true) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			//if (npc_pooling[pool_current].activeSelf) {
			//	NPC_Pooling_ReSize();
			//	pool_current = pool_count - 1;
			//}
			GameObject victim = npc_pooling[pool_current];
			victim.TryGetComponent(out Stage3_Victim victim_pattern);
			Rnd_Set_Pos();
			victim.SetActive(true);
			victim_pattern.set_pos(start_pos.position, end_pos.position);
			victim_pattern.start();
			pool_current++;
			if (pool_current.Equals(pool_count)) { pool_current = 0; }
			yield return seconds;
		}
	}
}
