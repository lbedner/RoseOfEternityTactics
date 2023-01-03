using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ExitGameButton : MonoBehaviour
{
	[SerializeField] private AudioSource _sfx;
	private InputAction _selectAction;

	public void OnSelect()
 	{
		if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
			_sfx.PlayOneShot(_sfx.clip);
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit ();
			#endif
		}
	}
}