using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayCoins : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CoinsManager coinsManager;

    /***/

    private void Start() {
        UpdateCoins();
    }

    public void UpdateCoins() {
        int coins = Mathf.Clamp(coinsManager.GetPlayerCoins(), 0, 9999999);
        text.text = coins.ToString();
    }
}
