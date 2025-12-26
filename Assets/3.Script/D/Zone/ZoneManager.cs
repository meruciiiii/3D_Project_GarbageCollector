using System;
using UnityEngine;

public class ZoneManager : MonoBehaviour {
	public Action onChangeArea;
	public int cur_area;

	private void Start() {
		cur_area = GameManager.instance.Current_Area;
		onChangeArea += Change_Area;
	}

	private void Change_Area() {
		GameManager.instance.Current_Area = cur_area;
	}

}
