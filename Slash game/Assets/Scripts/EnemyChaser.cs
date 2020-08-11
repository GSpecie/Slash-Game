using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public enum EnemyStates { Idle, Berserk, Dead };
    public EnemyStates zStates;

    //GameObject brain;
    //[SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;

    [SerializeField] Player m_player;

    [SerializeField] private float damage = 1;

    [SerializeField] private float currentHealth = 1;
    [SerializeField] private float maxHealth = 1;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;

    [SerializeField] private float moveSpeed = 2;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_collider;
    [SerializeField] private float impactIntensity;

    [SerializeField] private ScoreGameManager gameManager;

    // Use this for initialization
    void Start()
    {
        zStates = EnemyStates.Berserk;
    }
    private void FixedUpdate()
    {
        switch (zStates)
        {
            case (EnemyStates.Idle):
                Idle();
                break;
            case (EnemyStates.Berserk):
                Berserk();
                break;
            case (EnemyStates.Dead):
                //Dead();
                break;
        }
    }

    private void Update()
    {
        ResetForces();
    }

    private void OnEnable()
    {
        Reborn();
    }

    public void Reborn()
    {
        Debug.Log("renasci");
        currentHealth = maxHealth;
        m_collider.enabled = true;
        m_animator.SetTrigger("reborn");
        zStates = EnemyStates.Berserk;
    }

    void Dead()
    {
        zStates = EnemyStates.Dead;
        m_collider.enabled = false;
        m_animator.SetTrigger("death");
        gameManager.AddScore();
        gameManager.RespawnEnemy(this);
    }

    void Idle()
    {
    }

    void Berserk()
    {
        Vector3 dir = m_player.transform.position - transform.position;
        //agent.SetDestination(m_player.transform.position);

        if (Vector3.Distance(transform.position, m_player.transform.position) > 1)
        {
            rb.velocity = externalForce + transform.forward * moveSpeed;

            Vector3 target;
            target.x = m_player.transform.position.x;
            target.z = m_player.transform.position.z;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }

    public void TakeDamage(float damage, Vector3 ImpactValue)
    {
        if (Time.time - lastDamageTime > 0.15f)
        {
            Debug.Log("tomei dnao");
            currentHealth -= damage;
            if (currentHealth > 0) m_animator.SetTrigger("takeDamage");
            externalForce += ImpactValue;
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                //die
                Dead();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, (collision.gameObject.transform.position - transform.position).normalized * impactIntensity);

            //Debug.Log("colidiu com : " + collision.gameObject.name);
        }
        else
        {
            //Debug.Log("colisor não rotulado: " + collision.gameObject.name, collision.gameObject);
            //Debug.Log("colidiu com : " + collision.gameObject.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, (collision.gameObject.transform.position - transform.position).normalized * impactIntensity);

            //Debug.Log("colidiu com : " + collision.gameObject.name);
        }
        else
        {
            //Debug.Log("colisor não rotulado: " + collision.gameObject.name, collision.gameObject);
            //Debug.Log("colidiu com : " + collision.gameObject.name);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("UserWeapon"))
        {
            PlayerWeapon weapon = other.gameObject.GetComponent<PlayerWeapon>();
            TakeDamage(weapon.M_damage, (transform.position - other.gameObject.GetComponent<PlayerWeapon>().M_player.transform.position).normalized * weapon.ImpactIntensity);
        }
        else if (other.gameObject.tag.Contains("AirBullet"))
        {
            AirBullet bullet = other.gameObject.GetComponent<AirBullet>();
            TakeDamage(bullet.M_damage, (transform.position - other.gameObject.transform.position).normalized * bullet.ImpactIntensity);
        }
    }

    private void ResetForces()
    {
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
    }
}
