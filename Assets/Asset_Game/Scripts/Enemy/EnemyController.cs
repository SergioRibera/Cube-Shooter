using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int vida;
    [SerializeField][Range(0f, 5f)] float distance = 1.2f;
    [SerializeField][Range(0f, 5f)] float velocity = 1.7f;
    [SerializeField][Range(0f, 5f)] internal float timeBettwenAttack = 0.5f;
    [SerializeField][Range(0, 100)] int damage = 2;
    [SerializeField] Slider barraVida;

    public int idle_x;
    public int idle_y;
    public int walk_x;
    public int walk_y;

    internal bool move;
    Animator anim;
    GameObject canvasVida;
    internal Transform player;
    NavMeshAgent nav;
    bool playerInRange;
    bool Dead;
    [SerializeField] internal bool isAttacked = true;
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    RoundManager roundManager;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = velocity;
        nav.stoppingDistance = distance;
        canvasVida = transform.GetChild(0).gameObject;
        barraVida.maxValue = vida;
        isAttacked = true;
        roundManager = FindObjectOfType<RoundManager>();
        Idle();

    }

    void FixedUpdate()
    {
        if (!move) return;
        if (player == null) return;

        if (!Dead && !playerInRange) Walk();
        RaycastHit hit;

        //Physics.OverlapSphere(transform.position, transform.forward, out hit, 2f)

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Player") && isAttacked)
            {
                if (Vector3.Distance(hit.collider.transform.position, transform.position) <= distance && !Dead)
                {
                    AttackAnimation();
                    playerInRange = true;
                    move = false;
                    isAttacked = false;
                }
            }
            else
            {
                move = true;
                playerInRange = false;
                //Attacked = false;
            }
        }
    }

    void Idle()
    {
        move = false;
        idle_x = Random.Range(-1, 0);
        idle_y = Random.Range(-1, 1);

        anim.SetBool("walk", move);
        anim.SetFloat("random_idle_x", idle_x);
        anim.SetFloat("random_idle_y", idle_y);
    }
    void Walk()
    {
        walk_x = Random.Range(0, 1);
        walk_y = Random.Range(-1, 1);

        nav.SetDestination(player.position);
        nav.isStopped = false;
        anim.SetBool("walk", move);
        anim.SetFloat("random_walk_x", walk_x);
        anim.SetFloat("random_walk_y", walk_y);
    }
    void AttackAnimation()
    {
        int attack_x = Random.Range(0, 1);
        //int attack_y = Random.Range(0, 1);
        anim.SetBool("Attack", true);
        anim.SetFloat("random_attack_x", attack_x);
    }
    internal void Atack()
    {
        if (player != null)
            player.GetComponent<PlayerController>().TakeDamage(damage);
        StartCoroutine(SiguienteAtaque(timeBettwenAttack));
    }

    IEnumerator SiguienteAtaque(float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool("Attack", false);
        isAttacked = true;
    }
    void Die()
    {
        anim.SetBool("Dead", true);
        nav.enabled = false;
        Dead = true;
        FindObjectOfType<RoundManager>().ZombieDead();
        GetComponentInChildren<CapsuleCollider>().enabled = false;
        Invoke("SpawnObjectToDie", 3.9f);
        Destroy(gameObject, 4f);
    }

    bool primerDano;
    internal void TakeDamage(int d)
    {
        vida -= d;
        if (vida <= 0) { vida = 0; Die(); }
        barraVida.value = vida;
        if (!primerDano) primerDano = true;
        canvasVida.SetActive(primerDano);
    }
    internal void SetLive(int l)
    {
        vida = l;
    }
    void SpawnObjectToDie()
    {
        int indexObject = Random.Range(0, objects.Count * 2);
        if(objects[indexObject])
        {
            Instantiate(objects[indexObject], transform.position, Quaternion.identity);
        }
    }
}
