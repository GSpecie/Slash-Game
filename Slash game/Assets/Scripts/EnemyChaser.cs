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

    [SerializeField] private float currentHealth = 3;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;

    [SerializeField] private float moveSpeed = 2;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_collider;
    [SerializeField] private float impactIntensity;

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
                Dead();
                break;
        }
    }

    private void Update()
    {
        ResetForces();
    }

    void Dead()
    {
        m_collider.enabled = false;
        m_animator.SetTrigger("death");
    }

    void Idle()
    {
    }

    void Berserk()
    {
        Vector3 dir = m_player.transform.position - transform.position;
        //agent.SetDestination(m_player.transform.position);

        if(Vector3.Distance(transform.position, m_player.transform.position) > 1)
        {
            rb.velocity = externalForce + transform.forward * moveSpeed;
            transform.LookAt(m_player.transform.position);
        }
    }

    public void TakeDamage(float damage, Vector3 ImpactValue)
    {
        if (Time.time - lastDamageTime > 0.15f)
        {
            Debug.Log("tomei dnao");
            currentHealth -= damage;
            m_animator.SetTrigger("takeDamage");
            externalForce += ImpactValue;
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                //die
                zStates = EnemyStates.Dead;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, (collision.gameObject.transform.position - transform.position).normalized * impactIntensity);

            Debug.Log("colidiu com : " + collision.gameObject.name);
        }
        else
        {
            Debug.Log("colisor não rotulado: " + collision.gameObject.name, collision.gameObject);
            Debug.Log("colidiu com : " + collision.gameObject.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, (collision.gameObject.transform.position - transform.position).normalized * impactIntensity);

            Debug.Log("colidiu com : " + collision.gameObject.name);
        }
        else
        {
            Debug.Log("colisor não rotulado: " + collision.gameObject.name, collision.gameObject);
            Debug.Log("colidiu com : " + collision.gameObject.name);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("UserWeapon"))
        {
            PlayerWeapon weapon = other.gameObject.GetComponent<PlayerWeapon>();
            TakeDamage(weapon.M_damage, (transform.position - other.gameObject.GetComponent<PlayerWeapon>().M_player.transform.position).normalized * weapon.ImpactIntensity);
        }
    }

    private void ResetForces()
    {
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
    }
}
