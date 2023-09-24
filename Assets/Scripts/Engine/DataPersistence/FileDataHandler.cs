using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private const string BackupExtension = ".bak";
    private readonly string dataDirPath;
    private readonly string dataFileName;
    private readonly bool useEncryption;
    private readonly string encryptionCodeWord = "word";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileId, bool allowRestoreFromBackup = true)
    {
        if (profileId == null) return null;

        string fullPath = GetFullPath(profileId);
        if (!File.Exists(fullPath)) return null;

        try
        {
            string dataToLoad = ReadFile(fullPath);
            if (useEncryption) dataToLoad = EncryptDecrypt(dataToLoad);
            return JsonUtility.FromJson<GameData>(dataToLoad);
        }
        catch (Exception e)
        {
            return HandleLoadException(e, fullPath, profileId, allowRestoreFromBackup);
        }
    }

    private string ReadFile(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private GameData HandleLoadException(Exception e, string fullPath, string profileId, bool allowRestoreFromBackup)
    {
        Debug.LogWarning($"Failed to load data file. {e}");
        if (allowRestoreFromBackup && AttemptRollback(fullPath))
        {
            return Load(profileId, false);
        }
        Debug.LogError($"Error occurred when trying to load file at path: {fullPath}. {e}");
        return null;
    }

    public void Save(GameData data, string profileId)
    {
        Debug.Log($"Saving data for profile: {profileId}");
        if (profileId == null) return;

        string fullPath = GetFullPath(profileId);
        try
        {
            CreateDirectory(fullPath);
            string dataToStore = PrepareDataForStorage(data);
            WriteFile(fullPath, dataToStore);
            BackupFile(fullPath, profileId);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to save data to file: {fullPath}. {e}");
        }
    }

    public void Delete(string profileId)
    {
        // base case - if the profileId is null, return right away
        if (profileId == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            // ensure the data file exists at this path before deleting the directory
            if (File.Exists(fullPath))
            {
                // delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete profile data for profileId: "
                + profileId + " at path: " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        // loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            // defensive programming - check if the data file exists
            // if it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }

            // load the game data for this profile and put it in the dictionary
            GameData profileData = Load(profileId);
            // defensive programming - ensure the profile data isn't null,
            // because if it is then something went wrong and we should let ourselves know
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            // skip this entry if the gamedata is null
            if (gameData == null)
            {
                continue;
            }

            // if this is the first data we've come across that exists, it's the most recent so far
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            // otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                // the greatest DateTime value is the most recent
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }

    private void CreateDirectory(string fullPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
    }

    private string PrepareDataForStorage(GameData data)
    {
        string dataToStore = JsonUtility.ToJson(data, true);
        if (useEncryption) dataToStore = EncryptDecrypt(dataToStore);
        return dataToStore;
    }

    private void WriteFile(string path, string data)
    {
        Debug.LogWarning($"Writing data to file: {path}");
        using var stream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(stream);
        writer.Write(data);
    }

    private void BackupFile(string fullPath, string profileId)
    {
        GameData verifiedGameData = Load(profileId);
        if (verifiedGameData == null) throw new Exception("Save file could not be verified and backup could not be created.");
        File.Copy(fullPath, fullPath + BackupExtension, true);
    }

    private string GetFullPath(string profileId)
    {
        return Path.Combine(dataDirPath, profileId, dataFileName);
    }

    private string EncryptDecrypt(string data)
    {
        char[] buffer = new char[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            buffer[i] = (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return new string(buffer);
    }

    private bool AttemptRollback(string fullPath)
    {
        try
        {
            string backupFilePath = fullPath + BackupExtension;
            if (!File.Exists(backupFilePath)) throw new Exception("No backup file exists to roll back to.");
            File.Copy(backupFilePath, fullPath, true);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to roll back to backup file. {e}");
            return false;
        }
    }
}
