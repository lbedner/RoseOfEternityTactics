using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {

	public enum GameState {
		INITIALIZE,
		INITIALIZE_TURN,		
		PLAYER_TURN,
		PLAYER_MOVE_SELECTION,
		PLAYER_MOVE_START,
		PLAYER_MOVE_STOP,
		PLAYER_SHOW_ATTACK_INDICATORS,
		PLAYER_ATTACK,
		PLAYER_ATTACK_CONFIRMATION,
		PLAYER_ATTACK_SELECTION,
		CPU_TURN,
		CPU_MOVE_STOP,
		COMBAT_MENU,
		TURN_OVER,
		UNDO,
	}

	protected const float HIGHTLIGHT_COLOR_TRANSPARENCY = 0.7f;

	public Transform selectionCube;
	public Transform movementHighlightCube;

	public TerrainDetailsController terrainDetailsController;
	public ActionController actionController;

	// Audio related things
	public AudioSource cursorMoveSource;
	public AudioSource confirmationSource;

	public GameObject head2HeadPanel;

	private GameObject _head2HeadPanelConfirmation;
	
	private TileMap _tileMap;
	private Pathfinder _pathfinder;
	
	private Vector3 _currentTileCoord;
	private Vector3 _playerCurrentTileCoordinate;

	private Unit _highlightedCharacter;
	private Unit _selectedCharacter;

	private GameState _gameState;

	private Action _action;

	private bool _playerMoveFinished = false;
	private bool _unitAnimationPlaying = false;
	private bool _isAttacking = false;

	private GameManager _gameManager;

	private MusicController _musicController;

	private TileHighlighter _tileHighlighter;
	private TileDiscoverer _tileDiscoverer;

	private GameStateAction _gameStateAction;
	private GameStateOriginator _gameStateOriginator;
	private GameStateCaretaker _gameStateCaretaker;

	private List<Unit> _intendedActionTargets = new List<Unit> ();

	void Start() {
		print ("TileMapMouse.Start()");
		_tileMap = GetComponent<TileMap>();
		_gameManager = GameManager.Instance;
		_musicController = _gameManager.GetMusicController ();
		_musicController.Initialize ();
		_gameState = GameState.INITIALIZE;
		_gameStateAction = new GameStateAction ();
		_gameStateOriginator = new GameStateOriginator ();
		_gameStateCaretaker = new GameStateCaretaker ();
		_head2HeadPanelConfirmation = head2HeadPanel.transform.Find ("Confirmation").gameObject;
	}

	/// <summary>
	/// Transitions the state of the game.
	/// </summary>
	/// <param name="newState">New state.</param>
	public void TransitionGameState(GameState newState) {
		_gameState = newState;
	}

	/// <summary>
	/// Transitions the game state to turn over.
	/// </summary>
	public void TransitionGameStateToTurnOver() {
		TransitionGameState (GameState.TURN_OVER);
	}

	/// <summary>
	/// Transitions the game state to show attack indicators.
	/// </summary>
	public void TransitionGameStateToShowAttackIndicators() {
		TransitionGameState (GameState.PLAYER_SHOW_ATTACK_INDICATORS);
	}

	/// <summary>
	/// Transitions the game state to player attack.
	/// </summary>
	public void TransitionGameStateToPlayerAttack() {
		TransitionGameState (GameState.PLAYER_ATTACK);
	}

	// Update is called once per frame
	void Update () {

		switch (_gameState) {

		case GameState.INITIALIZE:
			Initialize ();
			TransitionGameState (GameState.INITIALIZE_TURN);
			break;			

		case GameState.INITIALIZE_TURN:
			ShowCursorAndTileSelector (false);
			Unit unit = _gameManager.GetTurnOrderController ().GetNextUp ();
			//Debug.Log (string.Format ("Initialzie Turn: {0}", unit));
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
				ShowCursorAndTileSelector (true);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				HandleTerrainMouseOver (ray);
				HandleCharacterMouseOver (ray);

				if (_selectedCharacter != null && _gameManager.GetTurnOrderController ().GetNextUp () == _selectedCharacter) {
					
					//_gameStateAction.GameState = GameState.PLAYER_TURN;
					//_gameStateAction.GameState = GameState.PLAYER_MOVE_SELECTION;
					//_gameStateAction.ShowCombatMenu = false;
					//_gameStateAction.UnitTile = _selectedCharacter.Tile;
					//_gameStateAction.ShowCursor = true;

					//_gameStateOriginator.GameStateAction = _gameStateAction;
					//_gameStateCaretaker.Add (_gameStateOriginator.StoreInMemento ());

					_gameState = GameState.PLAYER_MOVE_SELECTION;
				}
			}
			break;

		// Player can select a tile to move to
		case GameState.PLAYER_MOVE_SELECTION:
			HandleMovementSelection ();
			if ((Input.GetMouseButtonDown (0))) {
				_pathfinder.GeneratePath(_selectedCharacter.Tile, _currentTileCoord);

				//foreach (var node in _pathfinder.GetGeneratedPath())
				//	Debug.Log(node);
				
				_gameState = GameState.PLAYER_MOVE_START;
				confirmationSource.PlayOneShot (confirmationSource.clip);
			}
			break;
		
		// Player moves to tile
		case GameState.PLAYER_MOVE_START:
			ShowCursorAndTileSelector (false);
			_tileHighlighter.RemoveHighlightedTiles();
			StartCoroutine (MoveToTiles ());
			TransitionGameState (GameState.PLAYER_MOVE_STOP);
			break;

		// Player has stopped moving
		case GameState.PLAYER_MOVE_STOP:
			if (_playerMoveFinished) {
				_playerMoveFinished = false;
				_playerCurrentTileCoordinate = _currentTileCoord;
				TransitionGameState (GameState.COMBAT_MENU);
				ShowCursor (true);
			}
			break;

		// Player can make choices from combat menu
		case GameState.COMBAT_MENU:
			/*
			if (Input.GetKey (KeyCode.Escape)) {
				print ("COMABT_MENU.ESC");
				//_selectedCharacter.DeactivateCombatMenu ();
				TransitionGameState (GameState.UNDO);
			}*/
			ShowTileSelector (false);
			_selectedCharacter.ActivateCombatMenu ();
			break;

		case GameState.UNDO:
			GameStateMemento memento = _gameStateCaretaker.Get (0);
			GameStateAction gameStateAction = _gameStateOriginator.RestoreFromMemenrto (memento);

			if (_selectedCharacter.Tile != gameStateAction.UnitTile) {
				_selectedCharacter.transform.position = TileMapUtil.TileMapToWorldCentered (gameStateAction.UnitTile, _tileMap.tileSize);
				_selectedCharacter.Tile = gameStateAction.UnitTile;
				HighlightCharacter (_selectedCharacter);
			}
			if (gameStateAction.ShowCombatMenu)
				_selectedCharacter.ActivateCombatMenu ();
			else
				_selectedCharacter.DeactivateCombatMenu ();
			ShowCursor (gameStateAction.ShowCursor);
			TransitionGameState (gameStateAction.GameState);
			break;

		// Indicate where player can attack
		case GameState.PLAYER_SHOW_ATTACK_INDICATORS:
			ShowTileSelector (true);
			_selectedCharacter.DeactivateCombatMenu ();
			_tileHighlighter.HighlightAttackTiles (_selectedCharacter);
			HandleActionSelection ();
			if ((Input.GetMouseButtonDown (0)) && _tileMap.GetTileMapData ().GetTileDataAt (_currentTileCoord).Unit) {
				TransitionGameState (GameState.PLAYER_ATTACK_CONFIRMATION);
			}
			break;
		
		// Pop up head 2 head confirmation panel
		case GameState.PLAYER_ATTACK_CONFIRMATION:
			if (!head2HeadPanel.activeInHierarchy) {
				_gameManager.GetSourceHead2HeadPanelController ().Load (_selectedCharacter, Head2HeadPanelController.Head2HeadState.ATTACKING);
				_gameManager.GetTargetHead2HeadPanelController ().Load (_tileMap.GetTileMapData ().GetTileDataAt (_currentTileCoord).Unit, Head2HeadPanelController.Head2HeadState.DEFENDING);
				head2HeadPanel.SetActive (true);
			}
			break;

		// Attack selected enemy
		case GameState.PLAYER_ATTACK:
			if (!_isAttacking) {
				ClearActionTargets ();
				head2HeadPanel.SetActive (false);
				_isAttacking = true;
				ShowCursorAndTileSelector (false);
				Unit defender = _tileMap.GetTileMapData ().GetTileDataAt (_currentTileCoord).Unit;
				StartCoroutine (PerformAction (_selectedCharacter, defender));
			}
			break;

		// Select who to attack
		case GameState.PLAYER_ATTACK_SELECTION:
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
				_tileHighlighter.HighlightTiles(cpu, _playerCurrentTileCoordinate, false);

				// Get AI Action
				// TODO: Refactor the shit out of all of this. I'm so not proud of this.
				AI ai = new KamiKazeAI(_selectedCharacter, _tileMap.GetTileMapData(), _tileDiscoverer, _pathfinder);
				Action action = ai.GetAction ();
				_action = action;

				print (string.Format ("Start CPU Turn: {0}", _selectedCharacter));
				StartCoroutine (MoveCPU ());
				TransitionGameState (GameState.CPU_MOVE_STOP);
			}
			break;

		// CPU is done moving
		case GameState.CPU_MOVE_STOP:
			if (_playerMoveFinished) {
				_playerMoveFinished = false;

				// Perform attack if close enough
				print (_action);
				Unit target = _action.Target;
				if (target) {
					HighlightActionTarget (target);
					StartCoroutine(CPUPerformAction(_selectedCharacter, target));
				}
				else
					TransitionGameState (GameState.TURN_OVER);
			}
			break;

		// Turn is over for the character
		case GameState.TURN_OVER:
			if (!_unitAnimationPlaying) {
				_selectedCharacter.DeactivateCombatMenu ();
				_tileHighlighter.RemoveHighlightedTiles ();
				_gameManager.GetTurnOrderController ().FinishTurn (_selectedCharacter);
				_selectedCharacter = null;
				if (IsEnemyNearby (_gameManager.GetTurnOrderController ().GetAllUnits ()))
					_musicController.TransitionMusic (false);
				else
					_musicController.TransitionMusic (true);
				TransitionGameState (GameState.INITIALIZE_TURN);
			}
			break;
		}
	}

	/// <summary>
	/// Gets the tile highlighter.
	/// </summary>
	/// <returns>The tile highlighter.</returns>
	public TileHighlighter GetTileHighlighter() {
		return _tileHighlighter;
	}

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	private void Initialize() {
		_tileHighlighter = new TileHighlighter (_tileMap, movementHighlightCube);
		_tileDiscoverer = new TileDiscoverer (_tileMap.GetTileMapData ());
		_pathfinder = new Pathfinder (_tileMap.GetTileMapData(), _tileMap.GetGraph().GetGraph());
	}

	/// <summary>
	/// Highlights the character.
	/// </summary>
	/// <param name="character">Character.</param>
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

				// Don't allow movement on a non-highlighted or occupied tile
				if (_tileHighlighter.IsHighlightedMovementTile(tileMapPoint) && !_tileMap.GetTileMapData().GetTileDataAt(tileMapPoint).Unit) {
					_currentTileCoord.x = tileMapPoint.x;
					_currentTileCoord.z = tileMapPoint.z;

					cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

					selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
					ShowTerrainUI ((int) tileMapPoint.x, (int) tileMapPoint.z);
				}
			}
		}
	}

	private void HandleActionSelection() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {

			// Get hit point in tile map coordinates
			Vector3 tileMapPoint = TileMapUtil.WorldCenteredToTileMap (hitInfo.point, _tileMap.tileSize);

			// Don't run if still on the same tile
			if (_currentTileCoord.x != tileMapPoint.x || _currentTileCoord.z != tileMapPoint.z) {

				// Don't allow movement on a non-highlighted or occupied tile
				if (_tileHighlighter.IsHighlightedAttackTile(tileMapPoint)) {
					_currentTileCoord.x = tileMapPoint.x;
					_currentTileCoord.z = tileMapPoint.z;

					cursorMoveSource.PlayOneShot (cursorMoveSource.clip);

					selectionCube.transform.position = _currentTileCoord * _tileMap.tileSize;
					ShowTerrainUI ((int) tileMapPoint.x, (int) tileMapPoint.z);

					ClearActionTargets ();
					Unit unit = _tileMap.GetTileMapData ().GetTileDataAt (tileMapPoint).Unit;
					if (unit)
						HighlightActionTarget (unit);
				}
			}
		}
	}

	private void HighlightActionTargets(List<Unit> targets) {
		foreach (Unit unit in targets) {
			HighlightActionTarget (unit);
		}
	}

	private void HighlightActionTarget(Unit unit) {
		_intendedActionTargets.Add (unit);
		unit.ShowDamagedColor (true);
	}

	private void ClearActionTargets() {
		foreach (Unit unit in _intendedActionTargets)
			unit.ShowDamagedColor(false);
		_intendedActionTargets.Clear ();
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
					_highlightedCharacter = unit;
					unit.ActivateCharacterSheet ();

					// Show tiles you can move to
					_tileHighlighter.HighlightTiles(unit, _playerCurrentTileCoordinate);
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
				_tileHighlighter.RemoveHighlightedTiles();
			}
		}
	}

	/// <summary>
	/// CPUs performs an action.
	/// </summary>
	/// <returns>The perform action.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	private IEnumerator CPUPerformAction(Unit attacker, Unit defender) {

		// Show head 2 head panel for a bit
		_gameManager.GetSourceHead2HeadPanelController ().Load (attacker, Head2HeadPanelController.Head2HeadState.ATTACKING);
		_gameManager.GetTargetHead2HeadPanelController ().Load (defender, Head2HeadPanelController.Head2HeadState.DEFENDING);
		_head2HeadPanelConfirmation.SetActive (false);
		head2HeadPanel.SetActive (true);

		yield return new WaitForSeconds (2.0f);
		head2HeadPanel.SetActive (false);
		_head2HeadPanelConfirmation.SetActive (true);

		// Continue performing action
		ClearActionTargets ();
		StartCoroutine (PerformAction (attacker, defender));
	}		

	/// <summary>
	/// Performs the action of the attacker against the defender.
	/// </summary>
	/// <returns>The action.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	private IEnumerator PerformAction(Unit attacker, Unit defender) {

		_tileHighlighter.RemoveHighlightedTiles ();
		actionController.Activate (attacker.weaponName);
		yield return new WaitForSeconds (0.5f);
		Combat combat = new Combat (_selectedCharacter, defender);
		combat.Begin ();
		defender.ShowDamagedColor (true);
		yield return StartCoroutine (PlayAttackAnimations (attacker, defender));
		yield return new WaitForSeconds (1.0f);
		defender.ShowDamagedColor (false);

		// Show XP floaty text
		int awardedExperience = combat.GetAwardedExperience();
		PopupTextController.Initialize(attacker.GetCanvas());
		PopupTextController.CreatePopupText (string.Format("+ {0} XP", awardedExperience), attacker.transform.position, Color.yellow);

		yield return new WaitForSeconds (1.0f);

		TransitionGameState (GameState.TURN_OVER);
		_isAttacking = false;
		yield return null;
	}

	/// <summary>
	/// Plays the attack animations.
	/// </summary>
	/// <returns>The attack animations.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	private IEnumerator PlayAttackAnimations(Unit attacker, Unit defender) {

		_unitAnimationPlaying = true;

		//determine which way to swing, dependent on the direction the enemy is
		Unit.TileDirection facing = attacker.GetFacing (defender);
		UnitAnimationController animationController = attacker.GetAnimationController ();
		switch (facing) {
		case Unit.TileDirection.NORTH:
			animationController.AttackNorth ();
			break;
		case Unit.TileDirection.EAST:
			animationController.AttackEast ();
			break;
		case Unit.TileDirection.SOUTH:
			animationController.AttackSouth ();
			break;
		case Unit.TileDirection.WEST:
			animationController.AttackWest ();
			break;
		}

		actionController.Deactivate ();
		_unitAnimationPlaying = false;
		yield return null;
	}

	/// <summary>
	/// Moves the CPU.
	/// </summary>
	/// <returns>The CPU.</returns>
	private IEnumerator MoveCPU() {
		yield return StartCoroutine(MoveToTiles());
		TransitionGameState (GameState.CPU_MOVE_STOP);
		yield return null;
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
			Vector3 startingPosition = TileMapUtil.TileMapToWorldCentered (_pathfinder.GetGeneratedPathAt(index), _tileMap.tileSize);
			Vector3 endingPosition = TileMapUtil.TileMapToWorldCentered (newTile, _tileMap.tileSize);

			yield return StartCoroutine(MoveToTile(_selectedCharacter, startingPosition, endingPosition));
			index++;
			yield return null;
		}
		_playerMoveFinished = true;
		_pathfinder.Clear();

		// After move is finished, swap out tile unit is standing on
		if (!TileMapUtil.IsInvalidTile (oldTile)) {
			TileMapData tileMapData = _tileMap.GetTileMapData ();
			TileData oldTileData = tileMapData.GetTileDataAt (oldTile);
			oldTileData.SwapUnits (tileMapData.GetTileDataAt (newTile));
			_selectedCharacter.Tile = newTile;
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

	/// <summary>
	/// Determines if the mouse cursor should be shown.
	/// </summary>
	/// <param name="showCursor">If set to <c>true</c> show cursor.</param>
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

	/// <summary>
	/// Shows the tile selector.
	/// </summary>
	/// <param name="showTileSelector">If set to <c>true</c> show tile selector.</param>
	private void ShowTileSelector(bool showTileSelector) {
		selectionCube.gameObject.SetActive (showTileSelector);
	}

	/// <summary>
	/// Shows the cursor and tile selector.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	private void ShowCursorAndTileSelector(bool show) {
		ShowCursor(show);
		ShowTileSelector(show);
	}

	/// <summary>
	/// Determines whether there are nearby enemies.
	/// </summary>
	/// <returns><c>true</c> if there are nearby enemies; otherwise, <c>false</c>.</returns>
	/// <param name="units">Units.</param>
	private bool IsEnemyNearby(List<Unit> units) {
		foreach (Unit unit in units) {
			int range = (int) unit.GetMovementAttribute().CurrentValue + unit.weaponRange;
			Vector3 tileMapPosition = TileMapUtil.WorldCenteredToTileMap (unit.transform.position, _tileMap.tileSize);
			int x = (int) tileMapPosition.x;
			int z = (int) tileMapPosition.z;

			// Outer loop handles the straight lines going N, E, S, W
			for (int index1 = 1; index1 <= range; index1++) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= range - index1; index2++) {
					if (IsEnemyNearby (unit, x + index1, z + index2)) // North East
						return true;
					if (IsEnemyNearby (unit, x + index1, z - index2)) // South East
						return true;
					if (IsEnemyNearby (unit, x - index1, z + index2)) // North West
						return true; 
					if (IsEnemyNearby (unit, x - index1, z - index2)) // South West
						return true; 
				}
				if (IsEnemyNearby (unit, x, z + index1)) // North
					return true; 
				if (IsEnemyNearby (unit, x + index1, z)) // East
					return true; 
				if (IsEnemyNearby (unit, x, z - index1)) // South
					return true; 
				if (IsEnemyNearby (unit, x - index1, z)) // West
					return true; 
			}
		}
		return false;
	}

	/// <summary>
	/// Determines whether an enemy is nearby the unit.
	/// </summary>
	/// <returns><c>true</c> if this instance is enemy nearby the specified unit x z; otherwise, <c>false</c>.</returns>
	/// <param name="unit">Unit.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private bool IsEnemyNearby(Unit unit, int x, int z) {
		
		// Don't go out of boundary
		if (!TileMapUtil.IsInsideTileMapBoundary (_tileMap.GetTileMapData (), x, z))
			return false;
		
		TileData tileData = _tileMap.GetTileMapData ().GetTileDataAt (x, z);
		Unit targetUnit = tileData.Unit;

		if (targetUnit != null) {
			if (targetUnit.IsPlayerControlled != unit.IsPlayerControlled)
				return true;
		}
		return false;
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
			Unit.TileDirection tileDirection = unit.GetFacing (
				TileMapUtil.WorldCenteredToTileMap (sourceTile, _tileMap.tileSize),
				TileMapUtil.WorldCenteredToTileMap (targetTile, _tileMap.tileSize)
			);

			switch (tileDirection) {
			case Unit.TileDirection.NORTH:
				unit.GetAnimationController ().WalkNorth ();
				break;
			case Unit.TileDirection.EAST:
				unit.GetAnimationController ().WalkEast ();
				break;
			case Unit.TileDirection.WEST:
				unit.GetAnimationController ().WalkWest ();
				break;
			case Unit.TileDirection.SOUTH:
				unit.GetAnimationController ().WalkSouth ();
				break;
			}
		}
	}

	public class GameStateAction {
		public GameState GameState { get; set; }
		public Vector3 UnitTile { get; set; }
		public bool ShowCombatMenu { get; set; }
		public bool ShowCursor { get; set; }
	}		
		
	public class GameStateMemento {
		public GameStateAction GameStateAction { get; set; }

		public GameStateMemento(GameStateAction gameStateAction) {
			GameStateAction = gameStateAction;
		}
	}

	public class GameStateOriginator {
		public GameStateAction GameStateAction { get; set; }

		public GameStateMemento StoreInMemento() {
			return new GameStateMemento (GameStateAction);
		}

		public GameStateAction RestoreFromMemenrto(GameStateMemento memento) {
			GameStateAction = memento.GameStateAction;
			return GameStateAction;
		}
	}

	public class GameStateCaretaker {
		private List<GameStateMemento> gameStates = new List<GameStateMemento> ();

		public void Add(GameStateMemento memento) {
			gameStates.Add (memento);
		}

		public GameStateMemento Get(int index) {
			return gameStates [index];
		}
	}
}