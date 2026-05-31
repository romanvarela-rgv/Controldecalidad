using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPowerUp : MonoBehaviour
{
    public AudioSource Clip;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Clip.Play();
        }
    }

   
}
