using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Swipe swipeControls;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 2;
    private Vector3 direction;

    [SerializeField] private PlayerWeapon m_playerWeapon;
    [SerializeField] private PlayerAnimatorWrapper m_animatorWrapper;

    [SerializeField] private LayerMask stopLayermask;

    //coisas sobre levar dano
    [SerializeField] private float currentHealth = 3;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;
    [SerializeField] Text lifeText;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.zero;
        lifeText.text = currentHealth.ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        Move();
        ResetForces();
    }

    private void FixedUpdate()
    {
        CheckIdleOrWalk();
    }

    private void CheckInput()
    {
        if (swipeControls.SwipeLeft)
        {
            m_playerWeapon.CallWeaponAttack();
            m_animatorWrapper.AttackTrigger();
            direction = Vector3.left;
        }
        else if (swipeControls.SwipeRight)
        {
            m_playerWeapon.CallWeaponAttack();
            m_animatorWrapper.AttackTrigger();
            direction = Vector3.right;
        }
        else if (swipeControls.SwipeUp)
        {
            m_playerWeapon.CallWeaponAttack();
            m_animatorWrapper.AttackTrigger();
            direction = Vector3.forward;
        }
        else if (swipeControls.SwipeDown)
        {
            m_playerWeapon.CallWeaponAttack();
            m_animatorWrapper.AttackTrigger();
            direction = Vector3.back;
        }

        //if (swipeControls.SwipeDelta.magnitude > 125 && Input.mousePosition == swipeControls.SavedMousePosition)
        //{
        //    m_playerWeapon.CallWeaponAttack();
        //    direction = new Vector3(swipeControls.SavedSwipeDirection.x, 0, swipeControls.SavedSwipeDirection.z);
        //}


        if (swipeControls.Tap)
        {
            Debug.Log("Tap!");
        }


        if (Input.GetKeyDown(KeyCode.F)) TakeDamage(1, Vector3.zero);
    }

    private void Move()
    {
        if (direction != Vector3.zero) transform.forward = direction;

        rb.velocity = externalForce + direction * moveSpeed;
    }

    public void TakeDamage(float damage, Vector3 impactValue)
    {
        lifeText.text = currentHealth.ToString("0");
        if (Time.time - lastDamageTime > 1f)
        {
            currentHealth -= damage;
            lifeText.text = currentHealth.ToString("0");
            m_animatorWrapper.TakeDamageTrigger();
            externalForce += impactValue;
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                m_animatorWrapper.DeathTrigger();
                //die
            }
        }
    }

    public void CreateImpact(Vector3 impactValue)
    {
        if (Time.time - lastDamageTime > 0.15f) externalForce += impactValue;
    }

    private void ResetForces()
    {
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
    }

    private void CheckIdleOrWalk()
    {
        if (Physics.Raycast(transform.position, transform.forward, 0.8f, stopLayermask))
        {
            m_animatorWrapper.IsRunning = false;
        }
        else if(direction != Vector3.zero) m_animatorWrapper.IsRunning = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 0.8f);
    }
}
