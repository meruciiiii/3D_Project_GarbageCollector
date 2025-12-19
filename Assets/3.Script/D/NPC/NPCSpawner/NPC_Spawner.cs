using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour {
	[Header("NPC 프리팹")]
	[SerializeField] protected GameObject NPC;

	[Header("이동 경로 Vector")]
	[SerializeField] private List<Transform> spawn_locations;
	protected Transform start_pos;
	protected Transform end_pos;

	[Header("NPC 생성 시간 간격 (단위 : 초)")]
	[SerializeField] protected float min_sec;
	[SerializeField] protected float max_sec;
	protected WaitForSeconds seconds;

	protected void Rnd_Set_Pos() {
		List<Transform> copy_locations_list = new List<Transform>();
		int copy_count = spawn_locations.Count;
		for(int i = 0; i < copy_count; i++) {
			copy_locations_list.Add(spawn_locations[i]);
		}
		int rad_val = Random.Range(0, copy_locations_list.Count);
		start_pos = copy_locations_list[rad_val];
		copy_locations_list.RemoveAt(rad_val);
		rad_val = Random.Range(0, copy_locations_list.Count);
		end_pos = copy_locations_list[rad_val];
	}
}
