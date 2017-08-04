using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformActionState : CombatState {

	public override void Enter() {
		print ("PerformActionState.Enter");
		base.Enter ();
	}
		
	/// <summary>
	/// Performs the action of the attacker against their targets.
	/// </summary>
	/// <returns>The action.</returns>
	/// <param name="attacker">Attacker.</param>
	 protected IEnumerator PerformAbilityAction(Unit attacker) {

		// Lower music so you can hear ability SFX
		yield return StartCoroutine(controller.MusicController.LowerCombatMusic ());

		// Show name of ability being used
		attacker.TileHighlighter.RemoveHighlightedTiles ();
		Ability ability = attacker.Action.Ability;
		controller.ActionController.Activate (ability.Name);
		yield return new WaitForSeconds (0.5f);

		// Spawn VFX on all targets
		List<Unit> targets = attacker.Action.Targets;
		List<GameObject> vfxGameObjects = ApplyVFXToTargets(ability.VFXPath, targets);

		// Start Combat
		Combat combat = new Combat (controller.HighlightedUnit);
		combat.Begin ();

		// Play weapon sound effect
		if (attacker.Action.Ability.Type == Ability.AbilityType.ATTACK)
			attacker.PlayWeaponSound();

		// Show target(s) being damaged
		ShowDamagedColorOnTargets (true, targets);

		// Play animations
		yield return StartCoroutine (PlayAttackAnimations (attacker, targets[0].Tile));
		yield return new WaitForSeconds (1.0f);

		// Stop showing target(s) being damaged
		ShowDamagedColorOnTargets (false, targets);

		// Show XP floaty text
		ShowXPFloatyText(combat.GetAwardedExperience(), attacker);
		yield return new WaitForSeconds (1.0f);

		// Destroy VFX's
		DestroyVFX(vfxGameObjects);

		// Bring music back up to normal volume
		yield return StartCoroutine(controller.MusicController.RaiseCombatMusic ());

		controller.ChangeState<TurnOverState> ();
	}

	/// <summary>
	/// Plays the attack animations.
	/// </summary>
	/// <returns>The attack animations.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="direction">Direction.</param>
	protected IEnumerator PlayAttackAnimations(Unit attacker, Vector3 direction) {

		//determine which way to swing, dependent on the direction the enemy is
		Unit.TileDirection facing = attacker.GetDirectionToTarget (direction);
		UnitAnimationController animationController = attacker.GetAnimationController ();
		switch (facing) {
		case Unit.TileDirection.NORTH:
			animationController.AttackNorth ();
			break;
		case Unit.TileDirection.EAST:
			animationController.AttackEast ();
			break;
		case Unit.TileDirection.SOUTH:
			animationController.AttackSouth ();
			break;
		case Unit.TileDirection.WEST:
			animationController.AttackWest ();
			break;
		}

		controller.ActionController.Deactivate ();
		yield return null;
	}

	/// <summary>
	/// Shows the XP floaty text.
	/// </summary>
	/// <param name="awardedExperience">Awarded experience.</param>
	/// <param name="attacker">Attacker.</param>
	private void ShowXPFloatyText(int awardedExperience, Unit attacker) {
		PopupTextController.Initialize(attacker.GetCanvas());
		PopupTextController.CreatePopupText (string.Format("+ {0} XP", awardedExperience), attacker.transform.position, Color.yellow);		
	}

	/// <summary>
	/// Applies the VFX to targets.
	/// </summary>
	/// <returns>The VFX to targets.</returns>
	/// <param name="vfxPath">Vfx path.</param>
	/// <param name="targets">Targets.</param>
	private List<GameObject> ApplyVFXToTargets(string vfxPath, List<Unit> targets) {
		List<GameObject> vfxGameObjects = new List<GameObject> ();
		if (vfxPath != null && vfxPath != "") {
			GameObject VFXPrefab = Resources.Load<GameObject> (vfxPath);
			foreach (var target in targets) {
				GameObject VFX = Instantiate (VFXPrefab);
				VFX.transform.SetParent (target.transform, false);
				VFX.transform.localPosition = new Vector3 (0, 1, 0);
				vfxGameObjects.Add (VFX);
			}
		}
		return vfxGameObjects;
	}

	/// <summary>
	/// Destroys the VFX's.
	/// </summary>
	/// <param name="vfxGameObjects">Vfx game objects.</param>
	private void DestroyVFX(List<GameObject> vfxGameObjects) {
		foreach (var vfx in vfxGameObjects)
			Destroy(vfx);		
	}

	/// <summary>
	/// Shows the damaged color on targets.
	/// </summary>
	/// <param name="showDamagedColor">If set to <c>true</c> show damaged color.</param>
	/// <param name="targets">Targets.</param>
	private void ShowDamagedColorOnTargets(bool showDamagedColor, List<Unit> targets) {
		foreach (var target in targets)
			target.ShowDamagedColor (showDamagedColor);
	}
}