using UnityEngine;

public class MenuSelectionState : CombatState {

	private RadialMenuController _radialMenuController;

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		print ("MenuSelectionState.Enter");
		base.Enter ();
		Init ();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners ()	{
		controller.AttackButton.onClick.AddListener (OnAttackButtonClicked);
		controller.EndTurnButton.onClick.AddListener (OnEndTurnButtonClicked);
		RadialButtonController.buttonClickEvent += OnButtonClicked;
		InputController.keyDownEscapeEvent += OnKeyDownEscape;
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners () {
		controller.AttackButton.onClick.RemoveListener (OnAttackButtonClicked);
		controller.EndTurnButton.onClick.RemoveListener (OnEndTurnButtonClicked);
		RadialButtonController.buttonClickEvent -= OnButtonClicked;
		InputController.keyDownEscapeEvent -= OnKeyDownEscape;
	}

	/// <summary>
	/// Raises the button clicked event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnButtonClicked(object sender, InfoEventArgs<RadialButtonController.RadialButtonType> e) {

		// Play sound
		controller.ConfirmationSource.PlayOneShot (controller.ConfirmationSource.clip);

		// Handle all the various types of button clicks
		switch (e.info) {

		case RadialButtonController.RadialButtonType.ATTACK:
			OnAttackButtonClicked();
			break;

		case RadialButtonController.RadialButtonType.ITEM:
			OnItemButtonClicked ();
			break;

		case RadialButtonController.RadialButtonType.DEFEND:
			OnEndTurnButtonClicked();
			break;

		case RadialButtonController.RadialButtonType.CANCEL:
			HandleCancelEvent ();
			break;
		}
	}

	/// <summary>
	/// Raises the attack button clicked event.
	/// </summary>
	private void OnAttackButtonClicked() {
		_radialMenuController.gameObject.SetActive (false);
		controller.ChangeState<PlayerTargetSelectionState> ();
	}

	/// <summary>
	/// Raises the item button clicked event.
	/// </summary>
	private void OnItemButtonClicked() {
		_radialMenuController.InstantiateItemButtons ();
	}

	/// <summary>
	/// Raises the end turn button clicked event.
	/// </summary>
	private void OnEndTurnButtonClicked() {
		controller.ChangeState<TurnOverState> ();
	}

	/// <summary>
	/// Handles the cancel event.
	/// </summary>
	private void HandleCancelEvent() {
		if (!_radialMenuController.IsRootMenuOpen)
			_radialMenuController.DrillUp ();
		else 
			OnCancelButtonClicked ();
	}

	/// <summary>
	/// Raises the key down escape event.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	private void OnKeyDownEscape(object sender, InfoEventArgs<KeyCode> e) {
		HandleCancelEvent ();
	}

	/// <summary>
	/// Raises the cancel button clicked event.
	/// </summary>
	private void OnCancelButtonClicked() {

		// Get active unit when cancel was invoked
		Unit unit = controller.TurnOrderController.GetNextUp ();

		// Set back to selected color
		unit.Highlight();

		// Swap unit back to old tile
		TileMapData tileMapData = controller.TileMap.GetTileMapData();
		TileData oldTileData = tileMapData.GetTileDataAt (controller.OldUnitPosition);
		TileData currentTileData = tileMapData.GetTileDataAt (controller.CurrentUnitPosition);
		currentTileData.SwapUnits (oldTileData);

		// Update tile position on Unit
		unit.Tile = controller.OldUnitPosition;

		// Swap actual world centered position to old position
		unit.transform.position = TileMapUtil.TileMapToWorldCentered (controller.OldUnitPosition, controller.TileMap.TileSize);

		// Revert to old walking animation
		unit.FacedDirection = controller.OldUnitTileDirection;
		unit.GetAnimationController ().PlayWalkingAnimation (unit);

		// Swap cached position on controller
		controller.CurrentUnitPosition = controller.OldUnitPosition;

		// Highlight unit
		controller.HighlightCharacter (unit);

		// Highlight tiles around unit
		unit.TileHighlighter.HighlightTiles (unit, unit.Tile);

		// Clear out radial button container list
		controller.RadialButtonContainers.Clear();

		// Destroy radial menu
		Destroy (_radialMenuController.gameObject);

		// Revert back to prior state
		controller.ChangeState<PlayerSelectedState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		controller.ShowCursor (true);
		controller.ShowTileSelector (false);
		_radialMenuController = RadialMenuController.InstantiateInstance (controller.HighlightedUnit.GetCanvas ());
		_radialMenuController.transform.localPosition = new Vector3 (0.0f, 22.0f, 0.0f);
	}
}