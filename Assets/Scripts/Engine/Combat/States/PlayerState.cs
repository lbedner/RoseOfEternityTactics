using UnityEngine;

public class PlayerState : CombatState {

	protected Unit nextUnitInLine;

	protected Transform selectionIcon;

	protected CharacterSheetController characterSheetController;
	protected TerrainDetailsController terrainDetailsController;
	protected TileMap tileMap;

	protected TileHighlighter tileHighlighter;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners() {
		CameraController.moveEvent += OnMove;
		InputController.mouseMoveEvent += OnMove;
		InputController.mouseButtonLeftEvent += OnMouseButtonLeft;
		InputController.keyDownInventoryEvent += OnKeyDownInventory;
		InputController.keyDownEscapeEvent += OnKeyDownEscape;
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners() {
		CameraController.moveEvent -= OnMove;
		InputController.mouseMoveEvent -= OnMove;
		InputController.mouseButtonLeftEvent -= OnMouseButtonLeft;
		InputController.keyDownInventoryEvent -= OnKeyDownInventory;
		InputController.keyDownEscapeEvent -= OnKeyDownEscape;
	}

	/// <summary>
	/// Raises the move event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnMove(object sender, InfoEventArgs<Vector3> e) {}

	/// <summary>
	/// Raises the mouse button left event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnMouseButtonLeft(object sender, InfoEventArgs<int> e) {}

	/// <summary>
	/// Raises the key down inventory event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnKeyDownInventory(object sender, InfoEventArgs<KeyCode> e) {
		UnitMenuController unitMenuController = controller.UnitMenuController;
		if (unitMenuController.IsActive ()) {
			controller.ShowTileSelector (true);
			unitMenuController.Deactivate ();
		} 
		else {
			controller.ShowTileSelector (false);
			unitMenuController.Activate (nextUnitInLine);
		}
	}

	/// <summary>
	/// Raises the key down escape event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	protected virtual void OnKeyDownEscape(object sender, InfoEventArgs<KeyCode> e) {}

	/// <summary>
	/// Gets the cursor conditional impl.
	/// </summary>
	/// <returns><c>true</c>, if cursor conditional impl was gotten, <c>false</c> otherwise.</returns>
	/// <param name="tileMapPoint">Tile map point.</param>
	protected virtual bool GetCursorConditionalImpl(Vector3 tileMapPoint) {
		return true;
	}

	/// <summary>
	/// Gets the cursor's current tile data.
	/// This will only return it if it's new.
	/// </summary>
	/// <returns>The cursor tile data.</returns>
	protected TileData GetCursorCurrentTileData() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		if (tileMap.GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {

			// Get hit point on tile map coordinates
			Vector3 tileMapPoint = TileMapUtil.WorldCenteredToTileMap (hitInfo.point, tileMap.TileSize);

			// Don't run if still on the same tile
			if ((controller.CurrentTileCoordinates.x != tileMapPoint.x || controller.CurrentTileCoordinates.z != tileMapPoint.z) && GetCursorConditionalImpl(tileMapPoint)) {
				Vector3 tmp = controller.CurrentTileCoordinates;
				tmp.x = tileMapPoint.x;
				tmp.z = tileMapPoint.z;
				controller.CurrentTileCoordinates = tmp;

				TileData tileData = tileMap.GetTileMapData ().GetTileDataAt (tileMapPoint);

				return tileData;
			}
		}
		return null;
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {

		selectionIcon = controller.SelectionIcon;
		characterSheetController = controller.CharacterSheetController;
		terrainDetailsController = controller.TerrainDetailsController;
		tileMap = controller.TileMap;

		nextUnitInLine = controller.TurnOrderController.GetNextUp ();
		tileHighlighter = nextUnitInLine.TileHighlighter;

		controller.ShowCursorAndTileSelector (true);
	}
}