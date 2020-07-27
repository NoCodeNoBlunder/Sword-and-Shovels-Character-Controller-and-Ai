using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exiter : MonoBehaviour
{
    private RectTransform rectTransform;
    // So the baseclass allready uses the property animation. So if we want to override it we can say "new"! 
    // Its better to simply use another name!
    private Animation anim;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        anim = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        anim.Play();
    }

    public void Stop()
    {
        // We dont have to reset the rotation. Cause the End rotation is the same as the start rotation!
        rectTransform.localScale = Vector3.zero;
        
        // Wenn das Objekt ausgemacht wird wird nix mehr gecalled!
        gameObject.SetActive(false);
    }
}
