using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionIndicator : MonoBehaviour {

	// Master selection indicator icon. There should always be one.
	[SerializeField] private GameObject _masterSelectionIndicatorIcon;

	private float _size;
	private float _y;

	// Extra icons used when more than a single target is required
	private List<GameObject> _selectionIndicatorIcons = new List<GameObject>();

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		SpriteRenderer spriteRenderer = _masterSelectionIndicatorIcon.GetComponent<SpriteRenderer> ();
		_size = spriteRenderer.bounds.size.x;
		_y = _masterSelectionIndicatorIcon.transform.localPosition.y;
	}		

	/// <summary>
	/// Sets the area of effect indicators.
	/// </summary>
	/// <param name="aoeRange">Aoe range.</param>
	public void SetAreaOfEffectIndicators(float aoeRange) {

		Vector3 localPosition = _masterSelectionIndicatorIcon.transform.localPosition;
		float x = localPosition.x;
		float z = localPosition.z;
		
		// Outer loop handles the straight lines going N, E, S, W
		for (int index1 = 1; index1 <= aoeRange; index1++) {

			float coordinateFactor1 = index1 * _size;

			InstantiateSelectionIndicatorIcon(x, z + coordinateFactor1);
			InstantiateSelectionIndicatorIcon(x + coordinateFactor1, z);
			InstantiateSelectionIndicatorIcon(x, z - coordinateFactor1);
			InstantiateSelectionIndicatorIcon(x - coordinateFactor1, z);

			if (aoeRange > 1) {
				// Inner loop handles all the other tiles NE, SE, NW, SW
				for (int index2 = 1; index2 <= aoeRange - index1; index2++) {

					float coordinateFactor2 = index2 * _size;

					InstantiateSelectionIndicatorIcon(x + coordinateFactor1, z + coordinateFactor2); // North East
					InstantiateSelectionIndicatorIcon(x + coordinateFactor1, z - coordinateFactor2); // South East
					InstantiateSelectionIndicatorIcon(x - coordinateFactor1, z + coordinateFactor2); // North West
					InstantiateSelectionIndicatorIcon(x - coordinateFactor1, z - coordinateFactor2); // South West
				}
			}
		}
	}

	/// <summary>
	/// Clears the indicators (except the main, master one).
	/// </summary>
	public void ClearIndicators() {
		foreach (var indicator in _selectionIndicatorIcons)
			Destroy (indicator);
		_selectionIndicatorIcons.Clear ();
	}

	/// <summary>
	/// Instantiates the selection indicator icon.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	private void InstantiateSelectionIndicatorIcon(float x, float z) {
		GameObject selectionIndicatorIcon = Instantiate (_masterSelectionIndicatorIcon);
		selectionIndicatorIcon.transform.SetParent (this.transform, false);
		selectionIndicatorIcon.transform.localPosition = new Vector3 (x, _y, z);
		_selectionIndicatorIcons.Add (selectionIndicatorIcon);
	}
}