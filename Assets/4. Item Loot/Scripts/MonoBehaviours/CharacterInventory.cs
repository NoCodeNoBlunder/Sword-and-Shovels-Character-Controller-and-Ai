using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UI;

#region Why can we make a Singleton for CharacterInventory
// We can make a make a Singleton reference for this Object cause there is only going to be one CharacterInventory which is going to be the Players
// CharacterInventory! What if we want to make the Game multiplayer later on and there is more players we would not be able to use a Singleton 
// in this Context right?
#endregion

#region Explanation
// CharacterInventory is going to be responsible for keeping track what stackable items we have 
#endregion
public class CharacterInventory : MonoSingleton<CharacterInventory>
{
    #region Komment on Regions
    // Regions are used for Organization purposes but also for Platform variable Input. The code get read based on what plattform we are on PC,IOS etc etc.
    // They make sure that only the right logic gets executed
    #endregion

    #region Function Stubs Komment
    // Methods/Functions that are declared but dont yet have a body which will be written later on.
    #endregion

    #region Fields

    public CharacterStats charStats;
    GameObject foundStats;

    public Image[] hotBarDisplayHolders = new Image[4];
    public GameObject InventoryDisplayHolder;
    public Image[] inventoryDisplaySlots = new Image[30];

    public Dictionary<int, InventoryEntry> itemsInInventory = new Dictionary<int, InventoryEntry>();
    public InventoryEntry itemEntry;

    int inventoryItemCap = 20;
    int idCount = 1;
    bool addedItem = true;

    #endregion

    #region 

    private void Start()
    {
        itemEntry = new InventoryEntry(0, null, null);
        itemsInInventory.Clear();

        inventoryDisplaySlots = InventoryDisplayHolder.GetComponentsInChildren<Image>();

        foundStats = GameObject.FindGameObjectWithTag("Player");
        charStats = foundStats.GetComponent<CharacterStats>();
    }

    #endregion

    #region my Own
    /*
     *  private CharacterStats charStats;
    private InventoryEntry itemEntry;


    public Image[] hotBarDisplayHolders = new Image[4];
    public

    public Dictionary<int, InventoryEntry> itemsInInventory;
    private int idCount = 0;
    private int inventoryItemCap;

    private bool addedItem = false;

    #endregion

    #region Initialization

    private void Start()
    {
        // Why we have to use in GetComponentInParent?
        charStats = GetComponentInParent<CharacterStats>();
        itemsInInventory = new Dictionary<int, InventoryEntry>();
    }
     * 
     * */
    #endregion
    private void Update()
    {
        #region Input
        // Checking for a hotbar key to be pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("1 Button Input Detected");
            TriggerItemUse(101);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerItemUse(102);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerItemUse(103);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TriggerItemUse(104);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
        #endregion

        // Check to see if the item has allready been added - Prevemt duplicate adds for 1 item.
        if (!addedItem)
        {
            TryPickUp();
        }
    }

    // itemToStore will be the Item the Character passed through
    public void StoreItem(ItemPickUp itemToStore)
    {
        // Use Dictionary Collection to store item

        // This will make sure that we are not adding multiple items as the character picks up "one" item.
        addedItem = false;

        // Wenn dein Character das tragen kann, dann kann er es storen. (Emcumbrance)
        if ((charStats.charDefinition.currentEncumbrance + 
            itemToStore.itemDefinition.itemWeight) <= charStats.charDefinition.maxEncumbrance)
        {
            // The new itemEntry will be set to our item
            itemEntry.invEntry = itemToStore;
            itemEntry.stackSize = 1;
            itemEntry.hbSprite = itemToStore.itemDefinition.itemIcon;

            // The item we picked up will disapprear from the Environment!
            // addedItem is not used yet because when we set the Item inactive once we pick it up it cannot happend that we pick it up 
            // multiple times!
            itemToStore.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("The Item is to heavy!");
        }
    }
    
