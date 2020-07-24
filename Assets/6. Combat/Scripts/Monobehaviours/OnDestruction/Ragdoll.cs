using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody ragdollCore;
    public float timeTillDestroy;

    void Start()
    {
        Destroy(gameObject, timeTillDestroy);
    }

    public void ApplyForce(Vector3 forceDirection)
    {
        ragdollCore.AddForce(forceDirection);
    }

}
