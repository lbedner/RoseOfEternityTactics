using UnityEngine;
using System.Collections;

public class CPUTurnState : CombatState {

	private Unit _cpu;

	private TileMap _tileMap;

	private Pathfinder _pathfinder;

	private bool _hasPersistentHighlightedTiles = false;

	public override void Enter() {
		print ("CPUTurnState.Enter");
		base.Enter ();
		Init ();
	}

	private void Init() {
	    _tileMap = controller.TileMap;
		_pathfinder = controller.Pathfinder;

		_cpu = controller.TurnOrderController.GetNextUp ();
		controller.HighlightedUnit = _cpu;

		// If cpu has persistent highlighted tiles, temporarily remove, then re-apply after move
		_hasPersistentHighlightedTiles = _cpu.TileHighlighter.IsPersistent;
		if (_hasPersistentHighlightedTiles)
			_cpu.TileHighlighter.RemovePersistentHighlightedTiles ();

		_cpu.Highlight ();
		_cpu.Select ();

		// Show terrain related things
		Vector3 cpuTilemapCoordinates = TileMapUtil.WorldCenteredToTileMap (_cpu.transform.position, _tileMap.TileSize);
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (cpuTilemapCoordinates);
		controller.TerrainDetailsController.Activate(tileData);

		// Show character sheet
		_cpu.ActivateCharacterSheet ();

		// Show movement tiles
		_cpu.TileHighlighter.HighlightTiles(_cpu, cpuTilemapCoordinates, false);

		// Get AI Action
		// TODO: Refactor the shit out of all of this. I'm so not proud of this.
		AI ai = new KamiKazeAI(_cpu, _tileMap.GetTileMapData(), controller.TileDiscoverer, _pathfinder);
		_cpu.Action = ai.GetAction ();

		//print (string.Format ("Start CPU Turn: {0}", _selectedCharacter));
		StartCoroutine(Move());		
	}

	/// <summary>
	/// Moves the unit and switches to new state when move is finished.
	/// </summary>
	private IEnumerator Move() {

		yield return StartCoroutine (MoveToTiles ());

		// If unit was persistently selected, re-apply those settings
		if (_hasPersistentHighlightedTiles) {
			_cpu.Select ();
			_cpu.Highlight ();
			controller.TurnOrderController.HighlightUnitImage (_cpu);
			_cpu.TileHighlighter.HighlightPersistentTiles (_cpu, _cpu.Tile);
		}
		else {
			_cpu.Dehighlight ();
			_cpu.Unselect ();
		}

		if (_cpu.Action.Targets != null && _cpu.Action.Targets.Count > 0)
			controller.ChangeState<CPUPerformActionState> ();
		else
			controller.ChangeState<TurnOverState> ();
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
			Vector3 startingPosition = TileMapUtil.TileMapToWorldCentered (_pathfinder.GetGeneratedPathAt(index), _tileMap.TileSize);
			Vector3 endingPosition = TileMapUtil.TileMapToWorldCentered (newTile, _tileMap.TileSize);

			yield return StartCoroutine(MoveToTile(controller.HighlightedUnit, startingPosition, endingPosition));
			index++;
			yield return null;
		}
		_pathfinder.Clear();

		// After move is finished, swap out tile unit is standing on
		if (!TileMapUtil.IsInvalidTile (oldTile)) {
			TileMapData tileMapData = _tileMap.GetTileMapData ();
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
				TileMapUtil.WorldCenteredToTileMap (sourceTile, _tileMap.TileSize),
				TileMapUtil.WorldCenteredToTileMap (targetTile, _tileMap.TileSize)
			);

			unit.FacedDirection = tileDirection;
			unit.GetAnimationController ().PlayWalkingAnimation (unit);
		}
	}
}