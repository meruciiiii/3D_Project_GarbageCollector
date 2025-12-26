using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2_Window : MonoBehaviour {
	private NPC_Create_Trash npc_create_trash;
	private bool isEnable;

	[Header("쓰레기 생성 확률")]
	[SerializeField] [Range(0, 100)] private int percent;

	[Header("쓰레기 드랍 주기")]
	[SerializeField] private float min_sec;
	[SerializeField] private float max_sec;
	//private WaitForSeconds seconds;

	[Header("AreaManager")]
	[SerializeField] private AreaManager areaManager;

	private void Awake() {
		TryGetComponent(out npc_create_trash);
	}

	private void Start() {
		areaManager.onChangeArea += Event_ChangeArea;
	}

	private void Event_ChangeArea() {
		isEnable = !isEnable;
	}

	private IEnumerator throw_trash() {
		//왜 WaitforSeconds를 안쓰는지?
		//주기의 시간을 랜덤하게 갱신해야하므로, 결국 WFS를 계속 선언해야하는데
		//이건 메모리(Heap)에 지속적으로 가비지를 남김.

		//- WaitForSeconds
		// C#은 값 형식과 참조 형식으로 나뉘는데, new를 쓰면 대부분 Heap에 저장한다고 함.
		// 이건 코루틴이 끝나더라도 바로 사라지지 않고, 나중에 가비지 컬렉터가 수거할때 한번에 지움.
		// 게임이 순간적으로 버벅일 수 있음.

		//- float timer
		// 값 형식은 Stack 메모리 영역을 사용하거나, CPU 레지스터에서 처리함.
		// 함수나 루프가 끝나면 즉시 자동으로 제거가 되는건 같지만, 가비지 컬렉터가 관여할 쓰레기는 존재하지 않음.
		// 더하기 연산은 수만 번 반복해도 생각보다 메모리 누수가 없는 가벼운 연산임.

		//- 그러니까...
		//메모리 누수 (메모리 과부하) 영향은 가비지 컬렉터를 호출하냐 안하냐 차이.
		while(isEnable) {
			float maxTime = Random.Range(min_sec, max_sec);
			float timer = 0f;

			if(Random.Range(0, 101) <= percent) {
				npc_create_trash.trash_Spawn();
			}
			while(timer < maxTime) {
				timer += Time.deltaTime;
				yield return null;
			}
		}
	}
}
