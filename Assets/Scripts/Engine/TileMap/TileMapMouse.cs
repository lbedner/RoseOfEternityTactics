using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {

	public enum GameState {
		INITIALIZE_TURN,		
		PLAYER_TURN,
		PLAYER_MOVE_SELECTION,
		PLAYER_MOVE_START,
		PLAYER_MOVE_STOP,
		PLAYER_SHOW_ATTACK_INDICATORS,
		PLAYER_ATTACK_SELECTION,
		CPU_TURN,
		CPU_MOVE_STOP,
		COMBAT_MENU,
		TURN_OVER,
	}

	protected const float HIGHTLIGHT_COLOR_TRANSPARENCY = 0.7f;

	public Transform selectionCube;
	public Transform movementHighlightCube;

	public TerrainDetailsController terrainDetailsController;

	// Audio related things
	public AudioSource cursorMoveSource;
	public AudioSource confirmationSource;
	
	private TileMap _tileMap;
	private Pathfinder _pathfinder;
	
	private Vector3 _currentTileCoord;
	private Vector3 _playerCurrentTileCoordinate;

	private Unit _highlightedCharacter;
	private Unit _selectedCharacter;

	private Dictionary<Vector3, GameObject> _attackIndicators = new Dictionary<Vector3, GameObject>();

	private Color _attack_highlight_color = new Color (1.0f, 0.0f, 0.0f, HIGHTLIGHT_COLOR_TRANSPARENCY);

	private GameState _gameState;

	private bool _playerMoveFinished = false;

	private GameManager _gameManager;

	private MusicController _musicController;

	private TileHighlighter _tileHighlighter;

	void Start() {
		_tileMap = GetComponent<TileMap>();
		_gameManager = GameManager.Instance;
		_gameState = GameState.INITIALIZE_TURN;
		_musicController = _gameManager.GetMusicController ();
		_musicController.Initialize ();
		_tileHighlighter = new TileHighlighter (_tileMap, movementHighlightCube);
	}

	public void TransitionGameState(GameState newState) {
		_gameState = newState;
	}		

	// Update is called once per frame
	void Update () {

		switch (_gameState) {

		case GameState.INITIALIZE_TURN:
			ShowCursor (false);
			Unit unit = _gameManager.GetTurnOrderController ().GetNextUp ();
			Debug.Log (string.Format ("Initialzie Turn: {0}", unit));
			StartCoroutine (_gameManager.GetCameraController ().MoveToPosition (unit.transform.position));
			HighlightCharacter (unit);

			if (unit.IsPlayerControlled)
				_gameState = GameState.PLAYER_TURN;
			else
				_gameState = GameState.CPU_TURN;
			break;

		// Player is moving cursor around the tile map
		case GameState.PLAYER_TURN:
			if (!_gameManager.GetCameraController ().IsMoving) {
				ShowCursor (true);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				HandleTerrainMouseOver (ray);
				HandleCharacterMouseOver (ray);

				if (_selectedCharacter != null && _gameManager.GetTurnOrderController ().GetNextUp () == _selectedCharacter)
					_gameState = GameState.PLAYER_MOVE_SELECTION;
			}
			break;

		// Player can select a tile to move to
		case GameState.PLAYER_MOVE_SELECTION:
			HandleMovementSelection ();
			if ((Input.GetMouseButtonDown (0))) {
				_pathfinder = new Pathfinder ();
				List<Node> generatedPath = _pathfinder.GeneratePath(
					_tileMap.GetGraph().GetGraph(),
					(int) TileMapUtil.WorldCenteredToTileMap(_selectedCharacter.transform.position, _tileMap.tileSize).x,
					(int) TileMapUtil.WorldCenteredToTileMap(_selectedCharacter.transform.position, _tileMap.tileSize).z,
					(int) _currentTileCoord.x,
					(int) _currentTileCoord.z
				);
				foreach (var node in generatedPath)
					Debug.Log(node);

				_gameState = GameState.PLAYER_MOVE_START;
				confirmationSource.PlayOneShot (confirmationSource.clip);
			}
			break;
		
		// Player moves to tile
		case GameState.PLAYER_MOVE_START:
			ShowCursor (false);
			_tileHighlighter.RemoveMovementTiles();
			StartCoroutine (MoveToTiles ());
			TransitionGameState (GameState.PLAYER_MOVE_STOP);
			break;

		// Player has stopped moving
		case GameState.PLAYER_MOVE_STOP:
			if (_playerMoveFinished) {
				_playerMoveFinished = false;
				_playerCurrentTileCoordinate = _currentTileCoord;
				TransitionGameState (GameState.TURN_OVER);
			}
			break;

		// Indicate where player can attack
		case GameState.PLAYER_SHOW_ATTACK_INDICATORS:
			_tileHighlighter.RemoveMovementTiles();
			HighlightAttackTiles (_selectedCharacter);
			TransitionGameState (GameState.PLAYER_ATTACK_SELECTION);
			break;

		// Select who to attack
		case GameState.PLAYER_ATTACK_SELECTION:
			break;

		// Player can make choices from combat menu
		case GameState.COMBAT_MENU:
			break;

		// CPU starts their turn
		case GameState.CPU_TURN:
			if (!_gameManager.GetCameraController ().IsMoving) {
				Unit cpu = _gameManager.GetTurnOrderController ().GetNextUp ();
				_selectedCharacter = cpu;

				// Show terrain related things
				Vector3 cpuTilemapCoordinates = TileMapUtil.WorldCenteredToTileMap (cpu.transform.position, _tileMap.tileSize);
				ShowTerrainUI ((int) cpuTilemapCoordinates.x, (int) cpuTilemapCoordinates.z);
				_playerCurrentTileCoordinate = cpuTilemapCoordinates;

				// Show character sheet
				cpu.ActivateCharacterSheet ();

				// Show movement tiles
				_tileHighlighter.HighlightMovementTiles(cpu, _playerCurrentTileCoordinate);

				StartCoroutine (MoveCPU ());
				TransitionGameState (GameState.CPU_MOVE_STOP);
			}
			break;

		// CPU is done moving
		case GameState.CPU_MOVE_STOP:
			if (_playerMoveFinished) {
				_playerMoveFinished = false;
				TransitionGameState (GameState.TURN_OVER);
			}
			break;

		// Turn is over for the character
		case GameState.TURN_OVER:
			_gameManager.GetTurnOrderController ().FinishTurn (_selectedCharacter);
			_selectedCharacter = null;
			if (IsEnemyNearby (_gameManager.GetTurnOrderController().GetAllUnits()))
				_musicController.TransitionMusic (false);
			else
				_musicController.TransitionMusic (true);
			TransitionGameState(GameState.INITIALIZE_TURN);
			break;
		}
	}

	private void HighlightCharacter(Unit character) {
		selectionCube.position = TileMapUtil.WorldCenteredToUncentered(character.transform.position, _tileMap.tileSize);
	}

	private void HandleTerrainMouseOver(Ray ray) {
		
		RaycastHit hitInfo;
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			
			// Get hit point in tile map coordinates
			Vector3 tileMapPoint = TileMapUtil.WorldCenteredToTileMap (hitInfo.point, _tileMap.tileSize);

			// Don't run if still on the same tile
			if (_currentTileCoord.x != tileMapPoint.x || _currentTileCoord.z != tileMapPoint.z) {
				_currentTileCoord.x = tileMapPoint.x;
				_currentTileCoord.z = tileMapPoint.z;

				cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

				selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
				ShowTerrainUI ((int) tileMapPoint.x, (int) tileMapPoint.z);
			}
		}
	}
		
	private void HandleMovementSelection() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {

			// Get hit point in tile map coordinates
			Vector3 tileMapPoint = TileMapUtil.WorldCenteredToTileMap (hitInfo.point, _tileMap.tileSize);

			// Don't run if still on the same tile
			if (_currentTileCoord.x != tileMapPoint.x || _currentTileCoord.z != tileMapPoint.z) {

				// Don't allow outside movement
				if (_tileHighlighter.IsHighlightedTile(tileMapPoint)) {
					_currentTileCoord.x = tileMapPoint.x;
					_currentTileCoord.z = tileMapPoint.z;

					cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

					selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
					ShowTerrainUI ((int) tileMapPoint.x, (int) tileMapPoint.z);
				}
			}
		}
	}

	/// <summary>
	/// Shows the terrain UI.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private void ShowTerrainUI(int x, int z) {
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (x, z);
		terrainDetailsController.Activate (tileData);
	}

	private bool IsPlayerRaycastHitNew(RaycastHit hintinfo) {
		
		// Get hit point in tile map coordinates
		Vector3 tileMapPoint = TileMapUtil.WorldCenteredToTileMap (hintinfo.point, _tileMap.tileSize);

		if (_playerCurrentTileCoordinate.x != tileMapPoint.x || _playerCurrentTileCoordinate.z != tileMapPoint.z) {
			_playerCurrentTileCoordinate.x = tileMapPoint.x;
			_playerCurrentTileCoordinate.z = tileMapPoint.z;

			return true;
		}
		return false;
	}

	private void HandleCharacterMouseOver(Ray ray) {

		// If a player is currently selected, fall back
		if (_selectedCharacter != null)
			return;
		
		// Iterate over all characters to see if they're being selected and/or highlighted
		RaycastHit hitInfo;
		foreach (Unit unit in _gameManager.GetTurnOrderController().GetAllUnits()) {
			if (unit.GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {

				// Only run this if the cursor has moved
				if (IsPlayerRaycastHitNew(hitInfo)) {

					// Mark player as highlighted and show character sheet
					Debug.Log (unit);
					_highlightedCharacter = unit;
					unit.ActivateCharacterSheet ();

					// Show tiles you can move to
					_tileHighlighter.HighlightMovementTiles(unit, _playerCurrentTileCoordinate);
				}

				// If user has clicked, show combat menu
				if ((Input.GetMouseButtonDown (0)) && _selectedCharacter == null && unit == _gameManager.GetTurnOrderController().GetNextUp()) {
					_selectedCharacter = unit;
					confirmationSource.PlayOneShot (confirmationSource.clip);
				}
				return;
			}
		}

		// If character had been previously highlighted, undo it
		if (_highlightedCharacter != null) {

			// If player is selected, don't get rid of character sheet and movement highlight
			if (_selectedCharacter == null) {
				_highlightedCharacter.DeactivateCharacterSheet ();
				_highlightedCharacter = null;
				_playerCurrentTileCoordinate = Vector3.zero;

				// Delete movement highlight if player is not selected
				_tileHighlighter.RemoveMovementTiles();
			}
		}
	}

	/// <summary>
	/// Highlights the attack tiles for a character.
	/// </summary>
	/// <param name="player">Player.</param>
	private void HighlightAttackTiles(Unit player) {
		int attackDistance = 1;
		float x = _playerCurrentTileCoordinate.x;
		float z = _playerCurrentTileCoordinate.z;

		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= attackDistance; index1++) {
			// Inner loop handles all the other tiles NE, SE, NW, SW
			for (int index2 = 1; index2 <= attackDistance - index1; index2++) {
				InstantiateAttackHightlightCube (x + index1, z + index2); // North East
				InstantiateAttackHightlightCube (x + index1, z - index2); // South East
				InstantiateAttackHightlightCube (x - index1, z + index2); // North West
				InstantiateAttackHightlightCube (x - index1, z - index2); // South West
			}
			InstantiateAttackHightlightCube (x, z + index1); // North
			InstantiateAttackHightlightCube (x + index1, z); // East
			InstantiateAttackHightlightCube (x, z - index1); // South
			InstantiateAttackHightlightCube (x - index1, z); // West
		}
	}

	private void InstantiateAttackHightlightCube(float x, float z) {

		// Don't go out of boundary
		if (x < 0 || z < 0)
			return;

		// Set color based off of if it's a movement tile, or attack tile
		Color color = _attack_highlight_color;

		// Create movement highlight cube and set the material's color
		Vector3 vector = new Vector3 (x, 0, z) * _tileMap.tileSize;
		GameObject movementHightlightCubeClone = Instantiate (movementHighlightCube.gameObject, vector, Quaternion.identity) as GameObject;
		movementHightlightCubeClone.transform.Find ("Cube").gameObject.GetComponent<Renderer> ().material.color = color;
		_attackIndicators.Add (new Vector3 (x, 0, z), movementHightlightCubeClone);
	}

	private IEnumerator MoveCPU() {
		_playerMoveFinished = false;
		yield return new WaitForSeconds (3);
		_playerMoveFinished = true;
	}

	private IEnumerator MoveToTiles() {
		Vector3 oldTile = _pathfinder.GetGeneratedPathAt(0);
		Vector3 newTile = Vector3.zero;
		int index = 0;
		while (index < _pathfinder.GetGeneratedPath().Count - 1) {
			newTile = _pathfinder.GetGeneratedPathAt(index + 1);
			Vector3 startingPosition = TileMapUtil.TileMapToWorldCentered (_pathfinder.GetGeneratedPathAt(index), _tileMap.tileSize);
			Vector3 endingPosition = TileMapUtil.TileMapToWorldCentered (newTile, _tileMap.tileSize);

			print (_selectedCharacter);
			print (startingPosition);
			print(endingPosition);
			yield return StartCoroutine(MoveToTile(_selectedCharacter, startingPosition, endingPosition));
			index++;
			yield return null;
		}
		_playerMoveFinished = true;
		_pathfinder.Clear();

		// After move is finished, swap out tile unit is standing on
		TileMapData tileMapData = _tileMap.GetTileMapData();
		TileData oldTileData = tileMapData.GetTileDataAt (oldTile);
		oldTileData.SwapUnits(tileMapData.GetTileDataAt(newTile));
		yield break;
	}

	private IEnumerator MoveToTile(Unit character, Vector3 startingPosition, Vector3 endingPosition) {
		float elapsedTime = 0.0f;
		float timeToMove = 0.125f;
		while (elapsedTime < timeToMove) {
			character.transform.position = Vector3.Lerp (startingPosition, endingPosition, (elapsedTime / timeToMove));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	public void DrawGeneratedPathLine(List<Node> generatedPath) {
		if (generatedPath != null) {
			int index = 0;
			while (index < generatedPath.Count - 1) {
				Vector3 start = TileMapUtil.TileMapToWorldCentered (
					                new Vector3 (generatedPath [index].x, 10, generatedPath [index].z),
					                _tileMap.tileSize
				                );
				Vector3 end = TileMapUtil.TileMapToWorldCentered (
					              new Vector3 (generatedPath [index + 1].x, 10, generatedPath [index + 1].z),
					              _tileMap.tileSize
				              );

				Debug.DrawLine (start, end, Color.red);
				Debug.Log (string.Format("Line: [{0}, {1}]", start, end));
				index++;
			}
		}
	}

	private void ShowCursor(bool showCursor) {
		if (showCursor) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private bool IsEnemyNearby(List<Unit> units) {
		foreach (Unit unit in units) {
			int movement = unit.movement;
			Vector3 tileMapPosition = TileMapUtil.WorldCenteredToTileMap (unit.transform.position, _tileMap.tileSize);
			int x = (int) tileMapPosition.x;
			int z = (int) tileMapPosition.z;

			// Outer loop handles the straight lines going N, E, S, W
			for (int index1 = 1; index1 <= movement; index1++) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= movement - index1; index2++) {
					if (IsEmptyNearby (unit, x + index1, z + index2)) // North East
						return true;
					if (IsEmptyNearby (unit, x + index1, z - index2)) // South East
						return true;
					if (IsEmptyNearby (unit, x - index1, z + index2)) // North West
						return true; 
					if (IsEmptyNearby (unit, x - index1, z - index2)) // South West
						return true; 
				}
				if (IsEmptyNearby (unit, x, z + index1)) // North
					return true; 
				if (IsEmptyNearby (unit, x + index1, z)) // East
					return true; 
				if (IsEmptyNearby (unit, x, z - index1)) // South
					return true; 
				if (IsEmptyNearby (unit, x - index1, z)) // West
					return true; 
			}
		}
		return false;
	}

	private bool IsEmptyNearby(Unit unit, int x, int z) {
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (x, z);
		Unit targetUnit = tileData.Unit;

		if (targetUnit != null) {
			if (targetUnit.IsPlayerControlled != unit.IsPlayerControlled) {
				print (string.Format ("{0} - {1}", unit, targetUnit));
				return true;
			}
		}
		return false;
	}
}