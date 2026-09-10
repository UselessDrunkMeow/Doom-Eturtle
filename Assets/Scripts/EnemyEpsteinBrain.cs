using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEpsteinBrain : MonoBehaviour
{
    public float _Speed;
    public float _AttackRange;
    public float _AttackCooldown;
    public float _DamageStun;
    public LayerMask _LayerMask;

    HealthManager healthManager;
    Material color;
    NavMeshAgent agent;
    Transform playerPos;
    float distance;
    bool hitPlayer;
    public bool Death = false;

    void OnEnable()
    {
        healthManager = GetComponent<HealthManager>();
        healthManager.enabled = true;
        Death = false;
    }
    void Start()
    {
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Run");
        playerPos = FindAnyObjectByType<PlayerShoot>().transform;
        agent = GetComponent<NavMeshAgent>();
        //color = GetComponentInChildren<MeshRenderer>().material;
        healthManager = GetComponent<HealthManager>();
        agent.speed = _Speed;
    }

    void GoToPlayer()
    {
        if(Death == false)
        {
            agent.SetDestination(playerPos.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }
    void Update()
    {   //Set agent desitnation to the players possition every frame so it can chase it.
        GoToPlayer();

        //Checks the distance between the enemy and the player, and if its close enough, it will attack
        distance = Vector3.Distance(transform.position, playerPos.position);
        if (distance <= _AttackRange)
        {
            StartCoroutine(Attack());
        }
        Debug.DrawRay(transform.transform.position, transform.forward * _AttackRange, UnityEngine.Color.purple);
    }
    public void Chase()
    {
        //color.color = UnityEngine.Color.red;
        agent.isStopped = false;
    }

    //Freezes the enemy in place as it attacks, allowing it to move again after a short cooldown.
    IEnumerator Attack()
    {
        RaycastHit hit;

        agent.isStopped = true;
        //color.color = UnityEngine.Color.yellow;
        yield return new WaitForSeconds(_AttackCooldown / 2);
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("RandomAttack");
        //color.color = UnityEngine.Color.orange;

        hitPlayer = Physics.Raycast(transform.position, transform.forward, out hit, _AttackRange, _LayerMask);
        if (hitPlayer)
        {
            print(" WAafsdfhsigdsiughsdjghdsjghjiWGRUGDIFUR^ST%RFUNJMESXHNUJDEFRHJUDEFRHNJUK");
        }
        yield return new WaitForSeconds(_AttackCooldown / 2);

        Chase();
    }

    //Deletes the enemy after its HP reaches 0
    public void onDeath()
    {
        Death = true;
        healthManager.enabled = false;
        StartCoroutine(DeathCoroutine());
    }
    private IEnumerator DeathCoroutine()
    {
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Death");

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }

    IEnumerator Damage()
    {
        if (healthManager.enabled != false)
        {
            healthManager._CurrentHealth--;
            gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("TakeDamage");
            //color.color = UnityEngine.Color.darkRed;
            agent.isStopped = true;
            yield return new WaitForSeconds(_DamageStun);
            //color.color = UnityEngine.Color.red;
            agent.isStopped = false;
        }
        else
        {
            UnityEngine.Debug.LogError("no healthmanager");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            print(other.transform.name);
            if (other.transform.tag == "PlayerBullet")
            {
                print("HIT!" + other.transform.name);
                StartCoroutine(Damage());
            }
        }
    }  
}
