using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Head2HeadRootPanelController : MonoBehaviour {
	[SerializeField] private Head2HeadPanelController _sourceHead2HeadPanelPrefab;
	[SerializeField] private Head2HeadPanelController _targetHead2HeadPanelPrefab;

	[SerializeField] private GameObject _sourcePanel;
	[SerializeField] private GameObject _targetPanel;

	private List<GameObject> _sourcePanels = new List<GameObject>();
	private List<GameObject> _targetPanels = new List<GameObject>();

	/// <summary>
	/// Instantiates the source head2 head panel.
	/// </summary>
	/// <param name="sources">Sources.</param>
	public void InstantiateSourceHead2HeadPanel(List<Unit> sources) {
		InstantiateHead2HeadPanel (_sourceHead2HeadPanelPrefab, sources, Head2HeadPanelController.Head2HeadState.ATTACKING, _sourcePanel);
	}

	/// <summary>
	/// Instantiates the target head2 head panel.
	/// </summary>
	/// <param name="targets">Targets.</param>
	public void InstantiateTargetHead2HeadPanel(List<Unit> targets) {
		InstantiateHead2HeadPanel (_targetHead2HeadPanelPrefab, targets, Head2HeadPanelController.Head2HeadState.DEFENDING, _targetPanel);
	}

	/// <summary>
	/// Clears the panels.
	/// </summary>
	public void ClearPanels() {
		foreach (var panel in _sourcePanels)
			Destroy(panel);
		_sourcePanels.Clear();

		foreach (var panel in _targetPanels)
			Destroy(panel);
		_targetPanels.Clear();
	}

	/// <summary>
	/// Instantiates the head2 head panel.
	/// </summary>
	/// <param name="prefab">Prefab.</param>
	/// <param name="units">Units.</param>
	/// <param name="head2HeadState">Head2 head state.</param>
	private void InstantiateHead2HeadPanel(Head2HeadPanelController prefab, List<Unit> units, Head2HeadPanelController.Head2HeadState head2HeadState, GameObject parentPanel) {

		// Iterate through all units and instantiate UI panels
		foreach (var unit in units) {

			// Instantiate and attach to parent panel
			GameObject head2HeadPanel = Instantiate (prefab.gameObject);
			head2HeadPanel.transform.SetParent (parentPanel.transform, false);

			// Cache the UI panels that are created so they can be destroyed later
			if (head2HeadState == Head2HeadPanelController.Head2HeadState.ATTACKING)
				_sourcePanels.Add (head2HeadPanel);
			else if (head2HeadState == Head2HeadPanelController.Head2HeadState.DEFENDING)
				_targetPanels.Add (head2HeadPanel);

			// Load UI with unit data
			Head2HeadPanelController head2HeadPanelController = head2HeadPanel.transform.GetComponent<Head2HeadPanelController> ();
			head2HeadPanelController.Load (unit, head2HeadState);
		}

		// Position all UI panels dependent on how many there are
		if (units.Count > 1) {

			GridLayoutGroup gridLayoutGroup = parentPanel.GetComponent<GridLayoutGroup>();
			float head2HeadPanelHeight = gridLayoutGroup.cellSize.y;
			float newPanelYPosition = 0.0f;
			
			// Odd number of targets
			if (units.Count % 2 != 0) {
				int middleIndex = units.Count / 2;
				newPanelYPosition = head2HeadPanelHeight * middleIndex;
			}
			// Even number of targets
			else if (units.Count % 2 == 0) {
				newPanelYPosition = (head2HeadPanelHeight / 2.0f) * (units.Count - 1);
			}

			RectTransform rectTransform = (RectTransform)parentPanel.transform;
			Vector3 oldPosition = rectTransform.localPosition;
			rectTransform.localPosition = new Vector3 (oldPosition.x, newPanelYPosition, oldPosition.z);
		}
	}
}