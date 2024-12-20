using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image m_HealthBar;
    public float m_MaxHealth = 8.0f;
    public float m_CurrentHealth;
    public float m_DamageCooldown = 1.0f;
    private bool m_IsInvulnerable = false;
    public GameManager1 m_GameManager;

    Animator m_Animator;
    CharacterController m_CharacterController;

    // Reference to the Animation component of the canvas object
    public Animation m_CanvasAnimation;
    public string m_DamageAnimationName; // Name of the damage animation

    private void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        UpdateHealthUI();
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(1.0f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (m_IsInvulnerable) return;

        m_Animator.SetTrigger("Hit");
        m_CurrentHealth -= damage;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);
        UpdateHealthUI();

        // Play the animation on the canvas object
        if (m_CanvasAnimation != null && m_CanvasAnimation[m_DamageAnimationName] != null)
        {
            m_CanvasAnimation.Play(m_DamageAnimationName);
        }

        if (m_CurrentHealth <= 0)
        {
            Die();
            m_GameManager.GameOver();

        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private void UpdateHealthUI()
    {
        if (m_HealthBar != null)
        {
            m_HealthBar.fillAmount = m_CurrentHealth / m_MaxHealth;
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        m_IsInvulnerable = true;
        yield return new WaitForSeconds(m_DamageCooldown);
        m_IsInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Mario ha muerto");

        m_CharacterController.enabled = false;

        m_Animator.SetBool("Die", true);
        StartCoroutine(WaitForDeathAnimation());

    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(1.0f);

        GameManager.GetGameManager().RestartGame();
        m_Animator.SetBool("Die", false);
        m_CurrentHealth = m_MaxHealth;
        UpdateHealthUI();

        m_CharacterController.enabled = true;
    }

    public void Heal(float healAmount)
    {
        m_CurrentHealth += healAmount;
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth);
        UpdateHealthUI();
    }
}
