using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    // Diese Liste beinhaltet ein Struct!
    public List<SoundFXDefinition> SoundFX;
    public AudioSource SoundFXSource;

    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        // Filter List and Find based on enum
        // Unterschied Find() and Linq.Where()? Ich glaube Find returned nur ein Effekt und Where eine Collection!

        // Es ist Genial das eine Liste als Typen eine ganze Klasse nehemen kann mit all seine Properties!

        // sFX is an element from the List SoundFX
        // Das ist so ähnlich wie beim foreach loop wir definieren jedes einzelne element mit sFX
        // Wenn der richtige Sound gefunden wurde wollen wir auf dessen Clip zugreifen
        AudioClip effect = SoundFX.Find(sFX => sFX.Effect == soundEffect).Clip;

        SoundFXSource.PlayOneShot(effect);
    }
}
