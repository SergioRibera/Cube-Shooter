using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectilWeaponNetwork : MonoBehaviour
{
    [SerializeField] float radius = 0.02f;
    [SerializeField] int damage;
    [SerializeField] GameObject explosion;
    float velocity;
    PlayerControllerNetwork player;
    bool move = false;
    Vector3 forw;
    bool damagePlayer;

    internal void Init(int d, PlayerControllerNetwork p, float v, Color c, bool dP = false)
    {
        player = p;
        damage = d;
        forw = p.transform.forward;
        velocity = v;
        move = true;
        Material m = new Material(GetComponent<MeshRenderer>().materials[0]);
        m.color = c;
        damagePlayer = dP;
        GetComponent<MeshRenderer>().materials[0] = m;
        GetComponent<TrailRenderer>().materials[0] = m;
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        if (!move) return;
        transform.Translate(forw * velocity * Time.deltaTime);
        Collider[] colisions = Physics.OverlapSphere(transform.position, radius);
        if (colisions.Length > 0)
        {
            foreach (var item in colisions)
            {
                if (item.CompareTag("Enemy"))
                {
                    item.GetComponentInParent<EnemyController>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                if (item.CompareTag("Player"))
                {
                    if (damagePlayer)
                    {
                        item.GetComponentInParent<PlayerControllerNetwork>().TakeDamage(damage);
                        Destroy(gameObject);
                    }
                }
                if (item.CompareTag("Explodes"))
                {
                    Instantiate(explosion, item.transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }

        }
    }
}