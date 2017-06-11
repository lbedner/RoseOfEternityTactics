using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class RadialButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public static event EventHandler<InfoEventArgs<RadialButtonType>> buttonClickEvent;

	public enum RadialButtonType {
		ATTACK,
		DEFEND,
		CANCEL,
		ITEM,
		ITEM_USE,
	}

	//private const string RESOURCE_PATH = "Prefabs/UI/RadialMenu/RadialButton";
	private const string RESOURCE_PATH = "Prefabs/UI/RadialMenu/RadialButtonPanel";

	[SerializeField] private GameObject _icon;

	public RadialButtonType Type { get; private set; }
	public string Name { get; private set; }

	private Outline _highlightOutline;
	private bool _isScalingOut = true;
	private bool _isScalingIn = false;
	private bool _isScalingUp = false;
	private bool _isScalingDown = false;

	private static RadialButtonContainer _radioButtonContainer;

	/// <summary>
	/// Instantiates the instance.
	/// </summary>
	/// <returns>The instance.</returns>
	/// <param name="radialButtonContainer">Radial button container.</param>
	public static RadialButtonController InstantiateInstance(RadialButtonContainer radialButtonContainer) {
		GameObject instance = Instantiate (Resources.Load<GameObject> (RESOURCE_PATH));
		instance.transform.SetParent(radialButtonContainer.transform, false);

		_radioButtonContainer = radialButtonContainer;

		return instance.GetComponent<RadialButtonController> ();
	}

	/// <summary>
	/// Initialize the specified iconPath, type, Name and position.
	/// </summary>
	/// <param name="iconPath">Icon path.</param>
	/// <param name="type">Type.</param>
	/// <param name="Name">Name.</param>
	/// <param name="position">Position.</param>
	public void Initialize(string iconPath, RadialButtonType type, string name, Vector3 position) {
		SetIcon (iconPath);
		Type = type;
		Name = name;
		_highlightOutline =  GetComponentInChildren<Outline> ();
		StartCoroutine (ScaleButtonOut (position));
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		if (buttonClickEvent != null) {
			_highlightOutline.enabled = false;
			transform.localScale = new Vector3 (1, 1, 1);
			_radioButtonContainer.RadialMenuController.PopupText = "";
			buttonClickEvent (this, new InfoEventArgs<RadialButtonType> (Type));
		}
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter (PointerEventData eventData) {
		if (!_isScalingOut && !_isScalingIn) {
			_highlightOutline.enabled = true;
			StartCoroutine (ScaleButtonUp ());
			_radioButtonContainer.RadialMenuController.PopupText = Name;
		}
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit (PointerEventData eventData) {
		if (!_isScalingOut && !_isScalingIn) {
			_highlightOutline.enabled = false;
			StartCoroutine (ScaleButtonDown ());
			_radioButtonContainer.RadialMenuController.PopupText = "";
		}
	}

	/// <summary>
	/// Sets the icon.
	/// </summary>
	/// <param name="iconPath">Icon path.</param>
	private void SetIcon(string iconPath) {
		Sprite iconSprite = Resources.Load<Sprite> (iconPath);
		_icon.GetComponent<Image> ().sprite = iconSprite;		
	}

	/// <summary>
	/// Scales the button out.
	/// </summary>
	/// <returns>The button out.</returns>
	/// <param name="endingPosition">Ending position.</param>
	public IEnumerator ScaleButtonOut(Vector3 endingPosition) {
		_isScalingOut = true;
		yield return StartCoroutine(ScaleButton(0.25f, Vector3.zero, endingPosition, Vector3.zero, Vector3.one));
		_isScalingOut = false;
	}

	/// <summary>
	/// Scales the button out.
	/// </summary>
	/// <returns>The button out.</returns>
	/// <param name="endingPosition">Ending position.</param>
	public IEnumerator ScaleButtonIn() {
		_isScalingIn = true;
		yield return StartCoroutine(ScaleButton(0.25f, transform.localPosition, Vector3.zero, Vector3.one, Vector3.zero));
		_isScalingIn = false;
	}

	/// <summary>
	/// Scale up the button's size.
	/// </summary>
	/// <returns>The button up.</returns>
	private IEnumerator ScaleButtonUp() {
		if (!_isScalingUp) {
			_isScalingUp = true;
			yield return StartCoroutine (ScaleButton(0.125f, transform.localPosition, transform.localPosition, transform.localScale, new Vector3 (1.2f, 1.2f, transform.localScale.z)));
			_isScalingUp = false;
		}
	}

	/// <summary>
	/// Scale down the button's size.
	/// </summary>
	/// <returns>The button up.</returns>
	private IEnumerator ScaleButtonDown() {
		if (!_isScalingDown) {
			_isScalingDown = true;
			yield return StartCoroutine (ScaleButton (0.125f, transform.localPosition, transform.localPosition, transform.localScale, new Vector3 (1, 1, 0)));
			_isScalingDown = false;
		}
	}

	/// <summary>
	/// Scales the button.
	/// </summary>
	/// <returns>The button.</returns>
	/// <param name="timeToMove">Time to move.</param>
	/// <param name="startingPosition">Starting position.</param>
	/// <param name="endingPosition">Ending position.</param>
	/// <param name="startingScale">Starting scale.</param>
	/// <param name="endingScale">Ending scale.</param>
	private IEnumerator ScaleButton(float timeToMove, Vector3 startingPosition, Vector3 endingPosition, Vector3 startingScale, Vector3 endingScale) {
		float elapsedTime = 0.0f;
		while (elapsedTime < timeToMove) {

			// Don't change position if start and end are the same
			transform.localPosition = Vector3.Lerp (startingPosition, endingPosition, (elapsedTime / timeToMove));
			transform.localScale = Vector3.Lerp (startingScale, endingScale, (elapsedTime / timeToMove));
			elapsedTime += Time.deltaTime;
			yield return null;
		}		
	}
}
