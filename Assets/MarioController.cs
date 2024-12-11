using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.Build.Content;
using UnityEngine;

public class MarioController : MonoBehaviour, IRestartGameElement
{
    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_Camera;


    public float m_WalkSpeed = 0.5f;
    public float m_RunSpeed = 8.0f;
    public float m_LerpRotationPct = 0.8f;
    float m_VerticalSpeed = 0.0f;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    CheckPoint m_CurrentCheckPoint;

    [Header("Jump")]
    public float m_JumpVerticalSpeed = 5.0f;
    public float m_KillJumpVerticalSpeed = 8.0f;
    public float m_WaitStartJumpTime = 0.12f;
    public float m_MaxAngleNeededToKillGoomba = 15.0f;
    public float m_MinAngleNeededToKillGoomba = -1.0f;
    private int m_JumpCombo = 0;
    private float m_LastJumpTime;
    private float m_MaxJumps;
    private int m_CurrentJumpId;
    private float m_JumpComboAvaliable = 5.3f;
    public enum JumpType
    {
        m_Jump = 0,
        m_DoubleJump = 1,
        m_TripleJump = 2,
    }

    [Header("Punch")]
    public float m_PunchComboAvaliable = 1.3f;
    public int m_PunchHitButton = 0;
    private int m_CurrentPunchId;
    private int m_MaxPunches;
    private float m_LastPunchTime;

    [Header("Punch Colliders")]
    public GameObject m_RightHandCollider;
    public GameObject m_LeftHandCollider;
    public GameObject m_RightLegCollider;

    [Header("Bridge")]
    float n_BridgeForce;
    float m_MovingAngleMax = 10;

    float m_MaxAngleToAttach = 8.0f;
    Collider m_CurrentElevator;



    public enum PunchType
    {
        m_RightHand = 0,
        m_LeftHand = 1,
        m_RightLeg = 2,
    }

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;


    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_RightHandCollider.gameObject.SetActive(false);
        m_LeftHandCollider.gameObject.SetActive(false);
        m_RightLegCollider.gameObject.SetActive(false);
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 l_Forward = m_Camera.transform.forward;
        Vector3 l_Right = m_Camera.transform.right;
        l_Forward.y = 0.0f;
        l_Right.y = 0.0f;
        l_Forward.Normalize();
        l_Right.Normalize();
        bool l_HasMovement = false;


