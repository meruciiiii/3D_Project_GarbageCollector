using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {
	[Header("AreaManager")]
	[SerializeField] private AreaManager areaManager;

	[Header("현재 구역")]
	[SerializeField] private int area;
	
	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Player")) {
			if(area.Equals(GameManager.instance.Current_Area)) {
				GameManager.instance.Current_Area = area;
				areaManager.onChangeArea();
				Debug.Log("Area 변경!!");
			}
		}
	}

	//private void OnTriggerExit(Collider other) {
	//	if (other.CompareTag("Player")) {
	//		if (!area.Equals(GameManager.instance.Current_Area)) {
	//			areaManager.onChangeArea();
	//		}
	//	}
	//}
}
