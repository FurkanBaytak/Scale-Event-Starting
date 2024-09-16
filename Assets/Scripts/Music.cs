using UnityEngine;

public class Music : MonoBehaviour
{
    // AudioSource component to play the music
    public AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Check if AudioSource exists and play the music
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No AudioSource component found on this GameObject.");
        }
    }
}