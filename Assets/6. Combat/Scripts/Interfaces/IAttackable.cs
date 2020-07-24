using UnityEngine;

#region Explanation
// This is going to be the Interface that is going to be implemented by all MonoBehaviours which are attackable.
// Since we multiple different Object that each want to implement being attackable in some way it makes perfect sense
// to use an Interface for this. this way each type of Charcter can have different animations upon taking dmg etc. 
#endregion

public interface IAttackable
{
    // Whenever a target is attacked this Method will be called. Passing who the attacker is and what attack is hitting the target
    void OnAttack(GameObject attacker, Attack attack);
}
