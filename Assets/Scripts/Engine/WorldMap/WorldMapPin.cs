using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField] private PlayerInput _playerInput;

    private static bool _isActive = false;

	private InputAction _moveAction;
	private InputAction _selectMapPinAction;

    private bool _buttonPressed = false;

	private void Awake() {
		_selectMapPinAction = _playerInput.actions["SelectMapPin"];
		//_selectMapPinAction.performed += OnSelect;

        _moveAction = _playerInput.actions["Move"];
        _moveAction.performed += OnMove;
	}

	private void OnDestroy() {
		//_selectMapPinAction.performed -= OnSelect;
        _moveAction.performed -= OnMove;
	}

    void Start()
    {
        _textMeshPro.text = _name;
    }

    void OnMouseEnter()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        if (!_boardImage.activeSelf && !_boardTitleImage.activeSelf && !_isActive)
            _boardTitleImage.SetActive(true);
    }

    void OnMouseExit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        if (!_boardImage.activeSelf && _boardTitleImage.activeSelf && !_isActive)
            _boardTitleImage.SetActive(false);
    }

    public void OnButtonPressed(bool pressed)
    {
        _buttonPressed = pressed;
    }

    public bool GetButtonPressed()
    {
        return _buttonPressed;
    }

    /// <summary>
    /// Loads the scene.
    /// </summary>
    public void LoadScene()
    {
        print("LoadScene");
        if (!string.IsNullOrEmpty(_scene))
        {
            DeactivateUI();
            SceneManager.LoadScene(_scene);
            //print("//SceneManager.LoadScene(_scene);");
        }
    }

    /// <summary>
    /// Deactivates the user interface.
    /// </summary>
    public void DeactivateUI()
    {
        print(string.Format("DeactivateUI | {0}", this.gameObject));
        _boardImage.SetActive(false);
        _boardTitleImage.SetActive(false);
        _isActive = false;
        PlayConfirmationSFX();

        foreach(var child in _boardImage.GetComponentsInChildren<Transform>(true))
{
            // then set them all active
            child.gameObject.SetActive(false);
        }
    }

    public bool IsActive() {
        return _isActive;
    }

    /// <summary>
    /// Plays the confirmation sfx.
    /// </summary>
    private void PlayConfirmationSFX()
    {
        _confirmationSource.PlayOneShot(_confirmationSource.clip);
    }

	public void OnSelect(InputAction.CallbackContext context)
 	{
        if (context.performed)
        {
            if (EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                print(string.Format("OnSelect({0}) | this: {1} | EventSystem.currentSelectedGameObject: {2}", context, this.gameObject, EventSystem.current.currentSelectedGameObject));
                if (!_boardImage.activeSelf)
                {
                    _boardImage.SetActive(true);
                    _isActive = true;
                    PlayConfirmationSFX();
                    foreach(var child in _boardImage.GetComponentsInChildren<Transform>(true))
                    {
                        // then set them all active
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }
	}

	private void OnMove(InputAction.CallbackContext obj)
 	{
		if (EventSystem.current.currentSelectedGameObject == this.gameObject)
		{
		}
	}
}