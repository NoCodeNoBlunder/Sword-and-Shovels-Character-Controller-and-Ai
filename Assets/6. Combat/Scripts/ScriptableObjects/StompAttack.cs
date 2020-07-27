using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stomb.Asset", menuName = "Attack/Aoe/Stomb")]
public class StompAttack : AttackDefinition
{
    public float radius, timeInScene;
    public GameObject aoePrefab;

    public void Fire(GameObject caster, Vector3 aoeCenterSpawnPos, int layer)
    {
        Debug.Log("Fire is called");
        // Instantiate and destroy our aoe Prefab. Quternion.identity is nothing else but rotation Vector3(0,0,0) we accutally want -90,0,0 here!
        var aoe = Instantiate(aoePrefab, aoeCenterSpawnPos, Quaternion.identity);
        Destroy(aoe, timeInScene);

        // Wir werden die characterStats von dem Caster später brauchen. Damit das nicht jeden Loop neu gemacht werden muss
        // Mache ich das hier oben!
        var casterStats = caster.GetComponent<CharacterStats>();

        // Get objects inside of our Aoe Prefab
        // Physics.OverlapSphere checks what objects are inside the radius!
        var collidedObjects = Physics.OverlapSphere(aoeCenterSpawnPos, radius);

        // Iterate through all all collided Objects
        foreach (var collision in collidedObjects)
        {
            var collisionGo = collision.gameObject;

            // Check if we are ignoring the collision's layer, if so move to the next Object in collidedObjects
            // New Keyword "continue": Which is used in Loops. When a certain Condition is true. The loops skips the current object or current 
            // loop index and moves on to the next index! 
            if (Physics.GetIgnoreLayerCollision(layer, collisionGo.layer))
                continue;

            // Create Attack that is going to the attackable behaviour of our collided gameobject#

            var collisionGoStats = collisionGo.GetComponent<CharacterStats>();
            Attack attack = CreateAttack(casterStats, collisionGoStats);

            var attackables = collisionGo.GetComponentsInChildren<IAttackable>();

            foreach (var a in attackables)
            {
                a.OnAttack(caster, attack);
            }
        }
    }
}
