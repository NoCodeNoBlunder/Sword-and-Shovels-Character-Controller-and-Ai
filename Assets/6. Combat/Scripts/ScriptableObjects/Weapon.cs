using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 
// Explanation: This SO will extend AttackDefinition and define each weapon in the game!
// It Contains all the data that is contained in AttackDefinition and in addition it contains its own data!
#endregion

[CreateAssetMenu(fileName = "Weapon.asset", menuName = "Attack/Weapon", order = 0)]
public class Weapon : AttackDefinition
{
    // This will hold a reference to the physical representation of our weapon
    public Rigidbody weaponPrefab;

    // This weapon will simulate swinging the weapon at the deffender. 
    // We have to make sure the deffender is in front and in range of them attacker.
    public void ExecuteAttack(GameObject attacker, GameObject deffender)
    {
        // We dont always need to use brackets especilly for easy conditional like this.
        if (deffender == null)
            return;

        if (Vector3.Distance(attacker.transform.position, deffender.transform.position) > range)
            return;

        if (!ExtensionMethods.IsFacingTarget(attacker.transform, deffender.transform))
            return;

        // At this point this is a valid Attack!
        // We want to do these Operations only if the attack is valid. Ich retard!
        var attackerStats = attacker.GetComponent<CharacterStats>();
        var deffenderStats = deffender.GetComponent<CharacterStats>();

        var attack = CreateAttack(attackerStats, deffenderStats);

        // Why we have to say GetComponent"s"InChildren with an s plural???????
        // An object can implement multiple IAttackable behaviours!
        var attackables = deffenderStats.GetComponentsInChildren<IAttackable>();

        foreach (var attackable in attackables)
        {
            attackable.OnAttack(attacker, attack);
        }
    }
}
