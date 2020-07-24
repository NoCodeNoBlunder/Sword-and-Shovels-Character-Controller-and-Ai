using System.Collections;
using System.Collections.Generic;
//using UnityEditor;  // Siehe SaveCharacterStats Komment
using UnityEngine;

// Scriptable Objects: This Scriptable Object will define our our Characters!

// We have to use this attribute in order to use this in the Inspector by going to assets and then we see it there
// What this will do create a new file called NewStats containing a menu of called "Character" which will open the next context menu called "Stats"
// fileName will be the default name of the Scriptable Object when we create it.
// The menuName specifies under what name we will find it under Assest/Create/"menuName" we can have submenues!
// the order specifies on what position we find the menuName in the Assets/Create dropdown. Order doesnt seem to work for some reason tho!

// A ScriptableObject has to derive from ScriptableObeject!!!!

#region Scriptable Object Komment
// Scriptable Objects short(SO) are like a Custom Method. They define the Properties of a Object. Instead of having other Methods inheritimng from it.
// We simply create instances of this Class by declaring fields with this class as the type!

// Anyone can create their version of that scriptable Object and define stats for any Characters that they want!
// Can you organize it even more? And have a class inherit from a SO? So you have CharacterStats_SO and SO Enemy and Enemy_SO derives from CharcterStats_So??
// Again a SO can not be attached to a gameObject directly we are creating instanced of this class whenever we create a new SO via Assest/Create/Character/Stats
// And then attach this SO to another script!
#endregion

[CreateAssetMenu(fileName = "New Stats", menuName = "Character/Stats", order = 1)]
public class CharacterStats_SO : ScriptableObject
{
    // Attribute: What this means is that Unity will be able to serialize this class which enables Unity to display this class in the Inspector
    [System.Serializable]
    public class CharLevel // Auf diese Klasse hat nur CharacterStats_SO zugriff? Scheint so, weil sie in ihr deklariert ist?
    {
        #region CharLevelUps 
        // Every Character Scriptable Object can essentially have an array of LevelUp Objects. And each one of those will define what the new stats are when 
        // they reach that level. So maybe we want a figher class. And that figher will get a lot more strenght when he lvls up but probably not a lo of mana.
        // We could also look at a wizard, he wont be getting a lot of strenght but wisdom mana etc etc.
        #endregion
        // We can acctually have a class inside another class??? WTF?
        // In this Custom class we define what happends when a character levels up!
        // With this Custom class we can acctually define different level ups. For example a mage gets more mana and spellpower while a Tank gets more hp etc!
        public int maxHealth;
        public int maxMana;
        public int baseDamage;
        public int baseResistance;
        public int maxWealth;
        public float maxEmcumbrance;
        public int requiredXP;
    }

    #region Fields

    #region Access modifiers / Encapsulation
    // All these fields are public so they are to be accessed from anywhere but sometimes we want variables to only be modified by the Scriptable Object and other 
    // Classes can get them but cannot set them directly. But only call Methods that alter the value of these variables.
    // We can achieve this by using (auto)Properties with a public get but only a private set.
    // In this case it is ok that other classes can each the variables! In other cases this is not ok! Siehe Properties!
    #endregion

    // Die wir dieses SO sowohl für den Hero als auch für NPCs nutzten müssen wir checken, ob es sich um den Hero handelt oder nicht!
    public bool isHero = false;

    // This will keep track weather our stats are set manually or dynamically.
    public bool setManually = false;
    // This will keep track of weather or not our data needs to be stored when we close the programm or not!
    // We can set that per Scriptable Object!
    public bool saveDataOnClose = false;

    // The field prefixed with max or base will change as the hero levels.
    // Variables prefixed with current will change during gameplay!

    public int maxHealth = 0;
    public int currentHealth = 0;

    public int maxMana = 0;
    public int currentMana = 0;

    public int baseDamage = 0;
    public int currentDamage = 0;

    public float baseResistance = 0f;
    public float currentResistance = 0f;

    public int maxWealth = 0;
    public int currentWealth = 0;

    // Emcumbrance bedeutet belastung!
    public float maxEncumbrance = 0f;
    public float currentEncumbrance = 0;

    public int charExperience = 0;
    public int charLevel = 0;

    #region Level Up Object Array
    // This array is displayed in the Inspector and can be modified in the Inspector by designers who will define what how our stats get modified when we level up.
    // In this case The array lenght is the ammount of levels we offer.
    #endregion
    public CharLevel[] charLevels;

    #endregion

    #region Properties

