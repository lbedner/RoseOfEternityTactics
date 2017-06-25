using UnityEngine;
using System.Collections;

public class PerformActionState : CombatState {

	public override void Enter() {
		print ("PerformActionState.Enter");
		base.Enter ();
	}

	/// <summary>
	/// Performs the action of the attacker against the defender.
	/// </summary>
	/// <returns>The action.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	protected IEnumerator PerformAction(Unit attacker, Unit defender) {

		attacker.TileHighlighter.RemoveHighlightedTiles ();
		controller.ActionController.Activate (attacker.UnitData.InventorySlots.Get (InventorySlots.SlotType.RIGHT_HAND).Name);
		yield return new WaitForSeconds (0.5f);
		Combat combat = new Combat (controller.HighlightedUnit, defender);
		combat.Begin ();
		defender.ShowDamagedColor (true);
		yield return StartCoroutine (PlayAttackAnimations (attacker, defender));
		yield return new WaitForSeconds (1.0f);
		defender.ShowDamagedColor (false);

		// Show XP floaty text
		int awardedExperience = combat.GetAwardedExperience();
		PopupTextController.Initialize(attacker.GetCanvas());
		PopupTextController.CreatePopupText (string.Format("+ {0} XP", awardedExperience), attacker.transform.position, Color.yellow);

		yield return new WaitForSeconds (1.0f);

		controller.ChangeState<TurnOverState> ();
	}

	/// <summary>
	/// Plays the attack animations.
	/// </summary>
	/// <returns>The attack animations.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	protected IEnumerator PlayAttackAnimations(Unit attacker, Unit defender) {

		//determine which way to swing, dependent on the direction the enemy is
		Unit.TileDirection facing = attacker.GetDirectionToTarget (defender);
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
}