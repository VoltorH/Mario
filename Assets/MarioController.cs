using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioController : MonoBehaviour
{
    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_Camera;


    public float m_WalkSpeed = 0.5f;
    public float m_RunSpeed = 8.0f;
    public float m_LerpRotationPct = 0.8f;
    float m_VerticalSpeed = 0.0f;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;


    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
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
        l_Movement = l_Movement*l_Speed*Time.deltaTime;
        m_CharacterController.Move(l_Movement);
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
            

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below)!=0 || (l_CollisionFlags & CollisionFlags.Above)!=0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;
    }
}
