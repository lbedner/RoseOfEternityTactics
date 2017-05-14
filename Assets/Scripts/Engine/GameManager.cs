using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Game manager, implemented as a Singleton.
/// </summary>
public class GameManager : MonoBehaviour {

	private static GameManager _instance;

	[SerializeField]
	private TileMap _tileMap;

	// Controllers
	[SerializeField] private CameraController _cameraController;
	[SerializeField] private TurnOrderController _turnOrderController;
	[SerializeField] private MusicController _musicController;
	[SerializeField] private Head2HeadPanelController _sourceHead2HeadPanelController;
	[SerializeField] private Head2HeadPanelController _targetHead2HeadPanelController;
	[SerializeField] private UnitMenuController _unitMenuController;
	[SerializeField] private CharacterSheetController _characterSheetController;
	[SerializeField] private CombatMenuController _combatMenuController;

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static GameManager Instance { get { return _instance; } }

	/// <summary>
	/// Awake this instance and destroy duplicates.
	/// </summary>
	void Awake() {
		print ("GameManager.Awake()");
		if (_instance == null)
			_instance = this;
		else if (_instance != this)
			Destroy (this.gameObject);
		DontDestroyOnLoad (this.gameObject);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		print ("GameManager.Start()");

		// Initialize tilemap
		_tileMap.Initialize ();

		// Initialize camera controller
		_cameraController.Init (_tileMap.Width * _tileMap.TileSize, _tileMap.Height * _tileMap.TileSize, _tileMap.TileResolution);
	}

	/// <summary>
	/// Gets the allies.
	/// </summary>
	/// <returns>The allies.</returns>
	public List<Unit> GetAllies() {
		return _tileMap.GetAllies ();
	}

	/// <summary>
	/// Gets the camera controller.
	/// </summary>
	/// <returns>The camera controller.</returns>
	public CameraController GetCameraController() {return _cameraController;}

	/// <summary>
	/// Gets the turn order controller.
	/// </summary>
	/// <returns>The turn order controller.</returns>
	public TurnOrderController GetTurnOrderController() {return _turnOrderController;}

	/// <summary>
	/// Gets the tile map.
	/// </summary>
	/// <returns>The tile map.</returns>
	public TileMap GetTileMap() {return _tileMap;}

	/// <summary>
	/// Gets the music controller.
	/// </summary>
	/// <returns>The music controller.</returns>
	public MusicController GetMusicController() {return _musicController;}

	/// <summary>
	/// Gets the source head2 head panel controller.
	/// </summary>
	/// <returns>The source head2 head panel controller.</returns>
	public Head2HeadPanelController GetSourceHead2HeadPanelController() {return _sourceHead2HeadPanelController;}

	/// <summary>
	/// Gets the target head2 head panel controller.
	/// </summary>
	/// <returns>The source head2 head panel controller.</returns>
	public Head2HeadPanelController GetTargetHead2HeadPanelController() {return _targetHead2HeadPanelController;}

	/// <summary>
	/// Gets the unit menu controller.
	/// </summary>
	/// <returns>The unit menu controller.</returns>
	public UnitMenuController GetUnitMenuController() {return _unitMenuController;}

	/// <summary>
	/// Gets the character sheet controller.
	/// </summary>
	/// <returns>The character sheet controller.</returns>
	public CharacterSheetController GetCharacterSheetController() {return _characterSheetController;}

	/// <summary>
	/// Gets the combat menu controller.
	/// </summary>
	/// <returns>The combat menu controller.</returns>
	public CombatMenuController GetCombatMenuController() {return _combatMenuController;}
}