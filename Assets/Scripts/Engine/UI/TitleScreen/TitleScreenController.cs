using UnityEngine;
using UnityEngine.UI;

public class TitleScreenController : MonoBehaviour {

	/// <summary>
	/// Game state.
	/// </summary>
	private enum GameState {
		START_FADE,
		END_FADE,
		ACTIVATE_SCREEN,
		DONE,
	}

	[SerializeField] private RawImage _fadeImage;
	[SerializeField] private AudioSource _titleScreenMusic;
	[SerializeField] private float _fadeInTime = 2.0f;

	private GameState _gameState;
	private ScreenFader _screenFader;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_titleScreenMusic.Play ();
		_screenFader = new ScreenFader ();
		_gameState = GameState.START_FADE;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {

		switch (_gameState) {

		case GameState.START_FADE:
			StartCoroutine (_screenFader.FadeScreen (_fadeImage, ScreenFader.FadeType.FADE_IN, _fadeInTime));
			_gameState = GameState.END_FADE;
			break;

		case GameState.END_FADE:
			if (!_screenFader.IsFading ())
				_gameState = GameState.ACTIVATE_SCREEN;
			break;

		case GameState.ACTIVATE_SCREEN:
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			_gameState = GameState.DONE;
			break;
		}			
	}
}