using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask WhatisGround, WhatisPlayer;

    //Patrolling
    public Vector3 WalkPoint;
    bool WalkPointSet;
    public float WalkPointRange;
    

    //Atacking
    public float TimeBetweenAttacks;
    bool AlreadyAttacked;

    //States
    public float SightRange, AttackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;


    public Animator Anim;
    public AudioSource Sound;
    public AudioClip Death;
    public int MaxHealth = 100;
    int CurrentHealth;


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        
    }

    void Start()
    {
        Sound = gameObject.GetComponent<AudioSource>();
        CurrentHealth = MaxHealth;
    }


    private void Update()
    {
        //Check if player is in sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, WhatisPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, WhatisPlayer);

        if(!PlayerInSightRange && !PlayerInAttackRange)
        {
            Patrolling();
           
        }
        else if(PlayerInSightRange && !PlayerInAttackRange)
        {
            ChasePlayer();
           
        }
        else if(PlayerInSightRange && PlayerInAttackRange)
        {
            AttackPlayer();
        }
        
    }

    private void Patrolling()
    {
        if (!WalkPointSet)
        {
            
            StartCoroutine(ExecuteAfterTime(5.5f));
        }
        if (WalkPointSet)
        {
            agent.SetDestination(WalkPoint);
            Anim.SetBool("isIdle", false);
            Anim.SetBool("isWalking", true);
            agent.speed = 2;
        }
        Vector3 DistanceToWalkPoint = transform.position - WalkPoint;

        //Walkpoint reached
        if(DistanceToWalkPoint.magnitude < 1f)
        {
            WalkPointSet = false;
            Anim.SetBool("isIdle", true);
            Anim.SetBool("isWalking", false);
            Anim.SetBool("isRunning", false);
        }
        
    }

  

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        float RandomZ = Random.Range(-WalkPointRange, WalkPointRange);
        float RandomX = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(transform.position.x + RandomX, transform.position.y, transform.position.z + RandomZ);
        if (Physics.Raycast(WalkPoint, -transform.up, 2f, WhatisGround))
        {
            WalkPointSet = true;
        }
    }


    private void ChasePlayer()
    {
        Anim.SetBool("isIdle", false);
        Anim.SetBool("isWalking", false);
        Anim.SetBool("isRunning", true);
        agent.speed = 3;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesnt move
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!AlreadyAttacked)
        {
            AlreadyAttacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        AlreadyAttacked = false;
    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        Anim.SetTrigger("Hurt");
        if(CurrentHealth <= 0)
        {
            Die();
        }
    }


    void Die()
    {
       
        Anim.SetBool("isDead", true);
        Sound.PlayOneShot(Death, 1f);
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }

}
