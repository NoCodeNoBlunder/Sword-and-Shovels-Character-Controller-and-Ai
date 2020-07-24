using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;  // Use this NameSpace to access UI elements

[Serializable]
public class EventPauseState : UnityEvent { }

public class PauseMenu : MonoBehaviour
{
    // We assign this field in the Inspector but to make them not show an error we initialize them to null
    [SerializeField] Button resumeButton = null;
    [SerializeField] Button restartButton = null;
    [SerializeField] Button quitButton = null;
 
    // Start is only called when a GameObject is active makes sense i am an idiot! So we have to have an active gameObject and turn it off!
    void Start()
    {
        // -When do we need to use GetComponent? When we not referencing the Components by script!
        // We can access the Button click event via onClick and add subscribers.

        // The Pause Menu needs to be active in the hierarchy its HandleResumeClick it subscribed to the event.
        // As Start is only called when an Object is active in the Scene! We can later Set it to inactive!
        resumeButton.onClick.AddListener(HandleResumeClicked);

        // These 2 can only be done via Buttons while resume can also be done via Escape
        restartButton.onClick.AddListener(HandleRestartClicked);
        quitButton.onClick.AddListener(HandleQuitClicked);

        gameObject.SetActive(false);
    }

    // Diese Methode wird gecalled vom Button und triggered das event welched noch subscribed to werden muss!
    public void HandleResumeClicked()
    {
        GameManager.Instance.TogglePause();
    }

    public void HandleRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }

    public void HandleQuitClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
