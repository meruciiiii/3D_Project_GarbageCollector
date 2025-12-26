using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spanwer : MonoBehaviour {
	[Header("NPC 프리팹")]
	[SerializeField] private GameObject NPC_prefab;
	private GameObject npc;

	[Header("소환할 NPC 수")]
	[SerializeField] private int pool_size = 10; //기본 10명

	private GameObject[] npc_pooling;
	private int pool_cnt = 0;

	[Header("좌표들 설정값")]
	[SerializeField] private List<Transform> pos_transform_list;
	private List<Vector3> pos_list;

	private void Start() {
		List<Vector3> pos_list = new List<Vector3>();
		for (int i = 0; i < pos_transform_list.Count; i++) {
			pos_list.Add(pos_transform_list[i].position);
		}

		npc_pooling = new GameObject[pool_size];

		for (int i = 0; i < pool_size; i++) {
			npc = Instantiate(NPC_prefab);
			npc.transform.position = pos_list[0];
			npc.SetActive(false);
			if (npc.TryGetComponent(out NPC_Base npc_script)) {
				npc_script.pos_list = pos_list;
			}
			npc_pooling[i] = npc;
		}

		StartCoroutine(spawn_NPC());
	}

	private IEnumerator spawn_NPC() {
		WaitForSeconds wfs = new WaitForSeconds(3f);
		while (pool_cnt < pool_size) {
			npc = npc_pooling[pool_cnt];

			if (npc.TryGetComponent(out NPC_Base npc_script)) {
				npc.SetActive(true);
				npc_script.ChangeState(npc_script.setPosState);
			}

			pool_cnt += 1;
			yield return wfs;
		}
	}
}
