using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float MaxProjectileLifetime = 50;
    public float ProjectileLifetime;
    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
