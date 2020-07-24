/*using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // We need to include this namespace in any GameManager!
using UnityEngine;

// In this Scene "Boot" we will only have data that is "persistent data". Which means data which is needed for every Scene in the Game.
// Since we are not goint to have any gamePlay elements in this game we remove any gameObject including the Camerea and only have empty gameObjects which hold 
// the Manager scripts. 


// There is only one GamaManager per Game!

// Now with extending the MonoSingleton class there 
namespace Komment.GameManager
{
    public class GameManagerKomment : MonoBehaviour         // : MonoSingleton<GameManagerKomment> damit die Klasse funktioniert.
    {
        #region base Requirements of a GameManager

        // What level the game is currently in
        // load and unload game levels
        // keep track of the game state. Wether or not other systems are allowed to continue working. (There can only be one active state at a given time siehe FSM)
        // Generate other persistent Systems. So when we need to make other things in the future, we dont need to make changes to the GamaManager to continue supporting them

        #endregion

        #region Komment
        // We make this field private cause we only want to GameManager to be able to set the current Lvl
        // New string.Empty; Which means the container wont have anything in it not even Null
        // The reason we are usingn a sring is because there is a couple different ways to for Unity to keep track of scenes data.
        // Unity can access scene data by accessing it by its index or its name(string). The index is shown in the "Build list".
        // The problem with the index is that as we add new scenes the indexs of allready existing scenes might change. This will cause problems. 
        // As the order might change, we could add or remove scenes so using the index is not a very robust sollution.
        // Therefore we are always going to keep track of our scenes by names so we dont have to deal with this issue.
        #endregion

        #region Fields

        // This Array will consists of all the Systems that the GameManager needs to keep track of and live inside the Scope of the Boot Scene. 
        // It contains all our potential Prefabs wether or not they are allready created. There is LoadTimes keep that in mind. They wont be created instantly.
        // It will be populated via the Inspector and also keep things organized.
        // All Systems will be at one place. On first sight this may seem stupid why are we having an array
        public GameObject[] systemPrefabs; // Lists should always be initialized while arrays must not be initialized.

        // This List will contain only the Systems which have been instantiated(after they been created) and we want to keep track of. 
        private List<GameObject> instancedSystemPrefabs;

        // We want to keep track of how many Async operations are happening at any given time. Because if we load a hole bunch of levels we need to know when 
        // they are all done. So that we know when we can move on with other things, have a loading screen or some transition. Maybe we want to prevent more than one level 
        // from being loaded at the same time.
        private List<AsyncOperation> loadOperations;

        // string.Empty is litterally an empty string not even null. Default also returns null!
        private string currentLevelName = string.Empty;

        #endregion

        // An ovverride Method has to match the base Methods access modifier.
        protected override void Awake()
        {

        }

        private void Start()
        {   // This is when the GameManager will start up which will only happend once.

            #region Make 100% sure the GameManager wont be unloaded
            // To make absolutely sure that the GameManager will not be destroyed we make use of Unityies Method DontDestroyOnLoad.
            // It should not happend anyway cause we are never unloading the Boot Scene but to make 100% we do it regardless. IT can only happend when someone makes a mistake.
            // We pass it in this gameObject which carries the GameManager!
            // This object is now marked and wont be destroyed on any Scene Load! We dont need to call DontDestroyOnLoad as a gameObject is destroyed it works like a marker.
            // We can call it as soon as a gameObject is created even if it gets destroyed way later.
            #endregion
            DontDestroyOnLoad(gameObject);

            instancedSystemPrefabs = new List<GameObject>();
            loadOperations = new List<AsyncOperation>(); // We initialize the List.

            InstantiateSystemPrefabs();

            LoadLevel("Main");
        }

        #region Methods

        #region Why are we wrapping these SceneManager calls inside our own Methods
        // These Methods are going to contain the logic to interfacing with Unityties Scene Management System and also tracking what the GameManager need to track what the 
        // GameManager needs to track. 

        // You might be wondering why are we "wrapping" our SceneManager calls inside if our own Methods. The reason is because the rest of the game might want to
        // know when scenes load are happening and when Sceneloads are complete. So we are going to have our GameManager keep track of these events. To do this
        // We need to know about the AsychronusLoad Object. Because if it happends asynchronosly we are not going to know when the Scene is finished loading.
        // If the Scene is very big it might take a while, if the Scene is very small it might happend very quickly.
        #endregion

        private void InstantiateSystemPrefabs()
        {
            GameObject prefabInstance;
            for (int i = 0; i < systemPrefabs.Length; i++)  // We could have used a foreach loop instead. Lets use a for loop here.
            {
                #region Instanitation and keeping Track of systemsPrefabs Kommentar
                // Wir instantiaten die gameObjekt Prefabs an die unsere Systeme attached sind.
                // Whenever you instantiate something you need a referenc to it cause you always need to perform some action on the instantiated gameObject rather than all the Prefabs.

                // That allows us to both create the Object and the GameManager can keep track of them. The reason why we want to keep track of them is to be able to clean them up
                // later on. It is generally the responsibility of whoever creates something to make sure that that thing is also cleaned up at some point. 
                // If it isnt cleaned up then we could have memory leaks access objects dangling in our hierarchy and lead to other problems that are hard tp bebug.
                // If for instance you need to cleanup the GameManager and all presistend Systems you would want to do that in one centralized location instead of having to ask each 
                // system individually to clean themselfes up. To clean up we are going to use our MonoSingleton Method OnDestroy.
                #endregion
                prefabInstance = Instantiate(systemPrefabs[i]);
                instancedSystemPrefabs.Add(prefabInstance);
            }
        }

        private void OnLoadOperationComplete(AsyncOperation ao)
        {   // This is the Method we want to execute when this particual scene load is finished. So can can notify other Systems once it is done.

            if (loadOperations.Contains(ao))
            {
                loadOperations.Remove(ao);

                // Transition between Scenes
                // dispatch messages.
                // Anything that we decide we wanna do at the end of a scene load.
            }

            Debug.Log("Load Complete.");
        }

        private void OnUnLoadOperationComplete(AsyncOperation ao)
        {   // This is the Method we to execute when this particular scene unload is finished. So we can notify other Systes once it is done.
            Debug.Log("Unload Complete.");
        }

        public void LoadLevel(string levelName)
        {
            #region SceneManager internal Methods
            // As said earlier we can access any Scene by its index or its name or by the Path with "GetSceneBy...()"
            // We can also call load on these scenes with ´"Load Scenes" and "LoadScenesAsync". The difference between these 2 is that is wether or not the 
            // call is going to be whats called a "blocking call". If we call LoadScene this is a blocking call. This means untill i am done doing my operations
            // everyone else needs to wait (Like any other Method that is not a Couroutine right???). LoadScene is something you would use for something that you want to
            // garantee happening before everything else. But the problem with this is that if you call LoadScene then everything else has to wait and that might 
            // mean there is a stutter or a hidge or something like that happening between the loads of the Scenes. Instead we can use LoadSceneAsync which means 
            // its going to try to let other things happend in the background at the same time that the load is occuring. 

            // The LoadSceneAsync Method return an Object
            // called an "Asynch Operation". We are going to receive that Asynch operation by creating a new variable of type "AsynchOperation" which we will name ao which 
            // we will set equal to the return value of LoadScenesAsync. An Asynch Operation knows about everything that is happening inside of its operation and
            // contains a couple events. One of these events is the "completed" event. C# Actions are kind of like Unity events but instead of calling 

            // LoadSceneAsync also has the option so specify adding scenes additivly. We want the Boot Scene to run for the entire duration of the game.
            // Therefore we want this Scene to be running at all times so when we load a new scene we need to load it Additively which means any existing scenes wont be 
            // wiped as the new Scene is loaded. We have to remove them manually now using UnLoadLevel! Once we implement this we have 2 Scenes loaded at the same time. 
            #endregion
            AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            // To make sure that 
            // When ao is null it means that the Scene we want to load does not exist. We need to debug an error message when this happens!

            if (ao == null)
            {
                // We Specify where the Error is coming from with [Game Manager]. Good for Debugging!
                Debug.LogError("[Game Manager] Unable to unload level " + levelName);
                return; // If this if statement is entered the method ends here.
            }

            // completed is a Unity event which gets fired/ executed when the LoadSceneAsync Method is finished. We can have other Methods subscribe to that event.
            ao.completed += OnLoadOperationComplete;
            loadOperations.Add(ao);
            currentLevelName = levelName;   // When we load a level we want to set this variable.
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

        // -Protected in this case means that this Method can only be accessed by 
        // Classes that inherit from MonoSingleton!

        // We are extending this Method. Here we are destroying our Systems when the Game shuts down for instance.
        // It is a centralised location where we destroy all Systems.
        protected override void OnDestroy()
        {
            base.OnDestroy(); // We have to keep the base!

            #region Alternative Sollution using foreach loop.

            /*foreach (var system in instancedSystemPrefabs)
            {
                Destroy(system);
            }
            #endregion
            for (int i = 0; i < instancedSystemPrefabs.Count; i++)
            {
                Destroy(instancedSystemPrefabs[i]);
            }

            // Das cleared nur die Liste es Zerstört jedoch keine Objekte ich retard! When things are cleaned up we also want to freeup our references.
            instancedSystemPrefabs.Clear();
        }

        #endregion

        #region Summery

        // Now we have a System in place that will allow us to generate other persistent Systems. When we want to add other Systems we can do that via the Inspector.
        // By adding them to our Systems Array. And they will automatically be instaniated referenced and if needed destroyed!

        // We have to add the Systems Prefabs and not their instances.

        #endregion
    }

}*/
