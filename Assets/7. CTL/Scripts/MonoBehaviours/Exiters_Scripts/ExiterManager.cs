using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class ExiterManager : MonoBehaviour
{
    // Difference Sprite and Image?
    // Vorsicht es gibt 2 Images wir wollen das Image der namespace UnityEngine.UI;
    // Beide heißen identisch!
    [SerializeField] Image gameOver;
    [SerializeField] Image nextWave;
    [SerializeField] Image youWin;

    private void Start()
    {

    }

    public void HandlePlayGameOver()
    {
        gameOver.gameObject.SetActive(true);
    }

    public void HandlePlayNextWave()
    {
        nextWave.gameObject.SetActive(true);
    }

    public void HandlePlayYouWin()
    {
        youWin.gameObject.SetActive(true);
    }
}
