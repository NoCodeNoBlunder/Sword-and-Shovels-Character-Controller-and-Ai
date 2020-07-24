using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructedPlayer : MonoBehaviour, IDestructable
{
    public event Action OnPlayerDeath;

    public void OnDestruction(GameObject destroyer)
    {
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }
}
