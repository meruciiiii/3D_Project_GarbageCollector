using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger_forNPC : MonoBehaviour {
	[Header("현재 구역")]
	[SerializeField] private int area;

	private void OnTriggerEnter(Collider other) {
		if(other.CompareTag("NPC")) {
			if(other.TryGetComponent(out Stage3_NPC npc_script)) {
				if (npc_script.inArea) {
					//NPC가 Area 안에 있을 때
					//Trigger 충돌이 난다면, 구역 밖으로 나간다는 의미.
					npc_script.inArea = false;
					npc_script.stop_coroutine();
					npc_script.drop_cnt = 0;
				} else {
					//NPC가 Area 밖에 있을 때
					//Trigger 충돌이 난다면, 구역 안으로 들어온다는 의미.
					npc_script.inArea = true;
					if(area.Equals(GameManager.instance.Current_Area)) {
						//그런데 플레이어가 구역 내에 있을 때만!!
						npc_script.run_coroutine();
					}
				}
			}
		}
	}
}

/*
NPC가 안으로 들어왔을 때.
쓰레기를 버리는걸 활성화 해야한다.
그런데, 플레이어가 도중에 들어오거나 나갈때 코루틴이 어떻게 될 지 모름.
따라서 이에 대한 경우의 수를 생각해야함.
1. 플레이어가 구역에 없을 경우
어떤 경우에도 쓰레기 버리는걸 실행해서는 안됨.

2. 플레이어가 구역에 도중에 들어올 때
- 이미 구역 내에 있는 NPC
run이 된다.
- 구역 밖에 있던 NPC
아무런 행동 X
구역에 드렁오면 run

3. 플레이어가 구역에 존재할 때

*/