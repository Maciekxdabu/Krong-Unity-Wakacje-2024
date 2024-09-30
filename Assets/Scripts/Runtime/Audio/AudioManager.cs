using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Tracks")]
    [SerializeField] private AudioClip BGM;

    [Header("Common SFX")]
    [SerializeField] private AudioClip Fail;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.clip = BGM;
        musicSource.Play();
    }

    public void PlayFailSound()
    {
        PlayCustomSFX(Fail);
    }

    internal void PlayCustomSFX(AudioClip customSFX)
    {
        if (customSFX == null) { return; }
        sfxSource.Stop();
        sfxSource.clip = customSFX;
        sfxSource.Play();
    }
}
