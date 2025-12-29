using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecomeTrashState : IState {
	private NPC_Base npc;

	public BecomeTrashState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		npc.agent.isStopped = true;
		//대충 똑같은 오브젝트 소환 및 효과 부여
	}

	public void Update() {
		npc.ChangeState(npc.setPosState);
	}

	public void Exit() {
		npc.agent.isStopped = false;
	}
}
