using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		
		_screenFader = new ScreenFader ();
		_fadeOutUIImage.enabled = false;
		_videoPlayer.loopPointReached += EndReached;
		_videoPlayer.Play ();

		_gameState = GameState.MOVIE_PLAYING;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {

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

	void EndReached(VideoPlayer vp)
    {
        _gameState = GameState.START_FADE;
    }
}