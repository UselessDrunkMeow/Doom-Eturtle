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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnAwake()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void OnEnable()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tfrm.Rotate(0, i, 0);
        i =+ rotationspeed;
    }
}
