using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region Summery
// The Ui Manager will controll all of the Ui elements and will comunicate with the.
// The UI Manager will decide when to load 
// Properly transition into the main scene.

// The UI Manager will communicate witht the
// The Ui Manaager will also serve as a junction between the Ui Elements and the GameManager. So we dont have a lot of different things communcating with the GameManager
// we can funnel them through this one central manager.
#endregion

public class UIManager : MonoSingleton<UIManager>
{
    #region Fields

    public Events.EventFadeComplete OnMainMenuFadeComplete;

    #region Komment on disabling Warnings for private serialized fields
    // -This disables all Compiler warnings. We obv only want that to be the case for our [SerializeField] fields 
    // so we dont get the error that they have no value assigned to them! Therefore we need to enable the Warning after we declared the fields.
    // - Another sollution is to initiate the fields with a value of null instead. (Siehe MainMenu). Initializing to a value of null seems better tbh in terms of readability.
    #endregion
#pragma warning disable 0649
    [SerializeField] MainMenu mainMenu;
    [SerializeField] PauseMenu pauseMenu;
    // It makes sense to have the dummyCamera be controlled by the UIManager because it contains it.
    [SerializeField] Camera dummyCamera;
#pragma warning restore 0649

    #endregion

    private void Start()
    {
        mainMenu.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChange);
    }

    #region User Input

    private void Update()
    {
        // Conditions cause no FSM
        // These Conditions need to get modifed as more State are added. This is why FSM is better!
        // With FSM the Input is not going to be checked for here but in each individual State!

        if (GameManager.Instance.CurrentGameState == GameManager.GameState.PREGAME)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //mainMenu.FadeOut();
                GameManager.Instance.StartGame();
            }
        }
    }

    #endregion

    // SELF:
    // With FSM sollution this MEthod would still be here but we would only pass the desired GameState as the previous state would be clear based on what 
    // state got the "Input" to change the state!
    public void HandleGameStateChange(GameManager.GameState previousState, GameManager.GameState currentState)
    {
        // -This will also be modified as we are not using an FSM later on!
        //- ternary Operator again. NVM Ich bin garbage wir brauchen gar kein ternary Operator da wir einen bool returnen müssen und das geht auch ohne ternary.
        // Starkes Logic statement wir passen eine Condition als bool argument

        pauseMenu.gameObject.SetActive(currentState == GameManager.GameState.PAUSE);
    }

    // Here we are Propading the Information upwards. This Method is called when the Event OnMainMenuFadeComplete is raised
    // When this happends the UIManager raises its own event which is subscribed to by the GameManager! Which can then make use of this informartion.
    public void HandleMainMenuFadeComplete(bool isFadeOut)
    {
        OnMainMenuFadeComplete?.Invoke(isFadeOut);
    }

    // We create a Method that will allow other Systems to turn the dummy camera on and off!
    public void SetDummyCameraActive(bool isActive)
    {
        dummyCamera.gameObject.SetActive(isActive);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
