using UnityEngine;

public class MenuSelectionState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("MenuSelectionState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners ()	{
		controller.AttackButton.onClick.AddListener (OnAttackButtonClicked);
		controller.EndTurnButton.onClick.AddListener (OnEndTurnButtonClicked);
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners () {
		controller.AttackButton.onClick.RemoveListener (OnAttackButtonClicked);
		controller.EndTurnButton.onClick.RemoveListener (OnEndTurnButtonClicked);
	}

	/// <summary>
	/// Raises the attack button clicked event.
	/// </summary>
	private void OnAttackButtonClicked() {
		controller.ChangeState<PlayerTargetSelectionState> ();
	}

	/// <summary>
	/// Raises the end turn button clicked event.
	/// </summary>
	private void OnEndTurnButtonClicked() {
		controller.ChangeState<TurnOverState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.ShowCursor (true);
		controller.ShowTileSelector (false);
		controller.HighlightedUnit.ActivateCombatMenu ();
	}
}