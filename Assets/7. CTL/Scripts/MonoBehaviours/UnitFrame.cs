using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitFrame : MonoBehaviour
{
    #region Fields

    public Text levelText;
    public Image healthbar;

    #endregion

    public void InitUnitFrame()
    {
        levelText.text = "1";
        healthbar.fillAmount = 1;
    }

    public void UpdateUnitFrame(HeroController hero)
    {
        // Gather Data
        int currentHealth = hero.GetCurrentHealth();
        int maxHealth = hero.GetMaxHealth();
        int currentLevel = hero.GetCurrentLevel();

        // Perform Logik on thaz Data!
        float fillAmount = (float)currentHealth / (float)maxHealth;
        
        levelText.text = currentLevel.ToString();
        healthbar.fillAmount = fillAmount;
    }
}
