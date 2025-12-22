using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour {
	[Header("NPC 프리팹")]
	[SerializeField] protected GameObject NPC;

	[Header("이동 경로 Vector")]
	protected Transform start_pos;
	protected Transform end_pos;

	[Header("NPC 생성 시간 간격 (단위 : 초)")]
	[SerializeField] protected float min_sec;
	[SerializeField] protected float max_sec;
	protected WaitForSeconds seconds;

	protected void Rnd_Set_Pos() {
		List<Transform> locations_list = new List<Transform>();
		for(int i = 0; i < transform.childCount; i++) {
			locations_list.Add(transform.GetChild(i));
		}

		int rad_val = Random.Range(0, locations_list.Count);
		start_pos = locations_list[rad_val];
		locations_list.RemoveAt(rad_val);
		rad_val = Random.Range(0, locations_list.Count);
		end_pos = locations_list[rad_val];
	}
}
