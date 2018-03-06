using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using EternalEngine;

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

	protected const float HIGHTLIGHT_COLOR_TRANSPARENCY         = 0.5f;
	protected const float SELECTED_HIGHLIGHT_COLOR_TRANSPARENCY = 0.7f;

	public Image healthbar;

	public Color AttackTileColor { 
		get {
			return new Color (1.0f, 0.0f, 0.0f, current_highlight_color_transparency);
		}
	}

	protected float current_highlight_color_transparency = HIGHTLIGHT_COLOR_TRANSPARENCY;

	private Color _damagedColor = Color.red;

	private CharacterSheetController _characterSheetController;
	private CombatMenuController _combatMenuController;

	private UnitAnimationController _unitAnimationController;

	private SpriteRenderer _spriteRenderer;

	private bool isSelected = false;

	private AttributeCollection _attributeCollection;
	private Inventory _inventory;
	private InventorySlots _inventorySlots;
	private EffectCollection _effectCollection;
	private ModifyAttributeEffectCollection _modifyAttributeEffectCollection;
	private List<UnitColorEffect> _unitColorEffects;

	private Canvas _canvas;

	private AudioSource _movementAudioSource;
	private AudioSource _weaponAudioSource;

	[SerializeField] private Transform _movementHighlightCube;

	private Color _currentColor;
	private bool _changingColors = false;
	private Coroutine _currentChangingColorsCoroutine;
	private Coroutine _currentChangingColorsToCoroutine;
	private Coroutine _currentChangingColorsFromCoroutine;
	private bool _highlighted = false;

	public UnitData UnitData { get; set; }
	public string ResRef { get; private set; }

	public Action Action { get; set; }

	public TileDirection FacedDirection { get; set; }

	public TileHighlighter TileHighlighter { get; private set; }

	public bool HasDeferredAbility { get; set; }
	public bool HasExecutedDeferredAbility { get; set; }

	/// <summary>
	/// Gets or sets the tile.
	/// </summary>
	/// <value>The tile.</value>
	public Vector3 Tile { get; set; }

	public Color DefaultColor { get { return Color.white; } }

	// Implement these in children classes
	public abstract Color MovementTileColor { get; }
	public abstract bool IsPlayerControlled { get; }
	public abstract Color SelectedColor { get; }

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		Transform unitSpriteTransform = transform.Find ("Sprite(Clone)");
		_unitAnimationController = unitSpriteTransform.GetComponent<UnitAnimationController> ();
		_spriteRenderer = unitSpriteTransform.GetComponent<SpriteRenderer> ();
		_attributeCollection = UnitData.AttributeCollection;
		_effectCollection = new EffectCollection ();
		_modifyAttributeEffectCollection = new ModifyAttributeEffectCollection ();
		_unitColorEffects = new List<UnitColorEffect> ();
		SetMaximumValueToCurrentValue (AttributeEnums.AttributeType.HIT_POINTS);
		SetMaximumValueToCurrentValue (AttributeEnums.AttributeType.ABILITY_POINTS);
		_inventorySlots = UnitData.InventorySlots;
		_canvas = transform.Find ("Canvas").GetComponent<Canvas> ();
		FacedDirection = TileDirection.EAST;

		_characterSheetController = GameManager.Instance.GetCharacterSheetController ();
		_combatMenuController = GameManager.Instance.GetCombatMenuController ();

		TileHighlighter = new TileHighlighter (GameManager.Instance.GetTileMap (), _movementHighlightCube);

		_movementAudioSource = gameObject.AddComponent<AudioSource> ();
		_weaponAudioSource = gameObject.AddComponent<AudioSource> ();

		_currentColor = DefaultColor;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		
		// Show color effects on units if available
		if (!_highlighted)
			_currentChangingColorsCoroutine = StartCoroutine(ChangeColors());
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

	/// <summary>
	/// Gets the full name.
	/// </summary>
	/// <returns>The full name.</returns>
	public string GetFullName() {
		return string.Format ("{0} {1}".Trim (), UnitData.FirstName, UnitData.LastName);
	}

	/// <summary>
	/// Sets the unit as selected.
	/// </summary>
	public void Highlight() {

		// If unit is highlighted, need to stop all coroutines that change colors
		StopChangeColorCoroutines();

		// Change to highlighted/selected color
		_spriteRenderer.color = SelectedColor;
		_highlighted = true;
	}

	/// <summary>
	/// Sets the unit as unselected.
	/// </summary>
	public void Dehighlight() {
		if (!TileHighlighter.IsPersistent && _spriteRenderer != null)
			_spriteRenderer.color = _currentColor;
		_highlighted = false;
	}

	/// <summary>
	/// Select this instance.
	/// </summary>
	public void Select() {
		current_highlight_color_transparency = SELECTED_HIGHLIGHT_COLOR_TRANSPARENCY;
	}

	/// <summary>
	/// Unselect this instance.
	/// </summary>
	public void Unselect() {
		if (!TileHighlighter.IsPersistent)
			current_highlight_color_transparency = HIGHTLIGHT_COLOR_TRANSPARENCY;
	}

	/// <summary>
	/// Changes the colors of a unit over time.
	/// </summary>
	/// <returns>The colors.</returns>
	public IEnumerator ChangeColors() {

		// Show color effects on units if available
		float duration = 1.0f;
		if (!_changingColors && _unitColorEffects.Count > 0) {
			_changingColors = true;
			foreach (var effect in _unitColorEffects) {
				_currentChangingColorsToCoroutine = StartCoroutine (ChangeColor (duration, _currentColor, effect.GetColor ()));
				yield return _currentChangingColorsToCoroutine;
			}
			_currentChangingColorsFromCoroutine = StartCoroutine (ChangeColor (duration, _currentColor, DefaultColor));
			yield return _currentChangingColorsFromCoroutine;
			_changingColors = false;
		}
	}

	/// <summary>
	/// Changes the color of a unit over time.
	/// </summary>
	/// <returns>The colors.</returns>
	/// <param name="duration">Duration.</param>
	/// <param name="oldColor">Old color.</param>
	/// <param name="newColor">New color.</param>
	public IEnumerator ChangeColor(float duration, Color oldColor, Color newColor) {
		float elapsedTime = 0.0f;
		while (elapsedTime < duration) {
			_spriteRenderer.color = Color.Lerp (oldColor, newColor, (elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_currentColor = newColor;
	}

	/// <summary>
	/// Stops the change color coroutines.
	/// </summary>
	public void StopChangeColorCoroutines() {
		if (_currentChangingColorsCoroutine != null) {
			StopCoroutine (_currentChangingColorsCoroutine);
			_currentChangingColorsCoroutine = null;
		}
		if (_currentChangingColorsToCoroutine != null) {
			StopCoroutine (_currentChangingColorsToCoroutine);
			_currentChangingColorsToCoroutine = null;
		}
		if (_currentChangingColorsFromCoroutine != null) {
			StopCoroutine (_currentChangingColorsFromCoroutine);
			_currentChangingColorsFromCoroutine = null;
		}

		// Reset all color changing fields
		_currentColor = DefaultColor;
		_changingColors = false;
		_spriteRenderer.color = _currentColor;
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
				_spriteRenderer.color = _damagedColor;
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
	/// Determines whether this instance is dead.
	/// </summary>
	/// <returns><c>true</c> if this instance is dead; otherwise, <c>false</c>.</returns>
	public bool IsDead() {
		return GetHitPointsAttribute ().CurrentValue <= 0;
	}

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
	/// Gets the weapon in the right hand.
	/// </summary>
	/// <returns>The weapon.</returns>
	public Item GetWeaponInRightHand() {
		return GetItemInSlot (InventorySlots.SlotType.RIGHT_HAND);
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
	/// Plays the movement sound.
	/// </summary>
	public void PlayMovementSound() {
		if (_movementAudioSource.clip == null)
			_movementAudioSource.clip = UnitData.MovementSound;
		_movementAudioSource.PlayOneShot (_movementAudioSource.clip);
	}

	/// <summary>
	/// Plays the weapon sound.
	/// </summary>
	public void PlayWeaponSound() {
		if (_weaponAudioSource.clip == null) {
			Item weapon = GetWeaponInRightHand ();
			if (weapon.Sound != null)
				_weaponAudioSource.clip = weapon.Sound;
			else
				return;
		}
		_weaponAudioSource.PlayOneShot (_weaponAudioSource.clip);
	}

	// ---------------- EFFECTS ---------------- //

	/// <summary>
	/// Adds the effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void AddEffect(Effect effect) {
		_effectCollection.Add (effect);
	}

	/// <summary>
	/// Removes the effect.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void RemoveEffect(Effect effect) {
		_effectCollection.Remove (effect);
	}

	/// <summary>
	/// Decrements the effect turns.
	/// </summary>
	public void DecrementEffectTurns() {
		List<Effect> effectsToRemove = new List<Effect> ();
		List<Effect> effects = _effectCollection.GetEffects ();

		// Decrement turns on all effects
		foreach (var effect in effects) {
			Effect effectToRemove = effect.DecrementTurn (this);
			if (effectToRemove != null)
				effectsToRemove.Add (effectToRemove);
		}

		// Remove all effects that have no more returns
		foreach (var effect in effectsToRemove)
			RemoveEffect (effect);
	}

	/// <summary>
	/// Gets the effects.
	/// </summary>
	/// <returns>The effects.</returns>
	public List<Effect> GetEffects() {
		return _effectCollection.GetEffects ();
	}

	/// <summary>
	/// Adds the modify attribute effect.
	/// </summary>
	/// <param name="attributeType">Attribute type.</param>
	/// <param name="effect">Effect.</param>
	public void AddModifyAttributeEffect(AttributeEnums.AttributeType attributeType, ModifyAttributeEffect effect) {
		_modifyAttributeEffectCollection.Add (attributeType, effect);
	}

	/// <summary>
	/// Removes the modify attribute effect.
	/// </summary>
	/// <param name="attributeType">Attribute type.</param>
	public void RemoveModifyAttributeEffect(AttributeEnums.AttributeType attributeType) {
		_modifyAttributeEffectCollection.Remove (attributeType);
	}

	/// <summary>
	/// Gets the modify attribute effect.
	/// </summary>
	/// <returns>The modify attribute effect.</returns>
	/// <param name="attributeType">Attribute type.</param>
	public ModifyAttributeEffect GetModifyAttributeEffect(AttributeEnums.AttributeType attributeType) {
		return _modifyAttributeEffectCollection.Get (attributeType);
	}

	/// <summary>
	/// Modifies the attribute effects to string.
	/// </summary>
	/// <returns>The attribute effects to string.</returns>
	public string ModifyAttributeEffectsToString() {
		return _modifyAttributeEffectCollection.ToString();
	}

	/// <summary>
	/// Adds the unit color effect to a collection.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void AddUnitColorEffect(UnitColorEffect effect) {
		_unitColorEffects.Add (effect);
	}

	/// <summary>
	/// Removes the unit color effect from the collection.
	/// </summary>
	/// <param name="effect">Effect.</param>
	public void RemoveUnitColorEffect(UnitColorEffect effect) {
		_unitColorEffects.Remove (effect);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Unit"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Unit"/>.</returns>
	public override string ToString () {
		return string.Format("{0} - {1}", GetFullName(), _attributeCollection.ToString ());
	}
}