using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour {
	[Header("NPC 프리팹")]
	[SerializeField] protected GameObject NPC;

	[Header("이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3[] middle_pos_array;
	[SerializeField] protected Vector3 end_pos;

	[Header("NPC 생성 시간 간격 (단위 : 초)")]
	[SerializeField] protected float min_sec;
	[SerializeField] protected float max_sec;
	protected WaitForSeconds seconds;

	private void Awake() {
		int childCount = transform.childCount;
		start_pos = transform.GetChild(0).transform.position;
		if(childCount != 2) {
			middle_pos_array = new Vector3[childCount-2];
			for(int i = 1; i < childCount - 1; i++) {
				Debug.Log(transform.GetChild(i).transform.name);
				middle_pos_array[i-1] = transform.GetChild(i).transform.position;
			}
		}
		end_pos = transform.GetChild(childCount - 1).transform.position;
	}
}
