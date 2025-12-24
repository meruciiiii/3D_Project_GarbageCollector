using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour {
	[Header("NPC 프리팹")]
	[SerializeField] protected GameObject NPC_prefabs;
	public GameObject[] npc_pooling;
	protected int pool_count = 0;
	protected int pool_current = 0;

	[Header("이동 경로 Vector")]
	protected Transform start_pos;
	protected Transform end_pos;

	[Header("NPC 생성 시간 간격 (단위 : 초)")]
	[SerializeField] protected float min_sec;
	[SerializeField] protected float max_sec;
	protected WaitForSeconds seconds;

	private void Awake() {
		pool_count = npc_pooling.Length;
		//npc_pooling = new GameObject[pool_count];
		for (int i = 0; i < pool_count; i++) {
			//npc_pooling[i] = Instantiate(NPC_prefabs);
			npc_pooling[i].SetActive(false);
		}
	}

	protected void Rnd_Set_Pos() {
		List<Transform> locations_list = new List<Transform>();
		for(int i = 0; i < transform.childCount; i++) {
			locations_list.Add(transform.GetChild(i));
		}

		int rad_val = UnityEngine.Random.Range(0, locations_list.Count);
		start_pos = locations_list[rad_val];
		locations_list.RemoveAt(rad_val);
		rad_val = UnityEngine.Random.Range(0, locations_list.Count);
		end_pos = locations_list[rad_val];
	}

	//protected void NPC_Pooling_ReSize() {
	//	Debug.Log("추가생성");
	//	pool_count += 1;
	//	Array.Resize(ref npc_pooling, pool_count);
	//	npc_pooling[pool_count - 1] = Instantiate(NPC_prefabs);
	//}
}
