using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilWeapon : MonoBehaviour
{
    [SerializeField] float radius = 0.02f;
    [SerializeField] int damage;
    [SerializeField] GameObject explosion;
    float velocity;
    PlayerController player;
    bool move = false;
    Vector3 forw;

    internal void Init(int d, PlayerController p, float v)
    {
        player = p;
        damage = d;
        forw = p.transform.forward;
        velocity = v;
        move = true;
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        if (!move) return;
        transform.Translate(forw * velocity * Time.deltaTime);
        Collider[] colisions = Physics.OverlapSphere(transform.position, radius);
        if(colisions.Length > 0)
        {
            foreach (var item in colisions)
            {
                if (item.CompareTag("Enemy"))
                {
                    item.GetComponentInParent<EnemyController>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                if (item.CompareTag("Explodes"))
                {
                    Instantiate(explosion, item.transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, radius);
    }*/
}
