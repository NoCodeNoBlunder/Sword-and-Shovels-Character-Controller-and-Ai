using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    void Start()
    {
        // We want the SpawnPoint to be invisible as soon as the Game start!
        GetComponent<Renderer>().enabled = false;
    }
}
