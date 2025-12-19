using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_Controller : MonoBehaviour {
	protected NPC_Create_Trash npc_create_trash;

	[Header("NPC 이동 경로 Vector")]
	[SerializeField] protected Vector3 start_pos;
	[SerializeField] protected Vector3[] middle_pos_array;
	[SerializeField] protected Vector3 end_pos;

	[Header("NPC 높이")]
	[SerializeField] protected float npc_height = 0f;

	[Header("NPC 속도")]
	[SerializeField] private float moveSpeed = 150f;

	[Header("쓰레기 생성 확률")]
	[SerializeField] [Range(0, 100)] protected int cultive_percent;

	private void Awake() {
		TryGetComponent(out npc_create_trash);
	}

	//중간 포인트가 없을 경우
	public void set_pos(Vector3 start_pos, Vector3 end_pos) {
		start_pos.y = npc_height;
		end_pos.y = npc_height;

		this.start_pos = start_pos;
		middle_pos_array = null;
		this.end_pos = end_pos;
	}
	//중간 포인트가 있을 경우
	public void set_pos(Vector3 start_pos, Vector3[] middle_pos_array, Vector3 end_pos) {
		start_pos.y = npc_height;
		if (middle_pos_array != null) {
			for(int i =0; i<middle_pos_array.Length; i++) {
				middle_pos_array[i].y = npc_height;
			}
		}
		end_pos.y = npc_height;

		this.start_pos = start_pos;
		this.middle_pos_array = middle_pos_array;
		this.end_pos = end_pos;
	}

	public virtual void start() {
		revese();
		transform.position = start_pos;
		StartCoroutine(routine_co());
	}
	private void revese() {
		//50% 확률로 역순으로 가는 경우도 하기.
		if(Random.Range(0,2).Equals(0)) {
			Vector3 temp_pos = start_pos;
			start_pos = end_pos;
			end_pos = temp_pos;
			if(middle_pos_array != null) {
				//using System.Linq << 이거 써야 Reverse 가능!!!
				//LINQ(링크)는 Language INtegrated Query의 약어로, C#에서 컬렉션 형태의 데이터를 가공할 때 유용한 메서드를 많이 제공한다.
				//라고 합니다
				middle_pos_array = middle_pos_array.Reverse().ToArray();
			}
		}
	}
	protected virtual IEnumerator routine_co() {
		yield return null;
	}
	protected IEnumerator Move_co(Vector3 target_point) {
		transform.LookAt(target_point);
		while(Vector3.SqrMagnitude(transform.position - target_point) >= 0.00001f) {
			transform.position = Vector3.MoveTowards(transform.position, target_point, moveSpeed * Time.deltaTime);
			yield return null;
		}
		transform.position = target_point;
	}
}
