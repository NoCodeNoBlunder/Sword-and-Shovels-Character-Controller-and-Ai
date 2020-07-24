using UnityEngine;

#region Explanation 
// Our SpawnLoot MonoBehaviour is going to make use of this Interface!
#endregion

public interface ISpawns
{
    #region Properties Komment why we use them here
    // Each of these variables will be a Constructor cause we want them to be public 
    // but dont want them displayed in the Inspector!
    // Properties are never displayed in the Inspector even when they are public.
    // We dont need access modifers inside an Interface because everything is public by default!

    // What is the case convention for Properties
    #endregion

    Rigidbody itemSpawned { get; set; }  // This is going to be our item that gets spawned.
    Renderer itemMaterial { get; set; }  // The Material of the item
    ItemPickUp itemType { get; set; }

    // We cannot implement this Method here. Because it is just a template in an interface that has to be implemented.
    void CreateSpawn();
}
