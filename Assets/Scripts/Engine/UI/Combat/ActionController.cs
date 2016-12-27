using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionController : MonoBehaviour {

	public Text actionText;

	/// <summary>
	/// Activate the action UI with the specified text.
	/// </summary>
	/// <param name="newActionText">New action text.</param>
	public void Activate(string newActionText) {
		actionText.text = newActionText;
		this.gameObject.SetActive (true);
	}

	/// <summary>
	/// Deactivate the action UI and clears out the text.
	/// </summary>
	public void Deactivate() {
		this.gameObject.SetActive (false);
		actionText.text = "";
	}
}