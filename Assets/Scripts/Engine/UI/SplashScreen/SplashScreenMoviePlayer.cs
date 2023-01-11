using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(PlayerInput))]
public class SplashScreenMoviePlayer : MonoBehaviour {

	/// <summary>
	/// Game state.
	/// </summary>
	private enum GameState {
		MOVIE_PLAYING,
		START_FADE,
		END_FADE,
		LOAD_NEXT_SCENE,
		LOADING_SCENE,
	}


	[SerializeField] private RawImage _fadeOutUIImage;
	[SerializeField] private VideoPlayer _videoPlayer;

	private GameState _gameState;
	private ScreenFader _screenFader;

	private InputAction _clickAction;
	
	private void Awake() {
		PlayerInput playerInput = GetComponent<PlayerInput>();
		_clickAction = playerInput.actions["Click"];
		_clickAction.performed += OnClick;
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () {
		
		_screenFader = new ScreenFader ();
		_fadeOutUIImage.enabled = false;
		_videoPlayer.loopPointReached += EndReached;
		_videoPlayer.Play ();

		_gameState = GameState.MOVIE_PLAYING;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update() {

		switch (_gameState) {

		case GameState.START_FADE:
			StartCoroutine (_screenFader.FadeScreen (_fadeOutUIImage, ScreenFader.FadeType.FADE_OUT, 2.0f));
			_gameState = GameState.END_FADE;
			break;

		case GameState.END_FADE:
			if (!_screenFader.IsFading ())
				_gameState = GameState.LOAD_NEXT_SCENE;
			break;

		case GameState.LOAD_NEXT_SCENE:
			SceneManager.LoadScene ("title_screen");
			_gameState = GameState.LOADING_SCENE;
			break;
		}
	}

	private void EndReached(VideoPlayer vp) => _gameState = GameState.START_FADE;

	private void OnClick(InputAction.CallbackContext obj) => _gameState = GameState.LOAD_NEXT_SCENE;
}