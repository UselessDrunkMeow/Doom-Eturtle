using System;
using System.Numerics;
using UnityEngine;

public class rotateturtle : MonoBehaviour
{
    public Transform tfrm;
    public float rotationspeed = 0.5f;
    private float i = 0;

    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tfrm.Rotate(0, i, 0);
        i =+ rotationspeed;
    }
}
