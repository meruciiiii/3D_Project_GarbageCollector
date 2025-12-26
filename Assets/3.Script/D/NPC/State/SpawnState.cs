using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnState : IState {
	private NPC_Base npc;

	public SpawnState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		npc.agent.Warp(npc.start_pos);

		npc.agent.isStopped = false;
	}

	public void Update() {
		npc.ChangeState(npc.moveState);
	}

	public void Exit() { }
}
