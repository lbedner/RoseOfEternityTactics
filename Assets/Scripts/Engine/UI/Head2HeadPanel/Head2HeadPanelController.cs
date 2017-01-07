using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Head2HeadPanelController : MonoBehaviour {

	public enum Head2HeadState {
		ATTACKING,
		DEFENDING,
	}

	public Image portrait;

	public Text name;
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
		portrait.sprite = unit.portrait;

		name.text = unit.GetFullName ();
		level.text = string.Format ("Level: {0}", unit.level);
		@class.text = unit.@class;

		// Hit Points
		hitPoints.text = string.Format ("{0}/{1}", unit.CurrentHitPoints, unit.totalHitPoints);
		unit.UpdateAttributeBar (hitPointsBar, unit.CurrentHitPoints, unit.totalHitPoints);

		// Ability Points
		abilityPoints.text = string.Format ("{0}/{1}", unit.CurrentAbilityPoints, unit.totalAbilityPoints);
		unit.UpdateAttributeBar (abilityPointsBar, unit.CurrentAbilityPoints, unit.totalAbilityPoints);

		// Experience Points
		experiencePoints.text = string.Format ("{0}/100", unit.CurrentExperiencePoints);
		unit.UpdateAttributeBar (experiencePointsBar, unit.CurrentExperiencePoints, 100);

		// TODO: Mock up rest of attributes, at some point, pull them from Combat class
		if (head2HeadState == Head2HeadState.ATTACKING) {
			damage.text = string.Format ("Dmg: {0}", unit.level * 2);
			attackHitPercent.text = string.Format ("Hit: {0}%", 100);
			crititalHitPercent.text = string.Format ("Crit: {0}%", 0);
			usedAbility.text = unit.weaponName;
		} else {
			damage.text = "";
			attackHitPercent.text = "";
			crititalHitPercent.text = "";
			usedAbility.text = "";
		}
	}
}