﻿using UnityEngine;
using System.Collections;

public class UnitActionConfirmationMenuState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("UnitActionConfirmationMenuState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners() {
		controller.ActionConfirmButton.onClick.AddListener (OnActionConfirmButtonClicked);
		controller.ActionCancelButton.onClick.AddListener (OnActionCancelButtonClicked);
		InputController.keyDownEscapeEvent += OnKeyDownEscape;
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners () {
		controller.ActionConfirmButton.onClick.RemoveListener (OnActionConfirmButtonClicked);
		controller.ActionCancelButton.onClick.RemoveListener (OnActionCancelButtonClicked);
		InputController.keyDownEscapeEvent -= OnKeyDownEscape;
	}

	/// <summary>
	/// Raises the action confirm button clicked event.
	/// </summary>
	private void OnActionConfirmButtonClicked() {
		controller.ChangeState<PlayerPerformActionState> ();
	}

	/// <summary>
	/// Raises the action cancel button clicked event.
	/// </summary>
	private void OnActionCancelButtonClicked() {
		CancelAction ();
	}

	/// <summary>
	/// Raises the key down escape event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnKeyDownEscape(object sender, InfoEventArgs<KeyCode> e) {
		CancelAction ();
	}

	/// <summary>
	/// Cancels the action.
	/// </summary>
	/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
	private void CancelAction() {
		controller.Head2HeadPanel.SetActive (false);
		controller.HighlightedUnit.ActivateCharacterSheet ();
		controller.HighlightedUnit.Action.ClearTargets ();
		controller.ChangeState<PlayerTargetSelectionState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		if (!controller.Head2HeadPanel.activeInHierarchy) {
			controller.SourceHead2HeadPanelController.Load (controller.HighlightedUnit, Head2HeadPanelController.Head2HeadState.ATTACKING);
			controller.TargetHead2HeadPanelController.Load (controller.HighlightedUnit.Action.Targets[0], Head2HeadPanelController.Head2HeadState.DEFENDING);
			controller.Head2HeadPanel.SetActive (true);
			controller.HighlightedUnit.DeactivateCharacterSheet ();
		}		
	}
}