using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour {
	[Header("ZonManager 컴포넌트 가져오기")]
	[SerializeField] private ZoneManager zoneManager;

	[Header("구역")]
	[SerializeField] private int area = 0;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("PlayerArea")) {
			if (!area.Equals(zoneManager.cur_area)) {
				Debug.Log("에리어 변경됨! 현재 에리어 : " + area);
				zoneManager.cur_area = area;
				zoneManager.onChangeArea();
			}
		} else if (other.CompareTag("NPC")) {
			//NPC 특정 뭐시기 활성화
		}
	}

	//private void OnTriggerExit(Collider other) {
	//	if (other.CompareTag("PlayerArea")) {
	//		if (!area.Equals(zoneManager.cur_area)) {
				
	//		}
	//	} else if (other.CompareTag("NPC")) {
	//		//NPC 특정 뭐시기 활성화
	//	}
	//}
}
