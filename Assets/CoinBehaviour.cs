using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public CoinManager m_Coins;
    public Vector3 m_RotationSpeed = new Vector3(0, 3, 0);
    public float m_Amplitude = 0.3f;
    public float m_Frequency = 1f;
    AudioManager m_AudioManager;

    private Vector3 m_StartPosition;

    public void Awake()
    {
        m_AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }
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
           m_Coins = other.gameObject.GetComponent<CoinManager>();
            m_Coins.AddScore();
            m_AudioManager.PlaySFX(m_AudioManager.coin);
            Destroy(gameObject);
        }
    }



}
