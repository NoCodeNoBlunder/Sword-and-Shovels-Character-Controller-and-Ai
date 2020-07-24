using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

#region Explanation
// Now we need to write our CharacterStats MonoBehaviour we need to do this so 
// we can access all the data we setup inside of our CharacterStats Scriptable Object.

// When we create a new type of Character we create a new instance of CharacterStats and it will include a new CharacterStats_SO which will be able to be 
// modified in the Inspector specifing the stats of the character
#endregion
public class CharacterStats : MonoBehaviour
{
    #region Fields

    public CharacterStats_SO characterDefiniton_Template;
    // We are going to need our CharacterStats Scriptable Object that we are going to create for whichever object we are using.
    // The ChatacterStats_SO Object Instance will be created in the Inspector and attached via the Inspector to this script!
    public CharacterStats_SO charDefinition;
    public CharacterInventory charInv;
    // This is the gameObject which when we equip a weapon that we are going to have the actual weapon prefab snap to so it shows up in the scene at the correct spot and rotation.
    public GameObject charWeaponSlot;

    #endregion

    #region Constuctor

    #region Komment Constructor
    // Constuctor Definition: 
    // A Special Method of the class which gets automatically invoked whenever an instance of the class is created. Like Methods, a Constructor
    // also contains the collection if instructions that are executed at the time of Object creation.

    // This doesnt make a hole lot of sense to me cause it is a Custom Class and we cannot simply create an instance of this class??
    // We manually create a new instance of this class whenever we create a new type of Character. 

    // Self: When is a new Instance of this Class created? At Runtime When an gameObject(Character) in the active in the Scene holds this script!
    #endregion
    public CharacterStats()
    {
        // We are accessing our charInv via the CharacterInventory'ies Singleton instance and
        // are assigning it to charInv! 
        charInv = CharacterInventory.Instance;
    }

    #endregion

    #region Initializations

    private void Awake()
    {
        #region Komment CharacterDefnition_Template
        // This has to be done in the Awake Method to make sure it is saved before any changes are made to our values.
        // Cause these are the values we want to start the game with! They might be overridden if we have this take place in start!
        // We want to make sure that our charDefinition is fully initialized before manipulating its values!

        // There is a problem any changes to the our characterDefinition at runtime will persist across game sessions!
        // We dont want that cause we want out charcterstats to act as a template or definition something that is not affected at runtime!
        // Was hier passiert ist sombald das game Startet wird charDerfinition = unserer charDefintion_Template gesetzt und at runtime
        // werden nur die Werte der characterDefinition verändert jedoch nicht die der Template die als eine art Speicher dient!
        #endregion
        if (characterDefiniton_Template != null)
        {
            charDefinition = Instantiate(characterDefiniton_Template);
        }
    }

    private void Start()
    {
        // What are we doing here?
        // Das machen wir um die Werte abzuspeichern damit sie wenn das Game neu gestartet wird auf dem Anfangswert gesetzt werden, da die Werte 
        // im Spiel natürlich beinflusst werden!

        #region Initiate Hero Stats based on CharLevel
        // What do we want to happend at startup.
        //-Set our Heros Level to 1
        //-Set our base and currentStats to those defined in our first element of our CharLevels array.

        if (charDefinition.isHero)
        {
            // Akward, da wir eigentlich mit level 1 anfangen aber arrays it index 0 beginnen!
            // Wir sollten einfach index 0 empty lassen imo! Und index sollte level 1 sein!
            charDefinition.SetCharacterLevel(1);
        }

        #endregion

        #region Automatically Assign Values for NPCs 

        //-This only takes place when the values are not being set manually!
        // We need to check if the Stats are set manually or not.
        // If they are not Set manually we have to set them here via code.
        if (!charDefinition.setManually && !characterDefiniton_Template.isHero)
        {
            charDefinition.maxHealth = 100;
            charDefinition.currentHealth = 70;

            charDefinition.maxMana = 200;
            charDefinition.currentMana = 50;

            charDefinition.maxEncumbrance = 100f;
            charDefinition.currentEncumbrance = 0f;

            charDefinition.maxWealth = 500;
            charDefinition.currentHealth = 0;

            charDefinition.baseDamage = 10;
            charDefinition.currentDamage = charDefinition.baseDamage;

            charDefinition.baseResistance = 0;
            charDefinition.currentResistance = charDefinition.baseResistance;

            charDefinition.charExperience = 0;
            charDefinition.charLevel = 1;
        }
        #endregion
    }

    #endregion

    #region Update

    private void Update()
    {
        // Siehe CHaracterStats_SO SaveCharacterData Komment
        /*if (Update.GetMouseButtonDown(2))
        {
            charDefinition.SaveCharacterData();
        }*/
    }

    #endregion

    #region Stat Increasers

    #region Wrappers Komment
    // -This is going to function pretty similalrly to the Stat Increasers we wrote inside of our SO.
    // But this is going to give us access to these Method and Variables that we need.
    // What we are doing here is wrapping the characterDefinition(type CharacterStats_SO) methods inside of a Methods for CharacterStats.
    // Thatway if we have to call it we dont have to write out a super long line. Which would be CharacterStats.charDefinition.ApplyHealth
    // This is basically investing into the future to save time and have a better overview.
    // -These Methods need to be public so they are accessible from anywhere!
    #endregion

