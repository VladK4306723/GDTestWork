using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float HeavyAtackSpeed;
    public float AttackRange = 2f;
    public float speed = 2f;

    private float lastAttackTime = 0;
    private float lastHeavyAttackTime = 0;
    private bool isHeavyAttack;
    private bool isSingleAttack;
    private bool isDead = false;
    public Animator AnimatorController;

    public bool IsHeavyAttack
    {
        get { return isHeavyAttack; }
        set { isHeavyAttack = value; }
    }
    public bool IsSingleAttack
    {
        get { return isSingleAttack; }
        set { isSingleAttack = value; }
    }


    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        Move();


        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }

        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Attack(closestEnemie);
        }

        if (Input.GetKeyDown(KeyCode.Equals) && closestEnemie != null && Vector3.Distance(transform.position, closestEnemie.transform.position) <= AttackRange)
        {
            HeavyAttack(closestEnemie);
        }
    }

    private void Attack(Enemie enemy)
    {
        if (Time.time - lastAttackTime > AtackSpeed)
        {
            isSingleAttack = true;
            lastAttackTime = Time.time;
            AnimatorController.SetTrigger("Attack");
            
            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= AttackRange)
            {
                enemy.Hp -= Damage;
                transform.LookAt(enemy.transform);
                transform.transform.rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
            }
            
        }
    }

    private void HeavyAttack(Enemie enemy)
    {
        if (Time.time - lastHeavyAttackTime > HeavyAtackSpeed)
        {
            isHeavyAttack = true;
            lastHeavyAttackTime = Time.time;
            AnimatorController.SetTrigger("HeavyAttack");

            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= AttackRange)
            {
                enemy.Hp -= Damage * 2;
                transform.LookAt(enemy.transform);
                transform.transform.rotation = Quaternion.LookRotation(enemy.transform.position - transform.position);
            }
        }
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        movement.Normalize();

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 1000f);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }


}