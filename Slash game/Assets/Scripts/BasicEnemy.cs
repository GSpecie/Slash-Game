using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private Vector3 velocityLastFrameBeforeHit;
    private Vector3 direction;

    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float damage = 1;

    [SerializeField] private float currentHealth = 3;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;

    [SerializeField] private float impactIntensity;

    private bool dead;
    [SerializeField] private Collider m_collider;

    [SerializeField] private Animator m_animator;

    private void Start()
    {
        rb.velocity = externalForce + transform.forward * moveSpeed;
    }

    private void OnEnable()
    {
        rb.velocity = externalForce + transform.forward * moveSpeed;
    }

    void Update()
    {
        ResetForces();

        Move();

        velocityLastFrameBeforeHit = rb.velocity;

        if (dead == true)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void Move()
    {
        if(direction == Vector3.zero) rb.velocity = externalForce + transform.forward * moveSpeed;
        else rb.velocity = externalForce + direction * moveSpeed;

    }

    private void OnCollisionEnter(Collision collision)
    {
        //moveSpeed = Mathf.Clamp(moveSpeed, 2f, 8f);

        if (collision.gameObject.tag.Contains("Wall"))
        {
            Bounce(collision.contacts[0].normal);
            Debug.Log("colidiu com : " + collision.gameObject.name);
        }
        else if (collision.gameObject.tag.Contains("Player"))
        {
            Bounce(collision.contacts[0].normal);
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

    private void Bounce(Vector3 collisionNormal)
    {
        direction = Vector3.Reflect(velocityLastFrameBeforeHit.normalized, collisionNormal);
        rb.velocity = externalForce + direction * moveSpeed;
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
                dead = true;
                m_collider.enabled = false;
                m_animator.SetTrigger("death");
                //die
            }
        }
    }

    private void ResetForces()
    {
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
    }
}
