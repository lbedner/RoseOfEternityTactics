using UnityEngine;
using System.Collections;

public class InitTurnState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("InitTurnState.Enter");
		base.Enter ();
		StartCoroutine(Init ());
	}

	private IEnumerator Init() {
		print ("InitTurnState.Init");
		controller.ShowCursorAndTileSelector (false);
		controller.MissionObjectivesPanel.SetActive (false);

		Unit unit = controller.TurnOrderController.GetNextUp ();
		StartCoroutine (controller.CameraController.MoveToPosition (unit.transform.position));
		controller.HighlightCharacter (unit);

		yield return null;
		if (unit.IsPlayerControlled)
			controller.ChangeState<PlayerTurnState> ();
		else
			controller.ChangeState<CPUTurnState> ();
	}
}