    // Komplexestes geflecht aus Conditional statements das ich bisher hatte
    // Will check if the Item can be picked up based on inventory Slots and Encumbrance
    private void TryPickUp()
    {
        bool itsInInv = true;

        // Check if the Item was stored properly and submitted to the inventory - Continue of true
        // -invEntry is acctually the item we try to pickup
        // -Ist das die equivalente zur bedienung (itemEntry.invEntry != null)???
        if (itemEntry.invEntry)
        {
            // Check to see if any items exist in the inventory already - if not, add this item
            if (itemsInInventory.Count == 0)
            {
                // ASSERTION: If there is no items of the type in the Inventory we can add it for sure!
                addedItem = AddItemToInv(addedItem);
            }
            else
            {
                // ASSERTION: Item does exist in the Inventory allready
                // Check of see if the item is stackable - continue if stackable!
                if (itemEntry.invEntry.itemDefinition.isStackable)
                {
                    foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
                    {
                        // Does this item allready exist in the Inventroy? - Continie if yes
                        // 
                        if (itemEntry.invEntry.itemDefinition == ie.Value.invEntry.itemDefinition)
                        {
                            // Add 1 to stack and destroy the new instance
                            ie.Value.stackSize += 1;
                            AddItemToHotBar(ie.Value);
                            itsInInv = true;
                            Destroy(itemEntry.invEntry.gameObject);
                            // Once we have confirmation that the item exists allready in the Inventory we dont need to loop further!
                            break;
                        }
                        else
                        {
                            // ASSERTION: ItemEntry does not exist in the inventory allready
                            itsInInv = false;
                        }
                    }
                }
                else
                {
                    //ASSERTION the item is not stackable
                    itsInInv = false;

                    // If no space item is not stackable - say inventory full
                    if (itemsInInventory.Count == inventoryItemCap)
                    {
                        itemEntry.invEntry.gameObject.SetActive(true);
                        Debug.Log("Inventory is FUll!");
                    }
                }

                // Check if there is space in inventory - if yes, continue here
                if (!itsInInv)
                {
                    addedItem = AddItemToInv(addedItem);
                    itsInInv = true;
                }
            }
        }
    }

    // Will check if the Item is allready in the Inventory. And if it is if it is stackable!
    private bool AddItemToInv(bool finishedAdding)
    {
        #region Adding item to Dictionary
        // We are adding the IdCount which is the key of the item that is in the dictionary.
        // This is very simmilar to if you made this in array.
        // Then its going to pass a new InventoryEntry. The InventoryEntry is an Object.
        // The stacksize is going to define how many things are in there. If we have nono in there the stacksize would be 1.
        // We have to instantiate a new ItemEntry because we are wanting to duplicate every single Object and have its own special
        // new Object. We dont want to just point to a previous obj if that Obj happend to be deleted then all of these Objects would be deleted that were
        // referencing it. So Instantiate is going to create a hole new copy that we can use
        #endregion
        itemsInInventory.Add(
            idCount, new InventoryEntry(itemEntry.stackSize, 
            Instantiate(itemEntry.invEntry), itemEntry.hbSprite));

        //UseSwordDebug();

        // We allready created an Inventory entry so we dont need to use the one that we are pointing to.
        // After adding it to the Inventory we are going to destroy the Object which will remove it from the Environment but we will have it in our
        // Inventory.

        Destroy(itemEntry.invEntry.gameObject);

        FillInventoryDisplay();
        AddItemToHotBar(itemsInInventory[idCount]);
        idCount = IncreaseID(idCount);

        finishedAdding = true; // This method will return true which will be assigned to addedItem. When added item is true no item can be added
        return finishedAdding;          // which ensures each item is only picked up once!
    }

    // Will add item to our ui and display its icon there
    // -We dont yet have a type InventoryEntry
    private void AddItemToHotBar(InventoryEntry itemForHotBar)
    {
        int hotBarCounter = 0;
        bool increaseCount = false;

        // Check for open hotbar slot
        foreach (var images in hotBarDisplayHolders)
        {
            hotBarCounter += 1;

            if (itemForHotBar.hotBarSlot == 0)
            {
                //ÀSSERTION: WE have not yet changed any of the Sprites in the Hotbar yet.
                if (images.sprite == null)
                {
                    // Add item to open hotbar slot
                    itemForHotBar.hotBarSlot = hotBarCounter;
                    // Change hotbar sprite to show ite,
                    images.sprite = itemForHotBar.hbSprite;
                    increaseCount = true;
                    break;
                }
            }
            else if (itemForHotBar.invEntry.itemDefinition.isStackable)
            {
                increaseCount = true;
            }
        }

        // Here we will increase the red text on the Hotbar when a new item is added
        if (increaseCount)
        {
            hotBarDisplayHolders[itemForHotBar.hotBarSlot - 1].GetComponentInChildren<Text>().text =
                itemForHotBar.stackSize.ToString();
        }

        increaseCount = false;
    }

