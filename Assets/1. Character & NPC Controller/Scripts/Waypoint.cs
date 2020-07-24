using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    void Start()
    {
        // We make the Waypoints invisible in the game!
        GetComponent<Renderer>().enabled = false;
    }
}
