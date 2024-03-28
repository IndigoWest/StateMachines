using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent ai;

    public Transform patrolPoint;

    EnemyState enemyState;
    private Animator anim;
    private float distanceToTarget;
    private Coroutine idleToPatrol;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NavMeshAgent>();

        enemyState = EnemyState.Idle;
        anim = GetComponent<Animator>();
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));

        StartCoroutine(SwitchToPatrol());
    }

    // Update is called once per frame
    void Update()
    {
       //ai.SetDestination(target.position); 
       distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
       switch (enemyState)
        {
            case EnemyState.Idle:
                SwitchState(0);
                ai.SetDestination(transform.position);
                if (idleToPatrol == null)
                {
                    idleToPatrol = StartCoroutine(SwitchToPatrol());
                }
                break;
            case EnemyState.Patrol:
                float distanceToPatrolPoint = Mathf.Abs(Vector3.Distance(patrolPoint.position, transform.position));
                if (distanceToPatrolPoint > 2)
                {
                    SwitchState(1);
                    ai.SetDestination(patrolPoint.position);
                }
                else
                {
                    SwitchState(0);
                }
                if (distanceToTarget <= 15)
                {
                    enemyState = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                SwitchState(2);
                if (distanceToTarget <= 5)
                {
                    enemyState = EnemyState.Attack;
                }
                else if (distanceToTarget >= 15)
                {
                    enemyState = EnemyState.Idle;
                }
                ai.SetDestination(target.position);
                break;
            case EnemyState.Attack:
                SwitchState(3);
                if (distanceToTarget > 5 && distanceToTarget <= 15)
                {
                    enemyState = EnemyState.Chase;
                }
                else if (distanceToTarget > 15)
                {
                    enemyState= EnemyState.Idle;
                }
                break;
            default:
                break;
        }
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    IEnumerator SwitchToPatrol()
    {
        yield return new WaitForSeconds(5);
        enemyState = EnemyState.Patrol;
        idleToPatrol = null;
    }

    private void SwitchState(int newState)
    {
        if (anim.GetInteger("State") != newState)
        {
            anim.SetInteger("State", newState);
        }
    }
}