        Vector3 l_Movement = Vector3.zero;
        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
            l_HasMovement = true;
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
            l_HasMovement = true;
        }
        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
            l_HasMovement = true;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement -= l_Forward;
            l_HasMovement = true;
        }
        l_Movement.Normalize();
        float l_Speed = 0.0f;
        if (l_HasMovement) 
        {
            if (Input.GetKey(m_RunKeyCode))
            {
                l_Speed = m_RunSpeed;
                m_Animator.SetFloat("Speed", 1.0f);
            }
            else 
            {
                l_Speed = m_WalkSpeed;
                m_Animator.SetFloat("Speed", 0.2f);

            }

            Quaternion l_DesiredRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp (transform.rotation, l_DesiredRotation, m_LerpRotationPct * Time.deltaTime);



        }
        else
            m_Animator.SetFloat("Speed",0.0f);
        if (CanJump() && Input.GetKeyDown(m_JumpKeyCode))
            Jump();

        l_Movement = l_Movement*l_Speed*Time.deltaTime;
        
        m_CharacterController.Move(l_Movement);
        
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f)
            m_Animator.SetBool("Falling", false);
        else
            m_Animator.SetBool("Falling", true);

        if(((l_CollisionFlags & CollisionFlags.Below)!=0 && m_VerticalSpeed < 0.0f) ||
            (l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;

        UpdatedPunch();
        UpdatedElevator();

    }
    bool CanJump()
    {
        return true;
    }
    void Jump()
    {

            m_Animator.SetTrigger("Jump");
        float m_DiffTime = Time.time - m_LastJumpTime;
        if (m_DiffTime <= m_JumpComboAvaliable)
        {
            m_CurrentJumpId = (m_CurrentJumpId + 1) % 3;
        }
        else
        {
            m_CurrentJumpId = 0;
        }
        m_LastJumpTime = Time.time;
        m_Animator.SetInteger("Jump Combo",m_CurrentJumpId);
        StartCoroutine(ExecuteJump());  
        
    }
    IEnumerator ExecuteJump()
    {
        yield return new WaitForSeconds(m_WaitStartJumpTime);
        m_VerticalSpeed = m_JumpVerticalSpeed;
        m_Animator.SetBool("Falling",false);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Goomba"))
        {
            if (IsUpperHit(hit.transform))
            {
                hit.gameObject.GetComponent<GoombaController>().Kill();
                m_VerticalSpeed = m_KillJumpVerticalSpeed;
            }
            else
            {
                Debug.Log("Player must be hit");
            }

        }
        if (hit.gameObject.CompareTag("Bridge"))
        {
            hit.rigidbody.AddForceAtPosition(-hit.normal * n_BridgeForce, hit.point);
        }

    }

    bool IsUpperHit(Transform GoombaTransform)
    {
        Vector3 l_GoombaDirection = transform.position - GoombaTransform.position;
        l_GoombaDirection.Normalize();
        float l_DotAngle = Vector3.Dot(l_GoombaDirection, Vector3.up);
        Debug.Log("m_VerticalSpeed" + m_VerticalSpeed);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleNeededToKillGoomba * Mathf.Deg2Rad)&&
            m_VerticalSpeed <= m_MinAngleNeededToKillGoomba)
            return true;
        return false;

    }
    void UpdatedPunch()
    {
        if(Input.GetMouseButtonDown(m_PunchHitButton) && CanPunch())
        {
            PunchCombo();
        }
    }
    bool CanPunch()
    {
        return true;
    }
    void PunchCombo()
    {
        m_Animator.SetTrigger("Punch");
        float m_DiffTime = Time.time - m_LastPunchTime;
        if(m_DiffTime <= m_PunchComboAvaliable)
        {
            m_CurrentPunchId = (m_CurrentPunchId + 1) % 3;
        }
        else
        {
            m_CurrentPunchId = 0;
        }
        m_LastPunchTime = Time.time;
        m_Animator.SetInteger("Punch combo", m_CurrentPunchId);

    }
    public void EnableHitCollider(PunchType Punchtype, bool Active)
    {
        switch (Punchtype)
        {
            case PunchType.m_RightHand:
                m_RightHandCollider.SetActive(Active);
                break;
            case PunchType.m_LeftHand:
                m_LeftHandCollider.SetActive(Active);
                break;
            case PunchType.m_RightLeg:
                m_RightLegCollider.SetActive(Active);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            m_CurrentCheckPoint = other.GetComponent<CheckPoint>();

        }
        if (other.CompareTag("Elevator"))
        {
            if (CanAttachElevator(other))
            {
                AttachElevator(other);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Elevator") && other == m_CurrentElevator)
        {
            DetachElevator();
        }
    }
    void UpdatedElevator()
    {
        if(m_CurrentElevator == null)
        {
            return;
        }
        if (!IsAttacheableElevator(m_CurrentElevator))
        {
            DetachElevator();
        }
    }
    void LateUpdate()
    {
        Vector3 l_Angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0.0f, l_Angles.y, 0.0f);
    }
    bool CanAttachElevator(Collider Elevator)
    {
        if(m_CurrentElevator == null){
            return false;
        }
        return IsAttacheableElevator(Elevator);
    }
    bool IsAttacheableElevator(Collider Elevator)
    {
        float l_DotAngle = Vector3.Dot(Elevator.transform.forward, Vector3.up);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleToAttach * Mathf.Deg2Rad))
        {
            return true;
        }
        return false;
    }
    void AttachElevator(Collider Elevator)
    {
        transform.SetParent(Elevator.transform.parent);
        m_CurrentElevator = Elevator;
    }
    void DetachElevator() 
    { 
        m_CurrentElevator = null;
        transform.SetParent(null);
    }
    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        if (m_CurrentCheckPoint == null)
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckPoint.m_RespawnPosition.position;
            transform.rotation = m_CurrentCheckPoint.m_RespawnPosition.rotation;
        }
        m_CharacterController.enabled = true;

    }
    /*(public void Step (AnimationEvent _AnimationEvent)
    {
        AudioClip l_AudioClip =
    }
    */

}
