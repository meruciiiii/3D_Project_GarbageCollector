using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Re_NPC_Controller : MonoBehaviour {
	private NavMeshAgent agent;
	private Vector3 start_pos;
	private Vector3 end_pos;
	
	private Re_NPC_Controller(Vector3 start_pos, Vector3 end_pos) {
		this.start_pos = start_pos;
		this.end_pos = end_pos;
	}

	private void Awake() {
		TryGetComponent(out agent);
	}

	private void Move() {
		transform.position = start_pos;
		agent.SetDestination(end_pos);
	}

	private void Respawn() {
		
	}
}