    #region Encapsulation/ Creating a stack of instanced of ItemPickUp
    // We want to setup a way we can access the data in our Scriptable Objects but prevent other classes without permission to changing this data.
    // This idea is called Encapsulation: A fundamental of OOP. Encapsulation hides values or states of structured data objects in class. Blocking parties from unauthorized access.
    // Basically we want to setup the Scriptable object so the only thing that can change the data inside the Scriptable object is the Object itself! Now these Methods will
    // be accessible from setters and getters which will be public that other items can reference. So we will acctually have a CharacterStats script that can utilize the
    // Scriptable Object and call those methods. In our case we want some of these values to be accessed directly and others not.

    // To be precise autoproperties!
    // We define properties of the type ItemPickUp

    // We dont want anyone to make changed to what is stored inside this weapon reference!

    // We basically just created a stack of instances of the ItemPickUp(MonoBehaviour) class that we have yet to define dont in chapter 4.
    #endregion
    public ItemPickUp weapon { get; private set; }
    public ItemPickUp headArmour { get; private set; }
    public ItemPickUp chestArmour { get; private set; }
    public ItemPickUp handArmour { get; private set; }
    public ItemPickUp legArmour { get; private set; }
    public ItemPickUp footArmour { get; private set; }
    public ItemPickUp misc1 { get; private set; }
    public ItemPickUp misc2 { get; private set; }

    #endregion

    #region Stat Increasers

    // Here we will have all the Methods to change the values of our Stats which can be called from outside!
    public void ApplyHealth(int healthAmount)
    {
        // If the current Health + the health amount to add surpase the max health we obv want to be "only" full hp and not over full hp!
        if ((currentHealth + healthAmount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healthAmount;
        }
    }

    public void ApplyMana(int manaAmount)
    {
        // Wir müssen checken ob der aktuelle ManaAmount + díe ManaToAdd unsere MaxMane überschreiten! 
        if ((currentMana + manaAmount) > maxMana)
        {
            currentMana = maxMana;
        }
        else
        {
            currentMana += manaAmount;
        }
    }

    // I dislike this with this Method we kinda want to SetCurrentDamage seems better! Old was: GetDamage and GetResistance.
    public void SetCurrentDamage(int damageModifacator)
    {
        // Logik Assignement eigener name!
        currentDamage = baseDamage + damageModifacator;
    }

    public void SetCurrentResistance(int resistanceModifacator)
    {
        currentResistance = baseResistance + resistanceModifacator;
    }

    public void GiveWealth(int wealthAmount)
    {
        currentWealth = currentWealth + wealthAmount >= maxWealth ? maxWealth : currentWealth + wealthAmount;
    }

    #region Explanation EquipWeapon Method
    // We need to tell it which weapon to equip. Weapon are going to be classified as item pick ups.
    // We also need to know what inventory slot it will be stored in. Where the item is going
    // We are also going to need an gameObject which will represent our weaponSlot.
    // When you equip a weapon you need to set it as a child object of a weapon slot so the transform information will match up and
    // weapon will appear in the characters hand.
    // All the information will be passed as an Argument.

    // Dont we have to check if the Item is alleady equipped it seems like it???
    #endregion
    public void EquipWeapon(ItemPickUp weaponPickUp, CharacterInventory charInventory, GameObject weaponSlot)  // We are not yet using the weaponSlot!
    {
        // We made changes here in the Loot Chapter!
        Rigidbody newWeapon;

        // Armour is a property we declared earlier.
        weapon = weaponPickUp;
        charInventory.inventoryDisplaySlots[2].sprite = weaponPickUp.itemDefinition.itemIcon;
        newWeapon = Instantiate(weaponPickUp.itemDefinition.itemWeaponSlotObject.weaponPrefab, weaponSlot.transform);
        // This is acctually clever code here 3 classes are working together!
        // weaponPickUp stores information of type ItemPickUp which contains a field itemDefinition which is of type ItemPickUp_SO which contains a field itemAmount!
        currentDamage += baseDamage + weaponPickUp.itemDefinition.itemAmount;
        Debug.Log("current Damage is " + currentDamage);
    }

    public void EquipArmor(ItemPickUp armorPickUp, CharacterInventory charInventory)
    {
        #region Komment
        // Because we are dealing with many different armour types it makes sense to use a switch statement here. ICh IDIOT!!!!!!!!!!!!!!!!!!!!!!
        // By clicking on switch right after typing it value it checking it autocompletes the switch statement!! OP
        // Switch statement are great when you have more than 2 to 3 values and they are all specific values!!!!!
        // We access it via the armourPickUp parameter which is of type ItemPickUp and contains itemDefinitions which is of type ItemPickUp_SO which contains our armorTypes
        // inside an enum! I would have passed an extra argument for the type but it is smarter this way!
        #endregion
        switch (armorPickUp.itemDefinition.itemArmorSubType)
        {
            case ItemArmorSubType.HEAD:
                // We have to specify in what slot which type of armour is going to be stored!
                charInventory.inventoryDisplaySlots[3].sprite = armorPickUp.itemDefinition.itemIcon;
                headArmour = armorPickUp;
                // WHY += and not = ?? 
                currentResistance += baseResistance + armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.CHEST:
                charInventory.inventoryDisplaySlots[4].sprite = armorPickUp.itemDefinition.itemIcon;
                chestArmour = armorPickUp;
                currentResistance += baseResistance + armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.HANDS:
                charInventory.inventoryDisplaySlots[5].sprite = armorPickUp.itemDefinition.itemIcon;
                handArmour = armorPickUp;
                currentResistance += baseResistance + armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.LEGS:
                charInventory.inventoryDisplaySlots[6].sprite = armorPickUp.itemDefinition.itemIcon;
                legArmour = armorPickUp;
                currentResistance += baseResistance + armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.FEET:
                charInventory.inventoryDisplaySlots[7].sprite = armorPickUp.itemDefinition.itemIcon;
                footArmour = armorPickUp;
                currentResistance += baseResistance + armorPickUp.itemDefinition.itemAmount;
                break;
            default:
                break;
        }
    }

    public void GiveXP(int xpAmount)
    {
        charExperience += xpAmount;

        if (charLevels[charLevel].requiredXP <= xpAmount)
        {
            charLevels[charLevel].requiredXP = 0;
            xpAmount -= charLevels[charLevel].requiredXP;
            SetCharacterLevel(charLevel);
            charLevels[charLevel].requiredXP -= xpAmount;
        }
        else
        {
            charLevels[charLevel].requiredXP -= xpAmount;
        }
    }

    #endregion

    #region Stat Decreasers

    public void LoseHealth(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            // I am retarded i didint put <= 0 xd you would live with 0 hp!!!!
            // Character is dead
            Death();
        }
    }

