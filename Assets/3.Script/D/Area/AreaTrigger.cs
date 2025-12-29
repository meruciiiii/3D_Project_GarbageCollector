using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {
	[Header("AreaManager")]
	[SerializeField] private AreaManager areaManager;

	[Header("현재 구역")]
	[SerializeField] private int area;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("PlayerArea") && !area.Equals(GameManager.instance.Current_Area)) {
			areaManager.ChangeArea(area);
			//Debug.Log("Area 변경!!");
		}
	}
}
