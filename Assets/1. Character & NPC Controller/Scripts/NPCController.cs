using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    #region Field Declarations

    public Events.EventOnDeath OnMobDeath;

    // This will be the Attack the Npc inflicts on our Player!
    public AttackDefinition attack;

    // This will be the Position the Spell will be cast from. In our case the Goblins right hand!
    public Transform spellHotSpot;
    
    // Delay Value before the NPC sets the next waypoint as its destination! THis time starts when the NPC gets assigned a new destination and not when it arrives!
    // currently which would be better! 
    public float patrolTime = 2; // time in seconds to wait before seeking a new patrol destination
    public float aggroRange = 10; // distance in scene units below which the NPC will increase speed and seek the player
    public Transform[] waypoints; // collection of waypoints which define a patrol area

    CharacterStats charStats;
    int index; // the current waypoint index in the waypoints array
    float timeOfLastAttack; // This will be used to determine if the Npc can attack again!
    Transform player; // reference to the player object transform

    Animator animator; // reference to the animator component
    NavMeshAgent agent; // reference to the NavMeshAgent

    // Attribute to make a Header!
    [Header("Use for Debugging Aggro Radius")]
    public bool showAggro = true;

    bool isPersuing = false;
    bool isPlayerAlive;

    #endregion

    #region Startup

    void Awake()
    {
        charStats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        index = Random.Range(0, waypoints.Length);

        isPlayerAlive = true;

        // We said it to the smallest avaiable float instead of 0 so the programm doesnt think the Golbin has attacked when he spawn or the game Starts!
        timeOfLastAttack = float.MinValue;

        player.gameObject.GetComponent<DestructedPlayer>().OnPlayerDeath += HandlePlayerDeath;
        OnMobDeath.AddListener(MobManager.Instance.HandleMobDeath);

        // The way we are going to set this up: We are going to use a set of timers. One timer will be used for the NPC to check its status. So it will check if it should be in
        // Patroll mode or Persuing the Player! Then we are goint to use a different timer which will indicate how long the npc will presure/ stay at the currend Waypoint.
        // Tick will check the NPCs state every 0.5 seconds!
        InvokeRepeating("Tick", 0f, 0.5f);

        // Checking if there acctually are any waypoints!
        if (waypoints.Length > 0)
        {
            // The Patrol Method will run every patrolTime seconds!
            // We Also want an initial delay so it looks more natural and all goblins are not in sync! I Think we need some Polymorhism here!
            InvokeRepeating("Patrol", Random.Range(0, patrolTime + 7f), patrolTime);
        }
    }

    private void HandlePlayerDeath()
    {
        isPlayerAlive = false;
    }

    #endregion

    private void Update()
    {
        // Wir können natürlich auch Variablen haben die sich jeden Update 

        // Nicht trivial Cooldown! Neue möglichkeit den Cooldown zu setzen!
        float timeSinceLastAttack = Time.time - timeOfLastAttack;
        // We dont need ternary when working with bool Opeerators ICh IDdiot!
        // Das ist Überhaubt nicht trivial für mich wenn ich es nicht checke erstmal if statement aufschreiben!
        bool attackOnCooldown = timeSinceLastAttack <= attack.cooldown;

        if (isPlayerAlive)
        {
            // Mach immer variablen als zwischenSchritte um die Lesbarkeit des Programmes zu erhöhen!
            float distanceFromPlayer = Vector3.Distance(player.position, transform.position);
            // Hier wieder ein Boolean den man avoiden könnte, der jedoch der Lesbarkeit erhöhr und deshalb erwünscht ist!
            bool attackInRange = distanceFromPlayer <= attack.range;

            // Das ist next level! Wir beinflussen unsere agent movemt abhängig davon ob der attack Cooldown hat!
            // Am Anfang hat der attack keinen cooldown also is isStopped = false ergo der NPC bewegt sich. Sobald er jedoch einen Angriff ausführt 
            // ist der cd true und somit stopped der character! Wir wollen doch das er sich nur nicht bewegt während der duration vom attack und nicht
            // wenn der attack auf cooldown ist! 

            agent.isStopped = attackOnCooldown;

            if (!attackOnCooldown && attackInRange)
            {
                // Und wieder wollen wir, dass der Npc in die richting vom SPieler zeigt, wenn er angreift!
                transform.LookAt(player.position);
                animator.SetTrigger("Attack");
                timeOfLastAttack = Time.time;
            }
        }
        
        SynchrochizingAnimationWithMovement();
    }

    // Not really this is called when the Animation has send the attack! For a melee attack that is a hit for a ranged it starts the travel time!
    // Automatically called once the attack connects!
    // Here we create the attack!
    public void Hit()
    {
        // When the Player is dead we dont want to proceed here. It would cause a lot of errors since the Player is now null!
        if (!isPlayerAlive)
            return;

        // NEU! attack ist doch vom Typ AttackDefintion das checke ich nicht! Ich muss schneller typen lernen!
        // New keyword is!!
        if (attack is Weapon)
        {
            #region Meine Lösung
            /* charStats.GetCurrentWeapon().CreateAttack(charStats, player.gameObject.GetComponent<CharacterStats>()); */
            #endregion

            // NEU! NEU!
            // We check if out Attack is a weapon and if so we cast it as a Weapon allowing us to access the ExecuteAttack Method which we want 
            // to Invoke! Das hier muss ich wiederholen und verstehen!
            ((Weapon)attack).ExecuteAttack(gameObject, player.gameObject);
        }
        else if (attack is Spell)
        {
            // We are passing CastProjectile all the needed information! We are using a function is LayerMask which returns an int based
            // on the selected Layer! Since we want this projectile to be living on the EnemySpells layer we type that in and get this layers
            // corosponding int returned!
            ((Spell)attack).CastProjectile(gameObject, spellHotSpot.position, player.position, LayerMask.NameToLayer("EnemySpell"));
        }
    }

    #region States: 

    //We should really use a FSM as we add more features here!

    // I dislike the names of the Methods. Patroll is not really patrolling but setting altering the Index value
    // We are acctually Invoking this Method using Invoke Repeating
    // This Method manages the Patroll point Index it!
    // Imo it should have a different name.
    private void Patrol()
    {
        #region Komment for this new Logic inLine Method! Conditional bool asignment selber überlegter name!
        // All we need the Patroll MEthod to to is manage the patroll point index! To do this we use an inline (...) Method!!
        // New Logic Statement: this is an inline Method
        // We set the index based on the following evalutation: If the index is equal to waypoint.lenght - 1 we set index to 0. (This is the case when the index reaches the end of the
        // Array and therefore needs to reset back to the first waypoint). If the index is smaller than the lenght of the waypoints collections it gets incremented by 1.
        // Why is has to be waypoint.Lenght - 1????

        // It it is not true we set it to index to index + 1:
        // this is the same as saying:
        #region Alternative Lösung

        /* if (index == waypoints.Length - 1)
        {
            index = 0;
        }
        else
        {
            //index = index + 1;
            index++;
        }   */

        #endregion

        #endregion

        index = index == waypoints.Length - 1 ? 0 : index + 1;
    }

    // We are acctually Invoking this Method using Invoke Repeating
    // Tick will check in which state the Npc currently is and execute it!
    private void Tick()
    {
        if (isPlayerAlive)
        {
            float distance = Vector3.Distance(agent.transform.position, player.transform.position);
            //Debug.Log(distance);

            if (player != null && distance < aggroRange)
            {
                isPersuing = true;
                Persue();
            }
        }
       
        // IF we are not persuing we want to go to the next waypoint! Condition as we are not using a FSM!
        if (!isPersuing)
        {
            agent.speed = 2.0f;
            agent.destination = waypoints[index].position;
        }
    }

    private void Persue()
    {
        agent.speed = 5;
        agent.destination = player.transform.position;
    }

    #endregion

    #region Synchronys Animation with Movement

    private void SynchrochizingAnimationWithMovement()
    {
        // this is self Managing when the speed changes also the animation changes automatically cause it is in the update Method!
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    #region Debug AggroRange
    // Unity Method
    // New allows to draw certain information in the scene view. 
    // We are showing the aggro range of the NPCs 

    // How does this Method work it it is not in the upate loop and is never called yet displays the informatiom all the time?????
    // Why is there an error of of reference exception??????
    // This doesnt work properly

    // MAcht die ganze Zeit error messages

    /*
    private void OnDrawGizmos()
    {
        if (showAggro)
        {
            // In the Course he used an if statement here but it leads checking of the bool showAggro is set to true but this leads to an error!
            Gizmos.color = Color.red;
            // it gives a visual representation of the Npc aggro range which can really help to finetune when Level designing.

            // Why is this producing an Error?
            Gizmos.DrawWireSphere(agent.transform.position, aggroRange);
        }
    }
    */
    #endregion

    #endregion

    private void OnDestroy()
    {
        OnMobDeath?.Invoke();
    }
}
