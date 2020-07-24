using System;
using UnityEngine;
using UnityEngine.Events;

#region Summery
// This class will simply be a container for all our UnityEvents

// Question: should this class not be static as it lives for the duration of the programm and is a helper class???
#endregion
public class Events
{
    #region Komment
    // To make this class visible in the Inspector we have to make it visible and give it the Attribute [Serializable]
    // this allows other Objects to subscribe to the event via the Inspector!
    #endregion
    [Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }
    [Serializable] public class EventFadeComplete : UnityEvent<bool> { }
    // Since there is only 2 types of fades fade in and fade out we pass in a boolean
    [Serializable] public class EventGameObject : UnityEvent<GameObject> { }
    [Serializable] public class EventOnDeath : UnityEvent { }
    [Serializable] public class EventIntegerEvent : UnityEvent<int> { }
}
