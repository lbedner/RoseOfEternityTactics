using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using EternalEngine;

public class TurnOrderImage : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] Image _statusImage;
	
	private const float TIME_TO_SCALE = 0.125f;

	private readonly Vector3 DefaultScale = Vector3.one;
	private readonly Vector3 FlyoverScale = new Vector3 (1.2f, 1.2f, 1.2f);

	private readonly Color TargetedColor = Color.red;

	private TileMap          _tileMap;
	private CombatController _combatController;

	private Image _image;

	private bool _isScalingUp = false;
	private bool _isScalingDown = false;

	private TurnOrderController _turnOrderController;

	public Unit Unit { get; set; }

	void Awake() {
		_tileMap = GameManager.Instance.GetTileMap ();
		_image = GetComponent<Image> ();

		_turnOrderController = GameManager.Instance.GetTurnOrderController ();

		//TODO: Need better solutiuon here. Never reference game objects by name.
		_combatController = GameObject.Find ("CombatController").GetComponent<CombatController> ();
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter (PointerEventData eventData) {

		// Don't allow events if other UI is up
		if (!_combatController.MissionObjectivesPanel.activeSelf && 
			!_combatController.PostCombatStatsPanel.activeSelf && 
			!_combatController.UnitMenuController.IsActive() &&
			!_combatController.Head2HeadPanel.activeSelf) {
			if (!Unit.TileHighlighter.IsPersistent) {
				_turnOrderController.IsImageHighlighted = true;
				Highlight (true);
			}
			else
				_turnOrderController.ActivateTurnOrderUnitStatus (Unit);
		}
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit (PointerEventData eventData) {

		// Don't allow events if other UI is up
		if (!_combatController.MissionObjectivesPanel.activeSelf &&
			!_combatController.PostCombatStatsPanel.activeSelf && 
			!_combatController.UnitMenuController.IsActive() &&
			!_combatController.Head2HeadPanel.activeSelf) {
			if (!Unit.TileHighlighter.IsPersistent) {
				_turnOrderController.IsImageHighlighted = false;
				DeHighlight ();
			}
			else
				_turnOrderController.DeactivateTurnOrderUnitStatus ();
		}
	}

	/// <summary>
	/// Raises the pointer click event.
	/// When the portrait is clicked on, the camera will move to focus on the unit.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData) {
		
		// Don't allow events if mission panels are up
		if (!_combatController.MissionObjectivesPanel.activeSelf && !_combatController.PostCombatStatsPanel.activeSelf) {
			if (eventData.button == PointerEventData.InputButton.Left) {
				StartCoroutine (GameManager.Instance.GetCameraController ().MoveToPosition (Unit.transform.position));
				Unit.ActivateCharacterSheet ();
				Unit.TileHighlighter.HighlightTiles (Unit, TileMapUtil.WorldCenteredToTileMap (Unit.transform.position, _tileMap.TileSize));
			}
		}
	}

	/// <summary>
	/// Highlight this instance.
	/// </summary>
	public void Highlight(bool activateTurnOrderUnitStats) {
		Unit.Highlight ();
		_image.color = Unit.SelectedColor;
		StartCoroutine(ScaleComponentUp());

		if (activateTurnOrderUnitStats)
			_turnOrderController.ActivateTurnOrderUnitStatus (Unit);

		// If there is a deferred ability, target all units targeted by the ability
		if (Unit.HasDeferredAbility) {
			foreach (var unit in Unit.Action.Targets) {
				_turnOrderController.TargetUnitImage (unit);
				unit.Highlight ();
				unit.TileHighlighter.HighlightAttackTile (unit, unit.Tile.x, unit.Tile.z);
			}
		}
		else
			Unit.TileHighlighter.HighlightTiles (Unit, TileMapUtil.WorldCenteredToTileMap(Unit.transform.position, _tileMap.TileSize));
	}

	/// <summary>
	/// Sets the instance as targeted.
	/// </summary>
	public void SetTargeted() {
		_image.color = TargetedColor;
		StartCoroutine(ScaleComponentUp());
	}

	/// <summary>
	/// De-highlights this instance.
	/// </summary>
	public void DeHighlight() {
		Unit.Dehighlight ();
		_image.color = Unit.DefaultColor;
		StartCoroutine(ScaleComponentDown());
		Unit.TileHighlighter.RemoveHighlightedTiles ();
		_turnOrderController.DeactivateTurnOrderUnitStatus ();

		// If there is a deferred ability, un-target all units targeted by ability
		if (Unit.HasDeferredAbility) {
			_turnOrderController.UntargetUnitImages();
			foreach (var unit in Unit.Action.Targets) {
				unit.Dehighlight ();
				unit.TileHighlighter.RemoveHighlightedTiles ();
			}
		}	
	}

	/// <summary>
	/// Activates the status image.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	public void ActivateStatusImage(Sprite sprite) {
		_statusImage.sprite = sprite;
		_statusImage.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivates the status image.
	/// </summary>
	public void DeactivateStatusImage() {
		_statusImage.sprite = null;
		_statusImage.gameObject.SetActive(false);
	}

	/// <summary>
	/// Scale up the button's size.
	/// </summary>
	/// <returns>The button up.</returns>
	private IEnumerator ScaleComponentUp() {
		if (!_isScalingUp) {
			_isScalingUp = true;
			yield return StartCoroutine (Utility.ScaleComponent(TIME_TO_SCALE, transform, transform.localPosition, transform.localPosition, transform.localScale, FlyoverScale));
			_isScalingUp = false;
		}
	}

	/// <summary>
	/// Scale down the button's size.
	/// </summary>
	/// <returns>The button up.</returns>
	private IEnumerator ScaleComponentDown() {
		if (!_isScalingDown) {
			_isScalingDown = true;
			yield return StartCoroutine (Utility.ScaleComponent(TIME_TO_SCALE, transform, transform.localPosition, transform.localPosition, transform.localScale, DefaultScale));
			_isScalingDown = false;
		}
	}
}