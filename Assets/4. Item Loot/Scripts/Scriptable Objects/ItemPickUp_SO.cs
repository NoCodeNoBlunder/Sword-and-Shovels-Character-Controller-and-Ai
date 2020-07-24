using UnityEngine;

#region Enums
// enums can be declared outside the scope of our Event we need to differentiate between different items that can be picked up we do this by using enums.
// enums values are named in all CAPITAL letters.
// The ItemTypeDefinition will basically define what the item will be. 

// -The reason why these enums are declared outside of the scope of the class is that there might be classes that need to share enum typed that could be used
// in other scrips. 
#endregion
public enum ItemTypeDefinitions { HEALTH, MANA, WEAPON, ARMOUR, WEALTH, EMPTY}
// This is a Subsection of a Subsection. It is only going to be executed if the item is an armor. 
// Can we make ItemArmorSubType only visible if it is an armor via inheritance? Make a new ArmorPickUp_SO that extends ItemPickUp_SO???
public enum ItemArmorSubType { NONE, HEAD, CHEST, HANDS, LEGS, FEET}

// Again we need to make the SO visible in the Editor
[CreateAssetMenu(fileName = "New Item", menuName = "Spawnable Item/New Pick-up", order = 1)]
// A scriptable Object needs to extend ScriptableObject!

// This class will define our Items
public class ItemPickUp_SO : ScriptableObject
{
    // This field is accessed through 3 different Methods! Komment on that later?? Was have ich damit gemeint?

    public string itemName = "New Item";
    #region Initialize Enums Komment. We do this so they have a default value.
    // We initialize 2 variables of an enum type. These will be visible in the Inspector for the designer to modify.
    // The initializazion makes so the default value of these enums is the one we assigned.
    #endregion
    public ItemTypeDefinitions itemType = ItemTypeDefinitions.HEALTH;
    // How can we make it that this SubType only exists when we select an Armour??? It should work this way. We can inherit from a SO!!
    public ItemArmorSubType itemArmorSubType = ItemArmorSubType.NONE;

    // These variable will be used to deliniate how much of a certain stat an item recovers or takes away!
    public int itemAmount;
    public int spawnChanceWeight;     // This will be the change the item spawns

    #region Pipeline ???
    public Sprite itemIcon = null;                 // Will be the item that will be associated with the item.
    public Material itemMaterial = null;           // Will tell the SO which material to attach to the item.
    public Rigidbody itemSpawnObject = null;       // This Rigidbody is going to be used within gameplay. 
    public Weapon itemWeaponSlotObject = null;   // This is going to be used for the Rigidbody that is going to spawn out of the chest.
                                                // We changed it to be a Weapon now instead of a Rigidbody
    #endregion

    public bool isEquiped = false;          // Is the Item currently equipped 
    public bool isInteractable = false;     // Can we use the item
    public bool isStorable = false;         // Can we store this item??
    public bool isUnique = false;           // Are we going to spawn multiple clones of this item
    public bool isIndestructable = false;   // Is the item destrucable. Like a weapon that degrades.
    public bool isStackable = false;        // Can we have mutliple of the same item
    public bool isDestroyedOnUse = false;   // Is the item destroyed when we use it like mana Potion
    public float itemWeight = 0f;           // How heavy is the item.
}
