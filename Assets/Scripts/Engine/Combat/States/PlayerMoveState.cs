using UnityEngine;
using System.Collections;

public class PlayerMoveState : PlayerState {

	private Pathfinder _pathfinder;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("PlayerMoveState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		_pathfinder = controller.Pathfinder;
		_pathfinder.GeneratePath (controller.HighlightedUnit.Tile, controller.CurrentTileCoordinates);
		controller.ShowCursorAndTileSelector (false);
		tileHighlighter.RemoveHighlightedTiles ();
		controller.OldUnitTileDirection = controller.HighlightedUnit.FacedDirection;
		controller.HighlightedUnit.Dehighlight ();
		StartCoroutine (Move ());
	}

	/// <summary>
	/// Moves the unit and switches to new state when move is finished.
	/// </summary>
	private IEnumerator Move() {
		controller.OldUnitPosition = controller.HighlightedUnit.Tile;
		yield return StartCoroutine (MoveToTiles ());
		controller.CurrentUnitPosition = controller.HighlightedUnit.Tile;

		controller.ChangeState<MenuSelectionState> ();
	}

	/// <summary>
	/// Moves a unit across x tiles.
	/// </summary>
	private IEnumerator MoveToTiles() {
		Vector3 oldTile = _pathfinder.GetGeneratedPathAt(0);
		Vector3 newTile = Vector3.zero;
		int index = 0;
		while (_pathfinder.GetGeneratedPath() != null && index < _pathfinder.GetGeneratedPath().Count - 1) {
			newTile = _pathfinder.GetGeneratedPathAt(index + 1);
			Vector3 startingPosition = TileMapUtil.TileMapToWorldCentered (_pathfinder.GetGeneratedPathAt(index), tileMap.TileSize);
			Vector3 endingPosition = TileMapUtil.TileMapToWorldCentered (newTile, tileMap.TileSize);

			yield return StartCoroutine(MoveToTile(controller.HighlightedUnit, startingPosition, endingPosition));
			index++;
			yield return null;
		}
		_pathfinder.Clear();

		// After move is finished, swap out tile unit is standing on
		if (!TileMapUtil.IsInvalidTile (oldTile)) {
			TileMapData tileMapData = tileMap.GetTileMapData ();
			TileData oldTileData = tileMapData.GetTileDataAt (oldTile);
			oldTileData.SwapUnits (tileMapData.GetTileDataAt (newTile));
			controller.HighlightedUnit.Tile = newTile;
		}
		yield break;
	}

	/// <summary>
	/// Moves a unit to a tile.
	/// </summary>
	/// <returns>The to tile.</returns>
	/// <param name="character">Character.</param>
	/// <param name="startingPosition">Starting position.</param>
	/// <param name="endingPosition">Ending position.</param>
	private IEnumerator MoveToTile(Unit character, Vector3 startingPosition, Vector3 endingPosition) {
		PlayWalkingAnimation (character, startingPosition, endingPosition);
		float elapsedTime = 0.0f;
		float timeToMove = 0.25f;
		while (elapsedTime < timeToMove) {
			character.transform.position = Vector3.Lerp (startingPosition, endingPosition, (elapsedTime / timeToMove));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	/// <summary>
	/// Plays the walking animation.
	/// </summary>
	/// <param name="unit">Unit.</param>
	/// <param name="sourceTile">Source tile.</param>
	/// <param name="targetTile">Target tile.</param>
	private void PlayWalkingAnimation(Unit unit, Vector3 sourceTile, Vector3 targetTile) {

		// Only run if there is an animation controller
		if (unit.GetAnimationController()) {

			// Get tile direction
			Unit.TileDirection tileDirection = unit.GetDirectionToTarget (
				TileMapUtil.WorldCenteredToTileMap (sourceTile, tileMap.TileSize),
				TileMapUtil.WorldCenteredToTileMap (targetTile, tileMap.TileSize)
			);

			unit.FacedDirection = tileDirection;
			unit.GetAnimationController ().PlayWalkingAnimation (unit);
		}
	}
}