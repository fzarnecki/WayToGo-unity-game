using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour
{
    #region Singleton
    private static SaveManager _instance;
    public static SaveManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveManager>();
            }
            return _instance;
        }
    }
    #endregion


    [HideInInspector] public int playerCoins;
    [HideInInspector] public List<int> unlockedLevels;
    [HideInInspector] public List<int> upgradeLevels;
    [HideInInspector] public bool tip;


    public bool SaveGame() {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        
        string path = Application.persistentDataPath + "/saves/" + "save.hi";

        FileStream file = File.Create(path);

        SaveData s = new SaveData();
        s.savePlayerCoins = playerCoins;
        s.saveUnlockedLevels = unlockedLevels;
        s.saveUpgradeLevels = upgradeLevels;
        s.saveTip = tip;

        formatter.Serialize(file, s);

        file.Close();

        return true;
    }

    public bool LoadGame() {
        string path = Application.persistentDataPath + "/saves/" + "save.hi";

        if (!File.Exists(path))
            return false;

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            SaveData s = (SaveData)formatter.Deserialize(file);
            playerCoins = s.savePlayerCoins;
            while (s.saveUnlockedLevels.Count > unlockedLevels.Count)
                unlockedLevels.Add(1);
            unlockedLevels = s.saveUnlockedLevels;
            upgradeLevels = s.saveUpgradeLevels;
            tip = s.saveTip;

            file.Close();
            return true;
        }
        catch
        {
            // Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return false;
        }
    }

    public BinaryFormatter GetBinaryFormatter() {
        return new BinaryFormatter();
    }

    public void InitializeSaveData(int distinctMapSizesCount, int upgradesCount) {
        
        playerCoins = 0;    // Player is initially poor

        unlockedLevels = new List<int>();    // As many entries as different map sizes, each map size has only 1 level unlocked
        for (int i = 0; i < distinctMapSizesCount; i++)
            unlockedLevels.Add(1);

        upgradeLevels = new List<int>(); // As many entries as different upgrades, no upgrade at the beginning
        for (int i = 0; i < upgradesCount; i++)
            upgradeLevels.Add(0);
        
        tip = false;    // No tip, no no

        SaveGame();
    }
}
