using UnityEngine;
using System.Collections.Generic;

using EternalEngine;

public class PlayerTargetSelectionState : PlayerState {

	private int _range = 0;
	private int _aoeRange = 0;
	List<TileData> _tilesInRange = new List<TileData> ();

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
		if (controller.UnitMenuController.IsActive () || controller.HighlightedUnit.Action.Ability.TargetType == Ability.TargetTypeEnum.SELF)
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

		// Make sure there is at least one target
		foreach (var tileInRange in _tilesInRange) {
			Unit target = tileInRange.Unit;
			if (target != null && !target.IsPlayerControlled)
				controller.HighlightedUnit.Action.Targets.Add (target);
		}

		// If no target, bail out
		if (controller.HighlightedUnit.Action.Targets.Count == 0)
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
		controller.SelectionIndicator.ClearIndicators ();
		tileHighlighter.RemoveHighlightedTiles ();
		terrainDetailsController.Deactivate ();
		controller.ClearActionTargets ();
		_aoeRange = 0;
		_range = 0;
		_tilesInRange.Clear ();
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
		_range = 0;
		_aoeRange = 0;
		_tilesInRange.Clear ();
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);

		// Determine the selection indicator based on action
		Ability ability = controller.HighlightedUnit.Action.Ability;
		if (ability != null && ability.Id != AbilityConstants.ATTACK) {

			// Get AOE Range
			_aoeRange = ability.GetAOERange();

			// Get Range
			_range = ability.GetRange();			
		}
		else
			_range = controller.HighlightedUnit.GetWeaponRange ();

		// Widen selection indicator
		if (_aoeRange > 0)
			controller.SelectionIndicator.SetAreaOfEffectIndicators (_aoeRange);
		
		controller.ShowTileSelector (true);
		tileHighlighter.HighlightAttackTiles (controller.HighlightedUnit, _range);

		if (controller.HighlightedUnit.Action.Ability.TargetType == Ability.TargetTypeEnum.SELF) {

			TileData tileData = controller.TileMap.GetTileMapData ().GetTileDataAt (controller.HighlightedUnit.Tile);
			ProcessTilesInRange (tileData);
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

			// Clear existing action targets and reset them
			controller.ClearActionTargets ();
			controller.TurnOrderController.UntargetUnitImages ();
			_tilesInRange.Clear ();

			ProcessTilesInRange (tileData);
		}
	}

	/// <summary>
	/// Processes the tiles in range.
	/// </summary>
	/// <param name="tiledata">Tiledata.</param>
	private void ProcessTilesInRange(TileData tiledata) {

		// Add current tile data to a list of potential tiles to check
		_tilesInRange.Add (tiledata);

		// If selection indicator is more than one (i.e. Area of Effect), get surrounding tiles
		if (_aoeRange > 0) {
			var tilesInRange = controller.TileDiscoverer.DiscoverTilesInRange (tiledata.Position, _aoeRange);
			foreach (var tileInRange in tilesInRange)
				_tilesInRange.Add(controller.TileMap.GetTileMapData().GetTileDataAt(tileInRange.Key));
		}

		// Iterate over list of tile data and if there are units standing on them, process them accordingly
		foreach (var data in _tilesInRange) {
			Unit unit = data.Unit;
			if (unit && !(controller.HighlightedUnit.IsFriendlyUnit(unit))) {
				controller.HighlightActionTarget (unit);
				controller.TurnOrderController.TargetUnitImage (unit);
			}
		}
		
	}
}