using UnityEngine;
using System.Collections;

public class PlayerTurnState : PlayerState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerTurnState.Enter");
		base.Enter ();
	}

	/// <summary>
	/// Raises the move event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected override void OnMove(object sender, InfoEventArgs<Vector3> e) {
		if (controller.UnitMenuController.IsActive ())
			return;
		HandleCursorOver ();
	}

	/// <summary>
	/// Raises the mouse button left event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected override void OnMouseButtonLeft(object sender, InfoEventArgs<int> e) {
		if (controller.UnitMenuController.IsActive ())
			return;

		// Check for a hilighted unit
		if (controller.HighlightedUnit == nextUnitInLine) {
			controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
			controller.ChangeState<PlayerSelectedState> ();
		}
	}

	/// <summary>
	/// Handles the cursor over event.
	/// </summary>
	/// <param name="ray">Ray.</param>
	private void HandleCursorOver() {
		
		TileData tileData = GetCursorCurrentTileData ();
		if (tileData != null) {

			controller.CursorMoveSource.PlayOneShot (controller.CursorMoveSource.clip);

			// Move selection icon
			selectionIcon.transform.position = controller.CurrentTileCoordinates * tileMap.TileSize;

			// Show terrain UI
			terrainDetailsController.Activate(tileData);

			// Handle highlighting units
			Unit unit = tileData.Unit;
			if (unit != null) {
				unit.ActivateCharacterSheet ();
				tileHighlighter.HighlightTiles (unit, unit.Tile);
				controller.HighlightedUnit = unit;
			} 
			else {
				characterSheetController.Deactivate ();
				tileHighlighter.RemoveHighlightedTiles ();
				controller.HighlightedUnit = null;
			}
		}
	}
}