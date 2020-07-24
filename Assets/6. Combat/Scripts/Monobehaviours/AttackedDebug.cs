using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class AttackedDebug : MonoBehaviour, IAttackable
{
    // The Attacker is the GameObject that attacks and the attack is the individual attack that was created
    public void OnAttack(GameObject attacker, Attack attack)
    {
        if (attack.IsCritical)
        {
            Debug.Log("CRITICAL DAMAGE!");
        }

        #region Debug.LogFormat
        // Debug.LogFormat can be used instead of Debug.Log to save some time!
        // Basucally we write our our string and we can also include variables as arguments which will be displayed in the correct positon
        //- name returns the name of the gameObject! if no gameObject is specified it returns the name of the gameObject that holds this script!
        #endregion
        Debug.LogFormat("{0} attacked {1} for {2} damage.", attacker.name, name, attack.Damage);
    }
}
