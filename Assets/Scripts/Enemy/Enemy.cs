using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyType
{
    basic,
    fast,
    strong
}

public class Enemy : MonoBehaviour
{

    [Serializable]
    public struct Hitbox
    {
        public Collider collider;
        public float multiplier;
    }

    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyContainer parent;
    private GameObject target;
    private bool playerAggro;
    public float playerAggroDist;
    public float playerDeAggroDist;
    public float maxHealth;
    public float health;
    public GameObject player;
    public float attackRange;
    public float attackDamage;
    private float lastAttackTime;
    public float attackSpeed;
    public float attackDelay = 0.3f;
    public float totalAttackTime = 1f;
    public EnemyType type;
    public Animator enemyAnimator;
    public AudioClip gruntClip;
    private AudioSource source;
    public GameObject ragdollPrefab;

    public List<Hitbox> hitboxMultipliers;


    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void init(EnemyContainer parent, EnemyType type)
    {
        this.parent = parent;
        this.type = type;
        target = this.parent.getTarget();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        lastAttackTime = Time.time;
        if (target != null)
        {
            agent.destination = target.transform.position;
        }

        health = maxHealth;
    }

    public void damage(float damage, Collider hitCollider = null)
    {
        float multiplier = 1f;

        if (hitCollider != null)
        {
            foreach (Hitbox h in hitboxMultipliers)
            {
                if (h.collider == hitCollider)
                {
                    multiplier = h.multiplier;
                    break;
                }
            }
        }


        source.PlayOneShot(gruntClip);
        health -= damage * multiplier;

        if (health <= 0)
        {
            kill();
        }
    }

    private bool inRangeOfTarget()
    {
        return Vector3.Distance(transform.position, agent.destination) < attackRange;
    }

    private void attackTarget()
    {
        if (Time.time < lastAttackTime + attackSpeed || target == null)
        {
            return;
        }
        source.PlayOneShot(gruntClip);
        enemyAnimator.SetTrigger("Attack");
        // if (target.tag == "Target") {
        //     target.GetComponent<Heart>().damage(attackDamage);
        // } else if (target.tag == "Player") {
        //     target.GetComponent<PlayerController>().Damage(attackDamage);
        // }
        if (target.CompareTag("Target"))
        {
            StartCoroutine(AttackHeart(target.GetComponent<Heart>()));
        }
        else if (target.CompareTag("Player"))
        {
            StartCoroutine(AttackPlayer(target.GetComponent<PlayerController>()));
        }

        lastAttackTime = Time.time;
    }

    private bool switchPlayerAggro()
    {
        if (!playerAggro)
        {
            return Vector3.Distance(
                transform.position, player.transform.position) < playerAggroDist;
        }
        else
        {
            return Vector3.Distance(
                transform.position, player.transform.position) > playerDeAggroDist;
        }
    }

    private void kill()
    {
        Instantiate(ragdollPrefab, transform.position, transform.rotation);

        player.GetComponent<PlayerController>().notifyEnemyKilled(type);
        parent.removeEnemy(gameObject);
        Destroy(gameObject);
    }

    void Update()
    {
        if (playerAggro)
        {
            target = player;
            agent.destination = player.transform.position;
        }

        if (switchPlayerAggro())
        {
            playerAggro = !playerAggro;
            if (!playerAggro)
            {
                target = parent.getTarget();
                if (target != null)
                {
                    agent.destination = target.transform.position;
                }
            }
        }
        if (inRangeOfTarget())
        {
            attackTarget();
        }
    }

    IEnumerator AttackHeart(Heart heart)
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(attackDelay);

        if (heart != null)
        {
            heart.damage(attackDamage);
        }

        yield return new WaitForSeconds(totalAttackTime - attackDelay);

        agent.isStopped = false;
    }

    IEnumerator AttackPlayer(PlayerController player)
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(attackDelay);

        Ray ray = new Ray(transform.position, player.transform.position - transform.position);

        RaycastHit[] hits = Physics.RaycastAll(ray, attackRange, LayerMask.GetMask("Default", "Player"));

        if (hits.Length > 0)
        {
            float closestDist = Mathf.Infinity;
            GameObject closestObj = null;

            foreach (RaycastHit hit in hits)
            {
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    closestObj = hit.transform.gameObject;
                }
            }

            if (closestObj == player.gameObject)
            {
                player.Damage(attackDamage);
            }
        }

        // if (inRangeOfTarget())
        // {
        //     player.Damage(attackDamage);
        // }

        yield return new WaitForSeconds(totalAttackTime - attackDelay);

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    void FixedUpdate()
    {
        if (Random.Range(0.0f, 1.0f) > 0.999f)
        {
            source.PlayOneShot(gruntClip);
        }
    }
}
