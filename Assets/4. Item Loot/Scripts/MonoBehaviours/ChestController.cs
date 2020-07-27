using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour, ILootable
{
    public GameObject chestOpened;
    public SpawnItem spawner;

    bool isChestOpened = false;

    private void Start()
    {
        chestOpened.SetActive(isChestOpened);
    }

    public void OnLooting()
    {
        if (!isChestOpened)
        {
            isChestOpened = true;
            chestOpened.SetActive(isChestOpened);
            gameObject.SetActive(!isChestOpened);
            spawner.CreateSpawn();
        }
    }
}
