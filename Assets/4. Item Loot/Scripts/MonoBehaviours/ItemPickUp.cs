using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Explanation 
// This Class is a MonoBehaviour and will get attached to each Item which can get picked up. We need to have a MonoBehaviour because we 
// need to access all the previously defined data inside our ItemPickUp Scriptable Object!
#endregion

public class ItemPickUp : MonoBehaviour
{
    // We create a reference to a ItemPickUp_SO wich will define the item
    // Each Item instance implements an itemDefinition which is of type ItemPickUp_SO which defines all items in the game!
    // This SO will be attached via the Inspector defining the item it is attached to!
    public ItemPickUp_SO itemDefinition;
    // This needs to be public althout we assigne its value in the script since other script need to access this variable
    [HideInInspector]
    public CharacterStats charStats;        // We will have mutliple instances of this script. All types of characters Npcs ,Hero etc etc
    CharacterInventory charInventory;             // there is only one Instance of this! The Players Inventory

    GameObject foundStats;                  // This will be the Player Character that we need to reference in Order to access its charStats!

    #region Constructor Komment
    // Whenever a new ItemPickUp instance is created this Constructor gets called. This can happend at runtime or not.
    // When is a new instance ItemPickUp created? When an Item which is going to hold the ItemPickUp script is spawned in game for example!
    #endregion
    public ItemPickUp()
    {
        // We are referencing CharacterInventory via its Singleton and assigning its value to charInv
        charInventory = CharacterInventory.Instance;
    }

    private void Start()
    {
        if (itemDefinition != null)
        {
            //ASSERTION: The Player exists
            // Since there is different instances of charStats we need to reference the Players charStats!
            // Alternavily we could pass in the Data via Inspector
            foundStats = GameObject.FindGameObjectWithTag("Player");
            charStats = foundStats.GetComponent<CharacterStats>();
            Debug.Log("Characterstats value gets assigned");
        }
    }

    #region Methods

    private void StoreItemInInventory()
    {
        // We want to store this very item with all its data. Because the StoreItem Method needs a type ItemPickUp which "this" is.
        charInventory.StoreItem(this);
    }

    // Before we use an Item we need to know what type of item we are dealing with( Weapon, Armour etc etc).
    public void UseItem()
    {
        switch (itemDefinition.itemType)
        {
            case ItemTypeDefinitions.HEALTH:
                charStats.ApplyHealth(itemDefinition.itemAmount);
                break;
            case ItemTypeDefinitions.MANA:
                charStats.ApplyMana(itemDefinition.itemAmount);
                break;
            case ItemTypeDefinitions.WEAPON:
                charStats.ChangeWeapon(this); // Same as before "ChangeWeapon" is looking for an ItemPickUp which this Instance is so we pass this.
                break;
            case ItemTypeDefinitions.ARMOUR:
                charStats.ChangeArmor(this);
                break;
            case ItemTypeDefinitions.WEALTH:
                charStats.GiveWealth(itemDefinition.itemAmount);
                break;
        }
    }

    // When the Player touches the Item we want him to interact with it. 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (itemDefinition.isStorable)
            {
                StoreItemInInventory();
            }
            else
            {
                UseItem();
            }
        }
    }

    #endregion
}
