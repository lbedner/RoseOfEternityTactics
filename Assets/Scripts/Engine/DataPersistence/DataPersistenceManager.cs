using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = true;
    [SerializeField] private bool overrideSelectedProfileId = true;
    [SerializeField] private string testSelectedProfileId = "0";

    [Header("File Storage Config")]
    [SerializeField] private string fileName = "roe";
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitializeSelectedProfileId();
    }

    void Update()
    {
        if (HasGameData())
        {
            gameData.totalPlayTimeInSeconds += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (scene.name != "title_screen")
        {
            LoadGame();
        }

        // start up the auto saving coroutine
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        Debug.Log("Starting a new game...");
        gameData = new GameData();
    }

    public void LoadGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        // load any saved data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileId);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        //Debug.Log("Loading game...");
        //StartCoroutine(LoadSceneAndDataAsync());

        // if no data can be loaded, don't continue
        if (gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    private IEnumerator LoadSceneAndDataAsync()
    {
        if (gameData == null)
        {
            Debug.LogError("No data was found. A New Game needs to be started before data can be loaded.");
            yield break; // Exit if there's no game data
        }

        // Load the saved scene asynchronously

        // Load scene if not already loaded
        if (SceneManager.GetActiveScene().name != gameData.currentScene)
        {
            Debug.Log("Loading scene: " + gameData.currentScene);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameData.currentScene);

            // Wait for the scene to be fully loaded
            while (!asyncLoad.isDone)
            {
                Debug.Log("Loading scene...");
                yield return null;
            }
        }
        else
            Debug.LogWarning("Scene already loaded: " + gameData.currentScene);

        // Debug.Log("Loading scene: " + gameData.currentScene);
        // AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameData.currentScene);

        // Wait for the scene to be fully loaded
        // while (!asyncLoad.isDone)
        // {
        //     Debug.Log("Loading scene...");
        //     yield return null;
        // }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            Debug.LogWarning("Loading data for: " + dataPersistenceObj);
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        Debug.Log("Saving game...");
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
            return;
        }

        // if we don't have any data to save, log a warning here
        if (gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        // pass the data to other scripts so they can update it
        Debug.Log("Fetching data persistence objects to save from other scripts...");
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        if (dataPersistenceObjects != null && dataPersistenceObjects.Count > 0)
        {
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                Debug.Log("Saving data for: " + dataPersistenceObj);
                dataPersistenceObj.SaveData(gameData);
            }
        }
        else
        {
            Debug.LogWarning("No data persistence objects were found in the scene.");
        }

        // Save the current scene name to the game data
        gameData.currentScene = SceneManager.GetActiveScene().name;

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }
}