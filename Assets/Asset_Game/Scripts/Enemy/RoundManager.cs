using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoundManager : MonoBehaviour
{

    [SerializeField] int round = 0;
    [SerializeField] Vector2 random;
    [SerializeField] GameObject prefEnemy;
    [SerializeField] Vector2 randomPos;
    [SerializeField] int enemy_per_Round;



    bool next_Round;
    bool B;
    int enemys_in_map = 0;
    int enemy_live = 100;
    //Aumento De Enemigos Por Ronda
    int enemy_aument_in_Round = 12;
    //Aumento De Vida Por Ronda
    int live_aument_in_Round = 20;
    //Enemigos Istanciados
    int enemys_instaced;

    void Start()
    {
        next_Round = true;
        B = true;
    }
    private void Update()
    {
        if (enemys_instaced == enemy_per_Round && enemys_in_map == 0)
        {
            next_Round = true;
            B = true;
        }
        if(enemys_in_map == 0 && next_Round && B)
        {
            B = false;
            foreach (var item in GameObject.FindGameObjectsWithTag("Text_Rounds"))
            {
                
                item.GetComponent<Animator>().SetBool("Next", true);
            }
            Invoke("Restart", 5);
        }
    }


    private void Restart()
    {
        CancelInvoke("Restart");
        round++;
        next_Round = false;
        
        foreach (var item in GameObject.FindGameObjectsWithTag("Text_Rounds"))
        {
            item.GetComponent<TMPro.TextMeshProUGUI>().text = round + "";
            item.GetComponent<TMPro.TextMeshProUGUI>().GraphicUpdateComplete();
            item.GetComponent<Animator>().SetBool("Next", false);
        }
        enemys_in_map = 0;
        enemys_instaced = 0;
        enemy_per_Round += Random.Range(5, enemy_aument_in_Round);
        enemy_live += Random.Range(10, live_aument_in_Round);
        enemy_aument_in_Round += Random.Range(5, 10);
        live_aument_in_Round += Random.Range(5, 20);
        StartCoroutine(NextEnemy());
    }
    internal void ZombieDead()
    {
        enemys_in_map--;
    }

    IEnumerator NextEnemy()
    {
        while (enemys_instaced < enemy_per_Round)
        {
            float next = Random.Range(random.x, random.y);
            Vector3 newRandomPos = new Vector3(Random.Range(-randomPos.x, randomPos.x), 0, Random.Range(-randomPos.y, randomPos.y));
            Quaternion rotationInstance = Quaternion.Euler(new Vector3(0, Random.Range(-360, 360), 0)); 
            yield return new WaitForSeconds(next);
            GameObject enemy_instance = Instantiate(prefEnemy, newRandomPos, rotationInstance);
            enemy_instance.GetComponentInChildren<EnemyController>().SetLive(enemy_live);
            enemys_in_map++;
            enemys_instaced++;
        }
    }
}
