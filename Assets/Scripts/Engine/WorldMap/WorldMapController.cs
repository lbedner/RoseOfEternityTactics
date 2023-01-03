using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WorldMapController : MonoBehaviour
{
    /// <summary>
    /// Game state.
    /// </summary>
    private enum GameState
    {
        START_FADE,
        END_FADE,
        START_TEXT_FADE,
        END_TEXT_FADE,
        ACTIVATE_SCREEN,
        DONE,
    }

    [SerializeField] private TextMeshProUGUI _worldMapText;
    [SerializeField] private RawImage _fadeImage;
    [SerializeField] private List<AudioSource> _audioSources;

    private GameState _gameState;
    private ScreenFader _screenFader;
    private bool _isFading = false;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var audioSource in _audioSources)
            audioSource.Play();
        //_fadeImage.gameObject.SetActive(true);
        //_screenFader = new ScreenFader();
        _gameState = GameState.ACTIVATE_SCREEN;
    }

    // Update is called once per frame
    void Update()
    { 
        switch (_gameState)
        {
            case GameState.START_FADE:
                StartCoroutine(_screenFader.FadeScreen(_fadeImage, ScreenFader.FadeType.FADE_IN, 5.0f));
                _gameState = GameState.END_FADE;
                break;

            case GameState.END_FADE:
                if (!_screenFader.IsFading())
                    _gameState = GameState.START_TEXT_FADE;
                break;

            case GameState.START_TEXT_FADE:
                StartCoroutine(FadeTitle(_worldMapText, ScreenFader.FadeType.FADE_IN, 2.0f));
                _gameState = GameState.END_TEXT_FADE;
                break;

            case GameState.END_TEXT_FADE:
                if (!_isFading)
                    _gameState = GameState.ACTIVATE_SCREEN;
                break;
            
            case GameState.ACTIVATE_SCREEN:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _gameState = GameState.DONE;
                break;
        }
    }

    /// <summary>
    /// Fades the screen.
    /// </summary>
    /// <returns>The screen.</returns>
    /// <param name="fadeImage">Fade image.</param>
    /// <param name="fadeType">Fade type.</param>
    /// <param name="duration">Duration.</param>
    public IEnumerator FadeTitle(TextMeshProUGUI fadeImage, ScreenFader.FadeType fadeType, float duration)
    {

        _isFading = true;

        float currentAlpha = fadeImage.color.a;
        int endAlpha;

        switch (fadeType)
        {

            // Fade In
            case ScreenFader.FadeType.FADE_IN:
                currentAlpha = 1;
                endAlpha = 0;
                while (currentAlpha >= endAlpha)
                {
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
                    currentAlpha += Time.deltaTime * (1.0f / duration) * -1;
                    yield return null;
                }
                break;

            // Fade Out
            case ScreenFader.FadeType.FADE_OUT:
                currentAlpha = 0;
                endAlpha = 1;
                while (currentAlpha <= endAlpha)
                {
                    fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
                    currentAlpha += Time.deltaTime * (1.0f / duration) * 1;
                    yield return null;
                }
                break;
        }

        _isFading = false;
    }
}