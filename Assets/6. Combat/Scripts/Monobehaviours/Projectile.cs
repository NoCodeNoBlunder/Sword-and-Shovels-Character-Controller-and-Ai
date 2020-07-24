using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Explanation
// This Objekt will be attached to every Projectile Prefab defining each Projectile!
// Each Projectile will be fired towarda a target therefore we need a fire Method that is going define this behaviour!
#endregion
public class Projectile : MonoBehaviour
{
    // This event will get passed the Caster and the Target!
    public event Action<GameObject, GameObject> OnProjectileCollision;

    public GameObject firePrefab;

    // We need to hold a reference to:
    // Some of these Variables have been defined in our Spell So and its base AttackDefinition. But we need to 
    // have a reference to them and each instance want to have its own variables which will assign values to in Fire
    // which works simmilarly to a Constructor!
    private GameObject caster;
    private float speed, range, distanceTraveled;
    private Vector3 travelDirection;

    #region Komment Fire() Es fungiert wie ein Constructor!
    // Wieso schreiben wir keinen Constructor????

    // This Method will be called whenever we fire a Projectile. It need data to which we pass to it via arguments!
    // This Method works similar to a Constructor!!??
    // We could be firing different projectiles with different casters targets, speeds and range all. To all those 
    // variabled we need to hold a reference. And the Object thaz will spawn these Projectiles will need to specify 
    // the values for the Projectile!
    #endregion
    public void Fire(GameObject caster, Vector3 targetPosition, float speed, float range)
    {
        this.caster = caster;
        this.speed = speed;
        this.range = range;

        // Calculate travelDirection. The Travel direction is only calculated once so we will be able to dodge the bullets
        // Ich have einen Fehler gemacht wir brauchen den Vector von der Position der Kugel zum Spieler und nicht vom Caster zum Spieler!
        travelDirection = targetPosition - transform.position;

        // We dont want our Projectiles to travel in the y-axis
        travelDirection.y = 0;
        travelDirection.Normalize();

        // Wieso geht das jetzt auf einmal?
        firePrefab.transform.rotation = Quaternion.LookRotation(-travelDirection);

        // Initialize our distanceTraveled to 0
        // This variable will be how far our Projectile has traveled
        distanceTraveled = 0;
    }

    void Update()
    {
        // Here we will be moving the Projectile through space!
        // s = v * t; da wir hier jedoch im Update Loop sind vergeht zwischen den UpdateLoops Time.deltaTime!!!!!

        // Ich vergesse Time.delta Time xd!!!

        // How much to move this frame(as this is dependant on the frame rate)
        float distanceToTravel = speed * Time.deltaTime;

        transform.Translate(travelDirection * distanceToTravel);

        // Check if the projectile has traveled to far an destroy it if yes!
        distanceTraveled += distanceToTravel;
        if (distanceTraveled > (range * 1.0f))
        {
            Destroy(gameObject);
        }
    }

    // In order for a collision to be registered. Both Object need to have a Collider an at least one has to have a rigidbody!
    private void OnTriggerEnter(Collider other)
    {
        OnProjectileCollision?.Invoke(caster, other.gameObject);

        Destroy(gameObject);
    }
}
