using UnityEngine;

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

		Unit highlightedUnit = controller.HighlightedUnit;

		// Check for a hilighted unit
		if (highlightedUnit == nextUnitInLine) {
			controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
			controller.ChangeState<PlayerSelectedState> ();
		}
		// Do a persistent highlight of tiles in unit range if a unit was selected
		else if (highlightedUnit != null && !highlightedUnit.IsPlayerControlled) {

			// Unit has persistent tiles highlighted around them
			if (highlightedUnit.TileHighlighter.IsPersistent) {
				highlightedUnit.TileHighlighter.RemovePersistentHighlightedTiles ();
				highlightedUnit.Unselect ();
				highlightedUnit.TileHighlighter.HighlightTiles (highlightedUnit, highlightedUnit.Tile);
			}
			else {
				highlightedUnit.Select ();
				highlightedUnit.Highlight ();
				highlightedUnit.TileHighlighter.HighlightPersistentTiles (highlightedUnit, highlightedUnit.Tile);
			}
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

			// Handle highlighting units and unit turn order images
			if (!controller.TurnOrderController.IsImageHighlighted) {
				Unit unit = tileData.Unit;
				if (unit != null) {
					
					// If it's a new highlighted unit, remove highlighted tiles from previous one
					if (controller.HighlightedUnit != null && unit != controller.HighlightedUnit) {
						controller.TurnOrderController.DeHighlightUnitImage ();
						controller.HighlightedUnit.TileHighlighter.RemoveHighlightedTiles ();
					}
					
					unit.ActivateCharacterSheet ();
					unit.TileHighlighter.HighlightTiles (unit, unit.Tile);
					controller.HighlightedUnit = unit;
					controller.TurnOrderController.HighlightUnitImage (unit);
				}
				else {
					if (controller.HighlightedUnit != null) {

						// We only want to de-highlight unit if they don't have persistent tiles highlighted
						if (!controller.HighlightedUnit.TileHighlighter.IsPersistent) {
							controller.TurnOrderController.DeHighlightUnitImage ();
							controller.HighlightedUnit.TileHighlighter.RemoveHighlightedTiles ();
						}
						
						characterSheetController.Deactivate ();
						controller.HighlightedUnit = null;
					}
				}
			}
		}
	}
}