    public void LoseMana(int manaAmount)
    {
        // Logic statement self!
        // We dont want our mana to go below 0!
        currentMana = currentMana - manaAmount <= 0 ? 0 : currentMana - manaAmount;
    }

    #region Komment UnEquip Method!!!!
    // We can only UnEquip a weapon that is currently equiped. Therefore to avoid any bugs we need to check if the weapon is currently attached.
    // We are going to have a MEthod that returns a bool. We are going to have code to determine weather that thing is true or false and then we will return it.
    // What is really great about this is that we can then take this Method and put it inside an if statement(condition) and the Method will run and the if statement will do
    // what it needs to do to act as if it is true or false if(MethodOfReturnTypeBool) ... genial!! The Method will run as soon as the condition is evaluated. 
    // This is a really great way to setup equiping system because if the item is equipable there is some things we want to do (e.g. Put item in head change inventory etc.)
    // If we cant maybe it will just go back right into the inventory etc etc.

    // There is certain information we need to pass it. First which weapon should be unequiped.
    // We need to tell it which inventory this is coming from.
    // Also we need to tell it in which weaponSlot the weapon currently is.
    #endregion
    public bool UnEquipWeapon(ItemPickUp weaponToRemove, CharacterInventory charInventory, GameObject weaponSlot)
    {
        bool isWeaponEquiped = false;

        // weapon is the gameObject that stores our currently equiped weapon
        if (weapon == null)
        {
            // If the Function enters this if statement the funcion will end here and no additional code from the function will be run.
            return isWeaponEquiped;   
        }

        if (weapon == weaponToRemove)
        {
            charInventory.inventoryDisplaySlots[2].sprite = null;
            isWeaponEquiped = true;
            #region Destroy over DestroyObject, Inspector Setup Komment
            // DestroyObject is deprecated use Destroy instead!
            // We are going to destroy the Equipped gameObject. Which is the child of our weaponSlot!
            // Transform also holds information about child objects not only position, rotation and scale!!
            // GetChild(0) will give us the first child of this gameObject!

            // Since the weapon that is currently equipped, is destroyed using by desttoying the child of the WeaponSlot this need to be Setup accordingly in the Inspector.
            // The currently equpped weapon in our case the sword needs to be the child of the WeaponSlot and thw WeaponSlot needs to be paranted to where we want to spawn
            // The Weapon which is in the Hand of the Character therefore we parent it to the Characters Wrist. We also parent the CharInventory to our Hero to keep things
            // organized. We need this gameObj anyway cause we need to attach a Class to another Script which is only possible if it is attached to a gameObj.
            #endregion
            Destroy(weaponSlot.transform.GetChild(0).gameObject);
            weapon = null;
            currentDamage = baseDamage; 
        }

        return isWeaponEquiped;
    }

