
using System;
using UnityEngine;

// A Struct cannot inherit / extend anything!

// When working with Sound there is the AudioListener which registers noice!
// The AudioSource which plays AudioClips!

[Serializable]
public struct SoundFXDefinition
{
    public SoundEffect Effect;
    public AudioClip Clip;
}

// Wenn man ein Enum deklariert. Wird der Enum zum Typen!?
[Serializable]
public enum SoundEffect
{
    // Ein enum definiert alle möglichen Werte für den typ des enums!
    HEROHIT, // Check
    LEVELUP, // Check
    MOBDAMAGE,
    MOBDEATH, // Check?
    NEXTWAVE // Check?
}

// Sobald ein enum deklariert wurde können wir diesen eine Variable dieses Types bilden und ihr eine  Wert!

// Enums sind amazing wir können sie als parameter passen und damit bestimmte aktionen triggern.
// Wir können selber engeben welche aktion ausgeführt werden soll und müssen uns diese Information nirgendsowo anschaffen.
// Es ist sehr gut um unterschiedliche elemente des Selben DatenTypes voneiner zu unterscheiden!

// Wir könnten eine Klasse haben und ihr ein Enum field geben mit dem enum, dass sie definiert!
// Ich möchte Enums nochmal benutzen um Klassen zu Strukturieren.
// Beispiel Item als base class und dann ein Enum für den Typ des Items, was wiederrum darüber entscheidet welche Properties das Item hat!


