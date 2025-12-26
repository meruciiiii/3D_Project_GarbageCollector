using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour {
	public Action onChangeArea;

	public void Event_ChangeArea(int area) {
		switch(area) {
			case 1:
				//onStage1();
				break;
			case 2:
				//onStage2();
				break;
			case 3:
				//onStage3();
				break;
			case 4:
				//onStage4();
				break;
			default:
				Debug.LogWarning("[Area Error!!] Current Area : " + area);
				break;
		}
	}
}
