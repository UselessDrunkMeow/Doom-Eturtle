using System.Runtime.InteropServices;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float MaxProjectileLifetime = 5;
    public float ProjectileLifetime;

    public bool _Bounce = false;
    public int _MaxBounceCount;
    public float _AOERange;
    GameObject player;
    int BounceCount;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerShoot>().gameObject;
    }
    private void OnEnable()
    {
        BounceCount = 0;
        ProjectileLifetime = 0;
    }
    private void Update()
    {
        ProjectileLifetime = ProjectileLifetime + Time.deltaTime;
        if (ProjectileLifetime > MaxProjectileLifetime)
        {
            print("FUCK EVERYEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGEVERYGGDIOUIHYGIIYGGGDIOUIHYGIIYG");
            gameObject.SetActive(false);
            ProjectileLifetime = 0;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_Bounce)
        {
            BounceCount = BounceCount + 1;
            EffectSpawner.SpawnEffect(transform.position, "EnergyExplosion");
            if (Vector3.Distance(player.transform.position, transform.position) <= _AOERange)
            {
                player.GetComponent<HealthManager>().UpdateHealth(1);
            }
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
