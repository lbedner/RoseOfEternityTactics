using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

using RoseOfEternity;

public abstract class Unit : MonoBehaviour {

	/// <summary>
	/// Tile direction.
	/// </summary>
	public enum TileDirection {
		NORTH,
		EAST,
		SOUTH,
		WEST,
	}

	protected const float HIGHTLIGHT_COLOR_TRANSPARENCY = 0.7f;

	public Image healthbar;

	public Color damagedColor = Color.red;
	public Color attackTileColor = new Color (1.0f, 0.0f, 0.0f, HIGHTLIGHT_COLOR_TRANSPARENCY);

	private CharacterSheetController _characterSheetController;
	private CombatMenuController _combatMenuController;

	private UnitAnimationController _unitAnimationController;

	private SpriteRenderer _spriteRenderer;

	private bool isSelected = false;

	private AttributeCollection _attributeCollection;
	private Inventory _inventory;
	private InventorySlots _inventorySlots;

	private Canvas _canvas;

	public UnitData UnitData { get; set; }
	public string ResRef { get; private set; }

	public Action Action { get; set; }

	public TileDirection FacedDirection { get; set; }

	/// <summary>
	/// Gets or sets the tile.
	/// </summary>
	/// <value>The tile.</value>
	public Vector3 Tile { get; set; }

	// Use this for initialization
	void Start () {
		Transform unitSpriteTransform = transform.Find ("Sprite(Clone)");
		_unitAnimationController = unitSpriteTransform.GetComponent<UnitAnimationController> ();
		_spriteRenderer = unitSpriteTransform.GetComponent<SpriteRenderer> ();
		_attributeCollection = UnitData.AttributeCollection;
		SetMaximumValueToCurrentValue (AttributeEnums.AttributeType.HIT_POINTS);
		SetMaximumValueToCurrentValue (AttributeEnums.AttributeType.ABILITY_POINTS);
		_inventorySlots = UnitData.InventorySlots;
		_canvas = transform.Find ("Canvas").GetComponent<Canvas> ();
		FacedDirection = TileDirection.EAST;

		_characterSheetController = GameManager.Instance.GetCharacterSheetController ();
		_combatMenuController = GameManager.Instance.GetCombatMenuController ();
	}

	/// <summary>
	/// Instantiate the unit specified by the resRef.
	/// </summary>
	/// <param name="resRef">Res reference.</param>
	public static Unit InstantiateUnit(string resRef) {

		// Instantiate base unit and set unit data on it
		UnitData unitData = UnitDataManager.Instance.GlobalUnitDataCollection.GetByResRef (resRef).DeepCopy();
		string unitPrefabPath = "Prefabs/Characters/GameObjects/";
		string unitPrefab = "";
		float z = 0.0f;
		switch (unitData.Type) {
		case UnitData.UnitType.PLAYER:
			unitPrefab = "Player";
			z = 0.97f;
			break;
		case UnitData.UnitType.ENEMY:
			unitPrefab = "Enemy";
			z = -0.74f;
			break;
		}

		Unit unit = Instantiate(Resources.Load<Unit> (Path.Combine (unitPrefabPath, unitPrefab)));
		unit.ResRef = resRef;
		unit.UnitData = unitData;

		// Instantiate graphics for unit
		GameObject unitSprite = Instantiate (Resources.Load<GameObject> (unit.UnitData.Sprite));
		unitSprite.transform.SetParent (unit.transform, false);
		unitSprite.transform.localScale = new Vector3 (7.0f, 7.0f, 7.0f);
		unitSprite.transform.localPosition = new Vector3 (0.0f, 0.1f, z);
		unitSprite.transform.localEulerAngles = new Vector3 (90.0f, 0.0f, 0.0f);

		return unit;
	}

	// Implement these in children classes
	public abstract Color MovementTileColor { get; }
	public abstract bool IsPlayerControlled { get; }

	/// <summary>
	/// Gets the full name.
	/// </summary>
	/// <returns>The full name.</returns>
	public string GetFullName() {
		return string.Format ("{0} {1}".Trim (), UnitData.FirstName, UnitData.LastName);
	}

	/// <summary>
	/// Gets the animation controller.
	/// </summary>
	/// <returns>The animation controller.</returns>
	public UnitAnimationController GetAnimationController() {
		return _unitAnimationController;
	}

