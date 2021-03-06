﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Head2HeadPanelController : MonoBehaviour {

	public enum Head2HeadState {
		ATTACKING,
		DEFENDING,
	}

	public Image portrait;

	public new Text name;
	public Text level;
	public Text @class;

	public Text hitPoints;
	public Image hitPointsBar;
	public Text abilityPoints;
	public Image abilityPointsBar;
	public Text experiencePoints;
	public Image experiencePointsBar;

	public Text damage;
	public Text attackHitPercent;
	public Text crititalHitPercent;

	public Text usedAbility;
	public Text turns;

	public void Load(Unit unit, Head2HeadState head2HeadState) {
		int currentLevel = (int) unit.GetLevelAttribute ().CurrentValue;

		portrait.sprite = unit.GetPortrait ();

		name.text = unit.GetFullName ();
		level.text = string.Format ("Level: {0}", currentLevel);
		@class.text = unit.UnitData.Class;

		// Hit Points
		int currentHitPoints = (int) unit.GetHitPointsAttribute ().CurrentValue;
		int maxHitPoints = (int) unit.GetHitPointsAttribute ().MaximumValue;
		hitPoints.text = string.Format ("{0}/{1}", currentHitPoints, maxHitPoints);
		unit.UpdateAttributeBar (hitPointsBar, currentHitPoints, maxHitPoints);

		// Ability Points
		int currentAbilityPoints = (int) unit.GetAbilityPointsAttribute().CurrentValue;
		int maxAbilityPoints = (int)unit.GetAbilityPointsAttribute ().MaximumValue;
		abilityPoints.text = string.Format ("{0}/{1}", currentAbilityPoints, maxAbilityPoints);
		unit.UpdateAttributeBar (abilityPointsBar, currentAbilityPoints, maxAbilityPoints);

		// Experience Points
		int xp = (int) unit.GetExperienceAttribute().CurrentValue;
		experiencePoints.text = string.Format ("{0}/100", xp);
		unit.UpdateAttributeBar (experiencePointsBar, xp, 100);

		// TODO: Mock up rest of attributes, at some point, pull them from Combat class
		if (head2HeadState == Head2HeadState.ATTACKING) {

			Item weapon = unit.GetItemInSlot (InventorySlots.SlotType.RIGHT_HAND);
			int damageAttribute = (int) weapon.GetAttribute (AttributeEnums.AttributeType.DAMAGE).CurrentValue;
			int criticalChance = (int) weapon.GetAttribute (AttributeEnums.AttributeType.CRITICAL_CHANCE).CurrentValue;

			int finalDamage = 0;
			Ability ability = unit.Action.Ability;
			Item item = unit.Action.Item;
			if (ability != null) {
				finalDamage = new Calculator (unit).Action.DamageToTargets [0];				
				usedAbility.text = ability.Name;

				// Determine text to display for ability "turns"
				string turnsText = "";
				if (ability.Turns <= 0)
					turnsText = "Instant";
				else
					turnsText = string.Format ("{0} Turns", ability.Turns);
				turns.text = string.Format ("Execution Time: {0}", turnsText);
				damage.text = string.Format ("Dmg: {0}",(finalDamage));
				attackHitPercent.text = string.Format ("Hit: {0}%", 100);
				crititalHitPercent.text = string.Format ("Crit: {0}%", criticalChance);
			}
			else if (item != null) {
				//finalDamage = 10;
				usedAbility.text = item.Name;
				turns.text = "Instant";
				damage.text = "";
				//attackHitPercent.text = string.Format ("Heal: {0}",(finalDamage));
				attackHitPercent.text = "";
				crititalHitPercent.text = "";
			}
			else {
				finalDamage = damageAttribute * currentLevel;
				usedAbility.text = weapon.Name;
				damage.text = string.Format ("Dmg: {0}",(finalDamage));
				attackHitPercent.text = string.Format ("Hit: {0}%", 100);
				crititalHitPercent.text = string.Format ("Crit: {0}%", criticalChance);
			}
		} else {
			damage.text = "";
			attackHitPercent.text = "";
			crititalHitPercent.text = "";
			usedAbility.text = "";
			turns.text = "";
		}
		this.gameObject.SetActive (true);
	}
}