using Assets.Scripts.Runtime.Character;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Tracks")]
    [SerializeField] private AudioClip BGM;

    [Header("Common SFX")]
    [SerializeField] private AudioClip Fail;
    [SerializeField] private AudioClip LevelComplete;
    [SerializeField] private AudioClip HeroDied;
    [SerializeField] private AudioClip MinionSummoned;
    [SerializeField] private AudioClip[] Strikes;


    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxSource2;

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

    public void PlayCustomSFX(AudioClip customSFX, AudioSource source = null)
    {
        if (source == null) {
            source = sfxSource;
        }
        if (customSFX == null) { return; }
        source.Stop();
        source.clip = customSFX;
        source.Play();
    }

    public void PlayFinishLevel()
    {
        PlayCustomSFX(LevelComplete, sfxSource2);
    }

    public void PlayHeroDied()
    {
        PlayCustomSFX(HeroDied);
    }

    public void PlayHeroAttack(Hero hero)
    {
        AudioSource.PlayClipAtPoint(Strikes[2], hero.transform.position);
    }

    public void PlayEnemyAttack(Enemy enemy)
    {
        AudioSource.PlayClipAtPoint(Strikes[1], enemy.transform.position);
    }

    public void PlayMinionAttack(Minion minion)
    {
        AudioSource.PlayClipAtPoint(Strikes[0], minion.transform.position);
    }

    public void PlayMinionSummoned(Minion minion)
    {
        AudioSource.PlayClipAtPoint(MinionSummoned, minion.transform.position);
    }
}
