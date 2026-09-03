using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform _Firepoint;
    public Transform _CameraPoint;
    public GameObject _Bullet;
    public float _FireSpeed;
    public LayerMask _Mask;
    public GameObject testsphere;
    GameObject spawned_Bullet;
    Rigidbody _bullet_RB;
    Vector3 hit_pos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
            Instantiate(testsphere, hit_pos, new Quaternion(0,0,0,0));
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
        spawned_Bullet = Instantiate(_Bullet, _Firepoint.position, _Firepoint.rotation);
        _bullet_RB = spawned_Bullet.GetComponent<Rigidbody>();
        _bullet_RB.AddForce(_Firepoint.forward * _FireSpeed * 100);
    }
}