	/// <summary>
	/// Determines whether this instance is friendly to the specified unit.
	/// </summary>
	/// <returns><c>true</c> if this instance is friendly to the specified unit; otherwise, <c>false</c>.</returns>
	/// <param name="unit">Unit.</param>
	public bool IsFriendlyUnit(Unit unit) {
		return IsPlayerControlled == unit.IsPlayerControlled;
	}

	/// <summary>
	/// Updates the healthbar.
	/// </summary>
	public void UpdateHealthbar() {
		UpdateAttributeBar (healthbar, (int) GetHitPointsAttribute().CurrentValue, (int) GetHitPointsAttribute().MaximumValue);
	}

	/// <summary>
	/// Updates an attribute bar.
	/// </summary>
	/// <param name="hitPointsBar">Hit points bar.</param>
	public void UpdateAttributeBar(Image attributeBar, int currentValue, int totalValue) {

		// Get health % and clamp it between 0 and 1 so the health bar image doesn't go haywire
		float percent = Mathf.Clamp ((float)currentValue / (float)totalValue, 0.0f, 1.0f);

		// Update the health bar scale in order to show it go up/down
		Vector3 currentScale = attributeBar.rectTransform.localScale;
		attributeBar.rectTransform.localScale = new Vector3 (percent, currentScale.y, currentScale.z);
	}

	/// <summary>
	/// Gets the canvas.
	/// </summary>
	/// <returns>The canvas.</returns>
	public Canvas GetCanvas() {
		return _canvas;
	}

	/// <summary>
	/// Gets facing direction in relation to the passed in unit.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="unit">Unit.</param>
	public TileDirection GetDirectionToTarget(Unit unit) {
		return GetDirectionToTarget (unit.Tile);
	}

	/// <summary>
	/// Gets facing direction in relation to the passed in Vector.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="target">Target.</param>
	public TileDirection GetDirectionToTarget(Vector3 target) {
		return GetDirectionToTarget(Tile, target);
	}

	/// <summary>
	/// Gets facing direction from source to target Vector.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="target">Target tile.</param>
	public TileDirection GetDirectionToTarget(Vector3 source, Vector3 target) {
		TileDirection facing = TileDirection.NORTH;

		if (IsFacingNorth (source, target)) {
			facing = TileDirection.NORTH;
			if (IsFacingEast (source, target)) {
				if ((target.x - Tile.x) > target.z - Tile.z)
					facing = TileDirection.EAST;
			} else if (IsFacingWest (source, target)) {
				if ((Tile.x - target.x) > target.z - Tile.z)
					facing = TileDirection.WEST;
			}
		} else if (IsFacingSouth (source, target)) {
			facing = TileDirection.SOUTH;
			if (IsFacingEast (source, target)) {
				if ((target.x - Tile.x) > target.z - Tile.z)
					facing = TileDirection.EAST;
			} else if (IsFacingWest (source, target)) {
				if ((Tile.x - target.x) > target.z - Tile.z)
					facing = TileDirection.WEST;
			}
		}
		else if (IsFacingEast(source, target))
			facing = TileDirection.EAST;
		else if (IsFacingWest(source, target))
			facing = TileDirection.WEST;

		return facing;
	}

	/// <summary>
	/// This will color the unit to indicate that they've been hit
	/// </summary>
	/// <param name="showDamageColor">If set to <c>true</c> show damage color.</param>
	public void ShowDamagedColor(bool showDamageColor) {
		if (_spriteRenderer) {
			if (showDamageColor)
				_spriteRenderer.color = damagedColor;
			else
				_spriteRenderer.color = Color.white;
		}
	}

	/// <summary>
	/// Sets the maximum value to current value.
	/// </summary>
	/// <param name="type">Type.</param>
	private void SetMaximumValueToCurrentValue(AttributeEnums.AttributeType type) {
		_attributeCollection.Get (type).MaximumValue = _attributeCollection.Get (type).CurrentValue;
	}

