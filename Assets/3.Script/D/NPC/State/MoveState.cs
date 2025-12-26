using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState {
	private NPC_Base npc;

	public MoveState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		npc.agent.SetDestination(npc.end_pos);
	}

	public void Update() {
		//if(Vector3.SqrMagnitude(npc.transform.position - npc.end_pos) >= 0.01f) {
		//	//npc.ChangeState(npc.setPosState);
		//}
	}

	public void Exit() {
		npc.agent.isStopped = true;
	}
}
