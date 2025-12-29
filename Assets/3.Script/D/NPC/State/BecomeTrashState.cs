
public class BecomeTrashState : IState {
	private NPC_Base npc;

	public BecomeTrashState(NPC_Base npc) { this.npc = npc; }

	public void Enter() {
		npc.agent.isStopped = true;
		//대충 똑같은 오브젝트 소환 및 효과 부여
	}

	public void Update() {
		npc.ChangeState(null);
	}

	public void Exit() {

	}
}
