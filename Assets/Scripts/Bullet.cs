using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float MaxProjectileLifetime = 50;
    public float ProjectileLifetime;
    public bool _Bounce = false;
    public int _MaxBounceCount;
    int BounceCount;
    private void OnCollisionEnter(Collision collision)
    {
        if (_Bounce)
        {
            BounceCount = BounceCount + 1;
            if (BounceCount >= _MaxBounceCount)
            {
                gameObject.SetActive(false);
            }
        }
        else if (!_Bounce)
        {
            gameObject.SetActive(false);
        }
    }
}
