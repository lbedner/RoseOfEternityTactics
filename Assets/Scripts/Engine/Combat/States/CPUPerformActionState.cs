using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		controller.Head2HeadPanel.InstantiateSourceHead2HeadPanel (new List<Unit> { attacker });
		controller.Head2HeadPanel.InstantiateTargetHead2HeadPanel (new List<Unit> { defender });
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