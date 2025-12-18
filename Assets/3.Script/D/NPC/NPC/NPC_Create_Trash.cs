using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Create_Trash : MonoBehaviour {
	[Header("쓰레기 프리팹")]
	[SerializeField] private GameObject[] trash_prefabs;

	[Header("던지는 방향, 힘")]
	[SerializeField] private Vector3 throw_vector;
	[SerializeField] private float throw_power;


	public void Throw_Trash() {
		GameObject trash_prefab = trash_prefabs[Random.Range(0, trash_prefabs.Length)];
		GameObject trash = Instantiate(trash_prefab);
		trash.transform.position = transform.position;
		trash.TryGetComponent(out Rigidbody trashRB);
		trashRB.AddForce(throw_vector * throw_power, ForceMode.Impulse);
	}
}
