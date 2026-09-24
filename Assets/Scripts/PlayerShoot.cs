using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform _Firepoint;
    public Transform _CameraPoint;
    public GameObject _Bullet;

    public float _FireSpeed;
    public float _FireRate;

    public LayerMask _Mask;
    public GameObject testsphere;

    GameObject spawned_Bullet;
    Rigidbody _bullet_RB;
    Vector3 hit_pos;

    public float time;

    void Update()
    {
        time = time + Time.deltaTime;
        if (Input.GetMouseButton(0)&& _FireRate <= time)
        {
            time = 0;
            GetHitPosition();
        }

        //Debug Rays.
        Debug.DrawRay(_Firepoint.transform.position, _Firepoint.forward * 5000, Color.red);
        Debug.DrawRay(_CameraPoint.transform.position, _CameraPoint.forward * 5000, Color.red);
    }

    //Shoots a raycast from the center of the camera, and makes the firepoint object look at the hit location.
    void GetHitPosition()
    {       
        RaycastHit hit;
        Physics.Raycast(_CameraPoint.position, _CameraPoint.forward, out hit, Mathf.Infinity, _Mask);        
        if (hit.transform != null)
        {
            hit_pos = hit.point;
            _Firepoint.transform.LookAt(hit_pos);

            //Debug check to visually see where the hit point is located.
            //Instantiate(testsphere, hit_pos, new Quaternion(0,0,0,0));
        }
        else
        {
            //idk if this is needed but it broke without it so ig its here.
            _Firepoint.transform.position = _Firepoint.transform.position;
        }
        Shoot();
    }
    //spawns the bullet prefab at the firepoints position and rotation, then adds force to shoot it forward.
    void Shoot()
    {
        GameObject PooledBullet = ObjectPool.SharedInstance.GetPooledObject("Bullet"); 
        if (PooledBullet != null) {
            PooledBullet.SetActive(true);
            PooledBullet.transform.position = _Firepoint.position;
            PooledBullet.transform.rotation = _Firepoint.rotation;
            if (PooledBullet.TryGetComponent<Rigidbody>(out Rigidbody bulletRB))
            {
                // Clear lingering momentum from object pool reuse
                bulletRB.linearVelocity = Vector3.zero;
                bulletRB.angularVelocity = Vector3.zero;

                // Fire instantly with Impulse mode
                bulletRB.AddForce(_Firepoint.forward * _FireSpeed, ForceMode.Impulse);
            }
        }
        else
        {
            // Pool ran out of available objects
            Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
        }
        //spawned_Bullet = Instantiate(_Bullet, _Firepoint.position, _Firepoint.rotation);
    }
}
