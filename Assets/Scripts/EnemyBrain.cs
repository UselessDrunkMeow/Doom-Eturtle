using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public float _Speed;
    public float _AttackRange;
    public float _AttackCooldown;
    public float _DamageStun;
    public float _AttackDelay;
    public int _Damage;
    public LayerMask _LayerMask;
    public bool _Death = false;
    public Transform damageVFXPoint;

    HealthManager healthManager;
    Material color;
    NavMeshAgent agent;
    Transform playerPos;
    float distance;
    bool hitPlayer;
    bool isAttacking;

    [Header("Pathfinding Throttle")]
    [Tooltip("How often (seconds) the enemy recalculates its path to the player.")]
    [SerializeField] private float _RepathInterval = 0.25f;
    [Tooltip("Only repath if the player moved at least this far since the last path.")]
    [SerializeField] private float _RepathMinPlayerMove = 0.5f;
    float repathTimer;
    Vector3 lastTargetPos = new Vector3(float.MaxValue, 0, 0);
    void OnEnable()
    {
        healthManager = GetComponent<HealthManager>();
        healthManager.enabled = true;
        _Death = false;
        repathTimer = UnityEngine.Random.Range(0f, _RepathInterval);
        lastTargetPos = new Vector3(float.MaxValue, 0, 0);
    }
    void Start()
    {
        EffectSpawner.SpawnEffect(transform.position, "Teleport");
        playerPos = FindAnyObjectByType<PlayerShoot>().transform;
        agent = GetComponent<NavMeshAgent>();   
        healthManager = GetComponent<HealthManager>();
        agent.speed = _Speed;
    }
    void GoToPlayer()
    {
        if (_Death == false)
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
        // Recalculating a NavMesh path every frame for every enemy is what eats the CPU.
        // Only repath a few times per second, and only when the player actually moved.
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = _RepathInterval;
            if ((playerPos.position - lastTargetPos).sqrMagnitude >= _RepathMinPlayerMove * _RepathMinPlayerMove)
            {
                lastTargetPos = playerPos.position;
                agent.SetDestination(playerPos.position);
            }
        }

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
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Run");
        agent.isStopped = false;
    }

    [Header("Attack Box Settings")]
    [SerializeField] private Vector3 _AttackBoxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    //Freezes the enemy in place as it attacks, allowing it to move again after a short cooldown.
    IEnumerator Attack()
    {
        isAttacking = true;
        RaycastHit hit;
        agent.isStopped = true;

        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Attack");
        gameObject.GetComponent<PlayAnimation>().anim.SetBool("IsRunning", false);

        yield return new WaitForSeconds(_AttackDelay);

        hitPlayer = Physics.BoxCast(transform.position, _AttackBoxHalfExtents, transform.forward, out hit, transform.rotation, _AttackRange, _LayerMask);

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
        _Death = true;
        healthManager.enabled = false;        
        StartCoroutine(DeathCoroutine());

    }
    private IEnumerator DeathCoroutine()
    {
        gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("Death");

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        EffectSpawner.SpawnEffect(transform.position, "Jefsplosion"); 
    }

    IEnumerator Damage()
    {
        if (healthManager != null)
        {
            healthManager._CurrentHealth--;
            gameObject.GetComponent<PlayAnimation>().PlayAnimationFunction("TakeDamage");
            EffectSpawner.SpawnEffect(damageVFXPoint.position, "DamageVFX");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.transform.tag == "PlayerBullet")
            {
                print("HIT!" + collision.transform.name);
                StartCoroutine(Damage());
            }
        }
    }
    private void OnDrawGizmos() //Made with AI cuz IDK how ANY of this works XD
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
