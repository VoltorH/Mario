using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour, IRestartGameElement
{
    public float m_PatrolSpeed = 2.0f;
    public float m_DetectionRadius = 5.0f;
    public float m_DamagePerSecond = 1.0f;

    private CharacterController m_CharacterController;
    private Transform m_Player;
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;
    private bool m_IsAlert = false;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
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
        if (m_IsAlert)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }
    }

    private void Patrol()
    {
        // Movimiento de patrulla
        Vector3 forward = transform.forward * m_PatrolSpeed * Time.deltaTime;
        m_CharacterController.Move(forward);

        // Detectar colisión con paredes y girar
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
            m_IsAlert = true;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (m_Player.position - transform.position).normalized;
        direction.y = 0; // Mantenerse en el mismo nivel
        m_CharacterController.Move(direction * m_PatrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, m_Player.position) < 1.0f)
        {
            DealDamage();
        }

        // Salir del estado de alerta si el jugador se aleja
        if (Vector3.Distance(transform.position, m_Player.position) > m_DetectionRadius)
        {
            m_IsAlert = false;
        }
    }

    private void DealDamage()
    {
        // Aplicar daño al jugador
        m_Player.GetComponent<PlayerHealth>().TakeDamage(m_DamagePerSecond * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            float verticalSpeed = hit.gameObject.GetComponent<MarioController>().m_JumpVerticalSpeed;

            if (verticalSpeed < 0) // Mario está cayendo desde arriba
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_DetectionRadius);
    }
}
