using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    public TextMesh textMesh;
    public float displayDuration;
    public float scrollSpeed;

    private float startTime;

    private void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // We want to text to always face the Camera!
        

        // Nicht Trivial wenn man bad logic hat XD!
        if (Time.time - startTime < displayDuration)
        {
            transform.LookAt(Camera.main.transform);
            transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
        }
        else
        {
            // "This" refers to this Script Object and gameObject refers to the gameObject the script is attached to!
            Destroy(gameObject);
        }
    }

    public void SetText(string textToDisplay)
    {
        textMesh.text = textToDisplay;
    }

    public void SetColor(Color color)
    {
        textMesh.color = color;
    }
}
