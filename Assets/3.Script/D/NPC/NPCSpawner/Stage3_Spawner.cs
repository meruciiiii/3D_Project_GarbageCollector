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
			GameObject victim_npc = Instantiate(NPC);
			victim_npc.TryGetComponent(out Stage3_Victim victim_pattern);
			victim_pattern.set_pos(start_pos, middle_pos_array, end_pos);
			victim_pattern.start();
			yield return seconds;
		}
	}
}
