using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
//States the boss can be in, Set the state via code to execute the corresponding functions
//More states can be added if needed, and the names should be changed to describe the attack
public enum BossState
{
    Idle,
    Lazer,
    CrownSlam,
    LazerBurst,
    Summon,
    Teleporting
}

public class BossScript : MonoBehaviour
{
    public BossState _BossState;
    public GameObject _Player;
    public float _NormalLookSpeed;
    public float _CurrentLookSpeed;
    public LayerMask _Mask;
    public int randomNumber;
    UI_Manager uiman;
    HealthManager healthManager;
    EffectSpawner effectSpawner;

    [Header("Lazer Settings")]
    public float _LazerLookSpeed;
    public float _LazerDuration;
    public GameObject LaserPoint;

    [Header("Crown Settings")]
    public GameObject Crown;
    private Boolean MoveCrownToPlayer;
    private Boolean MoveCrownToBoss;
    bool SlamCrown;
    Vector3 TempScale;
    Transform TempParent;
    Vector3 TempPos;
    Quaternion TempRot;
    Vector3 GrowScale = new Vector3(25, 25, 25);

    [Header("Teleport Settings")]
    [Tooltip("Points the boss can teleport to")]
    public Transform[] _TeleportPoints;
    [Tooltip("Delay between teleports in the Teleport attack")]
    public float _TeleportDelay;

    [Header("Summon Settings")]
    float spawnCount;
    public float _SpawnRange;
    public float _AmountToSpawn;

    public Transform _Firepoint;
    public float _LazerSpeed;
    public float _TimeBetweenShots;


    private void Start()
    {
        healthManager = GetComponent<HealthManager>();
        effectSpawner = GetComponent<EffectSpawner>();
        uiman = FindAnyObjectByType<UI_Manager>();       
    }
    public void OnEnable()
    {
        Teleport();
        uiman.ToggleBossBar();
        LaserPoint.SetActive(false);
    }

    //sellect a random action to take, like one of its ttacks, or summoning of extra enemies, teleporting etc
    //change probibility of a action by changing the case's range
    //make sure to start each case with the highest number of the case before it.
    public void ChooseAction()
    {
        print("uuuuhm halloooo???");
        randomNumber = UnityEngine.Random.Range(0, 13);
        switch (randomNumber)
        {
            case >= 0 and < 3:
                _BossState = BossState.Lazer;
                print(_BossState);
                break;

            case >= 3 and < 5:
                _BossState = BossState.CrownSlam;
                print(_BossState);
                break;

            case >= 5 and < 8:
                _BossState = BossState.LazerBurst;
                print(_BossState);
                break;

            case >= 8 and < 10:
                print(_BossState);
                _BossState = BossState.Teleporting;
                break;

            case >= 10 and < 13:
                print(_BossState);
                _BossState = BossState.Summon;
                break;
        }

        switch (_BossState)
        {
            case BossState.Idle:
                StartCoroutine(Idle());
                break;

            case BossState.Lazer:
                StartCoroutine(Lazer());
                break;

            case BossState.CrownSlam:
                StartCoroutine(CrownSlam());
                break;

            case BossState.LazerBurst:
                StartCoroutine(LazerBurst());
                break;

            case BossState.Teleporting:
                StartCoroutine(Teleporting());
                break;

            case BossState.Summon:
                StartCoroutine(Summon());
                break;
        }
    }
    private void Update()
    {
        LookAtPlayer();
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(LazerBurst());
        }

        if (MoveCrownToPlayer == true)
        {
            Crown.transform.position = Vector3.MoveTowards(Crown.transform.position, new Vector3(_Player.transform.position.x, _Player.transform.position.y + 5, _Player.transform.position.z), 0.1f);
            Crown.transform.localScale = Vector3.Lerp(Crown.transform.localScale, GrowScale, 0.5f * Time.deltaTime);
        }
        
