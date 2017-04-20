using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
	[SerializeField] private AudioSource _audioSource;

	private GameState _gameState;
	private MovieTexture _movieTexture;
	private ScreenFader _screenFader;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		Renderer renderer = GetComponent<Renderer> ();
		_movieTexture = (MovieTexture)renderer.material.mainTexture;
		_screenFader = new ScreenFader ();

		_movieTexture.Play ();
		_audioSource.Play ();

		_gameState = GameState.MOVIE_PLAYING;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {

		switch (_gameState) {

		case GameState.MOVIE_PLAYING:
			if (!_movieTexture.isPlaying)
				_gameState = GameState.START_FADE;
			break;

		case GameState.START_FADE:
			StartCoroutine (_screenFader.FadeScreen (_fadeOutUIImage, ScreenFader.FadeType.FADE_OUT, 2.0f));
			_gameState = GameState.END_FADE;
			break;

		case GameState.END_FADE:
			if (!_screenFader.IsFading ())
				_gameState = GameState.LOAD_NEXT_SCENE;
			break;

		case GameState.LOAD_NEXT_SCENE:
			SceneManager.LoadScene ("scene_0000");
			_gameState = GameState.LOADING_SCENE;
			break;
		}
	}
}