using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnStart : MonoBehaviour
{
    public AudioSource audioSource; // Drag & drop AudioSource ke sini di Inspector
    public AudioClip easyModoSound; // Drag & drop file suara yang ingin diputar saat game dimulai
    public AudioClip BackgroundSong;
    public bool easyModo;

    void Start()
    {
        if (easyModo)
        {
            if (audioSource != null && easyModoSound != null)
            {
                audioSource.PlayOneShot(easyModoSound); // Memainkan suara sekali saat game dimulai

                audioSource.clip = BackgroundSong; // Set lagu yang akan diputar
                audioSource.loop = true; // Aktifkan loop agar terus berulang
                audioSource.Play();
            }
        }
        else
        {
            audioSource.clip = BackgroundSong; // Set lagu yang akan diputar
            audioSource.loop = true; // Aktifkan loop agar terus berulang
            audioSource.Play();
        }
    }
}
