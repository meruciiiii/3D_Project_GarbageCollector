using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour {
	public static AreaManager instance { get; private set; }

	public event Action<int> onAreaChanged;

	public int debug_area = 0;

	private void Awake() {
		if (instance == null) { instance = this; } 
		else { Destroy(gameObject); }
	}

	public void ChangeArea(int area) {
		debug_area = area;
		GameManager.instance.Current_Area = area;
		//이벤트 수신자들에게 area 값 전송

		onAreaChanged.Invoke(area);
		//Debug.Log("보냈음요");
	}
}
