using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource audioSource;

    public AudioClip swipeSound;
    public AudioClip newCardSound;

    // musica loop
    public AudioClip backgroundMusic;
    public float musicVolume = 0.2f;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            audioSource.clip = backgroundMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true;
            audioSource.Play();

        }
        else
            Destroy(gameObject);
    }

    public void PlaySwipe()
    {
        audioSource.PlayOneShot(swipeSound);
    }

    public void PlayNewCard()
    {
        StartCoroutine(PlaySoundWithDelay(newCardSound, 0.3f));
    }

    IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(clip);
    }

}