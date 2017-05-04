using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Character sheet controller.
/// </summary>
public class CharacterSheetController : MonoBehaviour {

	public Image uiPortrait;

	public Text uiName;
	public Text uiLevel;
	public Text uiClass;

	public Text uiHitPoints;
	public Image hitPointsBar;

	public Text uiAbilityPoints;
	public Image abilityPointsBar;

	public Text uiExperiencePoints;
	public Image experiencePointsBar;

	/// <summary>
	/// Activate the character sheet of the specified unit.
	/// </summary>
	/// <param name="player">Unit.</param>
	public void Activate(Unit unit) {
		Activate (
			unit,
			unit.GetPortrait(),
			unit.UnitData.FirstName,
			unit.UnitData.LastName,
			unit.UnitData.Class,
			(int) unit.GetLevelAttribute().CurrentValue,
			(int) unit.GetHitPointsAttribute().CurrentValue,
			(int) unit.GetHitPointsAttribute().MaximumValue,
			(int) unit.GetAbilityPointsAttribute().CurrentValue,
			(int) unit.GetAbilityPointsAttribute().MaximumValue,
			(int) unit.GetMovementAttribute().CurrentValue
		);
	}		

	/// <summary>
	/// Activate the character sheet with the specified arguments.
	/// </summary>
	/// <param name="unit">Unit.</param>"> 
	/// <param name="portrait">Portrait.</param>
	/// <param name="firstName">First name.</param>
	/// <param name="lastName">Last name.</param>
	/// <param name="level">Level.</param>
	/// <param name="className">Class Name.</param>
	/// <param name="currentHitPoints">Current hit points.</param>
	/// <param name="totalHitPoints">Total hit points.</param>
	/// <param name="currentAbilityPoints">Current ability points.</param>
	/// <param name="totalAbilityPoints">Total ability points.</param>
	/// <param name="movement">Movement.</param>
	public void Activate(
		Unit unit,
		Sprite portrait,
		string firstName,
		string lastName,
		string className,
		int level,
		int currentHitPoints,
		int totalHitPoints,
		int currentAbilityPoints,
		int totalAbilityPoints,
		int movement
	) {
		uiPortrait.sprite = portrait;

		uiName.text = string.Format ("{0} {1}", firstName, lastName);
		uiLevel.text = string.Format ("Level: {0}", level);
		uiClass.text = className;

		uiHitPoints.text = string.Format ("{0}/{1}", currentHitPoints, totalHitPoints);
		unit.UpdateAttributeBar (hitPointsBar, currentHitPoints, totalHitPoints);

		uiAbilityPoints.text = string.Format ("{0}/{1}", currentAbilityPoints, totalAbilityPoints);
		unit.UpdateAttributeBar (abilityPointsBar, currentAbilityPoints, totalAbilityPoints);

		int xp = (int) unit.GetExperienceAttribute().CurrentValue;
		uiExperiencePoints.text = string.Format ("{0}/100", xp);
		unit.UpdateAttributeBar (experiencePointsBar, xp, 100);

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the character sheet.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive (false);
	}
}