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

	private void Start() {
		//게임 오브젝트 좌표 뽑아내기
		List<Vector3> pos_list = new List<Vector3>();
		for (int i = 0; i < pos_transform_list.Count; i++) {
			pos_list.Add(pos_transform_list[i].position);
		}

		//NPC 풀링
		npc_pooling = new GameObject[pool_size];
		for (int i = 0; i < pool_size; i++) {

			//생성
			npc = Instantiate(NPC_prefab, transform);
			//비활성화
			npc.SetActive(false);
			//위치 임시로 잡아두기 (생성될때 NavMesh 배치 경고 메시지)
			npc.transform.position = pos_list[0];
			//npc에게 좌표 저장
			if (npc.TryGetComponent(out NPC_Base npc_script)) {
				npc_script.pos_list = pos_list;
			}
			//풀링에 npc 저장 
			npc_pooling[i] = npc;
		}

		StartCoroutine(spawn_NPC());
	}

	private IEnumerator spawn_NPC() {
		while (pool_cnt < pool_size) {
			float maxTime = Random.Range(2, 5);
			float timer = 0f;

			npc = npc_pooling[pool_cnt];

			if (npc.TryGetComponent(out NPC_Base npc_script)) {
				npc.SetActive(true);
				npc_script.ChangeState(npc_script.setPosState);
			}

			while (timer < maxTime) {
				timer += Time.deltaTime;
				yield return null;
			}
			pool_cnt += 1;
		}
	}
}