	/// <summary>
	/// Determines whether this instance is facing north.
	/// </summary>
	/// <returns><c>true</c> if this instance is facing north; otherwise, <c>false</c>.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	private bool IsFacingNorth(Vector3 source, Vector3 target) {
		return target.z > source.z;
	}
		
	/// <summary>
	/// Determines whether this instance is facing east.
	/// </summary>
	/// <returns><c>true</c> if this instance is facing east; otherwise, <c>false</c>.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	private bool IsFacingEast(Vector3 source, Vector3 target) {
		return target.x > source.x;
	}
		
	/// <summary>
	/// Determines whether this instance is facing south.
	/// </summary>
	/// <returns><c>true</c> if this instance is facing south; otherwise, <c>false</c>.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	private bool IsFacingSouth(Vector3 source, Vector3 target) {
		return target.z < source.z;
	}
		
	/// <summary>
	/// Determines whether this instance is facing west.
	/// </summary>
	/// <returns><c>true</c> if this instance is facing west; otherwise, <c>false</c>.</returns>
	/// <param name="source">Source.</param>
	/// <param name="target">Target.</param>
	private bool IsFacingWest(Vector3 source, Vector3 target) {
		return target.x < source.x;
	}

	/// <summary>
	/// Activates the character sheet.
	/// </summary>
	public void ActivateCharacterSheet() {
		_characterSheetController.Activate(this);
	}

	/// <summary>
	/// Deactivates the character sheet.
	/// </summary>
	public void DeactivateCharacterSheet() {
		_characterSheetController.Deactivate();
	}

	/// <summary>
	/// Activates the combat menu.
	/// </summary>
	public void ActivateCombatMenu() {
		if (!isSelected) {
			_combatMenuController.Activate ("Move", "Attack", "Ability", "Item", "End Turn");
			isSelected = true;
		}
	}

	/// <summary>
	/// Deactivates the combat menu.
	/// </summary>
	public void DeactivateCombatMenu() {
		if (isSelected) {
			_combatMenuController.Deactivate ();
			isSelected = false;
		}
	}

	// ---------------- ATTRIBUTES WRAPPERS ---------------- //

	/// <summary>
	/// Gets the attribute.
	/// </summary>
	/// <returns>The attribute.</returns>
	/// <param name="type">Type.</param>
	public Attribute GetAttribute(AttributeEnums.AttributeType type) {
		return _attributeCollection.Get (type);
	}

	/// <summary>
	/// Gets the experience attribute.
	/// </summary>
	/// <returns>The experience attribute.</returns>
	public Attribute GetExperienceAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.EXPERIENCE);
	}

	/// <summary>
	/// Gets the level attribute.
	/// </summary>
	/// <returns>The level attribute.</returns>
	public Attribute GetLevelAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.LEVEL);
	}

	/// <summary>
	/// Gets the hit points attribute.
	/// </summary>
	/// <returns>The hit points attribute.</returns>
	public Attribute GetHitPointsAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.HIT_POINTS);
	}

	/// <summary>
	/// Gets the ability points attribute.
	/// </summary>
	/// <returns>The ability points attribute.</returns>
	public Attribute GetAbilityPointsAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.ABILITY_POINTS);
	}

	/// <summary>
	/// Gets the movement attribute.
	/// </summary>
	/// <returns>The movement attribute.</returns>
	public Attribute GetMovementAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.MOVEMENT);
	}

	/// <summary>
	/// Gets the speed attribute.
	/// </summary>
	/// <returns>The speed attribute.</returns>
	public Attribute GetSpeedAttribute() {
		return GetAttribute (AttributeEnums.AttributeType.SPEED);
	}

	/// <summary>
	/// Gets the item in slot.
	/// </summary>
	/// <returns>The item in slot.</returns>
	/// <param name="slotType">Slot type.</param>
	public Item GetItemInSlot(InventorySlots.SlotType slotType) {
		return _inventorySlots.Get (slotType);
	}

	/// <summary>
	/// Gets the inventory slots.
	/// </summary>
	/// <returns>The inventory slots.</returns>
	public InventorySlots GetInventorySlots() {
		return _inventorySlots;
	}

	/// <summary>
	/// Gets the weapon range.
	/// </summary>
	/// <returns>The weapon range.</returns>
	public int GetWeaponRange() {
		return (int) _inventorySlots.Get (InventorySlots.SlotType.RIGHT_HAND).GetAttribute (AttributeEnums.AttributeType.RANGE).CurrentValue;
	}

	/// <summary>
	/// Gets the portrait.
	/// </summary>
	/// <returns>The portrait.</returns>
	public Sprite GetPortrait() {
		return UnitData.Portrait;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Unit"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Unit"/>.</returns>
	public override string ToString () {
		return string.Format("{0} - {1}", GetFullName(), _attributeCollection.ToString ());
	}
}