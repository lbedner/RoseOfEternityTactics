using UnityEngine;
using System.Collections;

public class PlayerSelectedState : PlayerState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerSelectedState.Enter");
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
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);
		controller.ChangeState<PlayerMoveState> ();
	}

	/// <summary>
	/// Gets the cursor conditional impl.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="tileMapPoint">Tile map point.</param>
	protected override bool GetCursorConditionalImpl(Vector3 tileMapPoint) {
		return tileHighlighter.IsHighlightedMovementTile (tileMapPoint) && !tileMap.GetTileMapData ().GetTileDataAt (tileMapPoint).Unit;
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
		}
	}
}