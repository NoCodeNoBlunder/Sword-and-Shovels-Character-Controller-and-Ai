using System;
using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour
{
    public GameObject target;
    Vector3 offset;

    bool b;

    // LateUpdate Avoid Stutters We move the Character in Update and in LateUpdate we follow the character with the Camera!
    private void LateUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        else
        {
            if (!b)
            {
                offset = transform.position - target.transform.position;
                b = true;
            }

            transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime * 5);
            return;
        }
    }
}

