using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public float _Speed;
    public float _AttackRange;
    public float _AttackCooldown;
    Material color;
    NavMeshAgent agent;
    Transform playerPos;
    float distance;

    void Start()
    {
        playerPos = FindAnyObjectByType<PlayerShoot>().transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = _Speed;
        color = GetComponentInChildren<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {        
        agent.SetDestination(playerPos.position);
        distance = Vector3.Distance(transform.position, playerPos.position);
        if(distance <= _AttackRange)
        {
            StartCoroutine(Attack());
        }
    }
    public void Chase()
    {
        color.color = UnityEngine.Color.red;
        agent.isStopped = false;
    }
    IEnumerator Attack()
    {
        agent.isStopped = true;
        color.color = UnityEngine.Color.yellow;
        yield return new WaitForSeconds(_AttackCooldown);
        Chase();
    }

    public void onDeath()
    {
        Destroy(this.gameObject);
    }
}
