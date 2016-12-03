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

	public Unit[] players;

	// Audio related things
	public AudioSource cursorMoveSource;
	public AudioSource confirmationSource;
	public AudioSource musicCalm;
	public AudioSource musicFire;

	private bool _playingCalm = true;
	
	private TileMap _tileMap;
	private Pathfinder _pathfinder;
	
	private Vector3 _currentTileCoord;
	private Vector3 _playerCurrentTileCoordinate;

	private Unit _highlightedCharacter;
	private Unit _selectedCharacter;

	private Dictionary<Vector3, GameObject> _movementIndicators = new Dictionary<Vector3, GameObject>();
	private Dictionary<Vector3, GameObject> _attackIndicators = new Dictionary<Vector3, GameObject>();

	private Color _attack_highlight_color = new Color (1.0f, 0.0f, 0.0f, HIGHTLIGHT_COLOR_TRANSPARENCY);

	private GameState _gameState;

	private bool _playerMoveFinished = false;

	private IEnumerator _moveToTiles;
	
	void Start() {
		_tileMap = GetComponent<TileMap>();
		_gameState = GameState.INITIALIZE_TURN;
		musicCalm.Play ();
		musicFire.Play();
		musicFire.volume = 0.0f;
	}

	public void TransitionGameState(GameState newState) {
		_gameState = newState;
	}		

	// Update is called once per frame
	void Update () {

		switch (_gameState) {

		case GameState.INITIALIZE_TURN:
			ShowCursor (false);
			Unit unit = _tileMap.GetTurnOrderController ().GetNextUp ();
			Debug.Log (string.Format ("Initialzie Turn: {0}", unit));
			StartCoroutine (_tileMap.GetCameraController ().MoveToPosition (unit.transform.position));
			HighlightCharacter (unit);

			if (unit.IsPlayerControlled)
				_gameState = GameState.PLAYER_TURN;
			else
				_gameState = GameState.CPU_TURN;
			break;

		// Player is moving cursor around the tile map
		case GameState.PLAYER_TURN:
			if (!_tileMap.GetCameraController ().IsMoving) {
				ShowCursor (true);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				HandleTerrainMouseOver (ray);
				HandleCharacterMouseOver (ray);

				if (_selectedCharacter != null && _tileMap.GetTurnOrderController ().GetNextUp () == _selectedCharacter)
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
					(int) WorldCenteredToTileMap(_selectedCharacter.transform.position).x,
					(int) WorldCenteredToTileMap(_selectedCharacter.transform.position).z,
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
			RemoveMovementTiles ();
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
			RemoveMovementTiles ();
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
			if (!_tileMap.GetCameraController ().IsMoving) {
				Unit cpu = _tileMap.GetTurnOrderController ().GetNextUp ();
				_selectedCharacter = cpu;

				// Show terrain related things
				Vector3 cpuTilemapCoordinates = WorldCenteredToTileMap (cpu.transform.position);
				ShowTerrainUI ((int) cpuTilemapCoordinates.x, (int) cpuTilemapCoordinates.z);
				_playerCurrentTileCoordinate = cpuTilemapCoordinates;

				// Show character sheet
				cpu.ActivateCharacterSheet ();

				// Show movement tiles
				HighlightMovementTiles(cpu);

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
			_tileMap.GetTurnOrderController ().FinishTurn (_selectedCharacter);
			_selectedCharacter = null;
			if (IsEnemyNearby (players)) {
				if (_playingCalm) {
					TransitionMusic (musicCalm, musicFire);
					_playingCalm = false;
				}
			} else {
				if (!_playingCalm) {
					TransitionMusic (musicFire, musicCalm);
					_playingCalm = true;
				}
			}
			TransitionGameState(GameState.INITIALIZE_TURN);
			break;
		}
	}

	private void HighlightCharacter(Unit character) {
		selectionCube.position = WorldCenteredToUncentered(character.transform.position);
	}

	private void HandleTerrainMouseOver(Ray ray) {
		//mouseMove.
		RaycastHit hitInfo;
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {

			int x = Mathf.FloorToInt( hitInfo.point.x / _tileMap.tileSize);
			int z = Mathf.FloorToInt( hitInfo.point.z / _tileMap.tileSize);
			var mousePos = Input.mousePosition;
			mousePos.z = 0;

			// Don't run if still on the same tile
			if (_currentTileCoord.x != x || _currentTileCoord.z != z) {
				_currentTileCoord.x = x;
				_currentTileCoord.z = z;

				cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

				selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
				ShowTerrainUI (x, z);
			}
		}
	}
		
	private void HandleMovementSelection() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {

			int x = Mathf.FloorToInt( hitInfo.point.x / _tileMap.tileSize);
			int z = Mathf.FloorToInt( hitInfo.point.z / _tileMap.tileSize);
			var mousePos = Input.mousePosition;
			mousePos.z = 0;

			// Don't run if still on the same tile
			if (_currentTileCoord.x != x || _currentTileCoord.z != z) {

				// Don't allow outside movement
				if (_movementIndicators.ContainsKey (new Vector3 (x, 0, z))) {
					_currentTileCoord.x = x;
					_currentTileCoord.z = z;

					cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

					selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
					ShowTerrainUI (x, z);
				}
			}
		}
	}

	private void ShowTerrainUI(int x, int z) {
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (x, z);
		terrainDetailsController.Activate (tileData);
	}

	private bool IsRaycastHitNew(RaycastHit raycastHit, Vector3 currentTileCoordinate) {
		int x = Mathf.FloorToInt( raycastHit.point.x / _tileMap.tileSize);
		int z = Mathf.FloorToInt( raycastHit.point.z / _tileMap.tileSize);
		var mousePos = Input.mousePosition;
		mousePos.z = 0;

		Debug.Log (string.Format("New Tile: {0},{1} - Old Tile: {2},{3}", x, z, currentTileCoordinate.x, currentTileCoordinate.z));

		if (currentTileCoordinate.x != x || currentTileCoordinate.z != z) {
			currentTileCoordinate.x = x;
			currentTileCoordinate.z = z;
			return true;
		}
		return false;
	}

	private bool IsPlayerRaycastHitNew(RaycastHit raycastHit) {
		int x = Mathf.FloorToInt( raycastHit.point.x / _tileMap.tileSize);
		int z = Mathf.FloorToInt( raycastHit.point.z / _tileMap.tileSize);
		var mousePos = Input.mousePosition;
		mousePos.z = 0;

		if (_playerCurrentTileCoordinate.x != x || _playerCurrentTileCoordinate.z != z) {
			_playerCurrentTileCoordinate.x = x;
			_playerCurrentTileCoordinate.z = z;

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
		for (int index = 0; index < players.Length; index++) {
			Unit player = players [index];
			if (player.GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {

				// Only run this if the cursor has moved
				if (IsPlayerRaycastHitNew(hitInfo)) {

					// Mark player as highlighted and show character sheet
					Debug.Log (player);
					_highlightedCharacter = player;
					player.ActivateCharacterSheet ();

					// Show tiles you can move to
					HighlightMovementTiles(player);
				}

				// If user has clicked, show combat menu
				if ((Input.GetMouseButtonDown (0)) && _selectedCharacter == null && player == _tileMap.GetTurnOrderController().GetNextUp()) {
					_selectedCharacter = player;
					confirmationSource.PlayOneShot (confirmationSource.clip);
					//player.ActivateCombatMenu();
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
				RemoveMovementTiles();
			}
		}
	}

	/// <summary>
	/// Highlights the movement tiles for a character.
	/// </summary>
	/// <param name="player">Player.</param>
	private void HighlightMovementTiles(Unit player) {
		
		// Clear out old movement tiles
		RemoveMovementTiles();

		// Get color, depending on the type of unit
		Color movementTileColor = player.MovementTileColor;

		int movement = player.movement;
		float x = _playerCurrentTileCoordinate.x;
		float z = _playerCurrentTileCoordinate.z;

		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= movement; index1++) {
			// Inner loop handles all the other tiles NE, SE, NW, SW
			for (int index2 = 1; index2 <= movement - index1; index2++) {
				InstantiateMovementHightlightCube (movement, index2, x + index1, z + index2, movementTileColor); // North East
				InstantiateMovementHightlightCube (movement, index2, x + index1, z - index2, movementTileColor); // South East
				InstantiateMovementHightlightCube (movement, index2, x - index1, z + index2, movementTileColor); // North West
				InstantiateMovementHightlightCube (movement, index2, x - index1, z - index2, movementTileColor); // South West
			}
			InstantiateMovementHightlightCube (movement, index1, x, z + index1, movementTileColor); // North
			InstantiateMovementHightlightCube (movement, index1, x + index1, z, movementTileColor); // East
			InstantiateMovementHightlightCube (movement, index1, x, z - index1, movementTileColor); // South
			InstantiateMovementHightlightCube (movement, index1, x - index1, z, movementTileColor); // West
		}
	}

	private void InstantiateMovementHightlightCube(int totalMovement, int currentMovement, float x, float z, Color movementTileColor) {

		// Don't go out of boundary
		if (x < 0 || z < 0 || x >= _tileMap.size_x || z >= _tileMap.size_z )
			return;

		// Don't highlight unwalkable areas (i.e. mountains or occupied tiles)
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt ((int) x, (int) z);
		if (!tileData.IsWalkable || tileData.Unit != null)
			return;

		// Create movement highlight cube and set the material's color
		Vector3 vector = new Vector3 (x, 0, z) * _tileMap.tileSize;
		GameObject movementHightlightCubeClone = Instantiate (movementHighlightCube.gameObject, vector, Quaternion.identity) as GameObject;
		movementHightlightCubeClone.transform.Find ("Cube").gameObject.GetComponent<Renderer> ().material.color = movementTileColor;
		_movementIndicators.Add (new Vector3 (x, 0, z), movementHightlightCubeClone);
	}

	private void RemoveMovementTiles() {
		foreach (var go in _movementIndicators.Values)
			Destroy (go);
		_movementIndicators.Clear ();
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
			Vector3 startingPosition = TileMapToWorldCentered (_pathfinder.GetGeneratedPathAt(index), _tileMap.tileSize);
			Vector3 endingPosition = TileMapToWorldCentered (newTile, _tileMap.tileSize);

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

	public Vector3 TileMapToWorld(Vector3 vector, float tileMapSize) {
		float x = (vector.x * _tileMap.tileSize);
		float z = (vector.z * _tileMap.tileSize);
		return new Vector3 (x, 0, z);
	}

	public Vector3 TileMapToWorldCentered(Vector3 vector, float tileMapSize) {
		float x = (vector.x * _tileMap.tileSize) + tileMapSize / 2.0f;
		float z = (vector.z * _tileMap.tileSize) + tileMapSize / 2.0f;
		return new Vector3 (x, vector.y, z);
	}

	public Vector3 WorldCenteredToTileMap(Vector3 vector) {
		int x = Mathf.FloorToInt( vector.x / _tileMap.tileSize);
		int z = Mathf.FloorToInt( vector.z / _tileMap.tileSize);
		return new Vector3 (x, 0, z);
	}

	public Vector3 WorldCenteredToUncentered(Vector3 vector) {
		float x = vector.x - _tileMap.tileSize / 2.0f;
		float z = vector.z - _tileMap.tileSize / 2.0f;
		return new Vector3 (x, 0, z);
	}

	public void DrawGeneratedPathLine(List<Node> generatedPath) {
		if (generatedPath != null) {
			int index = 0;
			while (index < generatedPath.Count - 1) {
				Vector3 start = TileMapToWorldCentered (
					                new Vector3 (generatedPath [index].x, 10, generatedPath [index].z),
					                _tileMap.tileSize
				                );
				Vector3 end = TileMapToWorldCentered (
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

	private void CalmToFire() {
		if (_playingCalm) {
			TransitionMusic (musicCalm, musicFire);
			_playingCalm = false;
		} else {
			TransitionMusic (musicFire, musicCalm);
			_playingCalm = true;
		}
	}

	private void TransitionMusic(AudioSource audioSource1Stop, AudioSource audioSource2Start) {
		print ("TransitionMusic");
		StartCoroutine (FadeOutMusic (audioSource1Stop));
		StartCoroutine (FadeInMusic (audioSource2Start));
	}

	private IEnumerator FadeInMusic(AudioSource audioSource) {
		float volume = 0.0f;
		while (volume < 1.0f) {
			volume += Time.deltaTime;
			audioSource.volume = volume;
			yield return null;
		}		
	}

	private IEnumerator FadeOutMusic(AudioSource audioSource) {
		float volume = 1.0f;
		while (volume > 0.0f) {
			volume -= Time.deltaTime;
			audioSource.volume = volume;
			yield return null;
		}
	}

	private bool IsEnemyNearby(Unit[] units) {
		foreach (Unit unit in units) {
			int movement = unit.movement;
			Vector3 tileMapPosition = WorldCenteredToTileMap (unit.transform.position);
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