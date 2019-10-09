using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public float radio;
    public int damage;

    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radio);

        foreach (var e in colliders)
        {
            if (e.CompareTag("Enemy"))
            {
                e.GetComponentInParent<EnemyController>().TakeDamage(damage);
            }
            if (e.CompareTag("Player"))
            {
                e.GetComponent<PlayerController>().TakeDamage(20);
            }
        }
    }
}
