using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructedSpawnRagdoll : MonoBehaviour, IDestructable
{
    // Das ist ein Ragdoll kein GameObject!
    public Ragdoll ragdollObject;
    // We can save time by doing this!
    public float force, lift;

    // We have to pass the Information that is needed through this Method!
    public void OnDestruction(GameObject destroyer)
    {
        Vector3 directionFromDestroyer = transform.position - destroyer.transform.position;
        directionFromDestroyer.y += lift;
        directionFromDestroyer.Normalize();

        // Ich bin retared wir wollen natürlich die Rotation von dem Ojekt übernehmen!
        var ragdoll = Instantiate(ragdollObject, transform.position, transform.rotation);

        ragdoll.ApplyForce(directionFromDestroyer * force);
    }
}
