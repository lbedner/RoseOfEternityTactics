using UnityEngine;
using System.Collections;

public class DisplayMissionObjectivesState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners () {
		controller.MissionObjectivesPanelContinueButton.onClick.AddListener (OnContinueButtonClicked);
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners() {
		controller.MissionObjectivesPanelContinueButton.onClick.RemoveListener (OnContinueButtonClicked);
	}

	/// <summary>
	/// Raises the continue button clicked event.
	/// </summary>
	private void OnContinueButtonClicked() {
		controller.ChangeState<InitTurnState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.MissionObjectivesPanel.SetActive (true);
		controller.ShowCursor (true);
	}
}