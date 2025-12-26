using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_NPC : NPC_Base {
	[HideInInspector] public IState throwTrashState;

	protected override void Awake() {
		base.Awake();
		throwTrashState = new ThrowTrashState(this);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Area")) {
			if(Random.Range(0, 101) <= percent) {
				ChangeState(throwTrashState);
			}
		}
	}
}
