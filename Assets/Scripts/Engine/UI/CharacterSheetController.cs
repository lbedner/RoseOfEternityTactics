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
	public Text uiHitPoints;
	public Text uiAbilityPoints;
	public Text uiMovement;

	/// <summary>
	/// Activate the character sheet of the specified unit.
	/// </summary>
	/// <param name="player">Unit.</param>
	public void Activate(Unit unit) {
		Activate (
			unit.portrait,
			unit.firstName,
			unit.lastName,
			unit.level,
			unit.CurrentHitPoints,
			unit.totalHitPoints,
			unit.CurrentAbilityPoints,
			unit.totalAbilityPoints,
			unit.movement
		);
	}		

	/// <summary>
	/// Activate the character sheet with the specified arguments.
	/// </summary>
	/// <param name="portrait">Portrait.</param>
	/// <param name="firstName">First name.</param>
	/// <param name="lastName">Last name.</param>
	/// <param name="level">Level.</param>
	/// <param name="currentHitPoints">Current hit points.</param>
	/// <param name="totalHitPoints">Total hit points.</param>
	/// <param name="currentAbilityPoints">Current ability points.</param>
	/// <param name="totalAbilityPoints">Total ability points.</param>
	/// <param name="movement">Movement.</param>
	public void Activate(
		Sprite portrait,
		string firstName,
		string lastName,
		int level,
		int currentHitPoints,
		int totalHitPoints,
		int currentAbilityPoints,
		int totalAbilityPoints,
		int movement
	) {
		uiPortrait.sprite = portrait;
		uiName.text = string.Format ("{0} {1}", firstName, lastName);
		uiLevel.text = string.Format ("LVL: {0}", level);
		uiHitPoints.text = string.Format ("HP: {0}/{1}", currentHitPoints, totalHitPoints);
		uiAbilityPoints.text = string.Format ("AP: {0}/{1}", currentAbilityPoints, totalAbilityPoints);
		uiMovement.text = string.Format ("MOV: {0}", movement);

		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the character sheet.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive (false);
	}
}