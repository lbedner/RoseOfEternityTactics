using UnityEngine;
using System.Collections;

public class UnitAnimationController : MonoBehaviour {

	// Animator states/parameters
	private const string WALK_HORIZONTAL = "Walk_Horizontal";
	private const string WALK_NORTH = "Walk_North";
	private const string WALK_SOUTH = "Walk_South";
	private const string ATTACK_HORIZONTAL = "Attack_Horizontal";
	private const string ATTACK_NORTH = "Attack_North";
	private const string ATTACK_SOUTH = "Attack_South";
	private const string WHIRLWIND_SLASH = "Whirlwind_Slash";
	private const string LEAPING_SLICE = "Leaping_Slice";

	private const float TRANSITION_DURATION = 0.0f;

	// Generic animator for units
	private Animator _animator;

	private bool _facingRight = true;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		InitializeAnimator ();
	}

	/// <summary>
	/// Start walking north.
	/// </summary>
	public void WalkNorth() {
		_animator.CrossFade (WALK_NORTH, TRANSITION_DURATION);
	}

	/// <summary>
	/// Start walking east.
	/// </summary>
	public void WalkEast() {
		DetermineProperFacing (Unit.TileDirection.EAST);
		_animator.CrossFade(WALK_HORIZONTAL, TRANSITION_DURATION);		
	}

	/// <summary>
	/// Start walking south.
	/// </summary>
	public void WalkSouth() {
		_animator.CrossFade (WALK_SOUTH, TRANSITION_DURATION);
	}

	/// <summary>
	/// Start walking west.
	/// </summary>
	public void WalkWest() {
		DetermineProperFacing (Unit.TileDirection.WEST);
		_animator.CrossFade (WALK_HORIZONTAL, TRANSITION_DURATION);
	}

	/// <summary>
	/// Attacks the north.
	/// </summary>
	public void AttackNorth() {
		_animator.SetTrigger (ATTACK_NORTH);
	}

	/// <summary>
	/// Attacks the east.
	/// </summary>
	public void AttackEast() {
		DetermineProperFacing (Unit.TileDirection.EAST);
		_animator.SetTrigger (ATTACK_HORIZONTAL);
	}

	/// <summary>
	/// Attacks the south.
	/// </summary>
	public void AttackSouth() {
		_animator.SetTrigger (ATTACK_SOUTH);
	}

	/// <summary>
	/// Attacks the west.
	/// </summary>
	public void AttackWest() {
		DetermineProperFacing (Unit.TileDirection.WEST);
		_animator.SetTrigger (ATTACK_HORIZONTAL);
	}

	/// <summary>
	/// Performs whirlwind slash animation.
	/// </summary>
	public void WhirlwindSlash() {
		_animator.SetTrigger (WHIRLWIND_SLASH);
	}

	/// <summary>
	/// Performs leaping slice animation.
	/// </summary>
	public void LeapingSlice() {
		_animator.SetTrigger (LEAPING_SLICE);
	}

	/// <summary>
	/// Plays the walking animation.
	/// </summary>
	/// <param name="unit">Unit.</param>
	public void PlayWalkingAnimation(Unit unit) {

		Unit.TileDirection tileDirection = unit.FacedDirection;
		switch (tileDirection) {
		case Unit.TileDirection.NORTH:
			unit.GetAnimationController ().WalkNorth ();
			break;
		case Unit.TileDirection.EAST:
			unit.GetAnimationController ().WalkEast ();
			break;
		case Unit.TileDirection.WEST:
			unit.GetAnimationController ().WalkWest ();
			break;
		case Unit.TileDirection.SOUTH:
			unit.GetAnimationController ().WalkSouth ();
			break;
		}
	}

	/// <summary>
	/// Initializes the animator.
	/// </summary>
	private void InitializeAnimator() {
		_animator = GetComponent<Animator> ();

		_animator.SetBool (WALK_HORIZONTAL, false);
		_animator.SetBool (WALK_NORTH, false);
		_animator.SetBool (WALK_SOUTH, false);
	}

	/// <summary>
	/// Determines the proper facing.
	/// </summary>
	/// <param name="tileDirection">Tile direction.</param>
	private void DetermineProperFacing(Unit.TileDirection tileDirection) {
		if (tileDirection == Unit.TileDirection.EAST && !_facingRight) {
			Flip ();
			_facingRight = true;
		} else if (tileDirection == Unit.TileDirection.WEST && _facingRight) {
			Flip ();
			_facingRight = false;
		}
	}
		
	/// <summary>
	/// Flip the sprite from left to right, or right to left.
	/// </summary>
	private void Flip(){
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}