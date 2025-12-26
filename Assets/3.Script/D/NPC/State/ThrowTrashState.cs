using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrashState : IState {
	private NPC_Base npc;
	private float timer;

	public ThrowTrashState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		timer = 0f;
		npc.agent.isStopped = true;
	}

	public void Update() {
		timer += Time.deltaTime;
		if(timer >= 2.0f) {
			npc.ChangeState(npc.moveState);
		}
	}

	public void Exit() {
		npc.agent.isStopped = false;
	}
}
