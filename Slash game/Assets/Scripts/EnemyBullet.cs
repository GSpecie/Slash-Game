using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int m_damage = 1;
    [SerializeField] private float impactIntensity;
    [SerializeField] private float speed;

    public int M_damage { get { return m_damage; } }
    public float ImpactIntensity { get { return impactIntensity; } }

    private float cooldownToDisappear;
    [SerializeField] private float cooldownToDisappearOriginal;

    private EnemyShooter enemy;

    // Start is called before the first frame update
    void Start()
    {
        ResetTiming();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownToDisappear -= Time.deltaTime;

        if (cooldownToDisappear <= 0)
        {
            this.gameObject.SetActive(false);
            ResetTiming();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    public void WhoIsTheEnemy(EnemyShooter enemy)
    {
        this.enemy = enemy;
    }

    public void ResetTiming()
    {
        cooldownToDisappear = cooldownToDisappearOriginal;
    }

    //private void OnEnable()
    //{
    //    cooldownToDisappear = cooldownToDisappearOriginal;
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag.Contains("Player"))
    //    {
    //        collision.gameObject.GetComponent<Player>().TakeDamage(m_damage, (collision.gameObject.transform.position - transform.position).normalized * impactIntensity);

    //        //Debug.Log("colidiu com : " + collision.gameObject.name);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(m_damage, (other.gameObject.transform.position - transform.position).normalized * impactIntensity);

            this.gameObject.SetActive(false);
            //Debug.Log("colidiu com : " + collision.gameObject.name);
        }
    }
}