        if (SlamCrown) //Shoots a raycast down and quickly moves the crown to the ray point.
        {
            RaycastHit hit;
            Physics.Raycast(Crown.transform.position, -Crown.transform.up, out hit, Mathf.Infinity);
            if (hit.transform != null)
            {
                Crown.transform.position = Vector3.MoveTowards(Crown.transform.position, hit.point, 1f);
            }
        }
    }

    void Shoot()
    {
        Debug.LogError("Shoot Function called! :D");
        GameObject PooledBullet = ObjectPool.SharedInstance.GetPooledObject("SmallLazer");
        if (PooledBullet != null)
        {
            Debug.LogError("Pulles THingsoiauhfdsjangsd");
            PooledBullet.transform.position = _Firepoint.position;
            PooledBullet.transform.rotation = _Firepoint.rotation;
            PooledBullet.SetActive(true);
            if (PooledBullet.TryGetComponent<Rigidbody>(out Rigidbody bulletRB))
            {
                Debug.LogError("kaboom");
                // Clear lingering momentum from object pool reuse
                bulletRB.linearVelocity = Vector3.zero;
                bulletRB.angularVelocity = Vector3.zero;

                // Fire instantly with Impulse mode
                bulletRB.AddForce(_Firepoint.forward * _LazerSpeed, ForceMode.Impulse);
            }
        }
    }
    //Randomly selects one of the transforms in the TeleportPoint Aray, and sets the boss to that location.
    void Teleport()
    {
        EffectSpawner.SpawnEffect(transform.position, "Teleport");
        transform.position = _TeleportPoints[UnityEngine.Random.Range(0, _TeleportPoints.Length)].transform.position;
        EffectSpawner.SpawnEffect(transform.position, "Teleport");
    }

    //Slerps the rotation of the boss to slowly and smoothly look at the player. change _LookSpeed to change the speed.
    void LookAtPlayer()
    {
        Vector3 lookDirection = transform.position - _Player.transform.position;
        lookDirection.Normalize();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), _CurrentLookSpeed * Time.deltaTime);
    }

    IEnumerator Idle() //Boss doesnt do anything and waits before choosing a new action
    {
        Teleport();
        _BossState = BossState.Idle;
        yield return new WaitForSeconds(2);
        ChooseAction();
    }

    IEnumerator Lazer() //Activates the Lazer, and deactivates it after.
    {
        _CurrentLookSpeed = _LazerLookSpeed;
        LaserPoint.SetActive(true);
        yield return new WaitForSeconds(_LazerDuration);
        LaserPoint.SetActive(false);
        _CurrentLookSpeed = _NormalLookSpeed;
        yield return new WaitForSeconds(1);
        StartCoroutine(Idle());
    }

    IEnumerator CrownSlam() //Saves all the values of the crown, then unparents it. After its done, sets all values back.
    {
        TempScale = Crown.transform.localScale;
        TempParent = Crown.transform.parent;
        TempPos = Crown.transform.localPosition;
        TempRot = Crown.transform.localRotation;
        Debug.Log(TempParent);
        Debug.Log(Crown.transform.parent);
        MoveCrownToPlayer = true;
        Crown.transform.parent = null;

        yield return new WaitForSeconds(5); //Moves crown to player
        MoveCrownToPlayer = false;

        yield return new WaitForSeconds(0.2f); //Slams the crown onto the ground
        SlamCrown = true;
        yield return new WaitForSeconds(0.1f);
        EffectSpawner.SpawnEffect(Crown.transform.position, "DustExplosion");
        yield return new WaitForSeconds(0.4f); 
        
        if (Vector3.Distance(Crown.transform.position, _Player.transform.position) <= 5)
        {
            _Player.GetComponent<HealthManager>().UpdateHealth(2);
        }

        yield return new WaitForSeconds(5f); //Moves crown back to the boss
        SlamCrown = false;
        MoveCrownToBoss = true;

        yield return new WaitForSeconds(2); //Sets the data back to how it was
        Crown.transform.parent = TempParent;
        Crown.transform.localScale = TempScale;
        Crown.transform.localRotation = TempRot;
        Crown.transform.localPosition = TempPos;
        StartCoroutine(Idle());
    }

    IEnumerator LazerBurst()
    {
        _CurrentLookSpeed = _LazerLookSpeed;      
        Debug.LogError("Lazer Burstg Called! :D");
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        yield return new WaitForSeconds(_TimeBetweenShots);
        Shoot();
        _CurrentLookSpeed = _NormalLookSpeed;
        StartCoroutine(Idle());
    }

    //Randomly teleports the boss a few times
    IEnumerator Teleporting()
    {
        Teleport();
        yield return new WaitForSeconds(_TeleportDelay);

        Teleport();
        yield return new WaitForSeconds(_TeleportDelay);

        Teleport();
        yield return new WaitForSeconds(_TeleportDelay);

        Teleport();
        yield return new WaitForSeconds(_TeleportDelay);
        StartCoroutine(Idle());
    }
    IEnumerator Summon()
    {
        spawnCount = 0;
        while (spawnCount != _AmountToSpawn)
        {
            GameObject PooledEnemy =
                ObjectPool.SharedInstance.GetPooledObject("BOSSMINI");

            if (PooledEnemy != null)
            {
                var BossLocation = transform.position;
                Vector3 position = new Vector3(
                    UnityEngine.Random.Range(
                        BossLocation.x - _SpawnRange,
                        BossLocation.x + _SpawnRange
                    ),
                    0,
                    UnityEngine.Random.Range(
                        BossLocation.z - _SpawnRange,
                        BossLocation.z + _SpawnRange
                    )
                );

                PooledEnemy.transform.position = position;
                PooledEnemy.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
            }
            spawnCount++;
        }
        yield return new WaitForSeconds(5);
        StartCoroutine(Idle());
    }

    //Damage functiom, stripped down version of the one in the EnemyBrain
    void Damage()
    {
        if (healthManager != null)
        {
            healthManager._CurrentHealth--;
        }
        else
        {
            UnityEngine.Debug.LogError("no healthmanager");
        }
    }
    public void Death()
    {
        uiman.ToggleBossBar();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.transform.tag == "PlayerBullet")
            {
                print("HIT!" + collision.transform.name);
                Damage();
            }
        }
    }


}
