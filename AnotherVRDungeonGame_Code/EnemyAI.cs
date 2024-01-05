using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Attacks
    [SerializeField] bool isMagic = false;
    [SerializeField] MagicAttack[] groundedMagicAttacks;
    [SerializeField] MagicAttack[] airMagicAttacks;
    MagicAttack nextMagicAttack;

    //[SerializeField] bool isMelee = false;
    [SerializeField] MeleeAttack[] meleeAttacks;
    MeleeAttack nextMeleeAttack;
    [SerializeField] float walkRange = 5;
    [SerializeField] MeleeDamage weapon;

    [SerializeField] bool hybrid = false;
    private float meleeAttackRange;
    [SerializeField] GameObject trailFX;

    public NavMeshAgent agent;
    public Transform player;
    public Transform mainCamera;
    public LayerMask whatisGround, whatIsPlayer;
    EnemyHealth health;
    Animator animator;
    [SerializeField] float runSpeed;
    [SerializeField] float walkSpeed;

    [SerializeField] Transform sightSphere;
    bool alerted = false;
    [SerializeField] bool healthOverride = false;
    bool rotateDuringWait = false;
    bool shootingLaser;

    int runID;
    int walkID;

    // attacking
    bool attacking;
    [SerializeField] public Transform attackPos;
    [SerializeField] public Transform projectilePos;
    [SerializeField] float attackingRotateSeed = 5f;

    // states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInWalkRange, playerInAttackRange;

    private void Awake()
    {
        player = FindObjectOfType<PlayerHealth>().transform;
        mainCamera = Camera.main.transform;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();

        sightSphere.localPosition = new Vector3(0, 0, sightRange/2);

        runID = Animator.StringToHash("Run");
        walkID = Animator.StringToHash("Walk");

        meleeAttackRange = attackRange;
        if (FindObjectOfType<ArenaManager>()) { StartCoroutine(IdleWaitToActive()); }
    }

    private IEnumerator IdleWaitToActive()
    {
        yield return new WaitForSeconds(.8f);
        alerted = true;
        GetNextAttack();
    }

    private void Update()
    {
        if (healthOverride)
        {
            attacking = false;
            agent.SetDestination(transform.position);
            return;
        }

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!alerted) Idle();
        else if (attacking) AttackWait();
        else if (!playerInAttackRange) ChasePlayer();
        else if (playerInAttackRange) AttackPlayer();

        if (agent.destination == transform.position)
        {
            animator.SetBool(runID, false);
            animator.SetBool(walkID, false);
        }/*
        else if (isMagic || (!isMagic && !playerInAttackRange))
        {
            animator.SetBool(runID, true);
            animator.SetBool(walkID, false);
            agent.speed = runSpeed;
        } else
        {
            animator.SetBool(runID, false);
            animator.SetBool(walkID, true);
            agent.speed = walkSpeed;
        }*/
    }

    public void AlertOfPlayer()
    {
        if (alerted) return;
        StartCoroutine(IdleWaitToActive());
    }

    private void Idle()
    {
        agent.SetDestination(transform.position);

        playerInSightRange = Physics.CheckSphere
            (sightSphere.position, sightRange/2, whatIsPlayer);

        if (playerInSightRange) 
        { 
            alerted = true;
            GetNextAttack();
            FindObjectOfType<RoomManager>().currentRoom.AlertAllEnemies();
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        if (isMagic)
        {
            animator.SetBool(runID, true);
        }
        else
        {
            playerInWalkRange = Physics.CheckSphere(transform.position, walkRange, whatIsPlayer);
            if (playerInWalkRange)
            {
                animator.SetBool(runID, false);
                animator.SetBool(walkID, true);
                agent.speed = walkSpeed;
            } else
            {
                animator.SetBool(runID, true);
                animator.SetBool(walkID, false);
                agent.speed = runSpeed;
            }
        }
    }

    private void AttackWait()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        agent.SetDestination(transform.position);

        if (rotateDuringWait)
        {
            //transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            Vector3 direction = mainCamera.position - transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, attackingRotateSeed * Time.deltaTime);
        } else if (shootingLaser)
        {
            Vector3 direction = mainCamera.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, attackingRotateSeed/2 * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        Vector3 targetPos = new Vector3(mainCamera.position.x, transform.position.y, mainCamera.transform.position.z);
        transform.LookAt(targetPos);

        // attack code here
        if (isMagic)
        {
            if (nextMagicAttack.isGrounded)
            {
                GameObject attackObj = Instantiate
                    (nextMagicAttack.attackPrefab, attackPos.position, transform.rotation);
            } else
            {
                GameObject attackObj = Instantiate
                    (nextMagicAttack.attackPrefab, projectilePos.position, projectilePos.rotation);
                attackObj.transform.LookAt(mainCamera);

                if (nextMagicAttack.rotateFollowPlayer)
                {
                    shootingLaser = true;
                    attackObj.GetComponent<ProgressiveDamage>().SetLaser(mainCamera, attackingRotateSeed, transform);
                }
            }
            rotateDuringWait = false;
            animator.Play(nextMagicAttack.animName);
            Invoke(nameof(ResetAttack), nextMagicAttack.attackCooldown);
        } else
        {
            animator.Play(nextMeleeAttack.animName);
            rotateDuringWait = false;
            weapon.SetDamage(nextMeleeAttack.damage);
        }

        attacking = true;
        
        GetNextAttack();
    }

    public void ResetAttack()
    {
        attacking = false;
    }

    private void GetNextAttack()
    {
        if (hybrid)
        {
            int numAttacks = meleeAttacks.Length + groundedMagicAttacks.Length;
            isMagic = (Random.Range(0, numAttacks) + 1 > meleeAttacks.Length);
            if (!isMagic) attackRange = meleeAttackRange;
        }
        
        
        if (isMagic)
        {
            if (SameYLevel())
            {
                nextMagicAttack = groundedMagicAttacks[Random.Range(0, groundedMagicAttacks.Length)];
            } else
            {
                nextMagicAttack = airMagicAttacks[Random.Range(0, airMagicAttacks.Length)];
            }
            attackRange = nextMagicAttack.attackRange;
        }
        else
        {
            nextMeleeAttack = meleeAttacks[Random.Range(0, meleeAttacks.Length)];
        }
    }

    private bool SameYLevel()
    {
        return player.position.y >= transform.position.y - 2 && player.position.y <= transform.position.y + 2;
    }

    public void SetHealthOverride(bool b)
    {
        if (!health.isAlive) return;

        healthOverride = b;

        if (b)
        {
            rotateDuringWait = true;
            shootingLaser = false;
            animator.SetBool(runID, false);
            animator.SetBool(walkID, false);
            attacking = false;
            agent.SetDestination(transform.position);
            if (!isMagic) DisableeWeaponCollider();
        }
    }

    public void HealthOverrideRecovery()
    {
        healthOverride = false;
    }

    public void CanRotate()
    {
        rotateDuringWait = true;
    }

    public void DisableRotate() { rotateDuringWait = false; }

    public void EndLaser()
    {
        shootingLaser = false;
    }

    public void EnableWeaponCollider()
    {
        weapon.SetCollider(true);
        foreach (ParticleSystem p in trailFX.GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }
    }
    
    public void DisableeWeaponCollider()
    {
        weapon.SetCollider(false);
    }

    public void AttackParried()
    {
        animator.SetTrigger("Hit");
        SetHealthOverride(true);
    }
}
