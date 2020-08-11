using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorWrapper : MonoBehaviour
{

    [SerializeField] private Animator m_playerAnimator;

    public bool IsRunning { get { return m_playerAnimator.GetBool("isRunning"); } set { m_playerAnimator.SetBool("isRunning", value); } }

    public void AttackTrigger() { m_playerAnimator.SetTrigger("attack"); }
    public void ReviveTrigger() { m_playerAnimator.SetTrigger("revive"); }
    public void TakeDamageTrigger() { m_playerAnimator.SetTrigger("takeDamage"); }
    public void DeathTrigger() { m_playerAnimator.SetTrigger("death"); }
}
