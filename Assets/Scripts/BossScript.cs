using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//States the boss can be in, Set the state via code to execute the corresponding functions
//More states can be added if needed, and the names should be changed to describe the attack
public enum BossState
{
    Idle,
    Attack1,
    Attack2,
    Attack3,
    Summon,
    Teleporting
}

public class BossScript : MonoBehaviour
{
    public  GameObject LaserPoint;
    public float SpawnCount;
    public BossState _BossState;
    public GameObject _Player;
    public float _LookSpeed;
    public float SpawnRange;
    
    [Tooltip("Points the boss can teleport to")]
    public Transform[] _TeleportPoints;
    [Tooltip("Delay between teleports in the Teleport attack")]
    public float _TeleportDelay;

    public int randomNumber;

    HealthManager healthManager;

    private void Start()
    {
        healthManager = GetComponent<HealthManager>();
        ChooseAction();
    }
    public void OnEnable()
    {
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
                _BossState = BossState.Attack1;
                print(_BossState);
                break;

            case >= 3 and < 5:
                _BossState = BossState.Attack2;
                print(_BossState);
                break;

            case >= 5 and < 8:
                _BossState = BossState.Attack3;
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

            case BossState.Attack1:
                StartCoroutine(Attack1());
                break;

            case BossState.Attack2:
                StartCoroutine(Attack2());
                break;

            case BossState.Attack3:
                StartCoroutine(Attack3());
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
    }

    //Randomly selects one of the transforms in the TeleportPoint Aray, and sets the boss to that location.
    void Teleport()
    {
        transform.position = _TeleportPoints[UnityEngine.Random.Range(0, _TeleportPoints.Length)].transform.position;
    }

    //Slerps the rotation of the boss to slowly and smoothly look at the player. change _LookSpeed to change the speed.
    void LookAtPlayer()
    {
        Vector3 lookDirection = transform.position - _Player.transform.position;
        lookDirection.Normalize();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), _LookSpeed * Time.deltaTime);
    }
    IEnumerator Idle()
    {
        _BossState = BossState.Idle;
        yield return new WaitForSeconds(1);
        ChooseAction();
    }
    IEnumerator Attack1()
    {
        LaserPoint.SetActive(true);
        yield return new WaitForSeconds(6);
        LaserPoint.SetActive(false);
        yield return new WaitForSeconds(1);
        StartCoroutine(Idle());
    }

    IEnumerator Attack2()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(Idle());
    }

    IEnumerator Attack3()
    {
        yield return new WaitForSeconds(1);
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
        SpawnCount = 0;
        while(SpawnCount != 5)
        {
            GameObject PooledEnemy =
                ObjectPool.SharedInstance.GetPooledObject("BOSSMINI");
        
            if (PooledEnemy != null)
                {
                    var BossLocation = transform.position;
                    Vector3 position = new Vector3(
                        UnityEngine.Random.Range(
                            BossLocation.x - SpawnRange,
                            BossLocation.x + SpawnRange
                        ),
                        0,
                        UnityEngine.Random.Range(
                            BossLocation.z - SpawnRange,
                            BossLocation.z + SpawnRange
                        )
                    );

                    PooledEnemy.transform.position = position;
                    PooledEnemy.SetActive(true);
                }
            else
                {
                    Debug.LogWarning("Object Pool is empty! Expanding or waiting...");
                }
                SpawnCount++;
            }
        
        
        yield return new WaitForSeconds(1);
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
