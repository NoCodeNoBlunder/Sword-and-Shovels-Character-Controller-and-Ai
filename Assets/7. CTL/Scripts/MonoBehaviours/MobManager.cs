using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// TEst
public class MobManager : MonoSingleton<MobManager>
{
    // These are our Events of types we declared in our Events class
    // Since we have them declared as Serializable in Events and have them public here they will be visivle in the Inspector which means
    // We can Add listeners to them via the Inspector!
    public Events.EventIntegerEvent OnMobKilled;
    public Events.EventIntegerEvent OnWaveCompleted;
    // Diese Beiden sind ähnlich aber in wirklichkeit sind sie sehr verschieden! NextWave wird nur Invoked wenn es noch eine Weite Wave geben wird!
    public UnityEvent OnWaveSpawned; 
    // We can do the same for UnityEvents without a parameter!
    public UnityEvent OnOutOfWaves;

    // Naming public variable with capital first letter?
    public MobWave[] Waves;
    public GameObject[] Mobs;

    private int currentWaveIndex = 0;
    private int activeMobs;

    // We are holding the SpawnPoints itself not their transform!!!
    // We are caching a reference to all the Spawnpoints in our scene!
    private Spawnpoint[] spawnpoints;

    void Start()
    {
        spawnpoints = FindObjectsOfType<Spawnpoint>();
        StartCoroutine(SpawnWave());
    }

    // Sehr Starke Methode!
    // Ich habe diese Methode selber zu einem IEnumarator gemacht, damit die Waves nicht instantly nacheiander spawnen!
    public IEnumerator SpawnWave()
    {
        // We have to include this check to see if we have run out of waves!
        // We substract the 1 cause arrays are 0 based!
        // Dieses minus 1 muss ich noch verstehen!
        if (Waves.Length -1 < currentWaveIndex)
        {
            OnOutOfWaves?.Invoke();
            yield break;
        }

        // Wir wollen diesen Sound erst spielen ab der zweiten Wave!
        if(currentWaveIndex > 0)
        {
            SoundManager.Instance.PlaySoundEffect(SoundEffect.NEXTWAVE);
            OnWaveSpawned?.Invoke();
        }

        if (currentWaveIndex == 0)
        {
            yield return new WaitForSeconds(0);
        }
        else
        {
            yield return new WaitForSeconds(2);
        }

        activeMobs = Waves[currentWaveIndex].NumberOfMobsToSpawn;

        for (int i = 0; i < Waves[currentWaveIndex].NumberOfMobsToSpawn; i++)
        {
            var spawnpoint = SelectRandomSpawnpoint();
            var mob = Instantiate(SelectRandomMob(), spawnpoint.transform.position, Quaternion.identity);

            // We can set the waypoints array of the mob to our closest waypoints array!
            mob.GetComponent<NPCController>().waypoints = FindClosestWayPoints(mob.transform);

            CharacterStats mobCharStats = mob.GetComponent<CharacterStats>();
            // this will save us some typing! So muss man arbeiten workflow smart!
            // Wenn man etwas öfter nutzen muss immer cachen!
            MobWave currentWave = Waves[currentWaveIndex];

            mobCharStats.SetInitialDamage(currentWave.DamageMultiplicator);
            mobCharStats.SetInitialHealth(currentWave.HealthMultiplicator);
            mobCharStats.SetInitialResistance(currentWave.ResistanceMultiplicator);
        }
    }

    private GameObject SelectRandomMob()
    {
        // Random.Range() mit int ist die ober Grenze exklusive!
        int mobIndex = Random.Range(0, Mobs.Length);
        return Mobs[mobIndex];
    }

    // We are returning a Spawnpoint. WIeso returnen wir nicht einen Vector3 für die spawnPos?
    private Spawnpoint SelectRandomSpawnpoint()
    {
        int pointIndex = Random.Range(0, spawnpoints.Length);
        return spawnpoints[pointIndex];
    }

    // This Method will find and return which group of waypoints is closets to the 
    // spawnLocation of the mob!
    private Transform[] FindClosestWayPoints(Transform mobTransform)
    {
        // We are using a labda expression!
        // SqrMagnite returns the lenght of a vector!
        // Wir müssen nicht Vector3.Disance benutzten um den Abstand zweier Punkte zu bestimmen.
        // Wir können der Vector von Punkt A nach B berechnen und von diesem Vector die  Wurzel Länge bestimmen mit 
        // der Property eines jeden Vectors sqrMagnitude.
        var closestWaypoint =
            FindObjectsOfType<Waypoint>().OrderBy(w => (w.transform.position - mobTransform.position).sqrMagnitude).First();

        // Get the Parent
        // Wieso nutzten wir Transform und nicht GameObject, weil Transform weniger Resscourcen frisst?
        // We can access the transform of the parent via transform.parent
        // Wir müssen eine Referent zum parent storen, da wir diese später wieder brauchen!
        Transform parent = closestWaypoint.transform.parent;
        Transform[] allWaypoints = parent.GetComponentsInChildren<Transform>();

        // The Problem is that this would return the Transform of our parent also which we dont want we only want the transforms of
        // our children! 
        var transforms = allWaypoints.Where(t => t != parent);
        return transforms.ToArray();
    }

    // Whenever a Mob dies this gets Invoked. Da eine Klasse nur seine eigenen Events invoken kann müssen wir eine Methode schreiben, die es erlaubt
    // unser Invoke von unserem NPCs aus zu triggern!
    public void HandleMobDeath()
    {
        // This should be done on top of the Method!
        SoundManager.Instance.PlaySoundEffect(SoundEffect.MOBDEATH);

        #region Kommentar wieso wir hier eine Referenz erstellen!
        // Anstatt das so "OnMobKilled.Invoke?.Invoke(Waves[currentWaveIndex].ExperiencePerKill) zu machen können wir eine Referenz erstellen!
        // Das spart uns auch schreibarbeit uns sollte immer gemacht werden! Außerdem erhöhr es die lesbarkeit nur pluspunkte! Es hat also nur vorteile 
        // Referenzen zu erstellen die prinzipiel gar nicht erstellt werden müssen!
        #endregion
        MobWave currentMobWave = Waves[currentWaveIndex];

        // Ich möchte eigentlich ein anderes System haben, denn hier gibt jeder Gegner die gleiche Anzahl an Experience.
        // Der WaveCount sollte hier auch nur einen multiplier darstellen, wenn du mich fragst.
        // Dafür müssten wir jedoch jedem Mob einen Individuellen Score geben auf den wir hier zugriiff bräuchten!
        OnMobKilled?.Invoke(currentMobWave.ExperiencePerKill);

        activeMobs--;
        Debug.LogFormat("There is {0} Mobs left", activeMobs);

        if (activeMobs <= 0)
        {
            //ASSERTION: There is no Mobs left: When there is less than 4 Mobs they should be going towards the Player automatically!
            OnWaveCompleted?.Invoke(currentMobWave.WaveCompletionValue);
            currentWaveIndex++;

            // Ich möchte nicht das SpawnWave direkt ausgeführt wird man soll etwas zeit haben!
            // Deshalb werde ich diese Wave callen sobald der Exiter verschwindet!
            StartCoroutine(SpawnWave());
        }
    }
}
