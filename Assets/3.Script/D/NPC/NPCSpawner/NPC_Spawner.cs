using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Spawner : MonoBehaviour {
	[Header("NPC 프리팹 넣기!!!!")]
	[Tooltip("리스트 순서는 꼭 Victim / Culprit으로 하기.")]
	[SerializeField] protected GameObject[] NPC_list = new GameObject[2];

	[Header("이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3[] middle_pos_array;
	[SerializeField] protected Vector3 end_pos;

	private void Awake() {
		int childCount = transform.childCount;
		start_pos = transform.GetChild(0).transform.position;
		if(childCount != 2) {
			middle_pos_array = new Vector3[childCount-2];
			for(int i = 1; i < childCount - 1; i++) {
				middle_pos_array[i] = transform.GetChild(i).transform.position;
			}
		}
		end_pos = transform.GetChild(childCount - 1).transform.position;
	}
}
