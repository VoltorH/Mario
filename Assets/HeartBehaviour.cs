using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBehaviour : MonoBehaviour
{
    public PlayerHealth m_health;
    public Vector3 m_RotationSpeed = new Vector3(0, 3, 0);
    public float m_Amplitude = 0.3f;
    public float m_Frequency = 1f;

    private Vector3 m_StartPosition;
    public void Start()
    {
            m_StartPosition = transform.position;
        
    }
    public void Update()
    {
        transform.Rotate(m_RotationSpeed);
        float floatOffset = Mathf.Sin(Time.time * m_Frequency) * m_Amplitude;
        transform.position = m_StartPosition + new Vector3(0, floatOffset, 0);


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           m_health = other.gameObject.GetComponent<PlayerHealth>();
            if(m_health.m_CurrentHealth < m_health.m_MaxHealth) 
            {
                m_health.Heal(2);
                Destroy(gameObject);
            }
        }
    }



}
