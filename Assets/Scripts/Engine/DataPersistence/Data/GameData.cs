
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public string version = "0.01";

    public float totalPlayTimeInSeconds;
    public string currentScene;
    public List<UnitData> units = new();

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        version = "0.01";
        totalPlayTimeInSeconds = 0;
        currentScene = "ExampleScene";
    }
}