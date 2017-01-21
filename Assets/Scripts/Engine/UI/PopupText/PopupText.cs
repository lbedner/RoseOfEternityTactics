using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupText : MonoBehaviour {

	public Animator animator;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {

		// Destroy popup text after the animation has run its course
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo (0);
		Destroy (gameObject, clipInfo [0].clip.length);
	}

	/// <summary>
	/// Sets the text of the popup.
	/// </summary>
	/// <param name="popupText">Popup text.</param>
	public void SetText(string text, Color color) {

		// Set the text for the popup
		Text popupText = animator.GetComponent<Text> ();
		popupText.text = text;

		// Set color
		popupText.color = color;

		// We have to set the Z coordinate to 0 so the text doesn't get obscured by anything else
		RectTransform rectTransform = (RectTransform)transform;
		Vector3 rectLocalPosition = rectTransform.localPosition;
		rectTransform.localPosition = new Vector3 (rectLocalPosition.x, rectLocalPosition.y, 0);
	}
}