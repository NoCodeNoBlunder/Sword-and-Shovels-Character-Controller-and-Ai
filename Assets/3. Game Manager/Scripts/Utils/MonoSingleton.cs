using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

#region Singleton Summery
// Singleton are a type Monobehaviour

// Singleton Pattern: It can only be used for classes which will exist only once. Where there is every only going to be one instance of this class.
// Commonly used for Manager classes as there is ever only going to be one instance of these classes. 
// 1 GameManager, 1 AudioManager etc.

// Templates are ways to pass in types as variables.
// Self: You can have multiple Templates T is simply the convention.

// MonoSingleton extends the class MonoBehaviour. 
// We are saying that we are declaring a class that is public it is accessible anywhere in your project.
// The Singleton is goint to be generic and we will be told what type of Singleton it will be
// The Singleton extends a MonoBehaviour and it is requiring that this particular verasion of Singleton the type that is passed in is an object that is meant to
// extend the Singleton of that same type. What this is goint to do is prevent us from Creating Singletons of arbitrary(willkürlich) objects that are not meant to be 
// Singletons. For instance if someone would create a Singleton of gameObject that would fail because gameObject does not extend the class Singleton. In this case
// if we create a GameManager that properly extends the Singleton, it would fullfile this requirement and thus it would pass sucessfully.

// The next thing we need to do is access this global instance but not change it externally. This is done via a Property.
#endregion

// Andere Version von MonoSingleton. Ich mag die Version
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    // A property with only a get is called an "Accessor".

    // Question: If we do it like this we can use an AutoProperty right with a private Set and public get?
    // Answer: No because not even this Class should be able to modify this value!
    private static T instance;
    public static T Instance
    {
        // I Would like the if statement here more which checks if the instance is null.
        get { return instance; }
    }

    #region Accessor: Checking wether the Singleon has been initialized
    // We can check if our Singleton has been Initialized. This will be helpfull later on if we wanna check wether the instance exists without explicitly checking for null
    // in a lot of places.
    // Another Accessor this time returning a bool. 
    #endregion
    #region Denkfehhler von mir
    // I am retarded a Property always has to be of a certain type it wether it is explicitly or implicitly.
    // eg: bool or T but in can never be void same as a variable can never be void. Only Methods can.
    #endregion
    public static bool IsInitialized
    {
        // ASSERTION: If in instance is not equal to null it has been initialized which returns true. Smart!!!!
        get { return instance != null; }
    }

    #region Hinterfragen
    // Hinterfragen:
    // Making Awake virtual seems retarded as we Set the assign instance its value in this Method. Now we have the option to ovverride this Method
    // which can cause us to not include this part of the Method when we dont include the base in the ovverriding Method.
    // Wenn ich Awake im GameManager überschreibe wird doch instance gar kein Wert assigned. Nur wenn ich die Base include??? 
    #endregion
    private void Awake()
    { 
        if (instance != null)
        {   //ASSERTION: There allready exists an Instance of this 
            // Very Strong Debugging Message!
            Debug.LogError("[Singleton] Trying to instaniate a second instance of " + typeof(T).ToString() + ".");
        }
        else
        {
            // We have to cast this to be T. It says ensure that this Object is of type T.
            instance = this as T;

            Init();
        }
    }

    // This is a much better solution than to have a virtual Awake Method. The awake Method sets the Instance. We dont want anybody to override the Awake Method
    // Cause when it is ovverriden ant the base Method is not included we are fucked! Cause we will try to access the GameManager Instance which wont exist
    // Getting a NullReference Exception!
    protected virtual void Init()
    {
        // Question: What can we use this for? A Custom class that doesnt have Access to Monobehaviour Methods???
    }

    // Additions: This doesnt have to be done but could be a usefull extension:
    
    // -In the future if GameManager extends Singleton we may want the GameManager to do something special on Awake or OnDestroy. So we are going to make these methods
    // Protected which means they are accessible by any Class that inhertits from Singleton. 
    // -One thing we might wanna do is make sure that if this obj is destroyed then another Singleton can be created.
    protected virtual void OnDestroy()
    {
        // We are checking if the instance is Equal to the particual Object that is beind destroyed. 
        // Why we have to check this?
        if (instance == this)
        {
            instance = null;
        }
    }
}
