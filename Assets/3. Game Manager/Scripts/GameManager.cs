using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine;

#region Komment
// We make this field private cause we only want to GameManager to be able to set the current Lvl
// New string.Empty; Which means the container wont have anything in it not even Null
// The reason we are usingn a sring is because there is a couple different ways to for Unity to keep track of scenes data.
// Unity can access scene data by accessing it by its index or its name(string). The index is shown in the "Build list".
// The problem with the index is that as we add new scenes the indexs of allready existing scenes might change. This will cause problems. 
// As the order might change, we could add or remove scenes so using the index is not a very robust sollution.
// Therefore we are always going to keep track of our scenes by names so we dont have to deal with this issue.
#endregion
public class GameManager : MonoSingleton<GameManager>
{
    #region Fields

    // We want this to be public so other Systems can access the gamestate too. Nameing convention for enum values is all CAPITAL LETTERS.
    public enum GameState
    {
        PREGAME, RUNNING, PAUSE
    }

    private GameState currentGameState = GameState.PREGAME;

    // This "Property" is called "Accessor"
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        private set { currentGameState = value; }
    }

    // We are accessing our EventGameState from our Events class container!
    public Events.EventGameState OnGameStateChanged;

    public GameObject[] systemPrefabs;

    private List<GameObject> instancedSystemPrefabs;
    private List<AsyncOperation> loadOperations;
    //private GameState currentGameState = GameState.PREGAME;

    private string currentScene = string.Empty;

    // Private Accessor for the Hero!
    private HeroController heroController;
    private HeroController hero
    {
        get
        {
            #region Lazy Inititialization Explained. 
            // Lazy Initialization! Das funktioniert nur weil wir nur eine Instanz vom Spieler haben!

            // You might be wondering why dont we just set the heroController in Start. The Problem with this approach is that
            // The GameManager will exists and Start before the Hero exists leaving the value null!
            // We could also use FindObjectOfType everytime we need the HeroController but that is a very expansive call!
            #endregion
            if (heroController == null)
                heroController = FindObjectOfType<HeroController>();  

            return heroController;
        }
    }

    #endregion

    #region Initialization

    protected override void Init()
    {
    
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        instancedSystemPrefabs = new List<GameObject>();
        loadOperations = new List<AsyncOperation>(); // We initialize the List.

        InstantiateSystemPrefabs();

        // We can register for other Systems events after they are created which happends with InstantiateSystemPrefabs. Order!
        UIManager.Instance.OnMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
    }

    #endregion

    private void Update()
    {
        if (Instance.CurrentGameState != GameState.PREGAME)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Instance.TogglePause();
            }
        }
    }

    #region Load/Unload Scenes

    public void LoadLevel(string levelName)
    {
        Debug.LogWarning("Scene has to be loaded");
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        // We want to make sure that the requestedload level exists. So we check if ao == null when it equals null the requested scene does not exist.
        if (ao == null)
        {
            Debug.LogError("[Game Manager] Unable to load level " + levelName);
            return;
        }

        ao.completed += OnLoadOperationComplete;
        loadOperations.Add(ao);
        currentScene = levelName;

        //OnLoadOperationComplete(null);
    }

    private void OnLoadOperationComplete(AsyncOperation ao)
    {
        // We check wether the ao to remove exists in the list alleady to make sure nobody is calling this Method who is not supposed to!
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);
        }
       
        if (loadOperations.Count == 0)
        {
            Debug.Log("Load Complete.");

            // We only switch our GameState when the Scene needed for the State is done loading!
            UpdateState(GameState.RUNNING);

            // Mein Workaround!
            // We can only set a Scene active when it is done loading!
            switch (currentScene)
            {
                case "Main":
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));
                    break;
                default:
                    break;
            }
        }
    }

    public void UnLoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);

        if (ao == null)
        {
            Debug.LogError("[Game Manager] Unable to unload level " + levelName);
            return;
        }

        ao.completed += OnUnLoadOperationComplete;
    }

    private void OnUnLoadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");

        UpdateState(GameState.PREGAME);
    }

    private void HandleMainMenuFadeComplete(bool isFadeOut)
    {
        // Once the FadeOut is complete we can sacely unload any Scene because it is no longer visible.
        if (!isFadeOut)
        {
            UnLoadLevel(currentScene);
        }
    }

    #endregion

    #region States

    // We are first chaning the State and then we make the action that needs to take place as the State is changed!
    // Only the GameManager is calling this Method directly all other systems only call Method that call this Method (Encapsulation)
    private void UpdateState(GameState state)
    {
        // Before we override our CurrentGameState we need to store it as our previous State
        GameState previousGameState = CurrentGameState;
        CurrentGameState = state;

        // We likely want to react to what the incoming state is.
        switch (CurrentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1;
                break;
            case GameState.RUNNING:
                Time.timeScale = 1;
                break;
            case GameState.PAUSE:
                Time.timeScale = 0;
                // No longer needed!
                //UIManager.Instance.gameObject.SetActive(true);
                break;

            default:
                break;
        }

        // Invoke is used to distinguish between methods and events. You can only Invoke events using Invoke!
        // This Current GameState is after we switched so it basically was our last gamestate!
        // We need to pass the Event which state we are coming from and what state we want to switch to.
        OnGameStateChanged.Invoke(previousGameState, CurrentGameState);
    }

    #endregion

    #region Public Method to Change State/Scene

    // These Methods allow other Systems to change States and Scenes indirectly. But never directly. Encapsulation!
    // Das hatte ich aich falsch gemacht meine States konnent direct die States verändern!
    public void StartGame()
    {
        LoadLevel("Main");
        // We need to set the current scene as our active scene as objects always spawn in the active scene and we want.

    }

    #region Komment TogglePause
    // This is only going to get called when we are in Pause or Running state therefore it can be setup as a toggle!

    #region Tasks For PauseState
    // Add method to enter and exit pause
    // Trigger via "Ecape" key
    // Trigger via pause menu
    // Pause simulation when in pause state
    // Modify cursor to use pointer when in pause state
    // Display Pause Menu UI
    #endregion

    // Toggle Pause has no Transition we only have a Transition when we are switching between States but not when we are switching between 
    // States. When we press space we instantly want to be in the Pause menu!
    #endregion
    public void TogglePause()
    {
        // tenranry Operator in practice here we need it because we need to return something that is not a bool based on a condition!
        UpdateState(CurrentGameState == GameState.RUNNING ? GameState.PAUSE : GameState.RUNNING);
    }

    #region Komment RestartGame()
    // Task: Unload Current Scene
    // When the Scene in unloaded turn on dummy camera and switch to PreGame state
    #endregion
    public void RestartGame()
    {
        // Bevor wir unsere Scene unloaden muss der FadeIn completed sein wtf!
        UpdateState(GameState.PREGAME);
    }

    // Wir müssen keine Systeme zerstören, da dass automatisch OnDestroy gemacht wird!
    public void QuitGame()
    {
        // Implement features for quitting the game. eg. Autosave
        Debug.Log("Quit Game");
        Application.Quit();
    }

    #endregion
   
    #region Instantiate/Destroy Systems

    private void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;

        for (int i = 0; i < systemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(systemPrefabs[i]);
            instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    #region EventHandlers/ CallBacks

    public void HandleHeroLevelUp(int newLevel)
    {
        Debug.LogFormat("The Hero is now Level {0} !", newLevel);
        UIManager.Instance.UpdateUnitFrame(hero);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.LEVELUP);
    }

    public void HandleHeroGainedHp(int currentHealth)
    {
        UIManager.Instance.UpdateUnitFrame(hero);
    }

    public void HandleHeroTakeDmg(int currentHealth)
    {
        UIManager.Instance.UpdateUnitFrame(hero);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.HEROHIT);
    }

    public void HandleHeroDeath()
    {
        UIManager.Instance.UpdateUnitFrame(hero);
        UIManager.Instance.PlayGameOver();
    }

    public void HandleHeroInit()
    {
        UIManager.Instance.InitUnitFrame();
    }

    public void HandleWaveSpawned()
    {
        UIManager.Instance.PlayNextWave();
    }

    public void HandleHeroWin()
    {
        UIManager.Instance.PlayYouWin();
    }

    #endregion

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Destroys all System Prefabs and clears List.
        for (int i = 0; i < instancedSystemPrefabs.Count; i++)
        {
            Destroy(instancedSystemPrefabs[i]);
        }

        instancedSystemPrefabs.Clear();
    }

    #endregion
}