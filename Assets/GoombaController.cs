using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour, IRestartGameElement
{
    private float m_PatrolSpeed = 3.5f;
    public float m_DetectionRadius = 5.0f;
    public float m_DamagePerSecond = 1.0f;
    public float m_PatrolDistance = 10.0f;

    private Animator m_Animator;
    private CharacterController m_CharacterController;
    private Transform m_Player;
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;
    private Vector3 m_TargetPatrolPosition;
    private bool m_IsAlert = false;
    private bool m_IsReturning = false;
    private bool m_PatrolForward = true;
    private AudioManager m_AudioManager;
    private Vector3 m_Velocity;
    private float m_Gravity = -9.81f;

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
        m_TargetPatrolPosition = m_StartPosition + transform.forward * m_PatrolDistance;
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
        else if (m_IsReturning)
        {
            m_Animator.SetBool("Running", true);
            ReturnToStart();
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
        Vector3 targetPosition = m_PatrolForward ? m_TargetPatrolPosition : m_StartPosition;
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        m_CharacterController.Move(direction * m_PatrolSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            m_PatrolForward = !m_PatrolForward;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, m_Player.position) < 2.0f)
        {
            DealDamage();
        }

        if (Vector3.Distance(transform.position, m_Player.position) > m_DetectionRadius)
        {
            m_IsAlert = false;
            m_IsReturning = true;
        }
    }

    private void ReturnToStart()
    {
        Vector3 direction = (m_StartPosition - transform.position).normalized;
        direction.y = 0;

        m_CharacterController.Move(direction * m_PatrolSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, m_StartPosition) < 0.5f)
        {
            m_IsReturning = false;
            m_PatrolForward = true;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Punch"))
        {
            Kill();
        }
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_TargetPatrolPosition = m_StartPosition + transform.forward * m_PatrolDistance;
        m_CharacterController.enabled = true;
        m_IsAlert = false;
        m_IsReturning = false;
        m_PatrolForward = true;
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
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(m_StartPosition, m_StartPosition + transform.forward * m_PatrolDistance);
    }
}
