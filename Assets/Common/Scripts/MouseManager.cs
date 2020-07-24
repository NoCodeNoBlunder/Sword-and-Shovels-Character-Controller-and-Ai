using UnityEngine;
using System;
using UnityEngine.Events; // New NameSpace
using System.Drawing;

#region Explanation
// The MousManager simply switches the Cursor type based on where the mouse is on pointing at!
// Also it has to contain the info where we clicked at which is stored inside the hit variable!

// In this project it is alos responsible for character movement.
#endregion

public class MouseManager : MonoSingleton<MouseManager>
{
    #region Fields Declarations

    // ???
    // This will give us a publicly accessible event in the Inspector to which we can connect our Nav Mesh. This can only be done with this
    public EventVector3 OnClickEnvironment;

    public Events.EventGameObject OnClickOnAttackable;

    // This makes the Rays only to be castin in this layer! look Physics.Raycast!
    // -We acctually run into a problem with this with our UIManager and ItemPickUp System we can only 
    // open the chest when the UI is set off!

    // With this we can assign a layer in the Inspector it will show all avaiable ones automatically!
    public LayerMask clickableLayer; // layermask used to isolate raycasts against clickable layers

    public Texture2D sword; // Attack mouse pointer
    public Texture2D pointer; // normal mouse pointer
    public Texture2D target; // target mouse pointer
    public Texture2D doorway; // doorway mouse pointer

    private bool useDefaultCursor = false;

    #endregion

    private void Awake()
    {
        // We add this conditional to be able to run our main scene without the boot scene!
        // Cause we want to work only on the Main Scene for now. Even tho later in a finished product the Boot Scene will always load first.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState previousState, GameManager.GameState currentState)
    {
        #region My inferior Sollution
        /*
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
        {
            useDefaultCursor = true;
        }
        */
        #endregion

        // Logic Statement das ist kein ternary Operator das muss ich lernen und beherschen.
        // Mit Datentypen bool kann man solche advanced logic assignment tätigen. Mit andern Datentype ist dafür ein
        // ternary Operator benötig.
        useDefaultCursor = (currentState != GameManager.GameState.RUNNING);
    }

    void Update()
    {
        #region My inferiot Sollution 2
        // We only want to Change the Cursor based on what we hover if the Game is Running!
        /*
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
        {
            Cursor.SetCursor(pointer, new Vector2(16,16), CursorMode.Auto);
            return;
        }
        */
        #endregion

        // What is the Vector2 of a Cursor? It is not world Space!
        Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);
        if (useDefaultCursor)
        {
            return;
        }

        RaycastHit hit;
        // The Ray will only detect objects which are in the specified layer/layers!
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, clickableLayer.value))  
        {
            #region Change Cursor Texture

            //Override Cursor: This Cursor is overridden unless noone of the conditions come true!
            Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);

            bool door = false;
            if (hit.collider.gameObject.tag == "Doorway")
            {
                Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                door = true;
            }

            // Funktioniert nicht!
            //bool chest = false;
            if (hit.collider.gameObject.tag == "Chest")
            {
                Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                door = true;
            }

            // If the object that is hit by the ray implements the IAttackable interface is it attackable!
            //-Alternative zu GetComponent<T>(); ist GetComponent(typeof(T));
            bool isAttackable = hit.collider.gameObject.GetComponent<IAttackable>() != null;
            if (isAttackable)
            {
                Cursor.SetCursor(sword, new Vector2(16, 16), CursorMode.Auto);
            }

            #region Verbesserung vom Code REPLACED
            // Das war die Vorherige Lösung die wir jetzt mit einem Ovverride Cursor verändert haben
            /*
            else
            {
                Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
            }
            */
            #endregion

            #region Komment How we communicate with the Nav Mesh Agent!

            // If this evaluated to true we want to comunicate the hitpoint in the environment to the Nav Mesh Agent.
            // To Do this we are going to create a Custom Event that will allow us the send this information out WITHOUT creating a direct connection to the Nav Mesh Agent!
            // You may be accustomed to other scrpting workflows which create public fields within a class which allow the connection of specific components in the Scene.
            // While this is effective it does create a dependencie which can limit the flexibilty of Systems and can make things more difficult to maintain. We are going to 
            // create a Custom event which will allow us to send the position information out of the script which will give us an option to connect the Nav Mesh Agent via
            // that event rather than connecting it via field. In order to do that we need to access a new namespace "UnityEngine.Events". By default unity event can invoke
            // 0 argument Methods. In our case we are going to have to send information out so we are going to have to creata custom ovverride for an event. Do do this we drop
            // below the class and add a new  custom class?? Now that we addes this it will give us the ability to send Vector 3 information through an event. 


            //Das ist veraltete
            // Informationen. Mitlerweile kann Information durch ein Event weitergereicht werden ohne eine Custom Klasse!
            // Was ist der Unterschied zwischen einem event und einem Event???????? Ein event Action ist nicht Visible im Inspector während eine Event im Inspector visible ist 
            // Wenn es public ist. Und dann können dem Event im Inspecor Methoden zugewiesen werden!

            #endregion

            #endregion

            #region Input Cursor
            // We could use an FSM for this!

            if (Input.GetMouseButtonDown(1))
            {
                // Clicked on Doorway
                if (door)
                {
                    // We dont say hit.collider.gameObject.transform.position cause this returns a Vector3 but we are looking for a Transform so we only say 
                    // hit.collider.gameObject.transform

                    // Remember a transform hold information about a gameObjects position, Rotation and scale!
                    Transform doorWayPos = hit.collider.gameObject.transform;

                    #region Komment Doorway

                    // We want to make the Character move passed the doorway and the doorways forward direction is its Z axis. The Doorways colliders are both setup in a way
                    // That the T axis always point in the right direcion. Forward is a normalized vector of lenght 1. Therefore we need to multiplay. Otherwise the player
                    // Would only move 1 unit into the doorway which would not be enought!
                    // We can add 2 Vectors3 together!
                    OnClickEnvironment?.Invoke(doorWayPos.position + doorWayPos.forward * 10);

                    #endregion
                }
                else if (isAttackable)
                {
                    GameObject attackable = hit.collider.gameObject;
                    // We Subscribe to this Method via Inspector. Downside: It wont be shown in the references what methods are subscribed!
                    OnClickOnAttackable.Invoke(attackable);
                }
                // Clicked on Environment. Observation: If we had a lot of more different types of objects to click on it would make sense to use a FSM for this!
                else
                {
                    // Invoke the OnCLickEnvironment event passing it the Position the mouse CLicked on!
                    OnClickEnvironment?.Invoke(hit.point);
                }
            }
            #endregion

        }
        // Ich hab das hier ausversehen removed und wundere mich dann das nichts geht
        /*else
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }*/
    }
}

// New!! we create a Custom override for an event
// This will allow us to send Vector 3 information trough an event!
[Serializable] // We use this Attribute so our Event is visible in the Inspector!
public class EventVector3 : UnityEvent<Vector3> { }



