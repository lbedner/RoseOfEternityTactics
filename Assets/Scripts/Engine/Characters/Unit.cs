using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Unit : MonoBehaviour {

	protected const float HIGHTLIGHT_COLOR_TRANSPARENCY = 0.7f;

	public string firstName = "Unknown";
	public string lastName = "";

	// Core attributes
	public int totalHitPoints = 1;
	public int totalAbilityPoints = 1;
	public int level = 1;
	public int movement = 4;
	public int speed = 3;

	public Sprite portrait;

	public Color color;

	public CharacterSheetController characterSheetController;
	public CombatMenuController combatMenuController;

	private bool isSelected = false;

	// Use this for initialization
	void Start () {
		transform.Find("Capsule").gameObject.GetComponent<Renderer> ().material.color = color;
		CurrentHitPoints = totalHitPoints;
		CurrentAbilityPoints = totalAbilityPoints;
	}

	public int CurrentHitPoints { get; set; }
	public int CurrentAbilityPoints { get; set; }

	// Implement these in children classes
	public abstract Color MovementTileColor { get; }
	public abstract bool IsPlayerControlled { get; }

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