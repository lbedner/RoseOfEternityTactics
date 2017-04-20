using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class NewGameButton : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// Game state.
	/// </summary>
	private enum GameState {
		WAITING,
		START_FADE,
		END_FADE,
		LOAD_NEXT_SCENE,
	}
		
	[SerializeField] private RawImage _fadeImage;

	private GameState _gameState;
	private ScreenFader _screenFader;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		_gameState = GameState.WAITING;
		_screenFader = new ScreenFader ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {

		switch (_gameState) {

		case GameState.START_FADE:
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			StartCoroutine (_screenFader.FadeScreen (_fadeImage, ScreenFader.FadeType.FADE_OUT, 2.0f));
			_gameState = GameState.END_FADE;
			break;

		case GameState.END_FADE:
			if (!_screenFader.IsFading ())
				_gameState = GameState.LOAD_NEXT_SCENE;
			break;

		case GameState.LOAD_NEXT_SCENE:
			SceneManager.LoadScene ("scene");
			_gameState = GameState.WAITING;
			break;
		}			
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{
		_gameState = GameState.START_FADE;
	}
}