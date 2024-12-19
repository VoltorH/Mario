using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;


    [Header("Audio Clip")]
    
    public AudioClip background;
    public AudioClip attack;
    public AudioClip jump;
    public AudioClip wallJump;
    public AudioClip Fall;
    public AudioClip coin;
    public AudioClip goombaStep;
    public AudioClip goombaDie;
    public AudioClip marioDie;
    public AudioClip steps;
    public AudioClip elevator;



    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
