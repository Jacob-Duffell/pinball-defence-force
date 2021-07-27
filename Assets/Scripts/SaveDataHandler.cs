using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveDataHandler
{
    /// <summary>
    /// Deletes all of the user's saved data.
    /// </summary>
    public static void DeleteSaveData()
	{
        string _savedataPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "savedata.gd";

        if (File.Exists(_savedataPath))
		{
            File.Delete(_savedataPath);
		}
    }

    /// <summary>
    /// Loads the user's saved data. If there is no saved data to be loaded, a save data file is created.
    /// </summary>
    public static void Load()
    {
        SaveData _saveData = new SaveData();
        string _savedataPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "savedata.gd";

        if (File.Exists(_savedataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(_savedataPath, FileMode.Open);
            _saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            GameHandler.CurrentLives = _saveData.CurrentLives;
            GameHandler.CurrentPremiumMoney = _saveData.CurrentPremiumMoney;
            GameHandler.DamageMultiplier = _saveData.DamageMultiplier;
            GameHandler.FacebookConnected = _saveData.FacebookConnected;
            GameHandler.MaxLives = _saveData.MaxLives;
            GameHandler.MaxPinballs = _saveData.MaxPinballs;
            GameHandler.PointsMultiplier = _saveData.PointsMultiplier;
            GameHandler.VolumeMusic = _saveData.VolumeMusic;
            GameHandler.VolumeSFX = _saveData.VolumeSFX;
        }
        else
		{
            GameHandler.CurrentPremiumMoney = 100;
            GameHandler.DamageMultiplier = 1.0f;
            GameHandler.FacebookConnected = false;
            GameHandler.MaxLives = 3;
            GameHandler.MaxPinballs = 3;
            GameHandler.PointsMultiplier = 1.0f;
            GameHandler.CurrentLives = GameHandler.MaxLives;
            GameHandler.VolumeMusic = 1.0f;
            GameHandler.VolumeSFX = 1.0f;
		}

        GameHandler.CurrentMoney = _saveData.CurrentMoney;
        
        GameHandler.HighestLevel = _saveData.HighestLevel;
        GameHandler.HighScore = _saveData.HighScore;
        GameHandler.SavedTime = _saveData.SavedTime;
        GameHandler.SavedTimeRemaining = _saveData.SavedTimeRemaining;
        GameHandler.TotalDeaths = _saveData.TotalDeaths;
        GameHandler.TotalKills = _saveData.TotalKills;
        GameHandler.TotalMoney = _saveData.TotalMoney;

        GameHandler.DamageMultiplierUpgradeLevel = _saveData.DamageMultiplierUpgradeLevel;
        GameHandler.LifeUpgradeLevel = _saveData.LifeUpgradeLevel;
        GameHandler.PinballMultiplierUpgradeLevel = _saveData.LifeUpgradeLevel;
        GameHandler.PointsMultiplierUpgradeLevel = _saveData.PointsMultiplierUpgradeLevel;

        Debug.Log("Game Loaded!");
    }

    /// <summary>
    /// Saves the user's data.
    /// </summary>
    public static void Save()
    {
        SaveData saveData = new SaveData
        {
            CurrentLives = GameHandler.CurrentLives,
            CurrentMoney = GameHandler.CurrentMoney,
            CurrentPremiumMoney = GameHandler.CurrentPremiumMoney,
            DamageMultiplier = GameHandler.DamageMultiplier,
            FacebookConnected = GameHandler.FacebookConnected,
            HighestLevel = GameHandler.HighestLevel,
            HighScore = GameHandler.HighScore,
            MaxLives = GameHandler.MaxLives,
            MaxPinballs = GameHandler.MaxPinballs,
            PointsMultiplier = GameHandler.PointsMultiplier,
            SavedTime = GameHandler.SavedTime,
            SavedTimeRemaining = GameHandler.SavedTimeRemaining,
            TotalDeaths = GameHandler.TotalDeaths,
            TotalKills = GameHandler.TotalKills,
            TotalMoney = GameHandler.TotalMoney,

            DamageMultiplierUpgradeLevel = GameHandler.DamageMultiplierUpgradeLevel,
            LifeUpgradeLevel = GameHandler.LifeUpgradeLevel,
            PinballMultiplierUpgradeLevel = GameHandler.PinballMultiplierUpgradeLevel,
            PointsMultiplierUpgradeLevel = GameHandler.PointsMultiplierUpgradeLevel,

            VolumeMusic = GameHandler.VolumeMusic,
            VolumeSFX = GameHandler.VolumeSFX
		};

		BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + "savedata.gd");
        bf.Serialize(file, saveData);
        file.Close();

        Debug.Log("Game Saved!");
    }
}
