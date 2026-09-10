using StarterAssets;
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
    public float _AttackDelay;
    public int _Damage;
    public LayerMask _LayerMask;

    HealthManager healthManager;
    Material color;
    NavMeshAgent agent;
    Transform playerPos;
    float distance;
    bool hitPlayer;
    bool isAttacking;

    void OnEnable()
    {
        healthManager = GetComponent<HealthManager>();
        healthManager.enabled = true;
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

    void Update()
    {   //Set agent desitnation to the players possition every frame so it can chase it.
        agent.SetDestination(playerPos.position);

        //Checks the distance between the enemy and the player, and if its close enough, it will attack
        distance = Vector3.Distance(transform.position, playerPos.position);
        if (distance <= _AttackRange && !isAttacking)
        {
            StartCoroutine(Attack());
        }
        Debug.DrawRay(transform.transform.position, transform.forward * _AttackRange, UnityEngine.Color.purple);
    }
    public void Chase()
    {
        //color.color = UnityEngine.Color.red;
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Run");
        agent.isStopped = false;
    }

    //Freezes the enemy in place as it attacks, allowing it to move again after a short cooldown.
    [Header("Attack Box Settings")]
    [SerializeField] private Vector3 _AttackBoxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    IEnumerator Attack()
    {
        isAttacking = true;
        RaycastHit hit;
        agent.isStopped = true;
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("RandomAttack");
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Run");
        yield return new WaitForSeconds(_AttackDelay);
        hitPlayer = Physics.BoxCast(transform.position, _AttackBoxHalfExtents, transform.forward, out hit, transform.rotation, _AttackRange, _LayerMask);
        print(hit.transform.name);
        if (hitPlayer && hit.transform == playerPos)
        {
            hit.transform.gameObject.GetComponent<HealthManager>().UpdateHealth(_Damage);
        }
        yield return new WaitForSeconds(_AttackCooldown);
        isAttacking = false;
        Chase();
    }

    //Deletes the enemy after its HP reaches 0
    public void onDeath()
    {
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
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;

        // Draw the starting box
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _AttackBoxHalfExtents * 2f);
        Gizmos.matrix = oldMatrix;

        // Draw the end box (where the cast finishes)
        Vector3 endPosition = transform.position + transform.forward * _AttackRange;
        Gizmos.matrix = Matrix4x4.TRS(endPosition, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _AttackBoxHalfExtents * 2f);
        Gizmos.matrix = oldMatrix;

        // Draw a line connecting the two boxes to show the cast path
        Gizmos.color = UnityEngine.Color.yellow;
        Gizmos.DrawLine(transform.position, endPosition);
    }
}
