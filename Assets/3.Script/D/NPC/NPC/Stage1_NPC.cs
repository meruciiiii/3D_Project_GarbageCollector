using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_NPC : NPC_Base {
	[HideInInspector] public IState throwTrashState;

	//쓰레기 생성 구역 부모 지정
	private Transform area_object;

	protected override void Awake() {
		base.Awake();
		isActive = true;
		throwTrashState = new ThrowTrashState(this);
		area_object = GameObject.FindGameObjectWithTag("Area01").transform;
		npc_create_trash.area = area_object;
	}

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Area")) {
			if(Random.Range(0, 101) <= percent && isActive) {
				ChangeState(throwTrashState);
			}
		}
	}

	protected override void Event_ChangeArea(int area) {
		//Debug.Log("NPC 수신!" + area);
		if(area.Equals(1)) {isActive = true;}
		else { isActive = false; }
	}
}
