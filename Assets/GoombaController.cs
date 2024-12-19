using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour, IRestartGameElement
{
    private float m_PatrolSpeed = 3.5f;
    public float m_DetectionRadius = 5.0f;
    public float m_DamagePerSecond = 1.0f;
    Animator m_Animator;
    private Vector3 m_Velocity; 
    private float m_Gravity = -9.81f;

    private CharacterController m_CharacterController;
    private Transform m_Player;
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;
    private bool m_IsAlert = false;
    AudioManager m_AudioManager;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    void Start()
    {
        m_Player = GameObject.FindWithTag("Player").transform;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameManager.GetGameManager().AddRestartGameElement(this);
    }

    void Update()
    {

         m_Velocity.y += m_Gravity * Time.deltaTime;
        m_CharacterController.Move(m_Velocity * Time.deltaTime);

        if (m_IsAlert)
        {
            m_Animator.SetBool("Running", true);
            m_AudioManager.PlaySFX(m_AudioManager.goombaStep);
            ChasePlayer();
        }
        else
        {
            m_Animator.SetBool("Running", false);

            Patrol();
            CheckForPlayer();
        }
        if (m_CharacterController.isGrounded && m_Velocity.y < 0)
        {
            m_Velocity.y = 0f;
        }
    }

    private void Patrol()
    {
        Vector3 forward = transform.forward * m_PatrolSpeed * Time.deltaTime;
        m_CharacterController.Move(forward);

        if (forward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                transform.Rotate(0, 180, 0); 
            }
        }
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, m_Player.position);
        if (distanceToPlayer <= m_DetectionRadius)
        {
            m_PatrolSpeed = 5.0f;
            m_IsAlert = true;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (m_Player.position - transform.position).normalized;
        direction.y = 0; 

        m_CharacterController.Move(direction * m_PatrolSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
        }

        if (Vector3.Distance(transform.position, m_Player.position) < 2.0f)
        {
            DealDamage();
        }

        if (Vector3.Distance(transform.position, m_Player.position) > m_DetectionRadius)
        {
            m_IsAlert = false;
        }
    }

    private void DealDamage()
    {
        m_Player.GetComponent<PlayerHealth>().TakeDamage(1);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            float verticalSpeed = hit.gameObject.GetComponent<MarioController>().m_JumpVerticalSpeed;

            if (verticalSpeed < 0) 
            {
                Kill();
            }
        }
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
        m_IsAlert = false;
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        m_AudioManager.PlaySFX(m_AudioManager.goombaDie);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRadius);
    }
}
