//using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Jobs; // ??? What is this nameSpace used for?

public class HeroController : MonoBehaviour
{
    #region Fields Declaration

    public AttackDefinition stompAttack;

    // Not used anymore
    //public AttackDefinition demoAttack;
    private GameObject attackTarget;

    Animator animator; // reference to the animator component
    NavMeshAgent agent; // reference to the NavMeshAgent

    CharacterStats charStats;
    bool isStomping = false;
    //float timeSinceStomp = 0;
    float currentCooldown = 0;

    #endregion

    #region Startup

    void Awake()
    {
        // "Caching" a reference to the animator and the agent components.
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // We need to pass the speed information of the agent to the Animator through the Speed parameter
        // This has to be done every frame so we need to do this inside update Ich idiot!

        // We allready have the CharacterStats attached to the Hero in the Characterstats script so we simply need to access it
        // via GetComponent!
        charStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        #region Kommentar wieso wir hier die GameManager Methode Subscriben und nicht imm GameManager selber
        // You might wonder why we are adding a listenern here rather than in the GameManager!
        // We want to make sure that the CharacterStats Component  of the Hero is fully initialized before 
        // adding a listener!

        // Der GameManager selber hat auch keine Referenz zum Character_SO

        // Wdh. Ein Event kann nur von der eigenen Klasse invoked werden. 
        // Es kann jedoch von jeder Klasse aus Subscribed werden!
        #endregion

        #region Add Listeners to Events
        charStats.charDefinition.OnHeroLevelUp += GameManager.Instance.HandleHeroLevelUp;
        charStats.charDefinition.OnHeroGainedHp += GameManager.Instance.HandleHeroGainedHp;
        charStats.charDefinition.OnHeroTakeDmg += GameManager.Instance.HandleHeroTakeDmg;
        charStats.charDefinition.OnHeroDeath += GameManager.Instance.HandleHeroDeath;
        charStats.charDefinition.OnHeroInitialized.AddListener(GameManager.Instance.HandleHeroInit);
        #endregion

        #region Invoking von einer anderen Klasse mit UnityEvents
        // ich dachte nur die Klasse, die das Event declared kann sein Event Invoken!
        // Ah lol UnityEvents können von überall invoked werden! Das ist das starke!
        // Actions können nur von der Klasse, die die Action declared Invoked werden!
        #endregion
        charStats.charDefinition.OnHeroInitialized?.Invoke();
    }

    #endregion

    #region Synchronizing Movement and Animation

    private void Update()
    {
        //timeSinceStomp += Time.deltaTime;
        currentCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Stomp();
        }

        #region Kommentar Nav Mesh automatic Rotation to destination
        // We dont need to Set the LookRotation this is done automatically by the Nav Mesh Agent!
        /* transform.rotation = Quaternion.LookRotation(agent.velocity.normalized) */
        #endregion
        #region Kommentar Animator
        // The animator has internal Methods that allow you to Set and ReSet its "Parameters" values which can trigger a Transition!
        // We need to specify which Properties value we want to alter which in this case is the Speed as the animation controller is setup in a way that changes the animation
        // based on the character speed!

        // This was brilliant acctually of me to come up with the agent.velocity.magnitude!!
        // This Line is birlliant in generall!
        #endregion
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    #endregion

    #region Right Click Basic Attack

    #region Komment AttackTarget This Method is OP
    // This Method is next level. LERN diesen SHIT!
    // Attackig simply means calling the targets OnAttack Method in simple words
    // - This Method will accept the GameObject of the target we want to attack!
    // We need to know who was attacked and access its CharacterStats to apply the dmg!
    // With this knowlegde we can create a new attack with our Function CreateAttack
    #endregion

    #region Replaced AttackTarget MEthod
    /*
    public void AttackTarget(GameObject target)
    {
        // We create a new attack which will use the Heroes charStats and the targets charStats
        Attack attack = demoAttack.CreateAttack(charStats, target.GetComponentInChildren<CharacterStats>());

        // ???? We only have one target or am i Wrong is this for aoe attacks ??
        var attackables = target.GetComponentsInChildren<IAttackable>();

        foreach (IAttackable attackable in attackables)
        {
            // We need to pass this gameObject which is the Hero Prefab and an attack which we just created
            // Remember CreateAttack returns an attack. This is genius sry to say but I hava a lot to learn!
            // Holy shit!
            // We call the Targets OnAttack passing it the Information about who is the attacker and what attack is hitting it.
            attackable.OnAttack(gameObject, attack);
        }
    }
    */
    #endregion

