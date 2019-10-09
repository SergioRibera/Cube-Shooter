using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendEvent : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    public void Attack()
    {
        enemyController.Atack();
    }
}
