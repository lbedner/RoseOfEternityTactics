using UnityEngine;
using System.Collections;

public class CPUPerformActionState : PerformActionState {

	public override void Enter() {
		print ("CPUPerformActionState.Enter");
		base.Enter ();
		StartCoroutine(Init ());
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private IEnumerator Init() {

		Unit attacker = controller.HighlightedUnit;
		Unit defender = attacker.Action.Targets[0];
		GameObject head2HeadPanel = controller.Head2HeadPanel.gameObject;
		GameObject head2HeadPanelConfirmation = head2HeadPanel.transform.Find ("Confirmation").gameObject;

		// Show head 2 head panel for a bit
		controller.SourceHead2HeadPanelController.Load (attacker, Head2HeadPanelController.Head2HeadState.ATTACKING);
		controller.TargetHead2HeadPanelController.Load (defender, Head2HeadPanelController.Head2HeadState.DEFENDING);
		attacker.DeactivateCharacterSheet ();
		head2HeadPanelConfirmation.SetActive (false);
		head2HeadPanel.SetActive (true);

		yield return new WaitForSeconds (2.0f);
		head2HeadPanel.SetActive (false);
		head2HeadPanelConfirmation.SetActive (true);
		controller.Head2HeadPanel.ClearPanels ();

		// Continue performing action
		controller.ClearActionTargets ();
		yield return StartCoroutine (PerformAbilityAction (attacker));	
	}
}