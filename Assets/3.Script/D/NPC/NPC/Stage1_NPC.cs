using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_NPC : NPC_Base {
	[HideInInspector] public IState throwTrashState;

	protected override void Awake() {
		base.Awake();
		isActive = true;
		throwTrashState = new ThrowTrashState(this);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Area")) {
			if(Random.Range(0, 101) <= percent && isActive) {
				ChangeState(throwTrashState);
			}
		}
	}

	protected override void Event_ChangeArea(int area) {
		Debug.Log("NPC ¼ö½Å!" + area);
		if(area.Equals(1)) {isActive = true;}
		else { isActive = false; }
	}
}
