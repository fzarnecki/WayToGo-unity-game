using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int savePlayerCoins;
    public List<int> saveUnlockedLevels;
    public List<int> saveUpgradeLevels;
    public bool saveTip;
}
