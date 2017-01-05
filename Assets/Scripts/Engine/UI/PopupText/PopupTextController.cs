using UnityEngine;
using System.Collections;

public class PopupTextController : MonoBehaviour {

	private static PopupText _popupText;
	private static Canvas _canvas;

	/// <summary>
	/// Initialize the popup text
	/// </summary>
	/// <param name="canvas">Canvas.</param>
	public static void Initialize(Canvas canvas) {
		_canvas = canvas;
		if (!_popupText)
			_popupText = Resources.Load<PopupText> ("Prefabs/UI/PopupText/PopupTextParent");
	}

	/// <summary>
	/// Creates the popup text.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="position">Position.</param>
	public static void CreatePopupText(string text, Vector3 position) {
		PopupText instance = Instantiate (_popupText);
		instance.transform.SetParent (_canvas.transform, false);
		instance.transform.position = position;
		instance.SetText (text);
	}
}