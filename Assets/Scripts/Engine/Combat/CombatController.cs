﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatController : StateMachine { 

	public CameraController CameraController { get; private set; }
	public CharacterSheetController CharacterSheetController { get; private set; }
	public TileMap TileMap { get; private set; }
	public TurnOrderController TurnOrderController { get; private set; }
	public MusicController MusicController { get; private set; }
	public UnitMenuController UnitMenuController { get; private set; }
	public ScreenFader ScreenFader { get; private set; }
	public TerrainDetailsController TerrainDetailsController { get; private set; }

	public Pathfinder Pathfinder { get; private set; }

	public TileDiscoverer TileDiscoverer { get; private set; }

	public ActionController ActionController { get { return _actionController; } }
	public Head2HeadRootPanelController Head2HeadPanel { get { return _head2HeadPanel; } }
	public SelectionIndicator SelectionIndicator { get { return _selectionIndicator; } }
	public Transform SelectionIcon { get { return _selectionIcon; } }
	public RawImage FadeOutUIImage { get { return _fadeOutUIImage; } }
	public GameObject MissionObjectivesPanel { get { return _missionObjectivesPanel; } }
	public GameObject PostCombatStatsPanel { get { return _postCombatStatsPanel; } }
	public AudioSource CursorMoveSource { get { return _cursorMoveSource; } }
	public AudioSource ConfirmationSource { get { return _confirmationSource; } }
	public Button AttackButton { get { return _attackButton; } }
	public Button EndTurnButton { get { return _endTurnButton; } }
	public Button ActionConfirmButton { get { return _actionConfirmButton; } }
	public Button ActionCancelButton { get { return _actionCancelButton; } }
	public Button MissionObjectivesPanelContinueButton { get { return _missionObjectivesPanelContinueButton; } }
	public Button MissionEndPanelContinueButton { get { return _missionEndPanelContinueButton; } }

	public Unit HighlightedUnit { get; set; }
	public Vector3 CurrentTileCoordinates { get; set; }
	public Vector3 CurrentUnitPosition { get; set; }
	public Vector3 OldUnitPosition { get; set; }
	public List<Unit> IntendedActionTargets { get; set; }
	public Unit.TileDirection CurrentUnitTileDirection { get; set; }
	public Unit.TileDirection OldUnitTileDirection { get; set; }

	public List<RadialButtonContainer> RadialButtonContainers { get; set; }
	public RadialButtonContainer CurrentRadialButtonContainer { get; set; }
	public RadialButtonContainer PreviousRadialButtonContainer { get; set; }

	[SerializeField] private ActionController _actionController;
	[SerializeField] private Head2HeadRootPanelController _head2HeadPanel;
	[SerializeField] private SelectionIndicator _selectionIndicator;
	[SerializeField] private Transform _movementHighlightCube;
	[SerializeField] private RawImage _fadeOutUIImage;
	[SerializeField] private GameObject _missionObjectivesPanel;
	[SerializeField] private GameObject _postCombatStatsPanel;
	[SerializeField] private AudioSource _cursorMoveSource;
	[SerializeField] private AudioSource _confirmationSource;
	[SerializeField] private Button _attackButton;
	[SerializeField] private Button _endTurnButton;
	[SerializeField] private Button _actionConfirmButton;
	[SerializeField] private Button _actionCancelButton;
	[SerializeField] private Button _missionObjectivesPanelContinueButton;
	[SerializeField] private Button _missionEndPanelContinueButton;

	private Transform _selectionIcon;

	// Use this for initialization
	void Start () {
		print ("CombatController.Start()");
		GameManager gameManager = GameManager.Instance;

		CameraController = gameManager.GetCameraController ();
		CharacterSheetController = gameManager.GetCharacterSheetController ();
		TileMap = gameManager.GetTileMap ();
		TurnOrderController = gameManager.GetTurnOrderController ();
		MusicController = gameManager.GetMusicController ();
		UnitMenuController = gameManager.GetUnitMenuController ();
		UnitMenuController.Initialize ();
		ScreenFader = new ScreenFader ();
		TerrainDetailsController = gameManager.GetTerrainDetailsController ();
		TileDiscoverer = new TileDiscoverer (TileMap.GetTileMapData ());
		Pathfinder = new Pathfinder (TileMap.GetTileMapData(), TileMap.GetGraph().GetGraph());

		_selectionIcon = _selectionIndicator.transform;

		IntendedActionTargets = new List<Unit> ();

		RadialButtonContainers = new List<RadialButtonContainer> ();

		ChangeState<InitCombatState> ();
	}

	/// <summary>
	/// Determines if the mouse cursor should be shown.
	/// </summary>
	/// <param name="showCursor">If set to <c>true</c> show cursor.</param>
	public void ShowCursor(bool showCursor) {
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
	public void ShowTileSelector(bool showTileSelector) {
		_selectionIcon.gameObject.SetActive (showTileSelector);
	}

	/// <summary>
	/// Shows the cursor and tile selector.
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public void ShowCursorAndTileSelector(bool show) {
		ShowCursor(show);
		ShowTileSelector(show);
	}

	/// <summary>
	/// Highlights the character.
	/// </summary>
	/// <param name="character">Character.</param>
	public void HighlightCharacter(Unit character) {
		_selectionIcon.position = TileMapUtil.WorldCenteredToUncentered(character.transform.position, TileMap.TileSize);
	}

	/// <summary>
	/// Highlights the action targets.
	/// </summary>
	/// <param name="targets">Targets.</param>
	public void HighlightActionTargets(List<Unit> targets) {
		foreach (Unit unit in targets) {
			HighlightActionTarget (unit);
		}
	}

	/// <summary>
	/// Highlights the action target.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void HighlightActionTarget(Unit unit) {
		IntendedActionTargets.Add (unit);
		unit.ShowDamagedColor (true);
	}

	/// <summary>
	/// Clears the action targets.
	/// </summary>
	public void ClearActionTargets() {
		foreach (Unit unit in IntendedActionTargets)
			if (!unit.TileHighlighter.IsPersistent)
				unit.ShowDamagedColor(false);
		IntendedActionTargets.Clear ();
		TurnOrderController.UntargetUnitImages ();
	}
}
