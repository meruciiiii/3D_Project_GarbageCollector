using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3_NPC : NPC_Base {
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Area")) {
			//ChangeState(throwTrashState);
		}
	}
}
