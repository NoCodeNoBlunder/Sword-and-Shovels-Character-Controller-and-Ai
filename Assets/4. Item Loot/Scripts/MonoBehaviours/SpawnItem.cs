using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Explanation
// This class will be attached to gameObject that spawn Items in our game. For example when loot spawns.
#endregion
public class SpawnItem : MonoBehaviour,ISpawns
{
    // This will hold all the Objects that we want to spawn.
    public ItemPickUp_SO[] itemDefinitions;

    int whichToSpawn = 0;
    int chosen = 0;
    int totalSpawnWeight = 0;

    public Rigidbody itemSpawned { get; set; }
    public Renderer itemMaterial { get; set; }
    public ItemPickUp itemType { get; set; }

    void Start()
    {
        foreach (var ip in itemDefinitions)
        {
            // We calculate the totalSpawnWeight of all the items to spawn!
            totalSpawnWeight = ip.spawnChanceWeight;
        }
    }

    public void CreateSpawn()
    {
        foreach (var ip in itemDefinitions)
        {
            // ???
            whichToSpawn += ip.spawnChanceWeight;
            // For now it basically saying when whichToSpawn is bigger than 0 to spawn the object.
            if (whichToSpawn >= chosen)
            {
                // ASSERTION:
                itemSpawned = Instantiate(ip.itemSpawnObject,
                    transform.position, Quaternion.identity);

                #region Frage warum müssen wir das Material etc assignen wenn das SO das bereits für die Items macht???
                // Antwort: Das SO hat die Items definiert, jedoch nie wirklich die Items erstellt!! Wir spawnen keine GameObject Prefabs direkt
                // sondern spawnen unsere Scriptable Objects, die von dem SO definiert wurden aber noch zusammengebaut werden müssen!!

                // We need to fill our other variables. Why tho is this not allready attached to the gameObjects automatically???????
                // Ich verstehe das hier nicht wir haben doch jedem Item sein Material im SO zugewiesen. Wieso müssen wir das jetzt erneut machen? 
                #endregion
                itemMaterial = itemSpawned.GetComponent<Renderer>();
                itemMaterial.material = ip.itemMaterial;

                itemType = itemSpawned.GetComponent<ItemPickUp>();
                itemType.itemDefinition = ip;
                // Since we are in a loop we have to type break because we want to Method to stop running after we spawned an Object!
                // We are looking for one specific thing and once we find that we want the loop to stop running
                break;
            }
        }
    }
}
