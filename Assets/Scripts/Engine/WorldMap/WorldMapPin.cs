using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldMapPin : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private GameObject _boardImage;
    [SerializeField] private GameObject _boardTitleImage;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private AudioSource _confirmationSource;
    [SerializeField] private string _scene = "scene";

    private static bool _isActive = false;

    void Start()
    {
        _textMeshPro.text = _name;
    }

    void OnMouseEnter()
    {
        if (!_boardImage.activeSelf && !_boardTitleImage.activeSelf && !_isActive)
            _boardTitleImage.SetActive(true);
    }

    void OnMouseExit()
    {
        if (!_boardImage.activeSelf && _boardTitleImage.activeSelf && !_isActive)
            _boardTitleImage.SetActive(false);
    }

    /// <summary>
    /// Loads the scene.
    /// </summary>
    public void LoadScene()
    {
        DeactivateUI();
        SceneManager.LoadScene(_scene);
    }

    /// <summary>
    /// Deactivates the user interface.
    /// </summary>
    public void DeactivateUI()
    {
        _boardImage.SetActive(false);
        _boardTitleImage.SetActive(false);
        _isActive = false;
        PlayConfirmationSFX();
    }

    /// <summary>
    /// Plays the confirmation sfx.
    /// </summary>
    private void PlayConfirmationSFX()
    {
        _confirmationSource.PlayOneShot(_confirmationSource.clip);
    }

    private void OnMouseDown()
    {
        if (!_boardImage.activeSelf)
        {
            _boardImage.SetActive(true);
            _isActive = true;
            PlayConfirmationSFX();
        }
    }
}