    public void ApplyHealth(int healthAmount)
    {
        charDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(int manaAmount)
    {
        charDefinition.ApplyMana(manaAmount);
    }

    public void GiveWealth(int wealthAmount)
    {
        charDefinition.GiveWealth(wealthAmount);
    }

    // Ich habe den Namen dieser Methode verändert, sie nicht den 
    // gleichen Namen hat wie ein Reporter weiter unden "GetDamage"

    // Der Name ist schwach! Sollte SetCurrentDamage sein!
    public void BuffDamage(int damageModifacator)
    {
        charDefinition.SetCurrentDamage(damageModifacator);
    }

    // Siehe IncreaseDamage Kommentar
    public void IncreaseResistance(int resistanceModifacator)
    {
        charDefinition.SetCurrentResistance(resistanceModifacator);
    }

    #endregion

    #region Stat Decreasers

    public void LoseHealth(int damageAmount)
    {
        charDefinition.LoseHealth(damageAmount);
    }

    public void LoseMana(int manaAmount)
    {
        charDefinition.LoseMana(manaAmount);
    }

    #endregion

    #region Weapon and Armor Change

    #region Komment ChangeWeapon Method
    // We are making use of the previously wrote Method UnEquipWeapon in CharacterStats_SO which is very smart

    // We need to check if the Current item we want to add is allready equiped and if it is we need to remove it. 
    // To make room for a new GameObject.
    // We now have defined the characters Inventors and weaponSlot in the field region. And are to use them now!!!!!!!!!!!!
    // We dont need to pass them because this class has direct access to them wtf am i doing????

    // ChangeWeapon needs to be able to Equip and UnEquip weapons.
    #endregion
    public void ChangeWeapon(ItemPickUp weaponPickUp)
    {
        // Logic statement here!
        if (!charDefinition.UnEquipWeapon(weaponPickUp, charInv, charWeaponSlot))
        {
            charDefinition.EquipWeapon(weaponPickUp, charInv, charWeaponSlot);
        }
    }

    // Remember Weapons and armor are indestructable
    public void ChangeArmor(ItemPickUp armorPickUP)
    {
        if (!charDefinition.UnEquipArmor(armorPickUP, charInv))
        {
            charDefinition.EquipArmor(armorPickUP, charInv);
        }
        else
        {
            // Character says: I am Allready wearing this
        }
    }

    #endregion

    #region Reporters 
    // Methods to get Access to a Characters current stats

    // This will be Methods that will get information from our SO which is this character
    // Here we can call any information we need to know about the character.

    // Will tell us what weapon our character has attached.

    // We have changed this from a ItemPickUp to be a Weapon now!
    public Weapon GetCurrentWeapon()
    {
        // Ich muss also nicht sagen != null ?? Manchmal geht der shit manchmal nicht ich bin verwirrt!
        if (charDefinition.weapon)
        {
            return charDefinition.weapon.itemDefinition.itemWeaponSlotObject;
        }
        else
        {
            // Wieso geht das jetzt das ich null returne aber sonst ging das irgendwie manchmal nicht????
            return null;
        }
    } 

    // Will tell us what armour out character has attached.
    public ItemPickUp GetCurrentArmor(ItemArmorSubType armorType)
    {
        // A local variable has to be initialized
        ItemPickUp currentItemPickUp = null;
        switch (armorType)
        {
            case ItemArmorSubType.NONE:
                currentItemPickUp = null;
                break;
            case ItemArmorSubType.HEAD:
                currentItemPickUp = charDefinition.headArmour;
                break;
            case ItemArmorSubType.CHEST:
                currentItemPickUp = charDefinition.chestArmour;
                break;
            case ItemArmorSubType.HANDS:
                currentItemPickUp = charDefinition.handArmour;
                break;
            case ItemArmorSubType.LEGS:
                currentItemPickUp = charDefinition.legArmour;
                break;
            case ItemArmorSubType.FEET:
                currentItemPickUp = charDefinition.footArmour;
                break;
        }

        return currentItemPickUp;
    }
    
    public int GetHealth()
    {
        return charDefinition.currentHealth;
    }

    public int GetMana()
    {
        return charDefinition.currentMana;
    }

    public int GetDamage()
    {
        return charDefinition.currentDamage;
    }

    public int GetWealth()
    {
        return charDefinition.currentWealth;
    }

    public int GetEncumbrance()
    {
        return charDefinition.currentWealth;
    }

    public float GetResistance()
    {
        return charDefinition.currentResistance;
    }


    #endregion

    #region LevelUp

    // How is this Method called? It need to be called once the Level Threshhold has been surpassed!
    // This Method is only going to be a Wrapper! Das hätte ich wissen sollen, dass es hier nur ein Wrapper ist, wie bei den anderen Methoden
    // in dieser Klasse. Man sieht auch warum. Man muss sehr viel charDefinition schreiben!
    public void IncreaseXP(int XPAmount)
    {
        charDefinition.GiveXP(XPAmount);
    }

    public void CharLevelUp()
    {
        charDefinition.SetCharacterLevel(charDefinition.charLevel);
    }

    #endregion

    #region NPC Stats Initializers

    public void SetInitialHealth(float multiplicator)
    {
        // We can make this here together as we cant the Goblins to be full health at the beginning!
        float charHealth = charDefinition.maxHealth;
        charHealth *= multiplicator;
        charDefinition.maxHealth = (int)charHealth;
        charDefinition.currentHealth = (int)charHealth;
    }
   
    public void SetInitialResistance(float multiplicator)
    {
        float charResistance = charDefinition.baseResistance;
        charResistance *= multiplicator;
        charDefinition.baseResistance = (int)charResistance;
        charDefinition.currentResistance = (int)charResistance;
    }

    public void SetInitialDamage(float multiplicator)
    {
        float charDamage = charDefinition.baseDamage;
        charDamage *= multiplicator;
        charDefinition.baseDamage = (int)charDamage;
        charDefinition.currentDamage = (int)charDamage;
    }

    /*
    public void SetInitialDamagae(int level)
    {
        charDefinition.charLevelUps[level].
    }
    */

    #endregion
}
