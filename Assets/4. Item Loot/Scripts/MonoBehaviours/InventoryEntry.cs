using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Explanation
// This is a Custom class which will be created everytime we pick up an item!
#endregion
public class InventoryEntry
{
    public ItemPickUp invEntry;
    public int stackSize;
    public int inventorySlot;
    public int hotBarSlot;
    public Sprite hbSprite;

    // Constructor! This could not be done with a Start Method cause we need to assign each instance of IventoryEntry with Individual values!
    // For where it is placed on the hotbar and inventorySlot and what sprite is displayed

    // Currently we are not even using the Constructor
    public InventoryEntry(int stackSize, ItemPickUp invEntry, Sprite hbSprite)
    {
        this.invEntry = invEntry;

        this.stackSize = stackSize;
        this.hotBarSlot = 0;
        this.inventorySlot = 0;
        this.hbSprite = hbSprite;
    }
}
