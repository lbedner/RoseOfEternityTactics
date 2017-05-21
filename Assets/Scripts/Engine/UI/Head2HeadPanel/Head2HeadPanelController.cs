using UnityEngine;
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

			damage.text = string.Format ("Dmg: {0}",(damageAttribute * currentLevel));
			attackHitPercent.text = string.Format ("Hit: {0}%", 100);
			crititalHitPercent.text = string.Format ("Crit: {0}%", criticalChance);
			usedAbility.text = weapon.Name;
		} else {
			damage.text = "";
			attackHitPercent.text = "";
			crititalHitPercent.text = "";
			usedAbility.text = "";
		}
	}
}