using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class AoeEffect : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    void Start()
    {
        transform.Rotate(new Vector3(-90, 0, 0));
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
    }

    void Update()
    {
        
    }
}
