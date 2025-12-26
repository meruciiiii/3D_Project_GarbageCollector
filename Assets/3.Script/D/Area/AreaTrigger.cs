using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {
	[Header("AreaManager")]
	[SerializeField] private AreaManager areaManager;

	[Header("현재 구역")]
	[SerializeField] private int area;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("PlayerArea")) {
			areaManager.ChangeArea(area);
			//Debug.Log("Area 변경!!");
		} else if (other.CompareTag("NPC")) {
			if (other.TryGetComponent(out Stage3_NPC npc_script)) {
				npc_script.run_coroutine();
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		 if (other.CompareTag("PlayerArea")) {
			if (other.TryGetComponent(out Stage3_NPC npc_script)) {
				npc_script.stop_coroutine();
				npc_script.drop_cnt = 0;
			}
		}
	}
}
