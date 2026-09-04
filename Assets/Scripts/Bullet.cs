using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float MaxProjectileLifetime = 50;
    public float ProjectileLifetime;
    private void Update()
    {
        ProjectileLifetime = ProjectileLifetime + Time.deltaTime;
        if(ProjectileLifetime >= MaxProjectileLifetime)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
