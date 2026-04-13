using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource audioSource;

    public AudioClip swipeSound;
    public AudioClip newCardSound;

    void Awake()
    {
        
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlaySwipe()
    {
        audioSource.PlayOneShot(swipeSound);
    }

    public void PlayNewCard()
    {
        audioSource.PlayOneShot(newCardSound);
    }
}