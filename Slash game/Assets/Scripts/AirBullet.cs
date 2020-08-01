using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int m_damage = 1;
    [SerializeField] private float impactIntensity;
    [SerializeField] private float speed;

    public int M_damage { get { return m_damage; } }
    public float ImpactIntensity { get { return impactIntensity; } }

    private float cooldownToDisappear;
    [SerializeField] private float cooldownToDisappearOriginal;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        cooldownToDisappear = cooldownToDisappearOriginal;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownToDisappear -= Time.deltaTime;

        if (cooldownToDisappear <= 0)
        {
            this.gameObject.SetActive(false);
            cooldownToDisappear = cooldownToDisappearOriginal;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    public void WhoIsThePlayer(Player player)
    {
        this.player = player;
    }

    //private void OnEnable()
    //{
    //    cooldownToDisappear = cooldownToDisappearOriginal;
    //}
}
