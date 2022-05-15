using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject coinDespawnParticlesPrefab;

    public void PlayCoinDespawnParticles()
    {
        transform.localScale = new Vector3(0, 0, 0);
        GameObject despawnParts = Instantiate(coinDespawnParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(despawnParts, 2);
    }
}
