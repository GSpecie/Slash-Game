using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Swipe swipeControls;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 2;
    private float moveSpeedStored;
    private Vector3 direction;

    [SerializeField] private PlayerWeapon m_playerWeapon;
    [SerializeField] private PlayerAnimatorWrapper m_animatorWrapper;

    [SerializeField] private LayerMask stopLayermask;

    //coisas sobre levar dano
    [SerializeField] private float currentHealth = 3;
    float lastDamageTime = Mathf.NegativeInfinity;
    private Vector3 externalForce = Vector3.zero;
    [SerializeField] Text lifeText;

    private Vector3 vertical, horizontal, rightVertical, rightHorizontal;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ScoreGameManager gameManager;
    private bool isFury = false;
    [SerializeField] AirBullet[] airBullets;
    private int airBulletIndex = 0;
    [SerializeField] Transform bulletPoint;

    [SerializeField] private AirBullet bulletPrefab;
    private Queue<AirBullet> airShots = new Queue<AirBullet>();

    private bool isDead = false;
    [SerializeField] private Collider myCollider;

    private bool firstInput;

    // Start is called before the first frame update
    void Start()
    {
        vertical = Camera.main.transform.forward;
        vertical.y = 0;
        vertical = Vector3.ProjectOnPlane(vertical, Vector3.up).normalized;

        horizontal = Camera.main.transform.right;
        horizontal = Vector3.ProjectOnPlane(horizontal, Vector3.up).normalized;

        direction = Vector3.zero;
        lifeText.text = currentHealth.ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.gameIsPaused == false && isDead == false)
        {
            CheckInput();
        }

        Move();
        ResetForces();
    }

    private void FixedUpdate()
    {
        CheckIdleOrWalk();
    }

    //public AirBullet GetBullet()
    //{
    //    if(airShots.Count == 0)
    //    {
    //        AddShots(1);
    //    }

    //    return airShots.Dequeue();
    //}

    //private void AddShots(int count)
    //{
    //    for(int i = 0; i < count; i++)
    //    {
    //        AirBullet airBullet = Instantiate(bulletPrefab);
    //        airBullet.gameObject.SetActive(false);
    //        airShots.Enqueue(airBullet);
    //    }
    //}

    //public void ReturnToPool(AirBullet airBullet)
    //{
    //    airBullet.gameObject.SetActive(false);
    //    airShots.Enqueue(airBullet);
    //}

    private void CheckInput()
    {
        if (swipeControls.SwipeLeft || swipeControls.SwipeRight || swipeControls.SwipeUp || swipeControls.SwipeDown || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (firstInput == false) gameManager.StartGameplay();
            firstInput = true;

            m_playerWeapon.CallWeaponAttack();
            m_animatorWrapper.AttackTrigger();

            if (isFury)
            {
                StartCoroutine(ShootAirBullet());
            }
        }

        if (swipeControls.SwipeLeft || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //direction = Vector3.left;
            direction = -horizontal;
        }
        else if (swipeControls.SwipeRight || Input.GetKeyDown(KeyCode.RightArrow))
        {
            //direction = Vector3.right;
            direction = horizontal;
        }
        else if (swipeControls.SwipeUp || Input.GetKeyDown(KeyCode.UpArrow))
        {
            //direction = Vector3.forward;
            direction = vertical;
        }
        else if (swipeControls.SwipeDown || Input.GetKeyDown(KeyCode.DownArrow))
        {
            //direction = Vector3.back;
            direction = -vertical;
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


        if (Input.GetKeyDown(KeyCode.F)) TakeDamage(20, Vector3.zero);
    }

    private void Move()
    {
        if (direction != Vector3.zero) transform.forward = direction;

        //externalForce.y = 0;
        rb.velocity = externalForce + direction * moveSpeed;
    }

    public void TakeDamage(float damage, Vector3 impactValue)
    {
        lifeText.text = currentHealth.ToString("0");
        if (Time.time - lastDamageTime > 1f)
        {
            currentHealth -= damage;
            lifeText.text = currentHealth.ToString("0");
            gameManager.ComboBreak();
            if(currentHealth > 0) m_animatorWrapper.TakeDamageTrigger();
            externalForce += impactValue;
            //effects
            gameManager.StopTime(0.05f, 10, 0.1f);
            StartCoroutine(gameManager.CamShake(0.2f));
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                moveSpeedStored = moveSpeed;
                moveSpeed = 0;
                m_animatorWrapper.DeathTrigger();
                myCollider.enabled = false;
                isDead = true;
                currentHealth = 0;
                lifeText.text = currentHealth.ToString("0");

                StartCoroutine(waitToCallContinue());

                //die
            }
        }
    }

    public void Revive()
    {
        myCollider.enabled = true;
        moveSpeed = moveSpeedStored;
        isDead = false;
        currentHealth = 20;
        lifeText.text = currentHealth.ToString("0");
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        direction = Vector3.zero;
        m_animatorWrapper.ReviveTrigger();
    }

    public void CreateImpact(Vector3 impactValue)
    {
        impactValue.y = 0;
        if (Time.time - lastDamageTime > 1f) externalForce += impactValue;
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
        else if (direction != Vector3.zero) m_animatorWrapper.IsRunning = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 0.8f);
    }

    public void ChangeFury(bool active, float bonusHealth)
    {
        isFury = active;
        currentHealth += bonusHealth;
    }

    IEnumerator ShootAirBullet()
    {
        yield return 0;
        airBullets[airBulletIndex].transform.position = bulletPoint.position;
        airBullets[airBulletIndex].transform.rotation = bulletPoint.rotation;
        airBullets[airBulletIndex].ResetTiming();
        airBullets[airBulletIndex].gameObject.SetActive(true);
        airBulletIndex++;
        if (airBulletIndex == airBullets.Length - 1) airBulletIndex = 0;
    }

    IEnumerator waitToCallContinue()
    {
        yield return new WaitForSecondsRealtime(3f);
        gameManager.EnableContinueScreen();
    }
}
