using UnityEngine;
using System.Collections;

public class UnitActionConfirmationMenuState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerSelectedState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners() {
		controller.ActionConfirmButton.onClick.AddListener (OnActionConfirmButtonClicked);
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners () {
		controller.ActionConfirmButton.onClick.RemoveListener (OnActionConfirmButtonClicked);
	}

	/// <summary>
	/// Raises the action confirm button clicked event.
	/// </summary>
	private void OnActionConfirmButtonClicked() {
		controller.ChangeState<PlayerPerformActionState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		if (!controller.Head2HeadPanel.activeInHierarchy) {
			controller.SourceHead2HeadPanelController.Load (controller.HighlightedUnit, Head2HeadPanelController.Head2HeadState.ATTACKING);
			controller.TargetHead2HeadPanelController.Load (controller.TileMap.GetTileMapData ().GetTileDataAt (controller.CurrentTileCoordinates).Unit, Head2HeadPanelController.Head2HeadState.DEFENDING);
			controller.Head2HeadPanel.SetActive (true);
			controller.HighlightedUnit.DeactivateCharacterSheet ();
		}		
	}
}