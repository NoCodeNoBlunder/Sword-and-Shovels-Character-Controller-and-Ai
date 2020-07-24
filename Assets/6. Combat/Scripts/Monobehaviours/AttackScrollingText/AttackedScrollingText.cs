using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Das kann man nur für Componenten nutzen nicht für Teile die an Componten in dem Fall Scripts attached werden!
//[RequireComponent(typeof(ScrollingText))]
public class AttackedScrollingText : MonoBehaviour, IAttackable
{
    // Coudl i make that the right text obj is attached automatically in the Inspector??
    public ScrollingText text;
    public Color textColor;

    public void OnAttack(GameObject attacker, Attack attack)
    {
        // Since the text is automatically facing the camera! We dont need to set the 
        ScrollingText spawnedText = Instantiate(text, transform.position, Quaternion.identity);
        spawnedText.SetText(attack.Damage.ToString());
        spawnedText.SetColor(textColor);
    }
}
