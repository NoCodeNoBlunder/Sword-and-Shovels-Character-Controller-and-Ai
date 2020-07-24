using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedApplyForce : MonoBehaviour, IAttackable
{
    public float forceToAdd;
    Rigidbody rbody;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    public void OnAttack(GameObject attacker, Attack attack)
    {
        Vector3 forceDirection = (transform.position - attacker.transform.position);
        // We add a little upwards force!
        forceDirection.y += 0.5f;
        // We can also normalize our Vector by using Normalize!
        forceDirection.Normalize();
        rbody.AddForce(forceDirection * forceToAdd);
    }
}
