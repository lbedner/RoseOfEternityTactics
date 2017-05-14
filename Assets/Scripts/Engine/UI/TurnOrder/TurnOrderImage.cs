using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnOrderImage : MonoBehaviour, IPointerClickHandler {

	private TileMap      _tileMap;
	private TileMapMouse _tileMapMouse;

	public Unit Unit { get; set; }

	void Awake() {
		_tileMap = GameManager.Instance.GetTileMap ();
		_tileMapMouse = _tileMap.GetComponent<TileMapMouse> ();
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
			_tileMapMouse.GetTileHighlighter ().HighlightTiles (Unit, TileMapUtil.WorldCenteredToTileMap(Unit.transform.position, _tileMap.TileSize));
		}
	}
}