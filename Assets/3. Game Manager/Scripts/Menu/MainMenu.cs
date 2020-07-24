using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Summery
// This script will be used to controll the actual main menu gameObject, trigger its animations and also receive those events from those animations to know when they are
// finished. 

// Track the Animation Component
// Track the AnimationClips for fade in fade out
// Function that can track animation events.
// Functions to play Fade in and Fade Out Animation
#endregion

#region Access modiferts Komment
// when you dont explicitly specify an access modifer it by default is the most restrictive one. Which for a field is always private.
#endregion
public class MainMenu : MonoBehaviour
{
    #region Fields

    // We need to notify the GameManager that the FadeIsComplete since the MainMenu is not globally accessible the best practice sollution is to 
    // propagate(verbreiten) the information upwards. First to the Ui Manager and then to the GameManager
    public Events.EventFadeComplete OnMainMenuFadeComplete;

    #region Komment
    // We want these to be private so no other classes/systems can access them. Which is good on our case. When they are private they are not visible in the Inspector.
    // To change that we can add a an [System.Serializable] attribute/ also refered to as a "decorater"
    // We want to make these field private because noone else needs to know about the animationclips those are all in the domain of the MainMenu script. We dont
    // want other scripts to be able to change this things so we make them private. To still make these field visible ih the Inspector we can use a Decorator
    //[SerializeField] which enables us to set the variables in the Inspector. To not get an error display we assign them null!
    #endregion
    [SerializeField] Animation mainMenuAnimator = null;
    [SerializeField] AnimationClip fadeOutAnimation = null;
    [SerializeField] AnimationClip fadeInAnimation = null;

    #region Komment OnFadeOutComplete() and OnFadeInComplete()
    // These Method will be called automatically once the animations are complete!
    // To have a reference to the events we simply need a Method which matched their name? Seems like it lol????
    #endregion
    public void OnFadeOutComplete()
    {
        Debug.Log("FadeOut Complete.");
        OnMainMenuFadeComplete?.Invoke(true);
    }

    public void OnFadeInComplete()
    {
        Debug.Log("FadeIn Complete.");

        // We need to inform other Systems that the FadeIn is Complete! We can do this by using another Event
        // We want to turn the dummy Camera on only when the Fade in is complete.
        OnMainMenuFadeComplete?.Invoke(false);

        UIManager.Instance.SetDummyCameraActive(true);
    }

    #endregion

    #region Initialization

    private void Start()
    {
        #region Register for Events
        // A good place to register for an event is when an Object is created.
        // AddListener has to be used with UnityEvents instead of += which is used with delegates and Actions.
        // Beim Subscriben werden keine Parameter benötigt. Erst wenn das Event eintritt und executed wird
        #endregion
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    #endregion

    #region FadeIn and FadeOut

    public void FadeOut()
    {
        // Before we start the FadeOut we want to turn off our dummy Camera
        UIManager.Instance.SetDummyCameraActive(false);

        #region Komment on Animator
        // Before playing anything we want to make sure there is no Animation currently being played. It is best practice to do so.
        // We can assign the Animation its default animation with "someAnimation.clip = someAnimationClip;" 
        // the Animator always plays its default clip with someAnimator.Play();
        #endregion
        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeOutAnimation;
        mainMenuAnimator.Play();
    }

    public void FadeIn()
    {
        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeInAnimation;
        mainMenuAnimator.Play(); 
    }

    #region How to convert to FSM
    // The Method itself that gets Invoked by the Event need to check what parameters get passed to the invoked event to make sure the right response
    // is fired when the event is raised. Das ist wo ich verkackt habe! The Method we call matched the Singnature of the Event for a reason!
    // They are also not using switch statement cause there is 2 factors that need to be checked.
    // I can use FSM with if statements also!
    #endregion
    private void HandleGameStateChanged(GameManager.GameState previousState, GameManager.GameState currentState)
    {
        // We only want the Fadeout to be called when we are transitioning from the Pregame to the Running state!
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }

        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
            FadeIn();
        }
    }

    #endregion

    #region Initialization Not used here

    // Could we use GetComponent instead of referencing everything in the Inspector?

    //void Start()
    //{
        // We dont need to grab the Component because we are referencing it via the Inspector!
        //mainMenuAnimator = GetComponent<Animation>();
        /*fadeOutAnimation = GetComponent<AnimationClip>();
        fadeInAnimation = GetComponent<AnimationClip>(); */
    //}

    #endregion
}
