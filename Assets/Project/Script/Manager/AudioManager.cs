using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton
    public static AudioManager Instance;
    private void OnEnable()
    {
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip pourSound;
    [SerializeField] private AudioClip completedSound;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource source;
    
    private void PlaySound(AudioClip audioClip) { 
        source.PlayOneShot(audioClip);
    }
    public void PlayPourSound() {
        PlaySound(pourSound);
    }
    public void PlayCompletedSound() {
        PlaySound(completedSound);
    }
}
