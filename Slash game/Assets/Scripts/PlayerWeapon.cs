using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] Player m_player;
    [SerializeField] private int m_damage = 1;
    [SerializeField] private float impactIntensity;
    [SerializeField] private float playerRecoil;
    [SerializeField] private float m_startUpTime;
    [SerializeField] private float m_ActiveTime;
    [SerializeField] private float m_RecoveryTime;
    [SerializeField] private Collider m_myCol;

    [SerializeField] private Animator myAnimator;
    private bool isDirectionClockwise = true;

    public int M_damage { get { return m_damage; } }
    public float ImpactIntensity { get { return impactIntensity; } }
    public Player M_player { get { return m_player; } }

    private IEnumerator WeaponAttack()
    {
        myAnimator.SetBool("isDirectionClockwise", isDirectionClockwise);
        myAnimator.SetTrigger("attack");
        isDirectionClockwise = !isDirectionClockwise;
        yield return new WaitForSeconds(m_startUpTime);
        m_myCol.enabled = true;
        yield return new WaitForSeconds(m_ActiveTime);
        m_myCol.enabled = false;
        yield return new WaitForSeconds(m_RecoveryTime);
    }

    public void CallWeaponAttack()
    {
        StartCoroutine(WeaponAttack());
    }

    public void SetWeapon(bool isWeaponActive)
    {
        if (isWeaponActive == true) m_myCol.enabled = true;
        else m_myCol.enabled = false;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Enemy")
    //    {
    //        m_player.CreateImpact(other.gameObject.transform.position - m_player.gameObject.transform.position * playerRecoil);
    //        //other.gameObject.GetComponent<BasicEnemy>().TakeDamage(1, (other.gameObject.transform.position - transform.position).normalized * impactIntensity);
    //    }
    //}
}
