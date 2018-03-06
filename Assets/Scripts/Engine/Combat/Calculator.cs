using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Calculator {

	public Action Action { get; private set; }

	private Unit _source;
	private List<Unit> _targets;
	private List<int> _damageToTargets;
	private Ability _ability;

	/// <summary>
	/// Initializes a new instance of the <see cref="Calculator"/> class.
	/// </summary>
	/// <param name="action">Action.</param>
	public Calculator(Unit source) {
		_source = source;
		Action = _source.Action;
		_targets = Action.Targets;
		_damageToTargets = Action.DamageToTargets;
		_ability = Action.Ability;

		CalculateDamage ();
	}

	/// <summary>
	/// Calculates the damage.
	/// </summary>
	private void CalculateDamage() {

		switch (Action.Ability.Id) {

		case AbilityConstants.ATTACK:
			CalculateAttackDamage ();
			break;

		//** TALENTS **/

		case AbilityConstants.MEASURED_STRIKE:
			CalculateAttackDamage (2.0f);
			break;

		case AbilityConstants.WHIRLWIND_SLASH:
			CalculateAttackDamage ();
			break;

		case AbilityConstants.LEAPING_SLICE:
			CalculateAttackDamage ();
			break;

		//** MAGIC **//

		case AbilityConstants.FLAME:
			CalculateFlameDamage ();
			break;

		case AbilityConstants.FREEZE:
			CalculateFreezeDamage ();
			break;

		case AbilityConstants.LIGHTNING:
			CalculateLightningDamage ();
			break;
		}
	}

	/// <summary>
	/// Calculates the attack damage.
	/// </summary>
	private void CalculateAttackDamage(float modifier = 1.0f) {
		foreach (var target in _targets) {
			Item weapon = _source.GetItemInSlot (InventorySlots.SlotType.RIGHT_HAND);
			int damageAttribute = (int)weapon.GetAttribute (AttributeEnums.AttributeType.DAMAGE).CurrentValue;
			int damage = (int)(_source.GetLevelAttribute ().CurrentValue * damageAttribute * modifier);
			_damageToTargets.Add (damage);

			List<Effect> effects = new List<Effect> ();
			effects.Add(new DamageEffect(value: damage, color: Color.red, vfxPath: _ability.VFXPath));

			Action.AddEffectsByUnit (target, effects);

		}
		Action.DamageToTargets = _damageToTargets;
	}

	/// <summary>
	/// Gets the calculated magic and level based damage.
	/// </summary>
	/// <param name="levelDivisor">Level divisor.</param>
	private void GetCalculatedMagicAndLevelBasedDamage(int levelDivisor) {
		foreach (var target in _targets) {
			float magic = _source.GetAttribute (AttributeEnums.AttributeType.MAGIC).CurrentValue;
			float level = _source.GetLevelAttribute ().CurrentValue;
			var damage = (magic * (level / levelDivisor));

			// Distribute the damage of the spell if multiple enemies
			if (_targets.Count > 1)
				damage /= (_targets.Count * 0.90f);

			int finalDamage = Mathf.CeilToInt(damage);
			_damageToTargets.Add(finalDamage);

			// Store reference of damage to the particular unit
			Action.AddDamageByUnit (target, finalDamage);
		}
		Action.DamageToTargets = _damageToTargets;
	}

	/// <summary>
	/// Calculates the flame damage.
	/// </summary>
	private void CalculateFlameDamage() {
		GetCalculatedMagicAndLevelBasedDamage (2);

		// Iterate over all targets and add effects
		foreach (var item in Action.GetDamageByUnit()) {

			int damage = item.Value;

			// Create effects
			int turns = 3;
			Color color = new Color32 (254, 161, 0, 1);
			List<Effect> effects = new List<Effect> ();
			effects.Add (new DamageEffect (value: damage, turns: turns, incrementalValue: Mathf.CeilToInt(damage / turns), color: color, incomingEffectType: EffectType.OVER_TIME, vfxPath: _ability.VFXPath));

			// Add to effects collection
			Action.AddEffectsByUnit(item.Key, effects);
		}
	}

	/// <summary>
	/// Calculates the freeze damage.
	/// </summary>
	private void CalculateFreezeDamage() {
		GetCalculatedMagicAndLevelBasedDamage (4);

		int turns = 2;

		// Iterate over all targets and add effects
		foreach (var item in Action.GetDamageByUnit()) {

			List<Effect> effects = new List<Effect> ();
			effects.Add (new DamageEffect (value: 5, color: Color.blue));
			effects.Add (new SpeedEffect (-5, turns: turns, incrementalValue: 0, color: Color.blue, incomingEffectType: EffectType.TEMPORARY, vfxPath: _ability.VFXPath));
			effects.Add (new AnimationSpeedEffect (0.5f, turns));
			effects.Add (new UnitColorEffect (Color.blue, turns));

			// Add to effects collection
			Action.AddEffectsByUnit(item.Key, effects);
		}
	}

	/// <summary>
	/// Calculates the lightning damage.
	/// </summary>
	private void CalculateLightningDamage() {
		GetCalculatedMagicAndLevelBasedDamage (3);

		// Iterate over all targets and add effects
		foreach (var item in Action.GetDamageByUnit()) {

			int damage = item.Value;
			Color color = new Color32 (143, 0, 254, 1);

			// Create effects
			List<Effect> effects = new List<Effect> ();
			effects.Add (new DamageEffect (value: damage, color: color, vfxPath: _ability.VFXPath));

			// Add to effects collection
			Action.AddEffectsByUnit(item.Key, effects);
		}
	}
}