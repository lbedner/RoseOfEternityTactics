using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class NewGameButton : MonoBehaviour
{

	/// <summary>
	/// Game state.
	/// </summary>
	private enum GameState
	{
		WAITING,
		START_FADE,
		END_FADE,
		LOAD_NEXT_SCENE,
	}

	[SerializeField] private RawImage _fadeImage;
	[SerializeField] private AudioSource _sfx;

	private GameState _gameState;
	private ScreenFader _screenFader;

	private InputAction _selectAction;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		_gameState = GameState.WAITING;
		_screenFader = new ScreenFader();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{

		switch (_gameState)
		{

			case GameState.START_FADE:
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				StartCoroutine(_screenFader.FadeScreen(_fadeImage, ScreenFader.FadeType.FADE_OUT, 2.0f));
				_gameState = GameState.END_FADE;
				break;

			case GameState.END_FADE:
				if (!_screenFader.IsFading())
					_gameState = GameState.LOAD_NEXT_SCENE;
				break;

			case GameState.LOAD_NEXT_SCENE:

				SceneManager.LoadScene("ExampleScene");
				_gameState = GameState.WAITING;
				break;
		}
	}

	public void OnSelect()
	{
		if (EventSystem.current.currentSelectedGameObject == gameObject)
		{
			_sfx.PlayOneShot(_sfx.clip);
			_gameState = GameState.START_FADE;
		}
	}
}