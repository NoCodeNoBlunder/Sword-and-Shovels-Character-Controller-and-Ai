using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MobWave_01.Asset", menuName = "MobWave")]
public class MobWave : ScriptableObject
{
    [Header("Wave Properties")]
    public int NumberOfMobsToSpawn;
    public int ExperiencePerKill;
    public int WaveCompletionValue;

    [Header("Mob Properties")]

    // These should be multipliers as we have multiple enemies and we want to adress them all!
    // Ich mache darauß Multiplikatoren
    public float HealthMultiplicator;
    public float DamageMultiplicator;
    public float ResistanceMultiplicator;
}
