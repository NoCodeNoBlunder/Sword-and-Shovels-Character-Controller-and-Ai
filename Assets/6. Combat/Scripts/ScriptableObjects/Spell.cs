using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Explanation
// This script will define our Spells and will be spawning our Projectiles
#endregion

// Die order können wir auch weglassen!
[CreateAssetMenu(fileName = "Spell.Asset", menuName = "Attack/Spell", order = 0)]
public class Spell : AttackDefinition
{
    // Since we are extending from AttackDefinition we allready have the Cooldown, Range and Damage fields!

    // We will attach the Prefab which holds the Projectile behaviour in the Inspector! We dont need to attach the GameObject Prefab!!
    public Projectile projectileToFire;
    public float projectileSpeed;

    #region Komment CastProjectile()   
    // This method will be called from the Behaviour that acctually casts the Spell. It will need to have access to all the information
    // That is needed fore Projectile.Fire and information it needs itself like hotSpot where the Projectile should be spawned!
    // -eine Methode hat Zugang zu allen Fields der Klasse und den Parametern die ihr übergeben werden. Diese Information kann die 
    // Methode dann entsprechend verarbeiten. Zuerst muss aber dafür gesorgt werden, dass die Methode alle Informationen 
    // zu verfügung hat die sie benötigt!

    // It will get passed a layer which is going to be the Layer we want the Projectile to be on.
   // Why? So the Projectiles dont collide with each other and also so EnemyProjectiles dont hit eniemies but only the player and vice versa.
    #endregion
    public void CastProjectile(GameObject caster, Vector3 hotSpot, Vector3 targetPosition, int layer)
    {
        // We can also Instantiate Objekts(Classes) not only gameObjects!
        // Das interessante ist, dass wir ein typ Projectile spawnen aber dadurch automatisch auch das Prefab gespawned wird, welched dieses
        // Behaviour als Componente trägt.
        Projectile projectile = Instantiate(projectileToFire, hotSpot, Quaternion.identity);

        projectile.Fire(caster, targetPosition, projectileSpeed, range);
        // each gameObject has a layer property which stores on which layer a gameObject is.
        projectile.gameObject.layer = layer;

        // We want to listen to the OnProjectileCollision event!
        projectile.OnProjectileCollision += HandleProjectileCollided;
    }

    // This Method will be Called when the Projectile connected with the Target!
    // Es beinhaltet 3 GetComponent calls was teuer ist kann man das umgehen?
    private void HandleProjectileCollided(GameObject caster, GameObject target)
    {

        // ASSERTION: The attack was successfull the target had been hit!
        // -> Create an Attack and attack the target!

        // To be on the save side dont forget a nullcheck as one or both of the characters involed here (caster, target) might have died 
        // during the flight time of the Projectile!
        if (caster == null || target == null)
            return;

        // Variables dont always need to be declared in the head of methods!
        // Besonder in einem Fall wie hier wenn die Damage Calculation erst stattfindet, wenn der attacker und der verteildiger noch leben!
        // und somit nicht Null sind!
        var casterStats = caster.GetComponent<CharacterStats>();
        var targetStats = target.GetComponent<CharacterStats>();

        Attack attack = CreateAttack(casterStats, targetStats);

        var attackables = target.GetComponentsInChildren<IAttackable>();

        foreach (var a in attackables)
        {
            a.OnAttack(caster, attack);
        }
    }
}
