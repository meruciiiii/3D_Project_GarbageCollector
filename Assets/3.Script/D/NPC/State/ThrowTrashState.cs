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
		//쓰레기 투척 애니메이션 활성화!
		npc.transform.LookAt(npc.npc_create_trash.throw_vector.normalized + npc.transform.position);
	}

	public void Update() {
		//쓰레기 투척 방향 바라보기
		timer += Time.deltaTime;
		if(timer >= 2.0f) {
			npc.npc_create_trash.trash_Spawn();
			npc.ChangeState(npc.moveState);
		}
	}

	public void Exit() {
		npc.agent.isStopped = false;
	}
}
