using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MobHealthBar : MonoBehaviour
{
    private Vector3 localScale;
    private CharacterStats stats;
    private Camera mainCamera;
    
    private void Awake()
    {
        // We can access a gameObjects child and parent via its transform
        // so a transform not only contains information about the position, rotation and scale but also about childs and parents
        // of a gameObject.

        // Get Reference to the stats of the golbin that has this healthbar attached!
        stats = transform.parent.GetComponent<CharacterStats>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Wir müssen localScale erst initializen!
        localScale = transform.localScale;
    }

    private void Update()
    {
        if (stats != null)
        {
            UpdateHealthBar();

            #region Unterschied transform.LookAt() und Quaternion.LookRotation
            // LookAt dreht ein gameObjekt so, dass seine Z Achse auf die Position zeigt die übergeben wird.
            // LookRotation dreht ein Objekt so, dass seine Z Achse in die Richtung des Vektors zeigt der übergeben wird.
            // Um also auf eine Bestimmte Stelle mit LookRotation zu schauen muss man den Vektor ausrechnen mit (target - startPos)?
            
            // Ein Vektor kann eine "Position" aber auch eine "Richtung" darstellen!

            // Das wäre die Lösung mit LookAt:
            //transform.LookAt(mainCamera.transform);
            #endregion
            transform.rotation = Quaternion.LookRotation(mainCamera.transform.position - transform.position);
        }
    }

    // Das Sollte nicht im Update ausgeführt werden, da sich das Hp nur ändern muss wenn ein Goblin getroffen wird und nicht jeden frame!
    private void UpdateHealthBar()
    {
        int currentHealth = stats.charDefinition.currentHealth;
        int maxHealth = stats.charDefinition.maxHealth;
        float healthPercent = (float)currentHealth / maxHealth;
        localScale.x = healthPercent;
        transform.localScale = localScale;
    }
}
