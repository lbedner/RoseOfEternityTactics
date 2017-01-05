﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	public string firstName = "Unknown";
	public string lastName = "";

	// Core attributes
	public int totalHitPoints = 1;
	public int totalAbilityPoints = 1;
	public int level = 1;
	public int movement = 4;
	public int speed = 3;
	public int weaponRange = 1;

	public string weaponName;

	public Sprite portrait;

	public Image healthbar;

	public Color damagedColor = Color.red;
	public Color attackTileColor = new Color (1.0f, 0.0f, 0.0f, HIGHTLIGHT_COLOR_TRANSPARENCY);

	public CharacterSheetController characterSheetController;
	public CombatMenuController combatMenuController;

	private UnitAnimationController _unitAnimationController;

	private SpriteRenderer _spriteRenderer;

	private bool isSelected = false;

	// Use this for initialization
	void Start () {
		CurrentHitPoints = totalHitPoints;
		CurrentAbilityPoints = totalAbilityPoints;
		_unitAnimationController = transform.Find ("Sprite").GetComponent<UnitAnimationController> ();
		_spriteRenderer = transform.Find ("Sprite").GetComponent<SpriteRenderer> ();
	}

	public int CurrentHitPoints { get; set; }
	public int CurrentAbilityPoints { get; set; }

	public Vector3 Tile { get; set; }

	// Implement these in children classes
	public abstract Color MovementTileColor { get; }
	public abstract bool IsPlayerControlled { get; }

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

		// Get health % and clamp it between 0 and 1 so the health bar image doesn't go haywire
		float healthPercent = Mathf.Clamp ((float)CurrentHitPoints / (float)totalHitPoints, 0.0f, 1.0f);

		// Update the health bar scale in order to show it go up/down
		Vector3 currentScale = healthbar.rectTransform.localScale;
		healthbar.rectTransform.localScale = new Vector3 (healthPercent, currentScale.y, currentScale.z);
	}

	public Canvas GetCanvas() {
		return transform.Find ("Canvas").GetComponent<Canvas> ();
	}

	/// <summary>
	/// Gets facing direction in relation to the passed in unit.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="unit">Unit.</param>
	public TileDirection GetFacing(Unit unit) {
		return GetFacing (unit.Tile);
	}

	/// <summary>
	/// Gets facing direction in relation to the passed in Vector.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="target">Target.</param>
	public TileDirection GetFacing(Vector3 target) {
		return GetFacing(Tile, target);
	}

	/// <summary>
	/// Gets facing direction from source to target Vector.
	/// </summary>
	/// <returns>The facing.</returns>
	/// <param name="target">Target tile.</param>
	public TileDirection GetFacing(Vector3 source, Vector3 target) {
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

	public void ActivateCharacterSheet() {
		characterSheetController.Activate(this);
	}

	public void DeactivateCharacterSheet() {
		characterSheetController.Deactivate();
	}

	public void ActivateCombatMenu() {
		if (!isSelected) {
			combatMenuController.Activate ("Move", "Attack", "Ability", "Item", "End Turn");
			isSelected = true;
		}
	}

	public void DeactivateCombatMenu() {
		if (isSelected) {
			combatMenuController.Deactivate ();
			isSelected = false;
		}
	}
}