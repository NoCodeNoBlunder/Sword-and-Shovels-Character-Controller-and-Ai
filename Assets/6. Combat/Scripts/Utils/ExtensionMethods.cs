using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Explanation
// This is called an ExtensionMethods it will contain "Extension methods".
// What are extension methods? They extend other Methods right?
#endregion
public static class ExtensionMethods
{
    // Was wenn diese Methode nicht static wäre?#
    // NEU: Parameter(this Transform attacker) this indicates which type will be extended. In our case it is going to be 
    // the Attacker which calls this Method who will get passed in automatically as the attacker for this Method.
    // We will only need to pass in the deffender!

    // This is a constant!
    public static float dotThreashhold = 0.8f;
    public static bool IsFacingTarget(this Transform attacker, Transform deffender)
    {
        // We need to determine if the attacker is facing the deffender
        //return Mathf.Approximately(), 1);
        return (Vector3.Dot(attacker.forward, (deffender.transform.position - attacker.transform.position).normalized) >= dotThreashhold);
    }
}
