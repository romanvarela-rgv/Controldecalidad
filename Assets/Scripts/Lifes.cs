using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Lifes : MonoBehaviour
{

    public AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            audioSource.Play();
            bool liveRecover = GameManager.Instance.RecoverLive();
            if (liveRecover)
            {
                Destroy(this.gameObject);
            }
        }
    }
}