    // For the Armour also we need to pass it info what item we want to remove and from what inventory. Since we dont have a slot for our Armour we dont need to
    // Specify an armourSlot!
    public bool UnEquipArmor(ItemPickUp armorToRemove, CharacterInventory charInventory)
    {
        bool isArmorEquiped = false;

        switch (armorToRemove.itemDefinition.itemArmorSubType)
        {
            case ItemArmorSubType.HEAD:
                if (headArmour != null)
                {
                    if (headArmour == armorToRemove)
                    {
                        isArmorEquiped = true;
                        headArmour = null;
                        currentResistance -= armorToRemove.itemDefinition.itemAmount;
                    }
                }
                 break;
            case ItemArmorSubType.CHEST:
                if (chestArmour != null)
                {
                    if (chestArmour == armorToRemove)
                    {
                        isArmorEquiped = true;
                        chestArmour = null;
                        currentResistance -= armorToRemove.itemDefinition.itemAmount;
                    }
                }
                break;
            case ItemArmorSubType.HANDS:
                if (handArmour != null)
                {
                    if (handArmour == armorToRemove)
                    {
                        isArmorEquiped = true;
                        handArmour = null;
                        currentResistance -= armorToRemove.itemDefinition.itemAmount;
                    }
                }
                break;
            case ItemArmorSubType.LEGS:
                if (legArmour != null)
                {
                    if (legArmour == armorToRemove)
                    {
                        isArmorEquiped = true;
                        legArmour = null;
                        currentResistance -= armorToRemove.itemDefinition.itemAmount;
                    }
                }
                break;
            case ItemArmorSubType.FEET:
                if (footArmour != null)
                {
                    if (footArmour == armorToRemove)
                    {
                        isArmorEquiped = true;
                        footArmour = null;
                        currentResistance -= armorToRemove.itemDefinition.itemAmount;
                    }
                }
                break;
        }

        return isArmorEquiped;
    }

    #endregion

    #region Character Level Up and Death

    // Ich bin ein Degenerate. Wieso mache ich nicht use von dieser Methode in ChatacterStats!
    // Dafür ist diese Methode doch da!

   


    public void SetCharacterLevel(int newLevel)
    {
        // Wieso newLevel + 1?? Weil arrays mit 0 beginnen!
        charLevel = newLevel + 1;

        // Display LevelUp visualization.
        // We need to increase all of the Stats
        // We setup a charater levelup array and we can access that accoring to the level.
        // Since we define our levelups via our Custom class CharLevelUps which is inside our CharacterStats_SO. We can now access the data for each levelUp.
        // Since our Character starts at lvl 1 but an array starts at index 0 we need to substract 1 from the index.

        // -We can make the choice to fill all the current Stats to the maximum on level up like a lot of games do!

        maxHealth = charLevels[newLevel].maxHealth;
        currentHealth = maxHealth;

        maxMana = charLevels[newLevel].maxMana;
        currentMana = maxMana;

        maxEncumbrance = charLevels[newLevel].maxEmcumbrance;

        maxWealth = charLevels[newLevel].maxWealth;

        baseDamage = charLevels[newLevel].baseDamage;
        // Hier haben wir ein Problem, da sich unser currentDamage aus dem baseDamage + weaponDamage zusammensetzt!
        // Das selbe gilt für currentResistance!

        baseResistance = charLevels[newLevel].baseResistance;
    }

    private void Death()
    {
        //Debug.Log("You are dead");

        // Actions that are triggered on Death!
        // Call to GameManger for DeathState to trigger respawn
        // Display Death visualazations
    }

    #endregion

    #region Save Character Data
    // Only used for debugging CANT go into finished product.

    public void SaveCharacterData()
    {
        //saveDataOnClose = true;
        #region SetDirty() new Method Komment
        // New Method SetDirt() using Unity.Editor namespace 
        // Basically what we are saying is whatever Instance this is called from Set it to Dirty
        // What SetDirty() does is it tells Unity that the Data of this Object needs to be resaved because it is "dirty" xd.
        #endregion
        #region UnityEditor namespace Komment
        // We have to be very carefull when using this. Our user are not going to install the hole UnityEditor when they install our application.
        // They are only going to install the Components and Scripts nesseccary to run it. So when you go to build the game using something from the Unity Editor 
        // on a script attached to a gameObject, the game wont build correctly. In other words: Your game will not build! of you have a script with unityEditor reference 
        // inside your scene!!! It is great for debugging and building out tools for your team to work with (what tools and how you build them?), defining things that artists can
        // use setting up "Editor Extensions". Thats all fine you just cant build the game if any of those Scripts are attached to a gameObj in your Scene. So we will leave it inside
        // our game but we are going to comment it out until we need it but remember it cannot stay in the game when it is Published!
        #endregion
        //EditorUtility.SetDirty(this);
    }

    #endregion
}
