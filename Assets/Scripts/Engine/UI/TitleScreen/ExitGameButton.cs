using UnityEngine;
using UnityEngine.EventSystems;

public class ExitGameButton : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// Exits the game when the button is clicked.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit ();
		#endif
	}
}