    // Attack Target is called by our Event in the Inspector!
    // - We dont see it referenced here which sucks!
    public void AttackTarget(GameObject target)
    {
        var weapon = charStats.GetCurrentWeapon();

        // This is called a "NullCheck"
        if (weapon != null)
        {
            // This will interrupt any running PersueAndAttack Operations which will allow us to switch targets on the fly!
            StopAllCoroutines();

            agent.isStopped = false;
            // We set our attackTarget that is passed in to the AttackTarget Method!
            attackTarget = target;
            StartCoroutine(PersueAndAttackTarget());
        }
    }

    // In Addition to trigger an attack we need our hero to face and approach its target!
    // We accomplish this by using a Couroutine as this need to happend over multiple frames!
    private IEnumerator PersueAndAttackTarget()
    {
        // We need to make sure that our Nav Mesh Agent is NOT stopped.
        agent.isStopped = false;
        // We only need to access the current weapon so we know its range!
        var weapon = charStats.GetCurrentWeapon();

        // We want to move towards the enemy while we are not inside the range of our weapon!
        while (Vector3.Distance(transform.position, attackTarget.transform.position) > weapon.range)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        // Once our Hero is in range we want him to stopp moving and attack!
        // We want to make sure our Hero is facing the Enemy
        // we want the hero to Start the Attack animation when he is facing the enemy!
        agent.isStopped = true;
        // Idito!
        transform.LookAt(attackTarget.transform);
        // How are we playing an Animation? We arw triggering its condition!
        animator.SetTrigger("Attack");
    }

    // Automatically Invoked when hit an enemy
    // Even tho it shows 0 references!
    // Once the Enemy is hit. We make our weapon attack the target!
    public void Hit()
    {
        // if attackTarget is not null. Wir schreiben jedoch attackTaget != null damit klar ist das AttackTarget kein bool ist.
        if (attackTarget != null)
        {
            // we attack always using our current Weapon!
            // This is next lvl need to learn tthat myself!
            charStats.GetCurrentWeapon().ExecuteAttack(gameObject, attackTarget);
        }
    }

    #endregion

    public void Stomp()
    {
        //timeSinceStomp = 0;

        if (currentCooldown <= 0)
        {
            currentCooldown = stompAttack.cooldown;
            StopAllCoroutines();
            StartCoroutine(ExecuteStomp());
        }
    }

    public IEnumerator ExecuteStomp()
    {
        //agent.destination = transform.position;
        isStomping = true;
        agent.isStopped = true;
        agent.destination = transform.position;
        animator.SetTrigger("Stomp");
        yield return new WaitForSeconds(0.48f);

        if (stompAttack is StompAttack)
        {
            ((StompAttack)stompAttack).Fire(gameObject, transform.position, LayerMask.NameToLayer("PlayerSpell"));
        }

        // Diese Zeit wird erst gewartet nachdem die erste Zeit fertig gewartet wurde!
        yield return new WaitForSeconds(0.45f);
        agent.isStopped = false;
        agent.destination = agent.destination;
        isStomping = false;
    }

    // This Method is called by the MouseManagers OnCLickOnEnvironment Event!
    public void SetDestination(Vector3 destination)
    {
        // We always want to be able to run aways and cancel our Persued and attack!
        // Unless we are stomping we need a state machine!
        if (!isStomping)
        {
            StopAllCoroutines();
            agent.isStopped = false;
            agent.destination = destination;
        }
    }

    #region HelperMethods/Wrappers

    public int GetCurrentLevel()
    {
        return charStats.charDefinition.charLevel;
    }

    public int GetCurrentHealth()
    {
        return charStats.charDefinition.currentHealth;
    }

    public int GetMaxHealth()
    {
        return charStats.charDefinition.maxHealth;
    }

    #endregion

    #region CallBack/ EventHandlers

    // These are attached in the Inspector
    public void HandleMobKilled(int xpAmount)
    {
        // This will call charStats IncreaseXP which is wrapper and will call Charcacter_SO IncreaseXP
        charStats.IncreaseXP(xpAmount);
        Debug.LogFormat("You killed a Mob for {0} XP!", xpAmount);
    }

    public void HandleWaveCompleted(int waveCompletionXpAmount)
    {
        charStats.IncreaseXP(waveCompletionXpAmount);
        Debug.LogFormat("You Completed the Wave for {0} XP!", waveCompletionXpAmount);
    }

    public void HandleOutOfWaves()
    {
        
    }

    #endregion
}
