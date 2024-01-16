using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    public GameObject spawnedEnemyAfterDie;


    public Animator AnimatorController;
    public NavMeshAgent Agent;
    private Player player;

    private float lastAttackTime = 0;
    private bool isDead = false;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        SceneManager.Instance.AddEnemie(this);
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);

    }

    private void Update()
    {
        if(isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            player.Hp += 1;
            Agent.isStopped = true;
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
     
        if (distance <= AttackRange)
        {
            Agent.isStopped = true;
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                SceneManager.Instance.Player.Hp -= Damage;
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        AnimatorController.SetFloat("Speed", Agent.speed); 
        //Debug.Log(Agent.speed);

    }



    private void Die()
    {
        if (gameObject.tag == "GigaGoblin")
        {
            GameObject smallEnemy1 = Instantiate(spawnedEnemyAfterDie, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            GameObject smallEnemy2 = Instantiate(spawnedEnemyAfterDie, transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
        }
        SceneManager.Instance.RemoveEnemie(this);
        isDead = true;
        AnimatorController.SetTrigger("Die");
        
    }

}
