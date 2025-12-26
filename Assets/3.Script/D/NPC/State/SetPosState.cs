using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetPosState : IState {
	private NPC_Base npc;

	public SetPosState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		#region 무작위 좌표 두개 뽑는 코드
		// 무작위로 두개 뽑고, 각각 Start_pos, End_pos에 적용
		// 위치가 겹치지 않게 하기 위한 작업
		List<Vector3> pos_list = new List<Vector3>();
		foreach(Vector3 pos in npc.pos_list) {pos_list.Add(pos);}

		//start_pos 적용
		int index = Random.Range(0, pos_list.Count);
		npc.start_pos = pos_list[index];
		pos_list.RemoveAt(index);
		//end_pos 적용
		index = Random.Range(0, pos_list.Count);
		npc.end_pos = pos_list[index];
		#endregion
	}

	public void Update() {
		npc.ChangeState(npc.spawnState);
	}

	public void Exit() { }
}
