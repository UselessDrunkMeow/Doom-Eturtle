using System.Numerics;
using UnityEngine;

public class rotateturtle : MonoBehaviour
{
    public Transform tfrm;
    public float rotationspeed = 0.5f;
    private float i = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tfrm.Rotate(0, i, 0);
        i =+ rotationspeed;
    }
}
