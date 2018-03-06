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

		if (attacker.Action.Item != null) {

			// Lower music so you can hear ability SFX
			yield return StartCoroutine (controller.MusicController.LowerCombatMusic ());

			// Show name of ability being used
			attacker.TileHighlighter.RemoveHighlightedTiles ();
			Item item = attacker.Action.Item;
			controller.ActionController.Activate (item.Name);
			yield return new WaitForSeconds (0.5f);

			// Spawn VFX on all targets
			List<Unit> targets = attacker.Action.Targets;
			List<GameObject> vfxGameObjects = ApplyVFXToTargets (item.VFXPath, targets);

			// Iterate over all effects, apply them, and if there is something to display, show it
			PopupTextController.Initialize (attacker.GetCanvas ());
			List<Effect> effects = ConsumableEffectCalculator.GetConsumableEffects(item);
			yield return StartCoroutine(ApplyEffects (targets, effects));
				
			attacker.UpdateHealthbar ();

			// Show XP floaty text
			ShowXPFloatyText (new ExperienceManager().AwardConsumableExperience(attacker), attacker);
			yield return new WaitForSeconds (1.0f);

			// Destroy VFX's
			DestroyVFX (vfxGameObjects);

			// Remove name of used ability
			controller.ActionController.Deactivate ();

			// Bring music back up to normal volume
			yield return StartCoroutine (controller.MusicController.RaiseCombatMusic ());

			controller.ChangeState<TurnOverState> ();
		}
		// If the ability takes more than x turns, defer the ability action
		else if (!attacker.HasDeferredAbility && attacker.Action.Ability.Turns > 1) {
			attacker.HasDeferredAbility = true;
			yield return null;
			controller.ChangeState<TurnOverState> ();
		}
		else {

			// Reset bool flags depending on if a deferred attack is being executed
			if (attacker.HasDeferredAbility) {
				attacker.HasDeferredAbility = false;
				attacker.HasExecutedDeferredAbility = true;
			}

			// Lower music so you can hear ability SFX
			yield return StartCoroutine (controller.MusicController.LowerCombatMusic ());

			// Show name of ability being used
			attacker.TileHighlighter.RemoveHighlightedTiles ();
			Ability ability = attacker.Action.Ability;
			controller.ActionController.Activate (ability.Name);
			yield return new WaitForSeconds (0.5f);

			// Spawn VFX on all targets
			List<Unit> targets = attacker.Action.Targets;
			List<GameObject> vfxGameObjects = ApplyVFXToTargets (ability.VFXPath, targets);

			// Start Combat
			Combat combat = new Combat (controller.HighlightedUnit);
			combat.Begin ();

			// Play weapon sound effect
			Ability.AbilityType abilityType = attacker.Action.Ability.Type;
			if (abilityType == Ability.AbilityType.ATTACK || abilityType == Ability.AbilityType.TALENT)
				attacker.PlayWeaponSound ();

			// Show target(s) being damaged
			ShowDamagedColorOnTargets (true, targets);

			// Play animations
			yield return StartCoroutine (PlayAttackAnimations (attacker, targets [0].Tile));

			// Iterate over all effects, apply them, and if there is something to display, show it
			PopupTextController.Initialize (attacker.GetCanvas ());
			yield return StartCoroutine(ApplyEffectsByUnit(attacker.Action.GetEffectsByUnit()));

			yield return new WaitForSeconds (1.0f);

			// Stop showing target(s) being damaged
			ShowDamagedColorOnTargets (false, targets);

			// Show XP floaty text
			ShowXPFloatyText (combat.GetAwardedExperience (), attacker);
			yield return new WaitForSeconds (1.0f);

			// Destroy VFX's
			DestroyVFX (vfxGameObjects);

			// Remove name of used ability
			controller.ActionController.Deactivate ();

			// Bring music back up to normal volume
			yield return StartCoroutine (controller.MusicController.RaiseCombatMusic ());

			controller.ChangeState<TurnOverState> ();
		}
	}

	/// <summary>
	/// Plays the attack animations.
	/// </summary>
	/// <returns>The attack animations.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="direction">Direction.</param>
	protected IEnumerator PlayAttackAnimations(Unit attacker, Vector3 direction) {

		Ability.AbilityType abilityType = attacker.Action.Ability.Type;
		if (abilityType == Ability.AbilityType.ATTACK || attacker.Action.Ability.Id == AbilityConstants.MEASURED_STRIKE) {

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
		}
		else if (attacker.Action.Ability.Id == AbilityConstants.WHIRLWIND_SLASH) {
			UnitAnimationController animationController = attacker.GetAnimationController ();
			animationController.WhirlwindSlash ();
		}
		else if (attacker.Action.Ability.Id == AbilityConstants.LEAPING_SLICE) {
			UnitAnimationController animationController = attacker.GetAnimationController ();
			animationController.LeapingSlice ();
		}

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
	/// Shows the damaged color on targets.
	/// </summary>
	/// <param name="showDamagedColor">If set to <c>true</c> show damaged color.</param>
	/// <param name="targets">Targets.</param>
	private void ShowDamagedColorOnTargets(bool showDamagedColor, List<Unit> targets) {
		foreach (var target in targets)
			target.ShowDamagedColor (showDamagedColor);
	}

	/// <summary>
	/// Applies the effects.
	/// </summary>
	/// <param name="targets">Targets.</param>
	/// <param name="effects">Effects.</param>
	private IEnumerator ApplyEffects(List<Unit> targets, List<Effect> effects) {
		if (effects.Count <= 0)
			yield break;

		int index = 0;
		do {
			Effect effect = effects [index];
			bool showedEffectPopupText = false;
			foreach (var target in targets) {
				if (target != null) {
					effect.ApplyEffect (target);

					if (effect.GetDisplayString () != "") {
						showedEffectPopupText = true;
						PopupTextController.CreatePopupText (effect.GetDisplayString (), target.transform.position, effect.GetColor ());
					}

					// If unit is killed after application of effect, handle death
					target.UpdateHealthbar ();
					if (target.IsDead())
						HandleDeath(target);
				}
			}

			// If pop up text was shown, wait x seconds so it doesn't stack with other potential ones
			if (showedEffectPopupText)
				yield return new WaitForSeconds (0.75f);
			index++;
		} while (index < effects.Count);

		yield return null;
	}

	/// <summary>
	/// Applies the effects by unit.
	/// </summary>
	/// <returns>The effects by unit.</returns>
	/// <param name="effectsByUnit">Effects by unit.</param>
	private IEnumerator ApplyEffectsByUnit(Dictionary<Unit, List<Effect>> effectsByUnit) {
		if (effectsByUnit.Count <= 0)
			yield break;

		// Iterate over each unit and apply effects
		List<Coroutine> applyEffectCoroutines = new List<Coroutine> ();
		foreach (var item in effectsByUnit) {
			List<Unit> targets = new List<Unit> () { item.Key };
			applyEffectCoroutines.Add(StartCoroutine(ApplyEffects(targets, item.Value)));
		}

		// wait for all coroutines to finish before proceeding
		foreach (var item in applyEffectCoroutines)
			yield return item;
		
		yield return null;
	}
}