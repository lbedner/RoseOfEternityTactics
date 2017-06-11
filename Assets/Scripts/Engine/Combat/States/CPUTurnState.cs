using UnityEngine;
using System.Collections;

public class CPUTurnState : CombatState {

	private TileMap _tileMap;

	private Pathfinder _pathfinder;

	public override void Enter() {
		print ("CPUTurnState.Enter");
		base.Enter ();
		Init ();
	}

	private void Init() {

	    _tileMap = controller.TileMap;
		_pathfinder = controller.Pathfinder;

		Unit cpu = controller.TurnOrderController.GetNextUp ();
		controller.HighlightedUnit = cpu;

		// Show terrain related things
		Vector3 cpuTilemapCoordinates = TileMapUtil.WorldCenteredToTileMap (cpu.transform.position, _tileMap.TileSize);
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (cpuTilemapCoordinates);
		controller.TerrainDetailsController.Activate(tileData);

		// Show character sheet
		cpu.ActivateCharacterSheet ();

		// Show movement tiles
		controller.TileHighlighter.HighlightTiles(cpu, cpuTilemapCoordinates, false);

		// Get AI Action
		// TODO: Refactor the shit out of all of this. I'm so not proud of this.
		AI ai = new KamiKazeAI(cpu, _tileMap.GetTileMapData(), controller.TileDiscoverer, _pathfinder);
		cpu.Action = ai.GetAction ();
		print (cpu.Action);

		//print (string.Format ("Start CPU Turn: {0}", _selectedCharacter));
		StartCoroutine(Move());		
	}

	/// <summary>
	/// Moves the unit and switches to new state when move is finished.
	/// </summary>
	private IEnumerator Move() {
		yield return StartCoroutine (MoveToTiles ());
		Unit target = controller.HighlightedUnit.Action.Target;
		if (target != null)
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