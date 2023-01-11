using UnityEngine;
using System.Collections;

public class DisplayPostCombatStatsState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("DisplayPostCombatStatsState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners() {
		controller.MissionEndPanelContinueButton.onClick.AddListener (OnContinueButtonClicked);
		InputController.selectEvent += OnSelect;
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners() {
		controller.MissionEndPanelContinueButton.onClick.RemoveListener (OnContinueButtonClicked);
		InputController.selectEvent -= OnSelect;
	}

	/// <summary>
	/// Raises the continue button clicked event.
	/// </summary>
	private void OnContinueButtonClicked() {
		controller.ChangeState<EndCombatState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.PostCombatStatsPanel.SetActive (true);
		controller.ShowCursor (true);
	}

	private void OnSelect(object sender, InfoEventArgs<int> e)
	{
		print(string.Format("{0}.OnSelect", this));
		OnContinueButtonClicked();
	}
}