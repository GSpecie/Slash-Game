using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public enum EnemyStates { Idle, Shooting, Running, Dead };
    public EnemyStates eStates;

    //GameObject brain;
    //[SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    private Vector3 velocityLastFrameBeforeHit;
    private Vector3 direction;
    private Quaternion runningDirectionStored;

    [SerializeField] Player m_player;

    [SerializeField] private float damage = 1;

    [SerializeField] private float currentHealth = 1;
    [SerializeField] private float maxHealth = 1;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;

    [SerializeField] private float moveSpeed = 2;
    private float moveSpeedStored;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Collider m_collider;
    [SerializeField] private float impactIntensity;

    [SerializeField] private ScoreGameManager gameManager;

    [SerializeField] private EnemyBullet[] myBullets;
    private int indexBullets = 0;
    [SerializeField] Transform bulletPoint;

    private Coroutine bulletAttack;

    [SerializeField] private float aimSpeed;
    [SerializeField] private float timeBetweenShoots;
    [SerializeField] private float aimingTimeOriginal;
    private float aimingTime;
    private bool alreadyAttacked = false;

    [SerializeField] private float timeRunningOriginal;
    private float timeRunning;

    private bool firstBorning = false;

    // Use this for initialization
    void Start()
    {
        eStates = EnemyStates.Shooting;
    }
    private void FixedUpdate()
    {
        switch (eStates)
        {
            case (EnemyStates.Idle):
                Idle();
                break;
            case (EnemyStates.Shooting):
                Shooting();
                break;
            case (EnemyStates.Running):
                Running();
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
        runningDirectionStored = transform.rotation;
        currentHealth = maxHealth;
        m_collider.enabled = true;
        m_animator.SetTrigger("reborn");

        if (firstBorning == false)
        {
            firstBorning = true;
            moveSpeedStored = moveSpeed;
        }
        moveSpeed = 0f;
        velocityLastFrameBeforeHit = Vector3.zero;
        externalForce = Vector3.zero;
        direction = Vector3.zero;
        aimingTime = aimingTimeOriginal;
        timeRunning = timeRunningOriginal;

        StartCoroutine(WakeUp());
    }

    IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(0.5f);
        eStates = EnemyStates.Shooting;
        yield break;
    }

    void Dead()
    {
        eStates = EnemyStates.Dead;
        m_collider.enabled = false;
        m_animator.SetTrigger("death");
        gameManager.AddScore();
        gameManager.RespawnEnemy(this);
    }

    void Idle()
    {
    }

    void Shooting()
    {
        Vector3 myDirection = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z) - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(myDirection);

        aimingTime -= Time.deltaTime;

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, aimSpeed * Time.deltaTime);

        if (aimingTime < 0 && alreadyAttacked == false)
        {
            alreadyAttacked = true;
            bulletAttack = StartCoroutine(ShootSequence());
        }
    }

    IEnumerator ShootSequence()
    {
        m_animator.SetTrigger("shoot");
        yield return new WaitForSeconds(timeBetweenShoots);
        Shoot();
        m_animator.SetTrigger("shoot");
        yield return new WaitForSeconds(timeBetweenShoots);
        Shoot();
        m_animator.SetTrigger("shoot");
        yield return new WaitForSeconds(timeBetweenShoots);
        Shoot();
        yield return new WaitForSeconds(timeBetweenShoots);

        //go to running
        moveSpeed = moveSpeedStored;
        aimingTime = aimingTimeOriginal;
        alreadyAttacked = false;
        transform.rotation = runningDirectionStored;
        eStates = EnemyStates.Running;

        yield break;
    }

    void Shoot()
    {
        myBullets[indexBullets].transform.position = bulletPoint.position;
        myBullets[indexBullets].transform.rotation = bulletPoint.rotation;
        myBullets[indexBullets].ResetTiming();
        myBullets[indexBullets].gameObject.SetActive(true);
        indexBullets++;
        if (indexBullets == myBullets.Length) indexBullets = 0;
    }

    void Running()
    {
        if (direction == Vector3.zero) rb.velocity = externalForce + transform.forward * moveSpeed;
        else
        {
            rb.velocity = externalForce + direction * moveSpeed;

            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        timeRunning -= Time.deltaTime;

        if (timeRunning <= 0)
        {
            timeRunning = timeRunningOriginal;
            runningDirectionStored = transform.rotation;
            moveSpeed = 0f;
            velocityLastFrameBeforeHit = Vector3.zero;
            externalForce = Vector3.zero;
            direction = Vector3.zero;
            rb.velocity = Vector3.zero;
            eStates = EnemyStates.Shooting;
        }

        velocityLastFrameBeforeHit = rb.velocity;
    }

    public void TakeDamage(float damage, Vector3 ImpactValue)
    {
        if (Time.time - lastDamageTime > 0.15f)
        {
            Debug.Log("tomei dnao");
            currentHealth -= damage;
            if (currentHealth > 0) m_animator.SetTrigger("takeDamage");
            externalForce += ImpactValue;
            gameManager.StopTime(0.25f, 10, 0f);
            StartCoroutine(gameManager.CamShake(0.05f));
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                //die
                Dead();
                if (bulletAttack != null)
                {
                    aimingTime = aimingTimeOriginal;
                    alreadyAttacked = false;
                    StopCoroutine(bulletAttack);
                }

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
        else if (collision.gameObject.tag.Contains("Wall"))
        {
            Bounce(collision.contacts[0].normal);
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

    private void Bounce(Vector3 collisionNormal)
    {
        direction = Vector3.Reflect(velocityLastFrameBeforeHit.normalized, collisionNormal);
        rb.velocity = externalForce + direction * moveSpeed;
    }
}
