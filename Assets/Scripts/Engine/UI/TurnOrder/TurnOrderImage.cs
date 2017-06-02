using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnOrderImage : MonoBehaviour, IPointerClickHandler {

	private TileMap          _tileMap;
	private CombatController _combatController;

	public Unit Unit { get; set; }

	void Awake() {
		_tileMap = GameManager.Instance.GetTileMap ();

		//TODO: Need better solutiuon here. Never reference game objects by name.
		_combatController = GameObject.Find ("CombatController").GetComponent<CombatController> ();
	}

	/// <summary>
	/// Raises the pointer click event.
	/// When the portrait is clicked on, the camera will move to focus on the unit.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData) {
		if (eventData.button == PointerEventData.InputButton.Left) {
			StartCoroutine (GameManager.Instance.GetCameraController ().MoveToPosition (Unit.transform.position));
			Unit.ActivateCharacterSheet ();
			_combatController.TileHighlighter.HighlightTiles (Unit, TileMapUtil.WorldCenteredToTileMap(Unit.transform.position, _tileMap.TileSize));
		}
	}
}