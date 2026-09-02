using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform _Firepoint;
    public GameObject _Bullet;
    public float _FireSpeed;
    GameObject spawned_Bullet;
    Rigidbody _bullet_RB;
    Vector3 hit_pos;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetHitPosition();
            Shoot();            
        }
    }
    void GetHitPosition()
    {
        RaycastHit hit;
        Physics.Raycast(_Firepoint.position, _Firepoint.forward, out hit, Mathf.Infinity);
        hit_pos = hit.transform.position;
        transform.LookAt(_Firepoint);
    }
    void Shoot()
    {
        spawned_Bullet = Instantiate(_Bullet,_Firepoint.position,_Firepoint.rotation);
        _bullet_RB = spawned_Bullet.GetComponent<Rigidbody>();
        _bullet_RB.AddForce(_Firepoint.forward * _FireSpeed * 100);
    }
}
