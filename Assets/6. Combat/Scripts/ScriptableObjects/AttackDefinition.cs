using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#region Explanation
// This will define class will define our attacks. It will declare variables which will define the details of all attacks in game!

#endregion

// Dont forget to [CreateAssetMenu] attribute otherwise you wont be able to use this SO
[CreateAssetMenu(fileName = "Attack", menuName = "Attack/BaseAttack", order = 0)]
public class AttackDefinition : ScriptableObject
{
    public float cooldown;
    public float range;
    public int minDamage;
    public int maxDamage;
    public float critMuliplayer;
    public float critChance;

    public int AverageDamage
    {
        get { return (minDamage + maxDamage) / 2; }
    }

    // This Method will Create and return an Attack Object. 
    // -It needs information about how is the attacker and who is receiving the damage
    public Attack CreateAttack(CharacterStats wielderStats, CharacterStats deffenderStats)
    {
        float coreDamage = wielderStats.GetDamage();
        // We make our damage increase randomly by a value between minDamage and maxDamage. 
        coreDamage += Random.Range(minDamage, maxDamage);

        // Ist Random.Value kleinern als critchance ist es ein Crit.
        // Das ist nicht trivial und kann leicht falsch gemacht werden aber macht letzen endes sinn wenn man drüber nachdenkt.
        bool isCritical = (Random.value <= critChance);

        if (isCritical)
            coreDamage *= critMuliplayer;

        // We have to check if the deffender acctually exists to avoid errors.
        // Why dont we have to check if the wielder acctually exists?
        // Das ist ein Common check merk dir das jetzt endlich mal?

        if (deffenderStats != null)
        {
            // Armor reduces damage addivitly not multiplicatily in this game!
            coreDamage -= deffenderStats.GetResistance();
        }


        // We create and return a new Attack Object which will define this attack!
        #region We need to cast our float as an int
        // - Since we are calculating damage in datatype int we need to convert our floats to ints!
        // By casting it as int! -> "(int)coreDamage"
        #endregion
        return new Attack((int)coreDamage, isCritical);
    }
}
