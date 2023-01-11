public class DisplayMissionObjectivesState : CombatState {

	/// <summary>
	/// Adds listeners when the state is entered.
	/// </summary>
	public override void Enter() {
		base.Enter ();
		Init ();
	}

	private void OnSelect(object sender, InfoEventArgs<int> e)
	{
		OnContinueButtonClicked();
	}

	/// <summary>
	/// Adds the listeners.
	/// </summary>
	protected override void AddListeners () {
		controller.MissionObjectivesPanelContinueButton.onClick.AddListener (OnContinueButtonClicked);
		InputController.selectEvent += OnSelect;
	}

	/// <summary>
	/// Removes the listeners.
	/// </summary>
	protected override void RemoveListeners() {
		controller.MissionObjectivesPanelContinueButton.onClick.RemoveListener (OnContinueButtonClicked);
		InputController.selectEvent -= OnSelect;
	}

	/// <summary>
	/// Raises the continue button clicked event.
	/// </summary>
	private void OnContinueButtonClicked() {
		print(string.Format("{0}.OnContinueButtonClicked() - [{1}]", this, controller));
		controller.ChangeState<InitTurnState> ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	private void Init() {
		print(string.Format("MissionObjective.Init() - {0}", controller));
		controller.MissionObjectivesPanel.SetActive (true);
		controller.ShowCursor (true);
	}
}