    // It increases our ID based on how many items are in our Inventory!
    private int IncreaseID(int currentID)
    {
        int newID = 1;

        for (int itemCount = 1; itemCount <= itemsInInventory.Count; itemCount++)
        {
            // if it find the acctuall id inside of our dictionary, thats our inventory, 
            //its going to increase the id to one more than that. The Key is basically our index and the value is our ItemEntry
            if (itemsInInventory.ContainsKey(newID))
            {
                newID += 1;
            }
            else return newID;
        }

        return newID;
    }

    // Will toggle wether our Inventory is currently visible or not
    // this will be triggered by an event as we dont want to see the Inventory all the time
    private void DisplayInventory()
    {
        if (InventoryDisplayHolder.activeSelf == true)
        {
            InventoryDisplayHolder.SetActive(false);
        }
        else
        {
            InventoryDisplayHolder.SetActive(true);
        }
    }

    // Will shot the ItemSprites in the ui based on whats equipped and used!
    // This Method knows where everything is on your grid in your UI.
    // - This will do a slot counter 
    private void FillInventoryDisplay()
    {
        int slotCounter = 9;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
        {
            slotCounter += 1;
            inventoryDisplaySlots[slotCounter].sprite = ie.Value.hbSprite;
        }

        // The Sprites wont show anything if there is no item in this slot!
        // All the Slots that are empty wont have a spirte!
        while (slotCounter < 29)
        {
            slotCounter++;
            inventoryDisplaySlots[slotCounter].sprite = null;
        }
    }

    // This will be the Event when you press to use an Item!
    // This gets called when we acctually hit a button
    public void TriggerItemUse(int itemToUseId)
    {
        bool triggerItem = false;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
        {
            // Why 100?
            if (itemToUseId > 100)
            {
                itemToUseId -= 100;

                if (ie.Value.hotBarSlot == itemToUseId)
                {
                    triggerItem = true;
                }
            }
            else
            {
                if (ie.Value.inventorySlot == itemToUseId)
                {
                    triggerItem = true;
                }
            }

            if (triggerItem)
            {
                Debug.Log("Trigger item is true");
                // Check if the item you are using is stackable. If it is not it will disappear completly on use.
                // Otherwise the counter wills simply be reduced by 1
                if (ie.Value.stackSize == 1)
                {
                    Debug.Log("UseItem is called");
                    if (ie.Value.invEntry.itemDefinition.isStackable)
                    {
                        Debug.Log("UseItem is called");
                        if (ie.Value.hotBarSlot != 0)
                        {
                            Debug.Log("UseItem is called");
                            hotBarDisplayHolders[ie.Value.hotBarSlot - 1].sprite = null;
                            hotBarDisplayHolders[ie.Value.hotBarSlot - 1].GetComponentInChildren<Text>().text = "0";
                        }

                        ie.Value.invEntry.UseItem();
                        itemsInInventory.Remove(ie.Key);
                        break;
                    }
                    else
                    {
                        //ASSERTION: Item is not stackable
                        //- Hier hab ich einen Fehler gemacht es ist crucial, dass die if und else statements an der richtigen Stelle platziert sind!
                        Debug.Log("Use Not Stackable Obj");
                        ie.Value.invEntry.UseItem();
                        if (!ie.Value.invEntry.itemDefinition.isIndestructable)
                        {
                            // ASSERTION: Item is Destructable
                            itemsInInventory.Remove(ie.Key);
                            break;
                        }
                    }
                }
                else
                {
                    //ASSERTION: We have multiple items in here that are being used.
                    Debug.Log("UseItem is called");
                    ie.Value.invEntry.UseItem();
                    ie.Value.stackSize -= 1;
                    hotBarDisplayHolders[ie.Value.hotBarSlot - 1].GetComponentInChildren<Text>().text =
                        ie.Value.stackSize.ToString();
                    break;
                }
            }
            else
            {
                // Nothing happends. Wir müssen dieses Else Statement hier nicht haben aber zur veranschaulichung lasse ich es hier!
                // Auch um zu zeigen wir crucial es ist alle conditional Statements an der richtigen Stelle zu haben! Alles kann stimmen
                // aber das Positioning nicht und schon ist alles rip!
            }
        }

        FillInventoryDisplay();
    }

    private void UseSwordDebug()
    {
        itemsInInventory[1].invEntry.UseItem();
    }
}
