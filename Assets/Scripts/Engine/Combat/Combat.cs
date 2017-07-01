using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Combat {

	private Unit _attacker;
	private List<Unit> _defenders = new List<Unit>();

	private int _awardedExperience;

	/// <summary>
	/// Initializes a new instance of the <see cref="Combat"/> class.
	/// </summary>
	/// <param name="attacker">Attacker.</param>
	public Combat(Unit attacker) {
		_attacker = attacker;
		_defenders = attacker.Action.Targets;
	}

	/// <summary>
	/// Begin the combat action(s).
	/// </summary>
	public void Begin() {

		ExperienceManager manager = new ExperienceManager ();
		_awardedExperience = 0;

		// iterate over all defenders
		foreach (var defender in _defenders) {
			int damage = GetDamage ();

			// Damage defender
			defender.GetHitPointsAttribute ().Decrement (damage);

			// Show pop up text for damage
			PopupTextController.Initialize (defender.GetCanvas ());
			PopupTextController.CreatePopupText (damage.ToString (), defender.transform.position, Color.red);

			// If dead, then destroy
			if (defender.GetHitPointsAttribute ().CurrentValue <= 0) {

				// Show death animation
				GameObject deathAnimation = Resources.Load<GameObject> ("Prefabs/Characters/Animations/Death/DeathParent");
				GameObject instance = GameObject.Instantiate (deathAnimation);
				Vector3 defenderPosition = defender.transform.position;
				instance.transform.position = new Vector3 (defenderPosition.x, 0.1f, defenderPosition.z);
				GameObject.Destroy (instance, 2.0f);

				// Remove all highlighted tiles
				defender.TileHighlighter.RemovePersistentHighlightedTiles ();

				GameManager.Instance.GetTurnOrderController ().RemoveUnit (defender);
				GameManager.Instance.GetTileMap ().GetTileMapData ().GetTileDataAt (defender.Tile).Unit = null;
				GameManager.Instance.GetTileMap ().GetEnemies ().Remove (defender);
				GameObject.Destroy (defender.gameObject, 1.0f);
			} else
				defender.UpdateHealthbar ();

			// Give out XP
			_awardedExperience += manager.AwardCombatExperience (_attacker, defender);
		}			
	}

	/// <summary>
	/// Gets the awarded experience.
	/// </summary>
	/// <returns>The awarded experience.</returns>
	public int GetAwardedExperience() {
		return _awardedExperience;
	}

	/// <summary>
	/// Gets the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	private int GetDamage() {
		Calculator calculator = new Calculator (_attacker);
		return calculator.Action.DamageToTargets [0];
	}
}