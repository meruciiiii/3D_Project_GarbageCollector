using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_Spawner : NPC_Spawner {
	//Victim�� �ݴ��� 
	//culprit(�߹��˱��� �ƴϰ�, '����˸� ����Ų' �̶�� ���̶�� �մϴ�. ������� �׷���... �����ڱ��� �ƴϴѱ�...)
	[Header("���� ��ġ ��ǥ")]
	[SerializeField] private GameObject cultive_pos;
	[SerializeField] private GameObject cultive_dir;

	private void OnEnable() {
		StartCoroutine(NPC_Spawn_co());
	}

	private void OnDisable() {
		StopAllCoroutines();
	}

	private IEnumerator NPC_Spawn_co() {
		while(true) {
			seconds = new WaitForSeconds(Random.Range(min_sec, max_sec));
			
			int inactiveNpcIndex = -1;
			for (int i = 0; i < pool_count; i++)
			{
				if (!npc_pooling[i].activeSelf)
				{
					inactiveNpcIndex = i;
					break;
				}
			}

			if (inactiveNpcIndex == -1)
			{
				NPC_Pooling_ReSize();
				inactiveNpcIndex = pool_count - 1;
			}

			pool_current = inactiveNpcIndex;

			GameObject victim = npc_pooling[pool_current];
			victim.TryGetComponent(out Stage1_Victim victim_pattern);
			Rnd_Set_Pos();
			victim_pattern.set_pos(start_pos.position, end_pos.position, cultive_pos.transform.position, cultive_dir.transform.position);
			victim.SetActive(true);
			victim_pattern.start();
			yield return seconds;
		}
	}
}