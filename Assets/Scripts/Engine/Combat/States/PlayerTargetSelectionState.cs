using UnityEngine;
using System.Collections.Generic;

public class PlayerTargetSelectionState : PlayerState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerSelectedState.Enter");
		base.Enter ();
		Init ();
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

		// Make sure target is valid
		Unit target = controller.TileMap.GetTileMapData ().GetTileDataAt (controller.CurrentTileCoordinates).Unit;
		if (target == null || target.IsPlayerControlled)
			return;
		
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
		controller.ChangeState<UnitActionConfirmationMenuState> ();
	}

	/// <summary>
	/// Raises the key down escape event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected override void OnKeyDownEscape(object sender, InfoEventArgs<KeyCode> e) {
		if (controller.UnitMenuController.IsActive ())
			return;

		controller.ShowTileSelector (false);
		tileHighlighter.RemoveHighlightedTiles ();
		terrainDetailsController.Deactivate ();
		controller.ClearActionTargets ();
		controller.ChangeState<MenuSelectionState> ();
	}

	/// <summary>
	/// Gets the cursor conditional impl.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="tileMapPoint">Tile map point.</param>
	protected override bool GetCursorConditionalImpl(Vector3 tileMapPoint) {
		return tileHighlighter.IsHighlightedAttackTile (tileMapPoint);
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
		controller.ShowTileSelector (true);
		tileHighlighter.HighlightAttackTiles (controller.HighlightedUnit);
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

			// Clear existing action targets and reset them
			controller.ClearActionTargets ();
			Unit unit = tileData.Unit;
			if (unit)
				controller.HighlightActionTarget (unit);
		}
	}
}