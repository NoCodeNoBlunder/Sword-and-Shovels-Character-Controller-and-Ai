using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
// Obv we dont include IDestrucable but we need IAttackable! I am dumb!
public class AttackedTakeDamage : MonoBehaviour, IAttackable
{
    private CharacterStats charStats;

    private void Awake()
    {
        charStats = GetComponent<CharacterStats>();
    }

    // When something gets attacked this Method is called ! Remember that!
    // We can not individualies this Method to our liking!
    public void OnAttack(GameObject attacker, Attack attack)
    {
        charStats.LoseHealth(attack.Damage);

        if (charStats.GetHealth() <= 0)
        {
            // Destroy Object
            // Wir nutzten den Plural, da es sein könnte, das mehrere IDestructable behaviours attached sind!
            var destructables = GetComponents<IDestructable>();

            foreach (var d in destructables)
            {
                d.OnDestruction(attacker);
            }
        }
    }
}
