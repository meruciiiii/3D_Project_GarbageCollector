using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Create_Trash : MonoBehaviour {
	[Header("쓰레기 프리팹")]
	[SerializeField] private GameObject[] trash_prefabs;

	[Header("던지는 방향, 힘")]
	public Vector3 throw_vector;
	[SerializeField] private float throw_power;


	public void Throw_Trash() {
		GameObject trash_prefab = trash_prefabs[Random.Range(0, trash_prefabs.Length)];
		GameObject trash = Instantiate(trash_prefab);
		Vector3 set_pos = transform.position;
		set_pos.y = transform.position.y + 1f;
		trash.transform.position = set_pos;
		Quaternion randomRotation = Quaternion.Euler(
				Random.Range(0f, 360f),
				Random.Range(0f, 360f),
				Random.Range(0f, 360f)
			);
		trash.transform.rotation = randomRotation;
		trash.TryGetComponent(out Rigidbody trashRB);
		trashRB.AddForce(throw_vector * throw_power, ForceMode.Impulse);
	}
}
